using EtherSharp.Client.Services.GasFeeProvider;
using EtherSharp.Client.Services.ResiliencyLayer;
using EtherSharp.Client.Services.TxPublisher;
using EtherSharp.Common.Exceptions;
using EtherSharp.RPC.Modules.Eth;
using EtherSharp.RPC.Transport;
using EtherSharp.Tx;
using EtherSharp.Tx.PendingHandler;
using EtherSharp.Tx.Types;
using EtherSharp.Types;
using EtherSharp.Wallet;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Runtime.InteropServices;

namespace EtherSharp.Client.Services.TxScheduler;

/// <summary>
/// Schedules transactions sequentially by nonce and drives their lifecycle until confirmation.
/// </summary>
/// <param name="provider">Service provider used to resolve gas fee providers and optional services.</param>
/// <param name="ethRpcModule">RPC module used for nonce, receipt, and chain state queries.</param>
/// <param name="signer">Signer representing the sender account for scheduled transactions.</param>
/// <param name="txPublisher">Publisher used to submit signed transactions to the network.</param>
/// <param name="options">Configuration for nonce synchronization and transaction confirmation behavior.</param>
public sealed class BlockingSequentialTxScheduler(
    IServiceProvider provider,
    IEthRpcModule ethRpcModule,
    IEtherSigner signer,
    ITxPublisher txPublisher,
    BlockingSequentialTxScheduler.Options options
) : ITxScheduler, IInitializableService, IDisposable
{
    /// <summary>
    /// Configuration for <see cref="BlockingSequentialTxScheduler"/>.
    /// </summary>
    public sealed class Options
    {
        /// <summary>
        /// Controls how nonce allocation is synchronized with the node.
        /// </summary>
        public NonceMode NonceMode { get; set; } = NonceMode.ExclusiveLocal;

        /// <summary>
        /// Time to wait for confirmation after publishing before invoking the error callback with a timeout.
        /// </summary>
        public TimeSpan ConfirmationTimeout { get; set; } = TimeSpan.FromSeconds(30);

        internal TimeSpan BackgroundSyncInterval { get; init; } = TimeSpan.FromSeconds(300);
    }

    private readonly IResiliencyLayer? _resiliencyLayer = provider.GetService<IResiliencyLayer>();
    private readonly ILogger? _logger = provider.GetService<ILoggerFactory>()?.CreateLogger<BlockingSequentialTxScheduler>();
    private readonly NonceMode _nonceMode = options.NonceMode;
    private readonly TimeSpan _confirmationTimeout = options.ConfirmationTimeout;
    private readonly TimeSpan _backgroundSyncInterval = options.BackgroundSyncInterval;

    private readonly CancellationTokenSource _cts = new CancellationTokenSource();

    private readonly Lock _stateLock = new Lock();
    private readonly SemaphoreSlim _preparationLock = new SemaphoreSlim(1);

    private readonly SemaphoreSlim _workerSignal = new SemaphoreSlim(0);

    private TxSenderKey? _senderKey;

    private ulong _confirmedNonce;
    private ulong _peakNonce;

    private readonly Dictionary<ulong, TaskCompletionSource> _nonceGates = [];

    private TxSenderKey SenderKey => _senderKey ?? throw new InvalidOperationException("Not initialized");

    /// <inheritdoc/>
    public async ValueTask InitializeAsync(
        ulong chainId, RpcRequestOptions requestOptions, CancellationToken cancellationToken
    )
    {
        var senderKey = new TxSenderKey(chainId, signer.Address);
        _confirmedNonce = await ethRpcModule.GetTransactionCountAsync(
            senderKey.Sender, TargetHeight.Latest, requestOptions, cancellationToken
        );

        if(_resiliencyLayer is null)
        {
            _peakNonce = _confirmedNonce;
        }
        else
        {
            ulong? dbNonce = await _resiliencyLayer.GetLastSubmittedNonceAsync(senderKey, cancellationToken);

            _peakNonce = dbNonce switch
            {
                ulong nonce => Math.Max(nonce + 1, _confirmedNonce),
                _ => _confirmedNonce
            };
        }

        _senderKey = senderKey;
        _ = Task.Run(() => AdaptiveNoncePollerAsync(_cts.Token), cancellationToken);
    }

    private void AdvanceConfirmedNonceLocked(ulong actualNonce)
    {
        if(actualNonce <= _confirmedNonce)
        {
            return;
        }

        for(ulong i = _confirmedNonce; i < actualNonce; i++)
        {
            if(_logger?.IsEnabled(LogLevel.Information) == true)
            {
                _logger.LogInformation("Transaction with nonce {nonce} confirmed on-chain", i);
            }

            if(_nonceGates.Remove(i, out var tcs))
            {
                tcs.TrySetResult();
            }
        }

        _confirmedNonce = actualNonce;

        if(_peakNonce < _confirmedNonce)
        {
            _peakNonce = _confirmedNonce;
        }
    }

    private Task WaitForNonceCompletionAsync(ulong nonceToWaitFor)
    {
        lock(_stateLock)
        {
            if(nonceToWaitFor < _confirmedNonce)
            {
                return Task.CompletedTask;
            }

            ref var gateTcs = ref CollectionsMarshal.GetValueRefOrAddDefault(_nonceGates, nonceToWaitFor, out _);
            return (gateTcs ??= new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously)).Task;
        }
    }

    private async Task AdaptiveNoncePollerAsync(CancellationToken cancellationToken)
    {
        while(!cancellationToken.IsCancellationRequested)
        {
            try
            {
                bool isActive;

                lock(_stateLock)
                {
                    isActive = _peakNonce > _confirmedNonce;
                }

                if(!isActive)
                {
                    if(_nonceMode != NonceMode.BackgroundSync)
                    {
                        await _workerSignal.WaitAsync(cancellationToken);
                        continue;
                    }

                    if(await _workerSignal.WaitAsync(_backgroundSyncInterval, cancellationToken))
                    {
                        continue;
                    }
                }

                ulong actualNonce = await ethRpcModule.GetTransactionCountAsync(
                    signer.Address, TargetHeight.Latest, default, cancellationToken
                );

                lock(_stateLock)
                {
                    AdvanceConfirmedNonceLocked(actualNonce);
                }

                if(isActive)
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(1500), cancellationToken);
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
        RpcRequestOptions requestOptions = default, CancellationToken cancellationToken = default
    )
        where TTransaction : class, ITransaction<TTransaction, TTxParams, TTxGasParams>
        where TTxParams : class, ITxParams<TTxParams>
        where TTxGasParams : class, ITxGasParams<TTxGasParams>
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(_cts.Token, cancellationToken);

        await _preparationLock.WaitAsync(cts.Token);

        try
        {
            if(_nonceMode == NonceMode.RefreshOnAllocate)
            {
                ulong latestNonce = await ethRpcModule.GetTransactionCountAsync(
                    signer.Address, TargetHeight.Latest, requestOptions, cts.Token
                );

                lock(_stateLock)
                {
                    if(_peakNonce < latestNonce)
                    {
                        _peakNonce = latestNonce;
                    }
                }
            }

            ulong myNonce;
            lock(_stateLock)
            {
                myNonce = _peakNonce;
            }

            txParams ??= TTxParams.Default;
            txGasParams ??= await provider.GetRequiredService<IGasFeeProvider<TTxParams, TTxGasParams>>()
                .EstimateGasParamsAsync(
                    call, txParams, signer.Address, requestOptions, cts.Token
                );

            var submission = await CreateSubmissionAsync<TTransaction, TTxParams, TTxGasParams>(
                myNonce,
                0,
                call,
                txParams,
                txGasParams,
                cts.Token
            );

            lock(_stateLock)
            {
                if(_peakNonce > myNonce)
                {
                    throw new ImpossibleException();
                }

                _peakNonce = myNonce + 1;
            }

            if(_workerSignal.CurrentCount == 0)
            {
                _workerSignal.Release();
            }

            return new PendingTxHandler<TTxParams, TTxGasParams>(
                Guid.NewGuid(),
                myNonce,
                [submission],
                (ctx, onError, publishRequestOptions) => ExecuteTransactionLifeCycleAsync<TTransaction, TTxParams, TTxGasParams>(
                    ctx, onError, true, publishRequestOptions
                )
            );
        }
        finally
        {
            _preparationLock.Release();
        }
    }

    private async ValueTask<TxSubmission<TTxParams, TTxGasParams>> CreateSubmissionAsync<TTransaction, TTxParams, TTxGasParams>(
        ulong nonce, uint sequence, ITxInput call, TTxParams txParams, TTxGasParams gasParams,
        CancellationToken cancellationToken
    )
        where TTransaction : class, ITransaction<TTransaction, TTxParams, TTxGasParams>
        where TTxParams : class, ITxParams<TTxParams>
        where TTxGasParams : class, ITxGasParams<TTxGasParams>
    {
        var transaction = TTransaction.Create(SenderKey.ChainId, nonce, call, txParams, gasParams);
        var signedTransaction = await transaction.SignAsync(signer, cancellationToken);
        return new TxSubmission<TTxParams, TTxGasParams>(
            SenderKey.ChainId, sequence, signedTransaction.Hash, signedTransaction.EncodedTx, call, txParams, gasParams
        );
    }

    private async Task<bool> TryCancelTransactionAsync(Guid id, ulong nonce, CancellationToken cancellationToken)
    {
        await _preparationLock.WaitAsync(cancellationToken);
        try
        {
            lock(_stateLock)
            {
                if(_peakNonce != nonce + 1)
                {
                    return false;
                }
            }

            if(_resiliencyLayer is not null)
            {
                await _resiliencyLayer.DeleteTxSubmissionsAsync(SenderKey, id, cancellationToken);
            }

            lock(_stateLock)
            {
                if(_peakNonce != nonce + 1)
                {
                    return false;
                }

                _logger?.LogWarning("Transaction with nonce {nonce} cancelled", nonce);
                _peakNonce--;
                _nonceGates.Remove(nonce, out _);
                return true;
            }
        }
        finally
        {
            _preparationLock.Release();
        }
    }

    /// <inheritdoc/>
    public async Task<IPendingTxHandler<TTxParams, TTxGasParams>> AttachPendingTxAsync<TTransaction, TTxParams, TTxGasParams>(
        Guid id, CancellationToken cancellationToken = default
    )
        where TTransaction : class, ITransaction<TTransaction, TTxParams, TTxGasParams>
        where TTxParams : class, ITxParams<TTxParams>
        where TTxGasParams : class, ITxGasParams<TTxGasParams>
    {
        if(_resiliencyLayer is null)
        {
            throw new NotSupportedException("Missing ResiliencyLayer");
        }

        using var cts = CancellationTokenSource.CreateLinkedTokenSource(_cts.Token, cancellationToken);

        var stored = await _resiliencyLayer.FetchTxSubmissionsAsync(SenderKey, id, cts.Token);
        if(stored.Count == 0)
        {
            throw new InvalidOperationException($"No persisted tx for ID {id}");
        }

        ulong nonce = stored[0].Nonce;
        foreach(var submission in stored)
        {
            if(submission.Id != id || submission.ChainId != SenderKey.ChainId || submission.Sender != SenderKey.Sender)
            {
                throw new InvalidOperationException($"Persisted submissions do not match transaction {id} for the current chain and sender");
            }

            if(submission.Nonce != nonce)
            {
                throw new InvalidOperationException($"Transaction {id} has multiple nonces and cannot be attached to the sequential scheduler");
            }
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
                    id,
                    nonce,
                    typedSubmissions,
                    (_, _, requestOptions) => FetchConfirmationInternal(typedSubmissions, 1, requestOptions)
                );
            }
        }

        if(_workerSignal.CurrentCount == 0)
        {
            _workerSignal.Release();
        }

        return new PendingTxHandler<TTxParams, TTxGasParams>(
            id,
            nonce,
            typedSubmissions,
            (ctx, onError, requestOptions) => ExecuteTransactionLifeCycleAsync<TTransaction, TTxParams, TTxGasParams>(
                ctx, onError, false, requestOptions
            )
        );
    }

    private async Task<TxConfirmationResult> ExecuteTransactionLifeCycleAsync<TTransaction, TTxParams, TTxGasParams>(
        PendingTxHandler<TTxParams, TTxGasParams> handlerContext,
        Func<TxConfirmationError, TxConfirmationActionBuilder<TTxParams, TTxGasParams>, TxSubmission<TTxParams, TTxGasParams>, TxConfirmationAction<TTxParams, TTxGasParams>> onError,
        bool isNewHandler, RpcRequestOptions requestOptions
    )
        where TTransaction : class, ITransaction<TTransaction, TTxParams, TTxGasParams>
        where TTxParams : class, ITxParams<TTxParams>
        where TTxGasParams : class, ITxGasParams<TTxGasParams>
    {
        if(handlerContext.Nonce > 0)
        {
            await WaitForNonceCompletionAsync(handlerContext.Nonce - 1);
        }

        if(isNewHandler && _resiliencyLayer is not null)
        {
            await _resiliencyLayer.StoreTxSubmissionAsync(
                handlerContext.TxSubmissions[0].ToStorageType(handlerContext.Id, handlerContext.Nonce, SenderKey.Sender),
                _cts.Token
            );
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
                            return await FetchConfirmationInternal(handlerContext.TxSubmissions, 3, requestOptions);
                        }

                        activeError = new TxConfirmationError.Timeout();
                        continue;
                    case TxConfirmationAction<TTxParams, TTxGasParams>.CancelTransaction:
                        if(hasPublished || !await TryCancelTransactionAsync(handlerContext.Id, handlerContext.Nonce, _cts.Token))
                        {
                            activeError = new TxConfirmationError.TransactionNotCancellable();
                            continue;
                        }

                        return new TxConfirmationResult.Cancelled();
                    case TxConfirmationAction<TTxParams, TTxGasParams>.MinimalGasFeeIncrement:
                        await ApplyRepricingAsync<TTransaction, TTxParams, TTxGasParams>(
                            currentSubmission.GasParams.IncrementByFactor(1105, 1000, 1), handlerContext
                        );
                        continue;
                    case TxConfirmationAction<TTxParams, TTxGasParams>.RepriceTransaction reprice:
                        await ApplyRepricingAsync<TTransaction, TTxParams, TTxGasParams>(reprice.GasParams, handlerContext);
                        continue;
                }
            }

            // --- Publish & Monitor Phase ---
            var pubResult = await txPublisher.PublishTxAsync(
                currentSubmission.SignedTx, requestOptions, _cts.Token
            );

            switch(pubResult)
            {
                case TxSubmissionResult.Success or TxSubmissionResult.AlreadyExists:
                {
                    hasPublished = true;
                    var timeoutTask = Task.Delay(_confirmationTimeout);
                    var completedTask = await Task.WhenAny(confirmationGateTask, timeoutTask);

                    if(completedTask == confirmationGateTask)
                    {
                        return await FetchConfirmationInternal(handlerContext.TxSubmissions, 3, requestOptions);
                    }

                    activeError = new TxConfirmationError.Timeout();
                    continue;
                }
                case TxSubmissionResult.NonceTooLow:
                    return await FetchConfirmationInternal(handlerContext.TxSubmissions, 3, requestOptions);
                case TxSubmissionResult.TransactionUnderpriced:
                    activeError = new TxConfirmationError.TransactionUnderpriced();
                    continue;
                case TxSubmissionResult.InsufficientFunds:
                    activeError = new TxConfirmationError.InsufficientFunds();
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
        TTxGasParams gasParams,
        PendingTxHandler<TTxParams, TTxGasParams> context
    )
        where TTransaction : class, ITransaction<TTransaction, TTxParams, TTxGasParams>
        where TTxParams : class, ITxParams<TTxParams>
        where TTxGasParams : class, ITxGasParams<TTxGasParams>
    {
        var oldSub = context.TxSubmissions[0];

        var newSub = await CreateSubmissionAsync<TTransaction, TTxParams, TTxGasParams>(
            context.Nonce,
            oldSub.Sequence + 1,
            oldSub.Call,
            oldSub.Params,
            gasParams,
            _cts.Token
        );

        context.TxSubmissions.Insert(0, newSub);

        if(_resiliencyLayer is not null)
        {
            await _resiliencyLayer.StoreTxSubmissionAsync(
                newSub.ToStorageType(context.Id, context.Nonce, SenderKey.Sender)
            );
        }
    }

    private async Task<TxConfirmationResult> FetchConfirmationInternal<TTxParams, TTxGasParams>(
        IEnumerable<TxSubmission<TTxParams, TTxGasParams>> submissions,
        int retries, RpcRequestOptions requestOptions
    )
        where TTxParams : class, ITxParams<TTxParams>
        where TTxGasParams : class, ITxGasParams<TTxGasParams>
    {
        foreach(var sub in submissions)
        {
            for(int i = 1; i < retries + 1; i++)
            {
                var receipt = await ethRpcModule.GetTransactionReceiptAsync(
                    sub.TxHash, requestOptions, _cts.Token
                );
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
