using EtherSharp.Client;
using EtherSharp.Client.Services.ResiliencyLayer;
using EtherSharp.Client.Services.TxPublisher;
using EtherSharp.Numerics;
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
    private readonly CancellationToken _clientDisposalToken;
    private readonly CancellationTokenSource _disposalCts = new CancellationTokenSource();

    private volatile TxSubmission<TTxParams, TTxGasParams>[] _txSubmissions;
    private volatile TxSubmission<TTxParams, TTxGasParams>? _lastPersistedSubmission;
    private volatile bool _isDisposed;

    public Guid Id { get; }
    public TxSubmission<TTxParams, TTxGasParams> LatestSubmission => _txSubmissions[0];

    public ImmutableArray<TxSubmission<TTxParams, TTxGasParams>> TxSubmissions
        => ImmutableCollectionsMarshal.AsImmutableArray(_txSubmissions);

    internal PendingTx(
        Guid id,
        TxSubmission<TTxParams, TTxGasParams>[] txSubmissions,
        EtherTxClient txClient,
        bool isPersisted,
        CancellationToken clientDisposalToken
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
        _clientDisposalToken = clientDisposalToken;
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
        ObjectDisposedException.ThrowIf(_clientDisposalToken.IsCancellationRequested, _txClient);
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
            _disposalCts.Token, _clientDisposalToken, cancellationToken
        );

        var submission = _txSubmissions[0];
        if(_txClient.ResiliencyLayer is { } resiliencyLayer && _lastPersistedSubmission != submission)
        {
            submission = await PersistLatestSubmissionAsync(resiliencyLayer, linkedCts.Token);
        }

        return await _txClient.TxPublisher.PublishTxAsync(submission.SignedTx, requestOptions, linkedCts.Token);
    }

    public async Task<TxSubmission<TTxParams, TTxGasParams>> RepriceAsync(
        TTxGasParams gasParams,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(gasParams);
        return await RepriceAsync(_ => gasParams, cancellationToken);
    }

    public Task<TxSubmission<TTxParams, TTxGasParams>> BumpFeesAsync(
        UInt256 multiplier, UInt256 divider, UInt256 minimumIncrement,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentOutOfRangeException.ThrowIfEqual(divider, UInt256.Zero);
        ArgumentOutOfRangeException.ThrowIfLessThan(multiplier, divider);
        return RepriceAsync(latest => latest.IncrementByFactor(multiplier, divider, minimumIncrement), cancellationToken);
    }

    private async Task<TxSubmission<TTxParams, TTxGasParams>> RepriceAsync(
        Func<TTxGasParams, TTxGasParams> getGasParams, CancellationToken cancellationToken
    )
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);
        ObjectDisposedException.ThrowIf(_clientDisposalToken.IsCancellationRequested, _txClient);
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
            _disposalCts.Token, _clientDisposalToken, cancellationToken
        );

        await _lock.WaitAsync(linkedCts.Token);
        try
        {
            var submissions = _txSubmissions;
            var latest = submissions[0];
            var gasParams = getGasParams(latest.GasParams);
            var transaction = TTransaction.Create(
                latest.ChainId,
                latest.Nonce,
                latest.Call,
                latest.Params,
                gasParams
            );
            var signedTransaction = await transaction.SignAsync(_txClient.Signer, linkedCts.Token);
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
                linkedCts.Token.ThrowIfCancellationRequested();
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

    public async Task<TxWatchResult> WaitForInclusionAsync(
        CancellationToken cancellationToken = default
    )
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);
        ObjectDisposedException.ThrowIf(_clientDisposalToken.IsCancellationRequested, _txClient);
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
            _disposalCts.Token, _clientDisposalToken, cancellationToken
        );
        return await _watch.WaitAsync(linkedCts.Token);
    }

    public async Task<TxWatchResult?> WaitForInclusionAsync(
        TimeSpan timeout,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(timeout, TimeSpan.Zero);
        ObjectDisposedException.ThrowIf(_isDisposed, this);
        ObjectDisposedException.ThrowIf(_clientDisposalToken.IsCancellationRequested, _txClient);
        cancellationToken.ThrowIfCancellationRequested();

        using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutCts.CancelAfter(timeout);

        try
        {
            return await WaitForInclusionAsync(timeoutCts.Token);
        }
        catch(OperationCanceledException) when(!cancellationToken.IsCancellationRequested && !_isDisposed && !_clientDisposalToken.IsCancellationRequested)
        {
            return null;
        }
    }

    public async Task<TxReceiptResult> WaitForReceiptAsync(
        RpcRequestOptions requestOptions = default,
        CancellationToken cancellationToken = default
    )
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);
        ObjectDisposedException.ThrowIf(_clientDisposalToken.IsCancellationRequested, _txClient);
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
            _disposalCts.Token, _clientDisposalToken, cancellationToken
        );

        var inclusion = await _watch.WaitAsync(linkedCts.Token);
        return inclusion switch
        {
            TxWatchResult.Confirmed confirmed => new TxReceiptResult.Confirmed(
                await FetchReceiptAsync(confirmed.TransactionHash, requestOptions, linkedCts.Token)
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
        ObjectDisposedException.ThrowIf(_clientDisposalToken.IsCancellationRequested, _txClient);
        cancellationToken.ThrowIfCancellationRequested();

        using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutCts.CancelAfter(timeout);

        try
        {
            return await WaitForReceiptAsync(requestOptions, timeoutCts.Token);
        }
        catch(OperationCanceledException) when(!cancellationToken.IsCancellationRequested && !_isDisposed && !_clientDisposalToken.IsCancellationRequested)
        {
            return null;
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
        _watch.Dispose();
    }

    private async Task<TxSubmission<TTxParams, TTxGasParams>> PersistLatestSubmissionAsync(
        IResiliencyLayer resiliencyLayer,
        CancellationToken cancellationToken
    )
    {
        await _lock.WaitAsync(cancellationToken);
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
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
