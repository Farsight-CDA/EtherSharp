using EtherSharp.RPC.Transport;
using EtherSharp.Tx.Types;
using EtherSharp.Types;
using System.Collections.Immutable;

namespace EtherSharp.Tx.PendingHandler;

/// <summary>
/// Handler class for transactions that are still pending and not confirmed on the blockchain.
/// </summary>
public sealed class PendingTxHandler<TTxParams, TTxGasParams> : IPendingTxHandler<TTxParams, TTxGasParams>, IInternalPendingTxHandler
    where TTxParams : class, ITxParams<TTxParams>
    where TTxGasParams : class, ITxGasParams<TTxGasParams>
{
    private readonly Func<
        PendingTxHandler<TTxParams, TTxGasParams>,
        Func<
            TxConfirmationError,
            TxConfirmationActionBuilder<TTxParams, TTxGasParams>,
            TxSubmission<TTxParams, TTxGasParams>,
            TxConfirmationAction<TTxParams, TTxGasParams>
        >,
        RpcRequestOptions,
        Task<TxConfirmationResult>
    > _processFunc;

    private readonly Lock _lock = new Lock();
    private readonly TaskCompletionSource<TxConfirmationResult> _completionCts
        = new TaskCompletionSource<TxConfirmationResult>();

    private ImmutableArray<TxSubmission<TTxParams, TTxGasParams>> _txSubmissions;
    private bool _isConfirmCalled;

    /// <inheritdoc/>
    public Guid Id { get; }

    /// <inheritdoc/>
    public ImmutableArray<TxSubmission<TTxParams, TTxGasParams>> TxSubmissions
    {
        get {
            lock(_lock)
            {
                return _txSubmissions;
            }
        }
    }

    internal PendingTxHandler(
        Guid id,
        ImmutableArray<TxSubmission<TTxParams, TTxGasParams>> txSubmissions,
        Func<
            PendingTxHandler<TTxParams, TTxGasParams>,
            Func<
                TxConfirmationError,
                TxConfirmationActionBuilder<TTxParams, TTxGasParams>,
                TxSubmission<TTxParams, TTxGasParams>,
                TxConfirmationAction<TTxParams, TTxGasParams>
            >,
            RpcRequestOptions,
            Task<TxConfirmationResult>
        > processFunc
    )
    {
        ArgumentNullException.ThrowIfNull(processFunc);

        if(txSubmissions.IsDefaultOrEmpty)
        {
            throw new ArgumentException("At least one transaction submission is required.", nameof(txSubmissions));
        }

        ulong nonce = txSubmissions[0].Nonce;
        for(int i = 1; i < txSubmissions.Length; i++)
        {
            if(txSubmissions[i].Nonce != nonce)
            {
                throw new ArgumentException("All submissions for a pending transaction must share the same nonce.", nameof(txSubmissions));
            }
        }

        Id = id;
        _txSubmissions = txSubmissions;
        _processFunc = processFunc;
    }

    internal void AddSubmission(TxSubmission<TTxParams, TTxGasParams> submission)
    {
        lock(_lock)
        {
            if(submission.Nonce != _txSubmissions[0].Nonce)
            {
                throw new InvalidOperationException("Replacement submission nonce must match the active transaction nonce.");
            }

            _txSubmissions = _txSubmissions.Insert(0, submission);
        }
    }

    internal void ReplaceSubmissions(TxSubmission<TTxParams, TTxGasParams> submission)
    {
        lock(_lock)
        {
            _txSubmissions = [submission];
        }
    }

    internal Bytes32[] GetSubmissionHashes()
    {
        var submissions = TxSubmissions;
        var hashes = new Bytes32[submissions.Length];
        for(int i = 0; i < submissions.Length; i++)
        {
            hashes[i] = submissions[i].TxHash;
        }

        return hashes;
    }

    async Task<TxConfirmationResult> IPendingTxHandler<TTxParams, TTxGasParams>.PublishAndConfirmAsync(
        Func<
            TxConfirmationError,
            TxConfirmationActionBuilder<TTxParams, TTxGasParams>,
            TxSubmission<TTxParams, TTxGasParams>,
            TxConfirmationAction<TTxParams, TTxGasParams>
        > onError,
        RpcRequestOptions requestOptions
    )
    {
        lock(_lock)
        {
            if(_isConfirmCalled)
            {
                throw new InvalidOperationException($"{nameof(IPendingTxHandler<,>.PublishAndConfirmAsync)} already called on this instance");
            }

            _isConfirmCalled = true;
        }

        TxConfirmationResult confirmationResult;

        try
        {
            confirmationResult = await _processFunc(this, onError, requestOptions);
        }
        catch(Exception ex)
        {
            confirmationResult = new TxConfirmationResult.UnhandledException(ex);
        }

        _completionCts.SetResult(confirmationResult);
        return confirmationResult;
    }

    Task<TxConfirmationResult> IInternalPendingTxHandler.WaitForCompletionAsync()
        => _completionCts.Task;
}
