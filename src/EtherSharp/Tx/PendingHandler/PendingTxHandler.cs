using EtherSharp.Tx.Types;
using EtherSharp.Types;

namespace EtherSharp.Tx.PendingHandler;
/// <summary>
/// Handler class for transactions that are still pending and not confirmed on the blockchain.
/// </summary>
public class PendingTxHandler<TTxParams, TTxGasParams>(
    uint nonce, 
    IEnumerable<TxSubmission<TTxParams, TTxGasParams>> txSubmissions,
    Func<
        PendingTxHandler<TTxParams, TTxGasParams>,
        Func<TxConfirmationError, ITxInput, TTxParams, TTxGasParams, TxConfirmationAction>,
        Task<TxConfirmationResult>
    > processFunc
) : IPendingTxHandler<TTxParams, TTxGasParams>, IInternalPendingTxHandler
    where TTxParams : class, ITxParams<TTxParams>
    where TTxGasParams : class, ITxGasParams<TTxGasParams>
{
    private readonly List<TxSubmission<TTxParams, TTxGasParams>> _txSubmissions = [.. txSubmissions];
    private readonly Func<
        PendingTxHandler<TTxParams, TTxGasParams>,
        Func<TxConfirmationError, ITxInput, TTxParams, TTxGasParams, TxConfirmationAction>,
        Task<TxConfirmationResult>
    > _processFunc = processFunc;

    private readonly Lock _isConfirmCalledLock = new Lock();
    private bool _isConfirmCalled;
    private readonly TaskCompletionSource<TxConfirmationResult> _completionCts 
        = new TaskCompletionSource<TxConfirmationResult>();

    /// <summary>
    /// The nonce allocated to this pending tx.
    /// </summary>
    public uint Nonce { get; } = nonce;

    /// <summary>
    /// Set of parameters that was used to submit this transaction to the mempool.
    /// </summary>
    public IReadOnlyList<TxSubmission<TTxParams, TTxGasParams>> TxSubmissions => [.. _txSubmissions];

    Task IInternalPendingTxHandler.WaitForCompletionAsync() 
        => _completionCts.Task;
    async Task<TxConfirmationResult> IPendingTxHandler<TTxParams, TTxGasParams>.PublishAndConfirmAsync(
        Func<TxConfirmationError, ITxInput, TTxParams, TTxGasParams, TxConfirmationAction> onError)
    {
        Task<TxConfirmationResult> resultTask;

        lock(_isConfirmCalledLock)
        {
            if(_isConfirmCalled)
            {
                resultTask = _completionCts.Task;
            }
            else
            {
                resultTask = _processFunc(this, onError)
                    .ContinueWith(prev =>
                    {
                        _completionCts.SetResult(prev.Result);
                        return prev.Result;
                    });
                _isConfirmCalled = true;
            }
        }

        return await resultTask;
    }
}
