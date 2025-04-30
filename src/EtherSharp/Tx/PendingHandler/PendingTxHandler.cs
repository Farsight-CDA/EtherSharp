using EtherSharp.Tx.Types;

namespace EtherSharp.Tx.PendingHandler;
/// <summary>
/// Handler class for transactions that are still pending and not confirmed on the blockchain.
/// </summary>
public class PendingTxHandler<TTxParams, TTxGasParams>(
    uint nonce,
    IEnumerable<TxSubmission<TTxParams, TTxGasParams>> txSubmissions,
    Func<
        PendingTxHandler<TTxParams, TTxGasParams>,
        Func<
            TxConfirmationError,
            TxConfirmationActionBuilder<TTxParams, TTxGasParams>,
            TxSubmission<TTxParams, TTxGasParams>,
            TxConfirmationAction<TTxParams, TTxGasParams>
        >,
        Task<TxConfirmationResult>
    > processFunc
) : IPendingTxHandler<TTxParams, TTxGasParams>, IInternalPendingTxHandler
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
        Task<TxConfirmationResult>
    > _processFunc = processFunc;

    private readonly Lock _isConfirmCalledLock = new Lock();
    private bool _isConfirmCalled;
    private readonly TaskCompletionSource _completionCts
        = new TaskCompletionSource();

    /// <summary>
    /// Direct access to underlying TxSubmissions list, meant for use in TxScheduler only.
    /// </summary>
    public readonly List<TxSubmission<TTxParams, TTxGasParams>> TxSubmissions = [.. txSubmissions];

    /// <inheritdoc/>
    public uint Nonce { get; } = nonce;
    /// <inheritdoc/>
    IReadOnlyList<TxSubmission<TTxParams, TTxGasParams>> IPendingTxHandler<TTxParams, TTxGasParams>.TxSubmissions => [.. TxSubmissions];

    async Task<TxConfirmationResult> IPendingTxHandler<TTxParams, TTxGasParams>.PublishAndConfirmAsync(
        Func<
            TxConfirmationError,
            TxConfirmationActionBuilder<TTxParams, TTxGasParams>,
            TxSubmission<TTxParams, TTxGasParams>,
            TxConfirmationAction<TTxParams, TTxGasParams>
        > onError
    )
    {
        lock(_isConfirmCalledLock)
        {
            if(_isConfirmCalled)
            {
                throw new InvalidOperationException($"{nameof(IPendingTxHandler<TTxParams, TTxGasParams>.PublishAndConfirmAsync)} already called on this instance");
            }

            _isConfirmCalled = true;
        }

        try
        {
            return await _processFunc(this, onError);
        }
        finally
        {
            _completionCts.SetResult();
        }
    }

    Task IInternalPendingTxHandler.WaitForCompletionAsync()
        => _completionCts.Task;
}
