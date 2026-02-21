namespace EtherSharp.ABI.Types.Base;

/// <summary>
/// Represents an ABI type whose value is encoded in the dynamic payload section.
/// </summary>
/// <typeparam name="T">The CLR type wrapped by this ABI type.</typeparam>
/// <param name="value">The value represented by this ABI type.</param>
public abstract class DynamicType<T>(T value) : IDynamicType
{
    /// <summary>
    /// Gets the encoded payload size in bytes.
    /// </summary>
    public abstract int PayloadSize { get; }

    /// <summary>
    /// Gets the underlying value being encoded.
    /// </summary>
    public readonly T Value = value;

    /// <summary>
    /// Encodes the dynamic value into metadata and payload spans.
    /// </summary>
    /// <param name="metadata">A 32-byte span containing the value metadata.</param>
    /// <param name="payload">A span receiving the dynamically sized payload bytes.</param>
    /// <param name="payloadOffset">The payload offset to encode into the metadata.</param>
    public abstract void Encode(Span<byte> metadata, Span<byte> payload, int payloadOffset);
}
