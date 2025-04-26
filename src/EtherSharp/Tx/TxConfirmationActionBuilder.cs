using EtherSharp.Tx.Types;

namespace EtherSharp.Tx;
public class TxConfirmationActionBuilder<TTxParams, TTxGasParams>
    where TTxParams : class, ITxParams<TTxParams>
    where TTxGasParams : class, ITxGasParams<TTxGasParams>
{
    public TxConfirmationAction<TTxParams, TTxGasParams> ContinueWaiting(TimeSpan duration)
        => new TxConfirmationAction<TTxParams, TTxGasParams>.ContinueWaiting(duration);

    public TxConfirmationAction<TTxParams, TTxGasParams> MinimalGasFeeIncrement()
        => new TxConfirmationAction<TTxParams, TTxGasParams>.MinimalGasFeeIncrement();
}
