using System.Numerics;

namespace EtherSharp.Tx;
public abstract record TxTimeoutAction
{
    public record ContinueWaiting(TimeSpan Duration) : TxTimeoutAction;
    public record IncreaseGasPriceTo(BigInteger NewFeePerGas, BigInteger NewPriorityFeePerGas) : TxTimeoutAction;
    public record IncreaseGasPriceToMarket() : TxTimeoutAction;
}
