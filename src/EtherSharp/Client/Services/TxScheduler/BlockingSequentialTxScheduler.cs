using EtherSharp.Client.Services.GasFeeProvider;
using EtherSharp.Client.Services.ResiliencyLayer;
using EtherSharp.Client.Services.TxPublisher;
using EtherSharp.Common.Exceptions;
using EtherSharp.RPC.Modules.Eth;
using EtherSharp.RPC.Transport;
using EtherSharp.Tx;
using EtherSharp.Tx.Nonce;
using EtherSharp.Tx.PendingHandler;
using EtherSharp.Tx.Types;
using EtherSharp.Tx.Watcher;
using EtherSharp.Types;
using EtherSharp.Wallet;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Immutable;
using System.Runtime.InteropServices;

namespace EtherSharp.Client.Services.TxScheduler;

/// <summary>
/// Schedules transactions sequentially by nonce and drives their lifecycle until confirmation.
/// </summary>
internal sealed class BlockingSequentialTxScheduler : IAsyncDisposable
{
    private readonly IServiceProvider _provider;
    private readonly IEthRpcModule _ethRpcModule;
    private readonly IEtherSigner _signer;
    private readonly ITxPublisher _txPublisher;
    private readonly ITxConfirmationWatcher _txConfirmationWatcher;
    private readonly INonceAllocator _nonceAllocator;
    private readonly INonceSubscription _nonceSubscription;
    private readonly IResiliencyLayer? _resiliencyLayer;
    private readonly ILogger? _logger;
    private readonly TxSenderKey _senderKey;
    private readonly TimeSpan _confirmationTimeout;

    private readonly CancellationTokenSource _cts = new CancellationTokenSource();
    private readonly Lock _stateLock = new Lock();
    private readonly SemaphoreSlim _preparationLock = new SemaphoreSlim(1);
    private readonly Dictionary<ulong, TaskCompletionSource> _nonceGates = [];

    private ulong _confirmedNonce;

    /// <summary>
    /// Creates a new <see cref="BlockingSequentialTxScheduler"/>.
    /// </summary>
    public BlockingSequentialTxScheduler(
        IServiceProvider provider,
        IEthRpcModule ethRpcModule,
        IEtherSigner signer,
        ITxPublisher txPublisher,
        INonceWatcher nonceWatcher,
        ITxConfirmationWatcher txConfirmationWatcher,
        INonceAllocator nonceAllocator,
        ulong chainId,
        ulong confirmedNonce,
        TimeSpan confirmationTimeout
    )
    {
        _provider = provider;
        _ethRpcModule = ethRpcModule;
        _signer = signer;
        _txPublisher = txPublisher;
        _txConfirmationWatcher = txConfirmationWatcher;
        _nonceAllocator = nonceAllocator;
        _resiliencyLayer = provider.GetService<IResiliencyLayer>();
        _logger = provider.GetService<ILoggerFactory>()?.CreateLogger<BlockingSequentialTxScheduler>();
        _senderKey = new TxSenderKey(chainId, signer.Address);
        _confirmedNonce = confirmedNonce;
        _confirmationTimeout = confirmationTimeout;

        _nonceSubscription = nonceWatcher.Subscribe(signer.Address, TimeSpan.Zero, confirmedNonce);
        _ = Task.Run(ObserveConfirmedNoncesAsync, CancellationToken.None);
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

    private async Task ObserveConfirmedNoncesAsync()
    {
        await foreach(var (observedNonce, _) in _nonceSubscription.ListenAsync())
        {
            lock(_stateLock)
            {
                AdvanceConfirmedNonceLocked(observedNonce);
            }
        }
    }

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
            txParams ??= TTxParams.Default;
            txGasParams ??= await _provider.GetRequiredService<IGasFeeProvider<TTxParams, TTxGasParams>>()
                .EstimateGasParamsAsync(
                    call, txParams, _signer.Address, requestOptions, cts.Token
                );

            ulong myNonce = await _nonceAllocator.AllocateAsync(requestOptions, cts.Token);
            TxSubmission<TTxParams, TTxGasParams> submission;

            try
            {
                submission = await CreateSubmissionAsync<TTransaction, TTxParams, TTxGasParams>(
                    myNonce,
                    0,
                    call,
                    txParams,
                    txGasParams,
                    cts.Token
                );
            }
            catch
            {
                _ = await _nonceAllocator.TryReleaseAsync(myNonce, CancellationToken.None);
                throw;
            }

            return new PendingTxHandler<TTxParams, TTxGasParams>(
                Guid.NewGuid(),
                [submission],
                (ctx, onError, publishRequestOptions) => ExecuteTransactionLifeCycleAsync<TTransaction, TTxParams, TTxGasParams>(
                    ctx, onError, true, publishRequestOptions
                )
            );
        }
        finally
        {
            _ = _preparationLock.Release();
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
        var transaction = TTransaction.Create(_senderKey.ChainId, nonce, call, txParams, gasParams);
        var signedTransaction = await transaction.SignAsync(_signer, cancellationToken);
        return new TxSubmission<TTxParams, TTxGasParams>(
            _senderKey.ChainId, nonce, sequence, signedTransaction.Hash, signedTransaction.EncodedTx, call, txParams, gasParams
        );
    }

    private async Task<bool> TryCancelTransactionAsync(Guid id, ulong nonce, CancellationToken cancellationToken)
    {
        await _preparationLock.WaitAsync(cancellationToken);
        try
        {
            if(!await _nonceAllocator.TryReleaseAsync(nonce, cancellationToken))
            {
                return false;
            }

            if(_resiliencyLayer is not null)
            {
                await _resiliencyLayer.DeleteTxSubmissionsAsync(_senderKey, id, cancellationToken);
            }

            lock(_stateLock)
            {
                _logger?.LogWarning("Transaction with nonce {nonce} cancelled", nonce);
                _ = _nonceGates.Remove(nonce, out _);
                return true;
            }
        }
        finally
        {
            _ = _preparationLock.Release();
        }
    }

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

        var stored = await _resiliencyLayer.FetchTxSubmissionsAsync(_senderKey, id, cts.Token);
        if(stored.Count == 0)
        {
            throw new InvalidOperationException($"No persisted tx for ID {id}");
        }

        ulong nonce = stored[0].Nonce;
        foreach(var submission in stored)
        {
            if(submission.Id != id || submission.ChainId != _senderKey.ChainId || submission.Sender != _senderKey.Sender)
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
            .ToImmutableArray();

        lock(_stateLock)
        {
            if(nonce < _confirmedNonce)
            {
                return new PendingTxHandler<TTxParams, TTxGasParams>(
                    id,
                    typedSubmissions,
                    async (_, _, requestOptions) =>
                    {
                        Bytes32[] hashes = [.. typedSubmissions.Select(x => x.TxHash)];
                        using var watch = _txConfirmationWatcher.Watch(_senderKey.Sender, nonce, hashes);
                        var watchResult = await watch.WaitAsync(_cts.Token);
                        return await CompleteFromWatchAsync(watchResult, nonce, requestOptions);
                    }
                );
            }
        }

        return new PendingTxHandler<TTxParams, TTxGasParams>(
            id,
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
        ulong nonce = handlerContext.TxSubmissions[0].Nonce;
        if(nonce > 0)
        {
            await WaitForNonceCompletionAsync(nonce - 1);
        }

        if(isNewHandler && _resiliencyLayer is not null)
        {
            await _resiliencyLayer.StoreTxSubmissionAsync(
                handlerContext.TxSubmissions[0].ToStorageType(handlerContext.Id, _senderKey.Sender),
                _cts.Token
            );
        }

        var initialHashes = handlerContext.GetSubmissionHashes();
        using var watch = _txConfirmationWatcher.Watch(_senderKey.Sender, nonce, initialHashes);
        var watchTask = watch.WaitAsync(_cts.Token);

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
                        var completedTask = await Task.WhenAny(watchTask, waitTask);

                        if(completedTask == watchTask)
                        {
                            return await CompleteFromWatchAsync(await watchTask, currentSubmission.Nonce, requestOptions);
                        }

                        activeError = new TxConfirmationError.Timeout();
                        continue;
                    case TxConfirmationAction<TTxParams, TTxGasParams>.CancelTransaction:
                        if(hasPublished || !await TryCancelTransactionAsync(handlerContext.Id, currentSubmission.Nonce, _cts.Token))
                        {
                            activeError = new TxConfirmationError.TransactionNotCancellable();
                            continue;
                        }

                        return new TxConfirmationResult.Cancelled();
                    case TxConfirmationAction<TTxParams, TTxGasParams>.MinimalGasFeeIncrement:
                        await ApplyRepricingAsync<TTransaction, TTxParams, TTxGasParams>(
                            currentSubmission.GasParams.IncrementByFactor(1105, 1000, 1), handlerContext, watch
                        );
                        continue;
                    case TxConfirmationAction<TTxParams, TTxGasParams>.RepriceTransaction reprice:
                        await ApplyRepricingAsync<TTransaction, TTxParams, TTxGasParams>(reprice.GasParams, handlerContext, watch);
                        continue;
                }
            }

            // --- Publish & Monitor Phase ---
            var pubResult = await _txPublisher.PublishTxAsync(
                currentSubmission.SignedTx, requestOptions, _cts.Token
            );

            switch(pubResult)
            {
                case TxSubmissionResult.Success or TxSubmissionResult.AlreadyExists:
                {
                    hasPublished = true;
                    var timeoutTask = Task.Delay(_confirmationTimeout, _cts.Token);
                    var completedTask = await Task.WhenAny(watchTask, timeoutTask);

                    if(completedTask == watchTask)
                    {
                        return await CompleteFromWatchAsync(await watchTask, currentSubmission.Nonce, requestOptions);
                    }

                    activeError = new TxConfirmationError.Timeout();
                    continue;
                }
                case TxSubmissionResult.NonceTooLow:
                    return await CompleteFromWatchAsync(await watchTask, currentSubmission.Nonce, requestOptions);
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
        PendingTxHandler<TTxParams, TTxGasParams> context,
        ITxWatch watch
    )
        where TTransaction : class, ITransaction<TTransaction, TTxParams, TTxGasParams>
        where TTxParams : class, ITxParams<TTxParams>
        where TTxGasParams : class, ITxGasParams<TTxGasParams>
    {
        var oldSub = context.TxSubmissions[0];

        var newSub = await CreateSubmissionAsync<TTransaction, TTxParams, TTxGasParams>(
            oldSub.Nonce,
            oldSub.Sequence + 1,
            oldSub.Call,
            oldSub.Params,
            gasParams,
            _cts.Token
        );

        _ = watch.TryAddHash(newSub.TxHash);
        context.AddSubmission(newSub);

        if(_resiliencyLayer is not null)
        {
            await _resiliencyLayer.StoreTxSubmissionAsync(
                newSub.ToStorageType(context.Id, _senderKey.Sender),
                _cts.Token
            );
        }
    }

    private async Task<TxConfirmationResult> CompleteFromWatchAsync(
        TxWatchResult watchResult,
        ulong nonce,
        RpcRequestOptions requestOptions
    )
    {
        lock(_stateLock)
        {
            AdvanceConfirmedNonceLocked(nonce + 1);
        }

        if(watchResult is not TxWatchResult.Confirmed(var txHash, _, _))
        {
            return new TxConfirmationResult.NonceAlreadyUsed();
        }

        for(int i = 1; i <= 3; i++)
        {
            var receipt = await _ethRpcModule.GetTransactionReceiptAsync(
                in txHash, requestOptions, _cts.Token
            );
            if(receipt is not null)
            {
                return new TxConfirmationResult.Success(receipt);
            }

            await Task.Delay(i * 1000, _cts.Token);
        }

        return new TxConfirmationResult.NonceAlreadyUsed();
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        _cts.Cancel();
        _nonceSubscription.Dispose();
        _nonceAllocator.Dispose();
        await _txPublisher.DisposeAsync();
        _cts.Dispose();
        _preparationLock.Dispose();
        GC.SuppressFinalize(this);
    }
}
