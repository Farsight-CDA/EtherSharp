namespace EtherSharp.ABI.Types.Base;

/// <summary>
/// Represents an ABI value encoded with a head offset and a dynamic payload.
/// </summary>
public interface IDynamicType
{
    /// <summary>
    /// Gets the size of the encoded payload in bytes.
    /// </summary>
    public int PayloadSize { get; }

    /// <summary>
    /// Encodes the head offset and dynamic payload into the supplied buffers.
    /// </summary>
    /// <param name="metadata">The destination for the 32-byte head offset.</param>
    /// <param name="payload">The destination for the encoded payload.</param>
    /// <param name="payloadOffset">The payload offset relative to the containing ABI encoding.</param>
    public void Encode(Span<byte> metadata, Span<byte> payload, int payloadOffset);
}
