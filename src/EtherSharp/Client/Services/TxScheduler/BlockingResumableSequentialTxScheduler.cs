using EtherSharp.Client.Services.GasFeeProvider;
using EtherSharp.Client.Services.ResiliencyLayer;
using EtherSharp.Client.Services.RPC;
using EtherSharp.Client.Services.TxPublisher;
using EtherSharp.Client.Services.TxTypeHandler;
using EtherSharp.Common.Exceptions;
using EtherSharp.Tx;
using EtherSharp.Tx.PendingHandler;
using EtherSharp.Tx.Types;
using EtherSharp.Types;
using EtherSharp.Wallet;
using Microsoft.Extensions.DependencyInjection;

namespace EtherSharp.Client.Services.TxScheduler;

public class BlockingSequentialResumableTxScheduler : ITxScheduler, IInitializableService
{
    private abstract record QueueEntry
    {
        public record ExistingHandler(IInternalPendingTxHandler TxHandler) : QueueEntry;
        public record PendingHandler(TaskCompletionSource<IInternalPendingTxHandler> TxHandlerCts) : QueueEntry;
    }

    private readonly IServiceProvider _provider;
    private readonly IRpcClient _rpcClient;
    private readonly IEtherSigner _signer;
    private readonly ITxPublisher _txPublisher;

    private readonly IResiliencyLayer? _resiliencyLayer;

    private readonly TimeSpan _txTimeout = TimeSpan.FromSeconds(30);

    private readonly Lock _pendingTxHandlersLock = new Lock();
    private readonly Dictionary<uint, QueueEntry> _pendingEntries = [];

    private ulong _chainId;

    private SemaphoreSlim _pendingNoncesSemaphore;

    /// <summary>
    /// Highest nonce that is not confirmed on-chain yet.
    /// </summary>
    private uint _activeNonce;
    /// <summary>
    /// Highest nonce that is still unallocated.
    /// </summary>
    private uint _peakNonce;

    public BlockingSequentialResumableTxScheduler(IServiceProvider provider, IRpcClient rpcClient, IEtherSigner signer,
        ITxPublisher txPublisher)
    {
        _provider = provider;
        _rpcClient = rpcClient;
        _signer = signer;
        _txPublisher = txPublisher;

        _resiliencyLayer = _provider.GetService<IResiliencyLayer>();
    }

    ValueTask<IPendingTxHandler<TTxParams, TTxGasParams>> ITxScheduler.AttachPendingTxAsync<TTransaction, TTxParams, TTxGasParams>(uint nonce) => throw new NotImplementedException();

    async ValueTask IInitializableService.InitializeAsync(ulong chainId, CancellationToken cancellationToken)
    {
        _chainId = chainId;

        _activeNonce = await _rpcClient.EthGetTransactionCount(
            _signer.Address.String, TargetBlockNumber.Latest, cancellationToken
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
                var txSubmissions = await _resiliencyLayer.FetchTxSubmissionsAsync(nonce);
                var entry = new QueueEntry.PendingHandler(new TaskCompletionSource<IInternalPendingTxHandler>());
                _pendingEntries.Add(nonce, entry);
            }
        }

        _ = Task.Run(NonceProcessor, default);
    }

    private async Task NonceProcessor()
    {
        while(true)
        {
            //Wait for _peakNonce + 1 > _activeNonce
            await _pendingNoncesSemaphore.WaitAsync();

            QueueEntry? entry;
            lock(_pendingTxHandlersLock)
            {
                _pendingEntries.Remove(_activeNonce, out entry);
            }

            if(entry is null)
            {
                throw new ImpossibleException();
            }

            switch(entry)
            {
                case QueueEntry.ExistingHandler existingHandlerEntry:
                {
                    await existingHandlerEntry.TxHandler.WaitForCompletionAsync();
                    break;
                }
                case QueueEntry.PendingHandler pendingHandlerEntry:
                {
                    var handlerTask = pendingHandlerEntry.TxHandlerCts.Task;

                    var noncePollTask = WaitForNonceAsync(x => x > _activeNonce);

                    //Do this in parallel:
                    //- Poll nonce, if it increments increment nonce by 1 and continue the loop
                    //- Wait for handlerTask, once it completes abort the task above and call WaitForCompletionAsync

                    throw new NotSupportedException();
                    break;
                }    
                default:
                    throw new NotSupportedException();
            }

            _activeNonce++;
        }
    }

    async ValueTask<IPendingTxHandler<TTxParams, TTxGasParams>> ITxScheduler.PrepareTxAsync<TTransaction, TTxParams, TTxGasParams>(
        ITxInput call, TTxParams? txParams, TTxGasParams? txGasParams
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

        uint nextNonce = Interlocked.Increment(ref _peakNonce) - 1;

        txParams ??= TTxParams.Default;
        txGasParams ??= await gasFeeProvider.EstimateGasParamsAsync(call.To, call.Value, call.Data, txParams, default);

        string txBytes = handler.EncodeTxToBytes(call, txParams, txGasParams, nextNonce, out string txHash);
        var submission = new TxSubmission<TTxParams, TTxGasParams>(txHash, txBytes, call, txParams, txGasParams);

        var pendingTxHandler = new PendingTxHandler<TTxParams, TTxGasParams>(nextNonce, [submission], ProcessPendingTxAsync);

        lock(_pendingTxHandlersLock)
        {
            _pendingEntries.Add(nextNonce, new QueueEntry.ExistingHandler(pendingTxHandler));
        }

        _pendingNoncesSemaphore.Release();
        return pendingTxHandler;
    }

    private async Task<TxConfirmationResult> ProcessPendingTxAsync<TTxParams, TTxGasParams>(
        PendingTxHandler<TTxParams, TTxGasParams> txHandler,
        Func<TxConfirmationError, ITxInput, TTxParams, TTxGasParams, TxConfirmationAction> onError
    )
        where TTxParams : class, ITxParams<TTxParams>
        where TTxGasParams : class, ITxGasParams<TTxGasParams>
    {
        using var cts = new CancellationTokenSource();
        var noncePollTask = WaitForNonceAsync(x => x > txHandler.Nonce);
        var errorHandlingTask = Task.Run(async () =>
        {
            bool requirePublish = true;
            TxSubmissionResult publishResult = null!;
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
                    publishResult = await _txPublisher.PublishTxAsync(latestSubmission.SignedTx, cts.Token);
                }

                if (publishResult is TxSubmissionResult.Success && attempt == 0)
                {
                    await Task.Delay(_txTimeout, cts.Token);
                }

                TxConfirmationError error = publishResult switch
                {
                    TxSubmissionResult.Success => new TxConfirmationError.Timeout(),
                    TxSubmissionResult.UnhandledException ex => new TxConfirmationError.UnhandledException(ex.Exception),
                    _ => throw new ImpossibleException()
                };

                requirePublish = false;

                var actions = onError.Invoke(error, latestSubmission.Call, latestSubmission.Params, latestSubmission.GasParams);

                switch(actions)
                {
                    case TxConfirmationAction.ContinueWaiting waitAction:
                        await Task.Delay(waitAction.Duration, cts.Token);
                        break;
                    
                    default:
                        throw new ImpossibleException();
                }
            }

        });

        await noncePollTask;

        foreach(var txSubmission in txHandler.TxSubmissions)
        {
            var txReceipt = await _rpcClient.EthGetTransactionReceiptAsync(txSubmission.TxHash);

            if (txReceipt is not null)
            {
                return new TxConfirmationResult.Success(txReceipt);
            }
        }

        return new TxConfirmationResult.NonceAlreadyUsed();
    }

    private async Task WaitForNonceAsync(Predicate<uint> predicate)
    {
        //ToDo: Consider configurable polling interval

        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(2));

        while (await timer.WaitForNextTickAsync())
        {
            try
            {
                uint txCount = await _rpcClient.EthGetTransactionCount(_signer.Address.String, TargetBlockNumber.Latest);
                if(predicate(txCount))
                {
                    return;
                }
            }
            catch
            {
                //ToDo: Consider how to handle exceptions here
            }
        }

        throw new ImpossibleException();
    }
}
