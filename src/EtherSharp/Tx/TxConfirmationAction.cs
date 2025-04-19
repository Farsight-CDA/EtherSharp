namespace EtherSharp.Tx;
public abstract record TxConfirmationAction
{
    public record ContinueWaiting(TimeSpan Duration) : TxConfirmationAction;
}
