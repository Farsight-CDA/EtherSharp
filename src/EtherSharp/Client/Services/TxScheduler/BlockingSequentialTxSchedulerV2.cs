using EtherSharp.Client.Services.GasFeeProvider;
using EtherSharp.Client.Services.ResiliencyLayer;
using EtherSharp.Client.Services.TxPublisher;
using EtherSharp.Client.Services.TxTypeHandler;
using EtherSharp.Common.Exceptions;
using EtherSharp.RPC.Modules.Eth;
using EtherSharp.Tx;
using EtherSharp.Tx.PendingHandler;
using EtherSharp.Tx.Types;
using EtherSharp.Types;
using EtherSharp.Wallet;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EtherSharp.Client.Services.TxScheduler;

public class BlockingSequentialTxSchedulerV2(
    IServiceProvider provider,
    IEthRpcModule ethRpcModule,
    IEtherSigner signer,
    ITxPublisher txPublisher
) : ITxScheduler, IInitializableService, IDisposable
{
    private readonly IResiliencyLayer? _resiliencyLayer = provider.GetService<IResiliencyLayer>();
    private readonly ILogger? _logger = provider.GetService<ILoggerFactory>()?.CreateLogger<BlockingSequentialTxSchedulerV2>();

    private readonly CancellationTokenSource _cts = new CancellationTokenSource();

    private readonly Lock _stateLock = new Lock();
    private readonly SemaphoreSlim _preparationLock = new SemaphoreSlim(1);

    private readonly SemaphoreSlim _workerSignal = new SemaphoreSlim(0);

    private ulong _chainId;

    private uint _confirmedNonce;
    private uint _peakNonce;

    private readonly Dictionary<uint, TaskCompletionSource> _nonceGates = [];

    /// <inheritdoc/>
    public async ValueTask InitializeAsync(ulong chainId, CancellationToken cancellationToken)
    {
        _chainId = chainId;

        _confirmedNonce = await ethRpcModule.GetTransactionCountAsync(
            signer.Address, TargetBlockNumber.Latest, cancellationToken
        );

        if(_resiliencyLayer is null)
        {
            _peakNonce = _confirmedNonce;
        }
        else
        {
            uint dbNonce = await _resiliencyLayer.GetLastSubmittedNonceAsync(cancellationToken);
            _peakNonce = Math.Max(_confirmedNonce, dbNonce + 1);
        }

        _ = Task.Run(() => AdaptiveNoncePollerAsync(_cts.Token), cancellationToken);
    }

    private Task WaitForNonceCompletionAsync(uint nonceToWaitFor)
    {
        lock(_stateLock)
        {
            if(nonceToWaitFor < _confirmedNonce)
            {
                return Task.CompletedTask;
            }

            if(_nonceGates.TryGetValue(nonceToWaitFor, out var gateTcs))
            {
                return gateTcs.Task;
            }

            gateTcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            _nonceGates[nonceToWaitFor] = gateTcs;
            return gateTcs.Task;
        }
    }

    private async Task AdaptiveNoncePollerAsync(CancellationToken cancellationToken)
    {
        bool isActive = false;

        while(!cancellationToken.IsCancellationRequested)
        {
            try
            {
                uint actualNonce = await ethRpcModule.GetTransactionCountAsync(signer.Address, TargetBlockNumber.Latest, cancellationToken);
                bool newIsActive;

                lock(_stateLock)
                {
                    if(actualNonce > _confirmedNonce)
                    {
                        for(uint i = _confirmedNonce; i < actualNonce; i++)
                        {
                            _logger?.LogInformation("Transaction with nonce {nonce} confirmed on-chain", i);

                            if(_nonceGates.Remove(i, out var tcs))
                            {
                                tcs.TrySetResult();
                            }
                        }
                        _confirmedNonce = actualNonce;
                    }

                    newIsActive = _peakNonce > _confirmedNonce;
                }

                if(isActive != newIsActive)
                {
                    _logger?.LogDebug("Switching polling mode to {mode}", isActive ? "Fast" : "Slow");
                }

                isActive = newIsActive;

                if(isActive)
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(1500), cancellationToken);
                }
                else
                {
                    await _workerSignal.WaitAsync(TimeSpan.FromSeconds(300), cancellationToken);
                }
            }
            catch(OperationCanceledException ex) when(ex.CancellationToken == cancellationToken)
            {
                break;
            }
            catch(Exception ex)
            {
                _logger?.LogError(ex, "Exception while polling nonce");
                await Task.Delay(5000, cancellationToken);
            }
        }
    }

    /// <inheritdoc/>
    public async ValueTask<IPendingTxHandler<TTxParams, TTxGasParams>> PrepareTxAsync<TTransaction, TTxParams, TTxGasParams>(
        ITxInput call, TTxParams? txParams, TTxGasParams? txGasParams,
        CancellationToken cancellationToken
    )
        where TTransaction : class, ITransaction<TTransaction, TTxParams, TTxGasParams>
        where TTxParams : class, ITxParams<TTxParams>
        where TTxGasParams : class, ITxGasParams<TTxGasParams>
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(_cts.Token, cancellationToken);
        var handler = provider.GetRequiredService<ITxTypeHandler<TTransaction, TTxParams, TTxGasParams>>();
        var gasFeeProvider = provider.GetRequiredService<IGasFeeProvider<TTxParams, TTxGasParams>>();

        await _preparationLock.WaitAsync(cts.Token);

        try
        {
            uint myNonce;
            lock(_stateLock)
            {
                myNonce = _peakNonce;
            }

            txParams ??= TTxParams.Default;
            txGasParams ??= await gasFeeProvider.EstimateGasParamsAsync(call, txParams, cts.Token);

            string encodedTx = handler.EncodeTxToBytes(call, txParams, txGasParams, myNonce, out string txHash);
            var submission = new TxSubmission<TTxParams, TTxGasParams>(_chainId, 0, txHash, encodedTx, call, txParams, txGasParams);

            lock(_stateLock)
            {
                if(_peakNonce != myNonce)
                {
                    throw new ImpossibleException();
                }
                _peakNonce++;
            }

            if(_workerSignal.CurrentCount == 0)
            {
                _workerSignal.Release();
            }

            return new PendingTxHandler<TTxParams, TTxGasParams>(
                myNonce,
                [submission],
                (ctx, onError) => ExecuteTransactionLifeCycleAsync<TTransaction, TTxParams, TTxGasParams>(ctx, onError, true)
            );
        }
        finally
        {
            _preparationLock.Release();
        }
    }

    private async Task<bool> TryCancelTransactionAsync(uint nonce, CancellationToken cancellationToken)
    {
        await _preparationLock.WaitAsync(cancellationToken);
        try
        {
            lock(_stateLock)
            {
                if(_peakNonce == nonce + 1)
                {
                    _logger?.LogWarning("Transaction with nonce {nonce} cancelled", nonce);
                    _peakNonce--;
                    _nonceGates.Remove(nonce, out _);
                    return true;
                }
                return false;
            }
        }
        finally
        {
            _preparationLock.Release();
        }
    }

    /// <inheritdoc/>
    public async Task<IPendingTxHandler<TTxParams, TTxGasParams>> AttachPendingTxAsync<TTransaction, TTxParams, TTxGasParams>(uint nonce, CancellationToken cancellationToken)
        where TTransaction : class, ITransaction<TTransaction, TTxParams, TTxGasParams>
        where TTxParams : class, ITxParams<TTxParams>
        where TTxGasParams : class, ITxGasParams<TTxGasParams>
    {
        if(_resiliencyLayer is null)
        {
            throw new NotSupportedException("Missing ResiliencyLayer");
        }

        var cts = CancellationTokenSource.CreateLinkedTokenSource(_cts.Token, cancellationToken);

        var stored = await _resiliencyLayer.FetchTxSubmissionsAsync(nonce, cts.Token);
        if(stored.Count == 0)
        {
            throw new InvalidOperationException($"No persisted tx for nonce {nonce}");
        }

        var typedSubmissions = stored
            .Select(x => x.ToTxSubmission<TTxParams, TTxGasParams>())
            .OrderByDescending(x => x.Sequence)
            .ToList();

        lock(_stateLock)
        {
            if(nonce < _confirmedNonce)
            {
                return new PendingTxHandler<TTxParams, TTxGasParams>(
                    nonce,
                    typedSubmissions,
                    (_, _) => FetchConfirmationInternal(typedSubmissions, 1)
                );
            }
        }

        return new PendingTxHandler<TTxParams, TTxGasParams>(
            nonce,
            typedSubmissions,
            (ctx, onError) => ExecuteTransactionLifeCycleAsync<TTransaction, TTxParams, TTxGasParams>(ctx, onError, false)
        );
    }

    private async Task<TxConfirmationResult> ExecuteTransactionLifeCycleAsync<TTransaction, TTxParams, TTxGasParams>(
        PendingTxHandler<TTxParams, TTxGasParams> handlerContext,
        Func<TxConfirmationError, TxConfirmationActionBuilder<TTxParams, TTxGasParams>, TxSubmission<TTxParams, TTxGasParams>, TxConfirmationAction<TTxParams, TTxGasParams>> onError,
        bool isNewHandler
    )
        where TTransaction : class, ITransaction<TTransaction, TTxParams, TTxGasParams>
        where TTxParams : class, ITxParams<TTxParams>
        where TTxGasParams : class, ITxGasParams<TTxGasParams>
    {
        var typeHandler = provider.GetRequiredService<ITxTypeHandler<TTransaction, TTxParams, TTxGasParams>>();

        if(handlerContext.Nonce > 0)
        {
            await WaitForNonceCompletionAsync(handlerContext.Nonce - 1);
        }

        if(isNewHandler && _resiliencyLayer is not null)
        {
            await _resiliencyLayer.StoreTxSubmissionAsync(handlerContext.TxSubmissions[0].ToStorageType(handlerContext.Nonce), _cts.Token);
        }

        var confirmationGateTask = WaitForNonceCompletionAsync(handlerContext.Nonce);

        TxConfirmationError? activeError = null;

        var actionBuilder = new TxConfirmationActionBuilder<TTxParams, TTxGasParams>();
        bool hasPublished = !isNewHandler;

        while(!_cts.IsCancellationRequested)
        {
            var currentSubmission = handlerContext.TxSubmissions[0];

            // --- Error Resolution Phase ---
            if(activeError is not null)
            {
                var action = onError(activeError, actionBuilder, currentSubmission);
                activeError = null;

                switch(action)
                {
                    case TxConfirmationAction<TTxParams, TTxGasParams>.ContinueWaiting waitAction:
                        var waitTask = Task.Delay(waitAction.Duration, _cts.Token);
                        var completedTask = await Task.WhenAny(confirmationGateTask, waitTask);

                        if(completedTask == confirmationGateTask)
                        {
                            return await FetchConfirmationInternal(handlerContext.TxSubmissions, 3);
                        }

                        activeError = new TxConfirmationError.Timeout();
                        continue;
                    case TxConfirmationAction<TTxParams, TTxGasParams>.CancelTransaction:
                        if(hasPublished || !await TryCancelTransactionAsync(handlerContext.Nonce, _cts.Token))
                        {
                            activeError = new TxConfirmationError.TransactionNotCancellable();
                            continue;
                        }

                        return new TxConfirmationResult.Cancelled();
                    case TxConfirmationAction<TTxParams, TTxGasParams>.MinimalGasFeeIncrement:
                    case TxConfirmationAction<TTxParams, TTxGasParams>.RepriceTransaction:
                        await ApplyRepricingAsync(action, handlerContext, typeHandler);
                        continue;
                }
            }

            // --- Publish & Monitor Phase ---
            var pubResult = await txPublisher.PublishTxAsync(currentSubmission.SignedTx, default);

            switch(pubResult)
            {
                case TxSubmissionResult.Success or TxSubmissionResult.AlreadyExists:
                {
                    hasPublished = true;
                    var timeoutTask = Task.Delay(TimeSpan.FromSeconds(30));
                    var completedTask = await Task.WhenAny(confirmationGateTask, timeoutTask);

                    if(completedTask == confirmationGateTask)
                    {
                        return await FetchConfirmationInternal(handlerContext.TxSubmissions, 3);
                    }

                    activeError = new TxConfirmationError.Timeout();
                    continue;
                }
                case TxSubmissionResult.NonceTooLow:
                    return await FetchConfirmationInternal(handlerContext.TxSubmissions, 3);
                case TxSubmissionResult.TransactionUnderpriced:
                    activeError = new TxConfirmationError.TransactionUnderpriced();
                    continue;
                case TxSubmissionResult.UnhandledException ex:
                    activeError = new TxConfirmationError.UnhandledException(ex.Exception);
                    continue;
                default:
                    throw new ImpossibleException();
            }
        }

        _cts.Token.ThrowIfCancellationRequested();
        throw new ImpossibleException();
    }

    private async Task ApplyRepricingAsync<TTransaction, TTxParams, TTxGasParams>(
        TxConfirmationAction<TTxParams, TTxGasParams> action,
        PendingTxHandler<TTxParams, TTxGasParams> context,
        ITxTypeHandler<TTransaction, TTxParams, TTxGasParams> typeHandler)
        where TTransaction : class, ITransaction<TTransaction, TTxParams, TTxGasParams>
        where TTxParams : class, ITxParams<TTxParams>
        where TTxGasParams : class, ITxGasParams<TTxGasParams>
    {
        var oldSub = context.TxSubmissions[0];

        var newGas = action is TxConfirmationAction<TTxParams, TTxGasParams>.MinimalGasFeeIncrement
            ? oldSub.GasParams.IncrementByFactor(1105, 1000, 1)
            : ((TxConfirmationAction<TTxParams, TTxGasParams>.RepriceTransaction) action).GasParams;

        string encoded = typeHandler.EncodeTxToBytes(oldSub.Call, oldSub.Params, newGas, context.Nonce, out string txHash);
        var newSub = new TxSubmission<TTxParams, TTxGasParams>(_chainId, oldSub.Sequence + 1, txHash, encoded, oldSub.Call, oldSub.Params, newGas);

        context.TxSubmissions.Insert(0, newSub);

        if(_resiliencyLayer is not null)
        {
            await _resiliencyLayer.StoreTxSubmissionAsync(newSub.ToStorageType(context.Nonce));
        }
    }

    private async Task<TxConfirmationResult> FetchConfirmationInternal<TTxParams, TTxGasParams>(
        IEnumerable<TxSubmission<TTxParams, TTxGasParams>> submissions, int retries)
        where TTxParams : class, ITxParams<TTxParams>
        where TTxGasParams : class, ITxGasParams<TTxGasParams>
    {
        foreach(var sub in submissions)
        {
            for(int i = 1; i < retries + 1; i++)
            {
                var receipt = await ethRpcModule.GetTransactionReceiptAsync(sub.TxHash);
                if(receipt is not null)
                {
                    return new TxConfirmationResult.Success(receipt);
                }
                await Task.Delay(i * 1000);
            }
        }
        return new TxConfirmationResult.NonceAlreadyUsed();
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        _cts.Cancel();
        _cts.Dispose();
        _workerSignal.Dispose();
        _preparationLock.Dispose();
        GC.SuppressFinalize(this);
    }
}