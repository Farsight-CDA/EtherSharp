using EtherSharp.Tx.Types;

namespace EtherSharp.Tx;

/// <summary>
/// Represents actions that can be taken to handle a <see cref="TxConfirmationError"/>.
/// </summary>
public abstract record TxConfirmationAction<TTxParams, TTxGasParams>
    where TTxParams : class, ITxParams<TTxParams>
    where TTxGasParams : class, ITxGasParams<TTxGasParams>
{
    /// <summary>
    /// Waits the given duration before calling the onError callback again.
    /// </summary>
    /// <param name="Duration">The duration to wait.</param>
    public record ContinueWaiting(TimeSpan Duration) : TxConfirmationAction<TTxParams, TTxGasParams>;

    /// <summary>
    /// Resubmits the same transaction with an increased gas fee. 
    /// The gas fee is incremented by the minimum amount necessary to replace the transaction in the mempool.
    /// </summary>
    public record MinimalGasFeeIncrement() : TxConfirmationAction<TTxParams, TTxGasParams>;
}
