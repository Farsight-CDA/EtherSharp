namespace EtherSharp.Tx.Types;

/// <summary>
/// Represents serializable, transaction-type-specific parameters.
/// </summary>
/// <typeparam name="TSelf">Concrete tx-parameter type.</typeparam>
public interface ITxParams<TSelf> : ITxParams
    where TSelf : ITxParams<TSelf>
{
    /// <summary>
    /// Gets the default tx params used when callers do not provide custom values.
    /// </summary>
    public abstract static TSelf Default { get; }

    /// <summary>
    /// Decodes an instance of <typeparamref name="TSelf"/> from encoded bytes.
    /// </summary>
    /// <param name="data">Encoded tx-parameter payload.</param>
    /// <returns>The decoded tx-parameter instance.</returns>
    public abstract static TSelf Decode(ReadOnlySpan<byte> data);

    /// <summary>
    /// Encodes this instance to bytes.
    /// </summary>
    /// <returns>Encoded tx-parameter payload.</returns>
    public byte[] Encode();
}

/// <summary>
/// Non-generic marker interface for transaction parameter types.
/// </summary>
public interface ITxParams;
