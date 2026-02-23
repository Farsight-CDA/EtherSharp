using EtherSharp.Numerics;

namespace EtherSharp.Tx.Types;

/// <summary>
/// Represents serializable gas-pricing parameters for a transaction type.
/// </summary>
/// <typeparam name="TSelf">Concrete gas-parameter type.</typeparam>
public interface ITxGasParams<TSelf> : ITxGasParams
    where TSelf : ITxGasParams<TSelf>
{
    /// <summary>
    /// Decodes an instance of <typeparamref name="TSelf"/> from encoded bytes.
    /// </summary>
    /// <param name="data">Encoded gas-parameter payload.</param>
    /// <returns>The decoded gas-parameter instance.</returns>
    public abstract static TSelf Decode(ReadOnlySpan<byte> data);

    /// <summary>
    /// Returns a new instance with fee-related values scaled by <paramref name="multiplier"/> / <paramref name="divider"/>.
    /// </summary>
    /// <param name="multiplier">Numerator for the scaling factor.</param>
    /// <param name="divider">Denominator for the scaling factor.</param>
    /// <param name="minimumIncrement">Minimum absolute increment to enforce.</param>
    /// <returns>A new gas-parameter instance with adjusted fee values.</returns>
    public TSelf IncrementByFactor(UInt256 multiplier, UInt256 divider, UInt256 minimumIncrement);

    /// <summary>
    /// Encodes this instance to bytes.
    /// </summary>
    /// <returns>Encoded gas-parameter payload.</returns>
    public byte[] Encode();
}

/// <summary>
/// Non-generic marker interface for transaction gas-parameter types.
/// </summary>
public interface ITxGasParams;
