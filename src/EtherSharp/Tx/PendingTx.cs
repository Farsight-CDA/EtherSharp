using EtherSharp.Client;
using EtherSharp.Client.Services.ResiliencyLayer;
using EtherSharp.Client.Services.TxPublisher;
using EtherSharp.RPC;
using EtherSharp.RPC.Transport;
using EtherSharp.Tx.Types;
using EtherSharp.Tx.Watcher;
using EtherSharp.Types;
using System.Collections.Immutable;
using System.Runtime.InteropServices;

namespace EtherSharp.Tx;

internal sealed class PendingTx<TTransaction, TTxParams, TTxGasParams> : IPendingTx<TTxParams, TTxGasParams>
    where TTransaction : class, ITransaction<TTransaction, TTxParams, TTxGasParams>
    where TTxParams : class, ITxParams<TTxParams>
    where TTxGasParams : class, ITxGasParams<TTxGasParams>
{
    private readonly EtherTxClient _txClient;
    private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);
    private readonly ITxWatch _watch;

    private volatile TxSubmission<TTxParams, TTxGasParams>[] _txSubmissions;
    private volatile TxSubmission<TTxParams, TTxGasParams>? _lastPersistedSubmission;
    private volatile bool _isDisposed;

    public Guid Id { get; }
    public ulong Nonce => _txSubmissions[0].Nonce;

    public ImmutableArray<TxSubmission<TTxParams, TTxGasParams>> TxSubmissions
        => ImmutableCollectionsMarshal.AsImmutableArray(_txSubmissions);

    internal PendingTx(
        Guid id,
        TxSubmission<TTxParams, TTxGasParams>[] txSubmissions,
        EtherTxClient txClient,
        bool isPersisted
    )
    {
        ArgumentNullException.ThrowIfNull(txSubmissions);
        ArgumentNullException.ThrowIfNull(txClient);

        if(txSubmissions.Length == 0)
        {
            throw new ArgumentException("At least one transaction submission is required.", nameof(txSubmissions));
        }

        ulong nonce = txSubmissions[0].Nonce;
        Id = id;
        _txSubmissions = txSubmissions;
        _txClient = txClient;
        _lastPersistedSubmission = isPersisted ? txSubmissions[0] : null;

        var sender = txClient.Signer.Address;
        if(txSubmissions.Length == 1)
        {
            _watch = txClient.TxConfirmationWatcher.Watch(in sender, nonce, txSubmissions[0].TxHash);
        }
        else
        {
            var hashes = new Bytes32[txSubmissions.Length];
            for(int i = 0; i < txSubmissions.Length; i++)
            {
                hashes[i] = txSubmissions[i].TxHash;
            }

            _watch = txClient.TxConfirmationWatcher.Watch(in sender, nonce, hashes);
        }
    }

    public async Task<TxSubmissionResult> PublishAsync(
        RpcRequestOptions requestOptions = default,
        CancellationToken cancellationToken = default
    )
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        var submission = _txSubmissions[0];
        if(_txClient.ResiliencyLayer is { } resiliencyLayer && _lastPersistedSubmission != submission)
        {
            submission = await PersistLatestSubmissionAsync(resiliencyLayer, cancellationToken);
        }

        return await _txClient.TxPublisher.PublishTxAsync(submission.SignedTx, requestOptions, cancellationToken);
    }

    public async Task<TxSubmission<TTxParams, TTxGasParams>?> TryRepriceAsync(
        TTxGasParams gasParams,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(gasParams);
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        await _lock.WaitAsync(cancellationToken);
        try
        {
            ObjectDisposedException.ThrowIf(_isDisposed, this);

            var submissions = _txSubmissions;
            var latest = submissions[0];
            var transaction = TTransaction.Create(
                latest.ChainId,
                latest.Nonce,
                latest.Call,
                latest.Params,
                gasParams
            );
            var signedTransaction = await transaction.SignAsync(_txClient.Signer, cancellationToken);
            var submission = new TxSubmission<TTxParams, TTxGasParams>(
                latest.ChainId,
                latest.Nonce,
                latest.Sequence + 1,
                signedTransaction.Hash,
                signedTransaction.EncodedTx,
                latest.Call,
                latest.Params,
                gasParams
            );

            if(!_watch.TryAddHash(submission.TxHash))
            {
                return null;
            }

            var updatedSubmissions = new TxSubmission<TTxParams, TTxGasParams>[submissions.Length + 1];
            updatedSubmissions[0] = submission;
            Array.Copy(submissions, 0, updatedSubmissions, 1, submissions.Length);
            _txSubmissions = updatedSubmissions;

            return submission;
        }
        finally
        {
            _ = _lock.Release();
        }
    }

    public Task<TxWatchResult> WaitForInclusionAsync(
        CancellationToken cancellationToken = default
    )
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);
        return _watch.WaitAsync(cancellationToken);
    }

    public async Task<TxWatchResult?> WaitForInclusionAsync(
        TimeSpan timeout,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(timeout, TimeSpan.Zero);
        ObjectDisposedException.ThrowIf(_isDisposed, this);
        cancellationToken.ThrowIfCancellationRequested();

        using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutCts.CancelAfter(timeout);

        try
        {
            return await _watch.WaitAsync(timeoutCts.Token);
        }
        catch(OperationCanceledException) when(!cancellationToken.IsCancellationRequested)
        {
            return null;
        }
    }

    public async Task<TxReceiptResult> WaitForReceiptAsync(
        RpcRequestOptions requestOptions = default,
        CancellationToken cancellationToken = default
    )
    {
        var inclusion = await WaitForInclusionAsync(cancellationToken);
        return inclusion switch
        {
            TxWatchResult.Confirmed confirmed => new TxReceiptResult.Confirmed(
                await FetchReceiptAsync(confirmed.TransactionHash, requestOptions, cancellationToken)
            ),
            TxWatchResult.Replaced replaced => new TxReceiptResult.Replaced(
                replaced.TransactionHash, replaced.BlockNumber, replaced.BlockHash
            ),
            TxWatchResult.NonceAlreadyUsed => new TxReceiptResult.NonceAlreadyUsed(),
        };
    }

    public async Task<TxReceiptResult?> WaitForReceiptAsync(
        TimeSpan timeout,
        RpcRequestOptions requestOptions = default,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(timeout, TimeSpan.Zero);
        ObjectDisposedException.ThrowIf(_isDisposed, this);
        cancellationToken.ThrowIfCancellationRequested();

        using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutCts.CancelAfter(timeout);

        try
        {
            return await WaitForReceiptAsync(requestOptions, timeoutCts.Token);
        }
        catch(OperationCanceledException) when(!cancellationToken.IsCancellationRequested)
        {
            return null;
        }
    }

    public async ValueTask DisposeAsync()
    {
        if(_isDisposed)
        {
            return;
        }

        await _lock.WaitAsync();
        try
        {
            if(_isDisposed)
            {
                return;
            }

            _isDisposed = true;
            _watch.Dispose();
        }
        finally
        {
            _ = _lock.Release();
        }
    }

    private async Task<TxSubmission<TTxParams, TTxGasParams>> PersistLatestSubmissionAsync(
        IResiliencyLayer resiliencyLayer,
        CancellationToken cancellationToken
    )
    {
        await _lock.WaitAsync(cancellationToken);
        try
        {
            ObjectDisposedException.ThrowIf(_isDisposed, this);
            var submission = _txSubmissions[0];

            if(_lastPersistedSubmission != submission)
            {
                await resiliencyLayer.StoreTxSubmissionAsync(
                    submission.ToStorageType(Id, _txClient.Signer.Address),
                    cancellationToken
                );
                _lastPersistedSubmission = submission;
            }

            return submission;
        }
        finally
        {
            _ = _lock.Release();
        }
    }

    private async Task<TxReceipt> FetchReceiptAsync(
        Bytes32 txHash,
        RpcRequestOptions requestOptions,
        CancellationToken cancellationToken
    )
    {
        var delay = _txClient.RetryOptions.RetryDelay;
        while(true)
        {
            ObjectDisposedException.ThrowIf(_isDisposed, this);

            var receipt = await _txClient.EthRpcModule.GetTransactionReceiptAsync(
                in txHash, requestOptions, cancellationToken
            );
            if(receipt is not null)
            {
                return receipt;
            }

            await Task.Delay(delay, cancellationToken);
            delay = RpcRetryOptions.NextBackoff(delay, _txClient.RetryOptions.MaxErrorBackoff);
        }
    }
}
