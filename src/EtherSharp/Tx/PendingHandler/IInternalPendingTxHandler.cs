namespace EtherSharp.Tx.PendingHandler;
public interface IInternalPendingTxHandler
{
    public Task WaitForCompletionAsync();
}
