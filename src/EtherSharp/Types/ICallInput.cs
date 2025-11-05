using System.Numerics;

namespace EtherSharp.Types;
/// <summary>
/// Represents a call payload.
/// </summary>
public interface ICallInput
{
    /// <summary>
    /// The To field of the transaction.
    /// </summary>
    public Address To { get; }

    /// <summary>
    /// The ETH Value of the transaction.
    /// </summary>
    public BigInteger Value { get; }

    /// <summary>
    /// The calldata of the transaction.
    /// </summary>
    public ReadOnlySpan<byte> Data { get; }
}
