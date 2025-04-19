using EtherSharp.Client.Services.ResiliencyLayer;
using EtherSharp.Tx.Types;
using EtherSharp.Types;

namespace EtherSharp.Tx.PendingHandler;
/// <summary>
/// Handler class for transactions that are still pending and not confirmed on the blockchain.
/// </summary>
public class PendingTxHandler<TTxParams, TTxGasParams>(
    uint nonce, 
    IEnumerable<TxSubmission> txSubmissions, 
    Func<ITxParams, ITxGasParams, Task<TxSubmissionResult>> publishFunc
) : IPendingTxHandler<TTxParams, TTxGasParams>, IInternalPendingTxHandler
    where TTxParams : class, ITxParams<TTxParams>
    where TTxGasParams : class, ITxGasParams<TTxGasParams>
{
    private readonly List<TxSubmission> _txSubmissions = [.. txSubmissions];

    private readonly TaskCompletionSource<TransactionReceipt> _confirmReturnCts = new TaskCompletionSource<TransactionReceipt>();
    private readonly TaskCompletionSource<(
        TaskCompletionSource<TransactionReceipt>,
        Func<TxConfirmationError, ITxInput, ITxParams, ITxGasParams, TxConfirmationAction>
        )> _confirmCallCts = new ();

    /// <summary>
    /// The nonce allocated to this pending tx.
    /// </summary>
    public uint Nonce { get; } = nonce;

    /// <summary>
    /// Set of parameters that was used to submit this transaction to the mempool.
    /// </summary>
    public IReadOnlyList<TxSubmission> TxSubmissions => [.. _txSubmissions];

    /// <summary>
    /// Function to encode and send the transaction to the mempool.
    /// </summary>
    public Func<ITxParams, ITxGasParams, Task<TxSubmissionResult>> PublishFunc { get; } = publishFunc;

    void IInternalPendingTxHandler.AddTxSubmission(TxSubmission txSubmission) 
        => _txSubmissions.Add(txSubmission);

    Task<(TaskCompletionSource<TransactionReceipt>, Func<TxConfirmationError, ITxInput, ITxParams, ITxGasParams, TxConfirmationAction>)>         IInternalPendingTxHandler.WaitForConfirmCallAsync()
        => _confirmCallCts.Task;

    /// <summary>
    /// Waits for the transaction to be confirmed on-chain and performs necessary changes based on a given callback to ensure it does.
    /// </summary>
    /// <returns></returns>
    public Task<TransactionReceipt> PublishAndConfirmAsync(Func<TxConfirmationError, ITxInput, TTxParams, TTxGasParams, TxConfirmationAction> onError)
    {


        return !_confirmCallCts.TrySetResult((_confirmReturnCts, (error, txInput, txParams, txGasParams) => onError(error, txInput, (TTxParams) txParams, (TTxGasParams) txGasParams)))
                ? throw new InvalidOperationException($"{nameof(PublishAndConfirmAsync)} already called")
                : _confirmReturnCts.Task;
    }
}
