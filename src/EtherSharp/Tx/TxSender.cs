using EtherSharp.Client;
using EtherSharp.Client.Services.ResiliencyLayer;
using EtherSharp.Client.Services.TxPublisher;
using EtherSharp.Common.Extensions;
using EtherSharp.Common.Instrumentation;
using EtherSharp.RPC;
using EtherSharp.RPC.Modules.Eth;
using EtherSharp.RPC.Transport;
using EtherSharp.Tx.Nonce;
using EtherSharp.Tx.Types;
using EtherSharp.Tx.Watcher;
using EtherSharp.Types;
using EtherSharp.Wallet;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EtherSharp.Tx;

/// <summary>
/// Signer-bound transaction pipeline shared by an <see cref="IEtherTxClient"/> and the pending transactions it creates.
/// </summary>
internal sealed class TxSender : IAsyncDisposable
{
    private readonly ulong _chainId;
    private readonly IEtherSigner _signer;
    private readonly INonceAllocator _nonceAllocator;
    private readonly ITxPublisher _txPublisher;
    private readonly ITxConfirmationWatcher _txConfirmationWatcher;
    private readonly IEthRpcModule _ethRpcModule;
    private readonly RpcRetryOptions _retryOptions;
    private readonly IResiliencyLayer? _resiliencyLayer;
    private readonly OTELCounter<long>? _txPublishResultsCounter;

    private readonly CancellationTokenSource _disposalCts = new CancellationTokenSource();
    private readonly SemaphoreSlim _preparationLock = new SemaphoreSlim(1, 1);
    private volatile bool _isDisposed;

    public Address Address => _signer.Address;
    public ILogger? Logger { get; }

    // Captured eagerly, as the source's token is unavailable once it is disposed.
    public CancellationToken DisposalToken { get; }

    public TxSender(
        ulong chainId,
        IEtherSigner signer,
        INonceAllocator nonceAllocator,
        ITxPublisher txPublisher,
        ITxConfirmationWatcher txConfirmationWatcher,
        IEthRpcModule ethRpcModule,
        RpcRetryOptions retryOptions,
        IResiliencyLayer? resiliencyLayer,
        IServiceProvider provider
    )
    {
        _chainId = chainId;
        _signer = signer;
        _nonceAllocator = nonceAllocator;
        _txPublisher = txPublisher;
        _txConfirmationWatcher = txConfirmationWatcher;
        _ethRpcModule = ethRpcModule;
        _retryOptions = retryOptions;
        _resiliencyLayer = resiliencyLayer;
        _txPublishResultsCounter = provider.CreateOTELCounter<long>("tx_publish_results");
        Logger = provider.GetService<ILoggerFactory>()?.CreateLogger<EtherTxClient>();
        DisposalToken = _disposalCts.Token;
    }

    public void ThrowIfDisposed()
        => ObjectDisposedException.ThrowIf(_isDisposed, typeof(IEtherTxClient));

    public async Task<PendingTx<TTransaction, TTxParams, TTxGasParams>> PrepareAsync<TTransaction, TTxParams, TTxGasParams>(
        Guid? replacedTxId, ITxInput call, TTxParams txParams, TTxGasParams txGasParams,
        RpcRequestOptions requestOptions, CancellationToken cancellationToken
    )
        where TTransaction : class, ITransaction<TTransaction, TTxParams, TTxGasParams>
        where TTxParams : class, ITxParams<TTxParams>
        where TTxGasParams : class, ITxGasParams<TTxGasParams>
    {
        ThrowIfDisposed();
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(DisposalToken, cancellationToken);

        // Serializes nonce allocation with signing so a failed or cancelled signature can always
        // release its nonce, instead of leaving a gap below nonces already handed to concurrent callers.
        await _preparationLock.WaitAsync(linkedCts.Token);
        try
        {
            ulong nonce = await _nonceAllocator.AllocateAsync(requestOptions, linkedCts.Token);

            try
            {
                var transaction = TTransaction.Create(_chainId, nonce, call, txParams, txGasParams);
                var signedTransaction = await _signer.SignTransactionAsync(transaction, linkedCts.Token);
                var submission = new TxSubmission<TTxParams, TTxGasParams>(
                    _chainId,
                    nonce,
                    0,
                    signedTransaction.Hash,
                    signedTransaction.EncodedTx,
                    call,
                    txParams,
                    txGasParams
                );

                linkedCts.Token.ThrowIfCancellationRequested();

                bool isPersisted = false;
                var id = replacedTxId ?? Guid.NewGuid();
                if(replacedTxId is not null && _resiliencyLayer is { } resiliencyLayer)
                {
                    await resiliencyLayer.ReplaceTxSubmissionsAsync(
                        submission.ToStorageType(id, Address),
                        linkedCts.Token
                    );
                    isPersisted = true;
                }

                Logger?.Tx.Prepared(id, Address, nonce, submission.TxHash, submission.GasParams);

                return CreatePendingTx<TTransaction, TTxParams, TTxGasParams>(id, [submission], isPersisted);
            }
            catch(Exception ex)
            {
                if(!_isDisposed && !await _nonceAllocator.TryReleaseAsync(nonce, CancellationToken.None))
                {
                    Logger?.Tx.NonceNotReleased(ex, Address, nonce);
                }

                throw;
            }
        }
        finally
        {
            _ = _preparationLock.Release();
        }
    }

    public async Task<PendingTx<TTransaction, TTxParams, TTxGasParams>?> AttachAsync<TTransaction, TTxParams, TTxGasParams>(
        Guid id, CancellationToken cancellationToken
    )
        where TTransaction : class, ITransaction<TTransaction, TTxParams, TTxGasParams>
        where TTxParams : class, ITxParams<TTxParams>
        where TTxGasParams : class, ITxGasParams<TTxGasParams>
    {
        ThrowIfDisposed();

        if(_resiliencyLayer is null)
        {
            throw new NotSupportedException("Missing ResiliencyLayer");
        }

        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(DisposalToken, cancellationToken);
        var senderKey = new TxSenderKey(_chainId, Address);
        var stored = await _resiliencyLayer.FetchTxSubmissionsAsync(senderKey, id, linkedCts.Token);
        linkedCts.Token.ThrowIfCancellationRequested();

        if(stored.Count == 0)
        {
            return null;
        }

        ulong nonce = stored[0].Nonce;
        foreach(var submission in stored)
        {
            if(submission.Id != id || submission.ChainId != senderKey.ChainId || submission.Sender != senderKey.Sender)
            {
                throw new InvalidOperationException($"Persisted submissions do not match transaction {id} for the current chain and sender");
            }

            if(submission.Nonce != nonce)
            {
                throw new InvalidOperationException($"Transaction {id} has persisted submissions across multiple nonces");
            }
        }

        var typedSubmissions = new TxSubmission<TTxParams, TTxGasParams>[stored.Count];
        for(int i = 0; i < stored.Count; i++)
        {
            typedSubmissions[i] = stored[i].ToTxSubmission<TTxParams, TTxGasParams>();
        }

        Array.Sort(typedSubmissions, static (a, b) => b.Sequence.CompareTo(a.Sequence));

        Logger?.Tx.Attached(
            id, Address, nonce, typedSubmissions.Length, typedSubmissions[0].TxHash, typedSubmissions[0].GasParams
        );

        return CreatePendingTx<TTransaction, TTxParams, TTxGasParams>(id, typedSubmissions, isPersisted: true);
    }

    private PendingTx<TTransaction, TTxParams, TTxGasParams> CreatePendingTx<TTransaction, TTxParams, TTxGasParams>(
        Guid id, TxSubmission<TTxParams, TTxGasParams>[] txSubmissions, bool isPersisted
    )
        where TTransaction : class, ITransaction<TTransaction, TTxParams, TTxGasParams>
        where TTxParams : class, ITxParams<TTxParams>
        where TTxGasParams : class, ITxGasParams<TTxGasParams>
        => new PendingTx<TTransaction, TTxParams, TTxGasParams>(
            id, txSubmissions, isPersisted, this, _txConfirmationWatcher, _ethRpcModule, _retryOptions
        );

    public ValueTask<SignedTransaction> SignAsync(ITransaction transaction, CancellationToken cancellationToken)
        => _signer.SignTransactionAsync(transaction, cancellationToken);

    /// <summary>
    /// Stores the submission in the resiliency layer, returning whether one is configured.
    /// </summary>
    public async Task<bool> TryPersistAsync<TTxParams, TTxGasParams>(
        Guid id, TxSubmission<TTxParams, TTxGasParams> submission, CancellationToken cancellationToken
    )
        where TTxParams : class, ITxParams<TTxParams>
        where TTxGasParams : class, ITxGasParams<TTxGasParams>
    {
        if(_resiliencyLayer is null)
        {
            return false;
        }

        await _resiliencyLayer.StoreTxSubmissionAsync(submission.ToStorageType(id, Address), cancellationToken);
        return true;
    }

    public async Task<TxSubmissionResult> PublishAsync<TTxParams, TTxGasParams>(
        Guid id, TxSubmission<TTxParams, TTxGasParams> submission,
        RpcRequestOptions requestOptions, CancellationToken cancellationToken
    )
        where TTxParams : class, ITxParams<TTxParams>
        where TTxGasParams : class, ITxGasParams<TTxGasParams>
    {
        var result = await _txPublisher.PublishTxAsync(submission.SignedTx, requestOptions, cancellationToken);
        _txPublishResultsCounter?.Add(1, new KeyValuePair<string, object?>("outcome", result.GetType().Name));
        Logger?.Tx.PublishResult(id, Address, submission.Nonce, submission.Sequence, submission.TxHash, result);
        return result;
    }

    public async Task<bool> TryAbandonNonceAsync(
        Guid id,
        ulong nonce,
        bool deletePersistedSubmissions,
        CancellationToken cancellationToken
    )
    {
        ThrowIfDisposed();

        await _preparationLock.WaitAsync(cancellationToken);
        try
        {
            ThrowIfDisposed();

            if(!await _nonceAllocator.TryReleaseAsync(nonce, CancellationToken.None))
            {
                return false;
            }

            if(deletePersistedSubmissions && _resiliencyLayer is { } resiliencyLayer)
            {
                try
                {
                    await resiliencyLayer.DeleteTxSubmissionsAsync(
                        new TxSenderKey(_chainId, Address),
                        id,
                        CancellationToken.None
                    );
                }
                catch(Exception ex)
                {
                    Logger?.Tx.AbandonedCleanupFailed(ex, id, Address, nonce);
                }
            }

            return true;
        }
        finally
        {
            _ = _preparationLock.Release();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if(Interlocked.Exchange(ref _isDisposed, true))
        {
            return;
        }

        await _disposalCts.CancelAsync();
        _disposalCts.Dispose();

        _nonceAllocator.Dispose();
        await _txPublisher.DisposeAsync();
    }
}
