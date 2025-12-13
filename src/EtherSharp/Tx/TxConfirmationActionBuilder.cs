using EtherSharp.Tx.Types;

namespace EtherSharp.Tx;
/// <summary>
/// Builder to contruct the further actions to perform on a tx publish error.
/// </summary>
/// <typeparam name="TTxParams"></typeparam>
/// <typeparam name="TTxGasParams"></typeparam>
public record TxConfirmationActionBuilder<TTxParams, TTxGasParams>
    where TTxParams : class, ITxParams<TTxParams>
    where TTxGasParams : class, ITxGasParams<TTxGasParams>
{
    /// <inheritdoc cref="TxConfirmationAction{TTxParams, TTxGasParams}.ContinueWaiting"/>
    public TxConfirmationAction<TTxParams, TTxGasParams> ContinueWaiting(TimeSpan duration)
        => new TxConfirmationAction<TTxParams, TTxGasParams>.ContinueWaiting(duration);

    /// <inheritdoc cref="TxConfirmationAction{TTxParams, TTxGasParams}.MinimalGasFeeIncrement"/>
    public TxConfirmationAction<TTxParams, TTxGasParams> MinimalGasFeeIncrement()
        => TxConfirmationAction<TTxParams, TTxGasParams>.MinimalGasFeeIncrement.Instance;

    /// <inheritdoc cref="TxConfirmationAction{TTxParams, TTxGasParams}.CancelTransaction"/>
    public TxConfirmationAction<TTxParams, TTxGasParams> CancelTransaction()
        => TxConfirmationAction<TTxParams, TTxGasParams>.CancelTransaction.Instance;
}
