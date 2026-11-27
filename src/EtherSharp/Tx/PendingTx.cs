using EtherSharp.Client;
using EtherSharp.Client.Services.TxPublisher;
using EtherSharp.Common.Extensions;
using EtherSharp.Numerics;
using EtherSharp.RPC;
using EtherSharp.RPC.Transport;
using EtherSharp.Tx.Types;
using EtherSharp.Tx.Watcher;
using EtherSharp.Types;
using Microsoft.Extensions.Logging;
using System.Collections.Immutable;
using System.Runtime.InteropServices;

namespace EtherSharp.Tx;

internal sealed class PendingTx<TTransaction, TTxParams, TTxGasParams> : IPendingTx<TTxParams, TTxGasParams>
    where TTransaction : class, ITransaction<TTransaction, TTxParams, TTxGasParams>
    where TTxParams : class, ITxParams<TTxParams>
    where TTxGasParams : class, ITxGasParams<TTxGasParams>
{
    private const int RECEIPT_FETCH_RETRY_COUNT = 3;

    private readonly EtherTxClient _txClient;
    private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);
    private readonly CancellationToken _clientDisposalToken;
    private readonly CancellationTokenSource _disposalCts = new CancellationTokenSource();

    private volatile ITxWatch _watch;
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

        Id = id;
        _txSubmissions = txSubmissions;
        _txClient = txClient;
        _clientDisposalToken = clientDisposalToken;
        _lastPersistedSubmission = isPersisted ? txSubmissions[0] : null;
        _watch = CreateWatch(txSubmissions, null);
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

        await _lock.WaitAsync(linkedCts.Token);
        try
        {
            ObjectDisposedException.ThrowIf(_isDisposed, this);

            var submission = LatestSubmission;
            if(_txClient.ResiliencyLayer is { } resiliencyLayer && _lastPersistedSubmission != submission)
            {
                await resiliencyLayer.StoreTxSubmissionAsync(
                    submission.ToStorageType(Id, _txClient.Signer.Address),
                    linkedCts.Token
                );
                _lastPersistedSubmission = submission;
            }

            var result = await _txClient.TxPublisher.PublishTxAsync(submission.SignedTx, requestOptions, linkedCts.Token);
            LogPublishResult(submission, result);
            return result;
        }
        finally
        {
            _ = _lock.Release();
        }
    }

    private void LogPublishResult(TxSubmission<TTxParams, TTxGasParams> submission, TxSubmissionResult result)
    {
        if(result.IsAccepted)
        {
            _txClient.Logger?.Tx.SubmissionPublished(
                Id, _txClient.Signer.Address, submission.Nonce, submission.Sequence, submission.TxHash, result.GetType().Name
            );
            return;
        }

        var level = result is TxSubmissionResult.NonceTooLow
            ? LogLevel.Information
            : LogLevel.Warning;

        _txClient.Logger?.Tx.SubmissionRejected(
            level,
            (result as TxSubmissionResult.UnhandledException)?.Exception,
            Id, _txClient.Signer.Address, submission.Nonce, submission.Sequence, submission.TxHash, result.GetType().Name
        );
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
            ObjectDisposedException.ThrowIf(_isDisposed, this);

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
            var signedTransaction = await _txClient.Signer.SignTransactionAsync(transaction, linkedCts.Token);
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

            _txClient.Logger?.Tx.Repriced(Id, _txClient.Signer.Address, submission.Nonce, submission.Sequence, submission.TxHash);

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

        while(true)
        {
            var watch = _watch;
            var inclusion = await watch.WaitAsync(linkedCts.Token);
            switch(inclusion)
            {
                case TxWatchResult.Confirmed confirmed:
                    if(await TryFetchReceiptAsync(confirmed.TransactionHash, requestOptions, linkedCts.Token) is { } receipt)
                    {
                        _txClient.Logger?.Tx.Confirmed(
                            Id, _txClient.Signer.Address, LatestSubmission.Nonce,
                            receipt.TransactionHash, receipt.BlockNumber, receipt.Success, receipt.GasUsed
                        );
                        return new TxReceiptResult.Confirmed(receipt);
                    }

                    // E.g. the including block was reorged out, so the slot is watched again.
                    _txClient.Logger?.Tx.ReceiptUnavailable(
                        Id, _txClient.Signer.Address, LatestSubmission.Nonce,
                        confirmed.TransactionHash, confirmed.BlockNumber, RECEIPT_FETCH_RETRY_COUNT
                    );
                    await RewatchAsync(watch, confirmed.BlockNumber, linkedCts.Token);
                    break;
                case TxWatchResult.Replaced replaced:
                    _txClient.Logger?.Tx.Replaced(
                        Id, _txClient.Signer.Address, LatestSubmission.Nonce, replaced.TransactionHash, replaced.BlockNumber
                    );
                    return new TxReceiptResult.Replaced(replaced.TransactionHash, replaced.BlockNumber, replaced.BlockHash);
                case TxWatchResult.NonceAlreadyUsed:
                    _txClient.Logger?.Tx.NonceAlreadyUsed(Id, _txClient.Signer.Address, LatestSubmission.Nonce);
                    return new TxReceiptResult.NonceAlreadyUsed();
            }
        }
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

    public async Task<bool> TryAbandonAsync(CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);
        ObjectDisposedException.ThrowIf(_clientDisposalToken.IsCancellationRequested, _txClient);
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
            _disposalCts.Token, _clientDisposalToken, cancellationToken
        );

        await _lock.WaitAsync(linkedCts.Token);
        try
        {
            ObjectDisposedException.ThrowIf(_isDisposed, this);

            ulong nonce = LatestSubmission.Nonce;
            if(!await _txClient.TryAbandonNonceAsync(Id, nonce, _lastPersistedSubmission is not null, linkedCts.Token))
            {
                return false;
            }

            _txClient.Logger?.Tx.Abandoned(Id, _txClient.Signer.Address, nonce);
            await DisposeAsync();
            return true;
        }
        finally
        {
            _ = _lock.Release();
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

    // Returns the receipt, or null when it stays unavailable after the retries.
    private async Task<TxReceipt?> TryFetchReceiptAsync(
        Bytes32 txHash,
        RpcRequestOptions requestOptions,
        CancellationToken cancellationToken
    )
    {
        var delay = _txClient.RetryOptions.RetryDelay;
        for(int attempt = 0; true; attempt++)
        {
            var receipt = await _txClient.EthRpcModule.GetTransactionReceiptAsync(
                in txHash, requestOptions, cancellationToken
            );
            if(receipt is not null || attempt == RECEIPT_FETCH_RETRY_COUNT)
            {
                return receipt;
            }

            // A lagging node may not serve the receipt yet.
            await Task.Delay(delay, cancellationToken);
            delay = RpcRetryOptions.NextBackoff(delay, _txClient.RetryOptions.MaxErrorBackoff);
        }
    }

    // Replaces a resolved watch whose inclusion could not be verified with a fresh watch on the same slot.
    private async Task RewatchAsync(ITxWatch staleWatch, ulong includedAtBlock, CancellationToken cancellationToken)
    {
        await _lock.WaitAsync(cancellationToken);
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            // A concurrent wait may already have replaced the stale watch.
            if(_watch != staleWatch)
            {
                return;
            }

            // The nonce was unused before the including block on any chain that shares the history up to it.
            var watch = CreateWatch(
                _txSubmissions,
                includedAtBlock > 0
                    ? includedAtBlock - 1
                    : null
                );
            _ = Interlocked.Exchange(ref _watch, watch);

            // Pairs with DisposeAsync, which flags disposal before disposing the current watch.
            if(_isDisposed)
            {
                watch.Dispose();
            }

            staleWatch.Dispose();
        }
        finally
        {
            _ = _lock.Release();
        }
    }

    private ITxWatch CreateWatch(TxSubmission<TTxParams, TTxGasParams>[] submissions, ulong? unusedAtBlock)
    {
        var hashes = new Bytes32[submissions.Length];
        for(int i = 0; i < submissions.Length; i++)
        {
            hashes[i] = submissions[i].TxHash;
        }

        return unusedAtBlock is { } block
            ? _txClient.TxConfirmationWatcher.Watch(_txClient.Signer.Address, submissions[0].Nonce, block, hashes)
            : _txClient.TxConfirmationWatcher.Watch(_txClient.Signer.Address, submissions[0].Nonce, hashes);
    }
}
