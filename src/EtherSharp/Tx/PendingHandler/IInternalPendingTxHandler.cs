using EtherSharp.Client.Services.ResiliencyLayer;
using EtherSharp.Tx.Types;
using EtherSharp.Types;

namespace EtherSharp.Tx.PendingHandler;
public interface IInternalPendingTxHandler
{
    public Task<(TaskCompletionSource<TransactionReceipt>, Func<TxConfirmationError, ITxInput, ITxParams, ITxGasParams, TxConfirmationAction>)> WaitForConfirmCallAsync();

    public void AddTxSubmission(TxSubmission txSubmission);
}
