using EtherSharp.Client.Services.GasFeeProvider;
using EtherSharp.Client.Services.ResiliencyLayer;
using EtherSharp.Client.Services.TxPublisher;
using EtherSharp.Client.Services.TxTypeHandler;
using EtherSharp.Common.Exceptions;
using EtherSharp.RPC;
using EtherSharp.RPC.Modules.Eth;
using EtherSharp.Tx;
using EtherSharp.Tx.PendingHandler;
using EtherSharp.Tx.Types;
using EtherSharp.Types;
using EtherSharp.Wallet;
using Microsoft.Extensions.DependencyInjection;

namespace EtherSharp.Client.Services.TxScheduler;

public class BlockingSequentialTxSchedulerV1 : ITxScheduler, IInitializableService
{
    private abstract record QueueEntry(TaskCompletionSource ProcessingCts)
    {
        public record ExistingHandler(IInternalPendingTxHandler TxHandler, TaskCompletionSource ProcessingCts) : QueueEntry(ProcessingCts);
        public record PendingHandler(TaskCompletionSource<IInternalPendingTxHandler> TxHandlerCts, TaskCompletionSource ProcessingCts) : QueueEntry(ProcessingCts);
    }

    private readonly IServiceProvider _provider;

    private readonly IEtherSigner _signer;
    private readonly ITxPublisher _txPublisher;

    private readonly IEthRpcModule _ethRpcModule;

    private readonly IResiliencyLayer? _resiliencyLayer;

    private readonly TimeSpan _txTimeout = TimeSpan.FromSeconds(30);

    private readonly Lock _pendingEntriesLock = new Lock();
    private readonly Dictionary<uint, QueueEntry> _pendingEntries = [];

    private ulong _chainId;

    private readonly SemaphoreSlim _nonceIncrementSemaphore = new SemaphoreSlim(1);
    private SemaphoreSlim _pendingNoncesSemaphore = null!;

    private readonly Lock _activeNonceLock = new Lock();
    /// <summary>
    /// Highest nonce that is not confirmed on-chain yet.
    /// </summary>
    private uint _activeNonce;
    /// <summary>
    /// Highest nonce that is still unallocated.
    /// </summary>
    private uint _peakNonce;

    public BlockingSequentialTxSchedulerV1(IServiceProvider provider, IRpcClient rpcClient, IEthRpcModule ethRpcModule, IEtherSigner signer,
        ITxPublisher txPublisher)
    {
        _provider = provider;
        _signer = signer;
        _txPublisher = txPublisher;

        _ethRpcModule = ethRpcModule;

        _resiliencyLayer = _provider.GetService<IResiliencyLayer>();
    }

    async ValueTask IInitializableService.InitializeAsync(ulong chainId, CancellationToken cancellationToken)
    {
        _chainId = chainId;

        _activeNonce = await _ethRpcModule.GetTransactionCountAsync(
            _signer.Address, TargetBlockNumber.Latest, cancellationToken
        );

        if(_resiliencyLayer is null)
        {
            _peakNonce = _activeNonce;
            _pendingNoncesSemaphore = new SemaphoreSlim(0);
        }
        else
        {
            _peakNonce = Math.Max(_activeNonce, await _resiliencyLayer.GetLastSubmittedNonceAsync(cancellationToken) + 1);
            _pendingNoncesSemaphore = new SemaphoreSlim((int) (_peakNonce - _activeNonce));

            for(uint nonce = _activeNonce; nonce < _peakNonce; nonce++)
            {
                var entry = new QueueEntry.PendingHandler(new TaskCompletionSource<IInternalPendingTxHandler>(), new TaskCompletionSource());
                _pendingEntries.Add(nonce, entry);
            }
        }

        _ = Task.Run(NonceProcessor, default);
    }

    private async Task NonceProcessor()
    {
        while(true)
        {
            await _pendingNoncesSemaphore.WaitAsync();

            QueueEntry? entry;
            lock(_pendingEntriesLock)
            {
                if(!_pendingEntries.TryGetValue(_activeNonce, out entry))
                {
                    continue;
                }
            }

            if(entry is null)
            {
                throw new ImpossibleException();
            }

            var txHandler = entry switch
            {
                QueueEntry.ExistingHandler existingHandlerEntry => existingHandlerEntry.TxHandler,
                QueueEntry.PendingHandler pendingHandlerEntry => await pendingHandlerEntry.TxHandlerCts.Task,
                _ => throw new ImpossibleException()
            };

            entry.ProcessingCts.SetResult();
            var result = await txHandler.WaitForCompletionAsync();

            if(result is TxConfirmationResult.Cancelled)
            {
                continue;
            }

            lock(_pendingEntriesLock)
            {
                _pendingEntries.Remove(_activeNonce);
            }

            _activeNonce++;
        }
    }

    async ValueTask<IPendingTxHandler<TTxParams, TTxGasParams>> ITxScheduler.PrepareTxAsync<TTransaction, TTxParams, TTxGasParams>(
        ITxInput call, TTxParams? txParams, TTxGasParams? txGasParams, CancellationToken cancellationToken
    )
        where TTxParams : class
        where TTxGasParams : class
    {
        var handler = _provider.GetService<ITxTypeHandler<TTransaction, TTxParams, TTxGasParams>>()
            ?? throw new InvalidOperationException(
                $"No ITxTypeHandler found that supports {typeof(TTxParams).FullName};{typeof(TTxGasParams).FullName} is not registered");
        var gasFeeProvider = _provider.GetService<IGasFeeProvider<TTxParams, TTxGasParams>>()
            ?? throw new InvalidOperationException(
                $"No IGasFeeProvider found that supports {typeof(TTxParams).FullName};{typeof(TTxGasParams).FullName} is not registered");

        await _nonceIncrementSemaphore.WaitAsync();

        uint nextNonce = _peakNonce;

        try
        {
            txParams ??= TTxParams.Default;
            txGasParams ??= await gasFeeProvider.EstimateGasParamsAsync(call, txParams, default);
            _peakNonce++;
        }
        finally
        {
            _nonceIncrementSemaphore.Release();
        }

        string txBytes = handler.EncodeTxToBytes(call, txParams, txGasParams, nextNonce, out string txHash);
        var submission = new TxSubmission<TTxParams, TTxGasParams>(_chainId, 0, txHash, txBytes, call, txParams, txGasParams);

        var pendingTxHandler = new PendingTxHandler<TTxParams, TTxGasParams>(
            nextNonce,
            [submission],
            PublishAndConfirmPendingTxAsync<TTransaction, TTxParams, TTxGasParams>
        );

        lock(_pendingEntriesLock)
        {
            _pendingEntries.Add(nextNonce, new QueueEntry.ExistingHandler(pendingTxHandler, new TaskCompletionSource()));
        }

        _pendingNoncesSemaphore.Release();
        return pendingTxHandler;
    }

    async Task<IPendingTxHandler<TTxParams, TTxGasParams>> ITxScheduler.AttachPendingTxAsync<TTransaction, TTxParams, TTxGasParams>(uint nonce, CancellationToken cancellationToken)
    {
        if(_resiliencyLayer is null)
        {
            throw new NotSupportedException($"No {nameof(IResiliencyLayer)} configured");
        }

        var txSubmissions = await _resiliencyLayer.FetchTxSubmissionsAsync(nonce, cancellationToken);
        var typedTxSubmissions = txSubmissions.Select(x => x.ToTxSubmission<TTxParams, TTxGasParams>()).OrderByDescending(x => x.Sequence);

        QueueEntry.PendingHandler pendingHandler;
        lock(_pendingEntries)
        {
            if(!_pendingEntries.TryGetValue(nonce, out var entry))
            {
                if(_activeNonce <= nonce)
                {
                    throw new InvalidOperationException($"No pending tx with nonce {nonce} found");
                }
                //
                return new PendingTxHandler<TTxParams, TTxGasParams>(
                    nonce,
                    typedTxSubmissions,
                    (handler, _) => FetchTxConfirmationResultAsync(handler.TxSubmissions, 1)
                );
            }

            if(entry is not QueueEntry.PendingHandler pendingHandlerEntry)
            {
                throw new InvalidOperationException($"There already is an existing handler for tx nonce {nonce}");
            }

            pendingHandler = pendingHandlerEntry;
        }

        var handler = new PendingTxHandler<TTxParams, TTxGasParams>(
            nonce,
            typedTxSubmissions,
            PublishAndConfirmPendingTxAsync<TTransaction, TTxParams, TTxGasParams>
        );

        if(!pendingHandler.TxHandlerCts.TrySetResult(handler))
        {
            throw new InvalidOperationException($"{nameof(IEtherTxClient.AttachPendingTxAsync)} already called for nonce {nonce}");
        }
        //
        return handler;
    }

    private async Task<TxConfirmationResult> PublishAndConfirmPendingTxAsync<TTransaction, TTxParams, TTxGasParams>(
        PendingTxHandler<TTxParams, TTxGasParams> txHandler,
        Func<
            TxConfirmationError,
            TxConfirmationActionBuilder<TTxParams, TTxGasParams>,
            TxSubmission<TTxParams, TTxGasParams>,
            TxConfirmationAction<TTxParams, TTxGasParams>
        > onError
    )
        where TTransaction : class, ITransaction<TTransaction, TTxParams, TTxGasParams>
        where TTxParams : class, ITxParams<TTxParams>
        where TTxGasParams : class, ITxGasParams<TTxGasParams>
    {
        var handler = _provider.GetService<ITxTypeHandler<TTransaction, TTxParams, TTxGasParams>>()
            ?? throw new InvalidOperationException(
                $"No ITxTypeHandler found that supports {typeof(TTxParams).FullName};{typeof(TTxGasParams).FullName} is not registered");

        QueueEntry entry;
        lock(_pendingEntriesLock)
        {
            entry = _pendingEntries[txHandler.Nonce];
        }

        await entry.ProcessingCts.Task;

        if(entry is QueueEntry.ExistingHandler && _resiliencyLayer is not null)
        {
            await _resiliencyLayer.StoreTxSubmissionAsync(txHandler.TxSubmissions.Single().ToStorageType(txHandler.Nonce));
        }

        using var cts = new CancellationTokenSource();
        var noncePollTask = WaitForNonceAsync(x => x > txHandler.Nonce, cts.Token);
        var errorHandlingTask = Task.Run(async () =>
        {
            bool txInMempool = false;

            bool requirePublish = true;
            bool requireCancel = false;

            TxConfirmationError error = null!;
            var latestSubmission = txHandler.TxSubmissions[0];

            for(int attempt = 0; ; attempt++)
            {
                if(cts.IsCancellationRequested)
                {
                    return;
                }

                if(requirePublish)
                {
                    latestSubmission = txHandler.TxSubmissions[0];
                    var publishResult = await _txPublisher.PublishTxAsync(latestSubmission.SignedTx, cts.Token);

                    bool isSuccessfulPublish = publishResult switch
                    {
                        TxSubmissionResult.Success => true,
                        TxSubmissionResult.AlreadyExists => true,
                        _ => false
                    };

                    if(isSuccessfulPublish)
                    {
                        txInMempool = true;
                        await Task.Delay(_txTimeout, cts.Token);
                    }

                    error = publishResult switch
                    {
                        TxSubmissionResult.Success => new TxConfirmationError.Timeout(),
                        TxSubmissionResult.AlreadyExists => new TxConfirmationError.Timeout(),
                        TxSubmissionResult.TransactionUnderpriced => new TxConfirmationError.TransactionUnderpriced(),
                        TxSubmissionResult.UnhandledException ex => new TxConfirmationError.UnhandledException(ex.Exception),
                        _ => throw new ImpossibleException()
                    };
                }
                else if(requireCancel)
                {
                    bool cancelled = !txInMempool && await TryCancelTransactionAsync(txHandler.Nonce);

                    if(cancelled)
                    {
                        cts.Cancel();
                        return;
                    }

                    error = new TxConfirmationError.TransactionNotCancellable();
                }

                requirePublish = false;
                requireCancel = false;

                var actions = onError.Invoke(error, new TxConfirmationActionBuilder<TTxParams, TTxGasParams>(), latestSubmission);

                TTxParams? newTxParams = null;
                TTxGasParams? newGasParams = null;

                switch(actions)
                {
                    case TxConfirmationAction<TTxParams, TTxGasParams>.ContinueWaiting waitAction:
                        await Task.Delay(waitAction.Duration, cts.Token);
                        break;
                    case TxConfirmationAction<TTxParams, TTxGasParams>.MinimalGasFeeIncrement minimalGasIncrementAction:
                        newGasParams = latestSubmission.GasParams.IncrementByFactor(1105, 1000, 1);
                        break;
                    case TxConfirmationAction<TTxParams, TTxGasParams>.CancelTransaction cancelTransactionAction:
                        requireCancel = true;
                        break;
                    default:
                        throw new ImpossibleException();
                }

                if(newTxParams is not null || newGasParams is not null)
                {
                    newTxParams ??= latestSubmission.Params;
                    newGasParams ??= latestSubmission.GasParams;

                    string newSignedTx = handler.EncodeTxToBytes(
                        latestSubmission.Call,
                        newTxParams,
                        newGasParams,
                        txHandler.Nonce,
                        out string newTxHash
                    );

                    var newSubmission = new TxSubmission<TTxParams, TTxGasParams>(
                        _chainId,
                        latestSubmission.Sequence + 1,
                        newTxHash,
                        newSignedTx,
                        latestSubmission.Call,
                        newTxParams,
                        newGasParams
                    );

                    if(_resiliencyLayer is not null)
                    {
                        await _resiliencyLayer.StoreTxSubmissionAsync(newSubmission.ToStorageType(txHandler.Nonce), cts.Token);
                    }

                    requirePublish = true;
                    txHandler.TxSubmissions.Insert(0, newSubmission);
                }
            }
        });

        try
        {
            await noncePollTask;
        }
        catch(OperationCanceledException) { }

        if(cts.IsCancellationRequested)
        {
            if(_resiliencyLayer is not null)
            {
                await _resiliencyLayer.DeleteTxSubmissionsForNonceAsync(txHandler.Nonce);
            }

            return new TxConfirmationResult.Cancelled();
        }

        cts.Cancel();
        return await FetchTxConfirmationResultAsync(txHandler.TxSubmissions, 3);
    }

    private async Task<TxConfirmationResult> FetchTxConfirmationResultAsync<TTxParams, TTxGasParams>(IEnumerable<TxSubmission<TTxParams, TTxGasParams>> txSubmissions, int attempts)
        where TTxParams : class, ITxParams<TTxParams>
        where TTxGasParams : class, ITxGasParams<TTxGasParams>
    {
        for(int i = 0; i < attempts; i++)
        {
            foreach(var txSubmission in txSubmissions)
            {
                var txReceipt = await _ethRpcModule.GetTransactionReceiptAsync(txSubmission.TxHash);

                if(txReceipt is not null)
                {
                    return new TxConfirmationResult.Success(txReceipt);
                }
            }

            await Task.Delay(1000 * i);
        }

        return new TxConfirmationResult.NonceAlreadyUsed();
    }

    private async Task WaitForNonceAsync(Predicate<uint> predicate, CancellationToken cancellationToken = default)
    {
        //ToDo: Consider configurable polling interval

        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(2));

        while(await timer.WaitForNextTickAsync(cancellationToken))
        {
            try
            {
                uint txCount = await _ethRpcModule.GetTransactionCountAsync(_signer.Address, TargetBlockNumber.Latest, cancellationToken);
                if(predicate(txCount))
                {
                    return;
                }
            }
            catch(OperationCanceledException ex) when(ex.CancellationToken == cancellationToken)
            {
                break;
            }
            catch
            {
                //ToDo: Consider how to handle exceptions here
            }
        }
    }

    private async Task<bool> TryCancelTransactionAsync(uint nonce)
    {
        await _nonceIncrementSemaphore.WaitAsync();

        try
        {
            if(_peakNonce != nonce + 1)
            {
                return false;
            }

            _peakNonce = nonce;

            lock(_pendingEntriesLock)
            {
                _pendingEntries.Remove(nonce);
            }

            return true;
        }
        finally
        {
            _nonceIncrementSemaphore.Release();
        }
    }
}
