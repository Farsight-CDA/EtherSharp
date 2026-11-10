using EtherSharp.Numerics;

namespace EtherSharp.Query;

/// <summary>
/// Represents a low-level query operation that can be encoded into the aggregated query payload.
/// </summary>
public interface IQueryOperation : IQuery
{
    /// <summary>
    /// Gets the number of bytes this operation contributes to the encoded query calldata.
    /// </summary>
    public int CallDataLength { get; }

    /// <summary>
    /// Gets the ETH value that should be sent when executing this operation.
    /// </summary>
    public UInt256 EthValue { get; }

    /// <summary>
    /// Encodes this operation into <paramref name="buffer"/>.
    /// </summary>
    /// <param name="buffer">Destination buffer sized to <see cref="CallDataLength"/>.</param>
    public void Encode(Span<byte> buffer);

    /// <summary>
    /// Reads the expected byte length for this operation's raw result from the beginning of <paramref name="resultData"/>.
    /// </summary>
    /// <param name="resultData">The result payload beginning at this operation's output offset.</param>
    /// <returns>The total number of bytes consumed by this operation's result.</returns>
    public int ParseResultLength(ReadOnlySpan<byte> resultData);
}
