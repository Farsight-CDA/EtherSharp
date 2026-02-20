namespace EtherSharp.Tx.PendingHandler;

internal interface IInternalPendingTxHandler
{
    Task<TxConfirmationResult> WaitForCompletionAsync();
}
