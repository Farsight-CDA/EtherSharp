using System.Numerics;

namespace EtherSharp.Tx;
public abstract record TxTimeoutAction
{
    public record ContinueWaiting(TimeSpan Duration) : TxTimeoutAction;
}
