
namespace EtherSharp.ABI.Types.Base;

/// <summary>
/// Represents an ABI type whose value is encoded in the fixed head section.
/// </summary>
/// <typeparam name="T">The CLR type wrapped by this ABI type.</typeparam>
/// <param name="value">The value represented by this ABI type.</param>
public abstract class FixedType<T>(T value) : IFixedType
{
    /// <summary>
    /// Gets the underlying value being encoded.
    /// </summary>
    public T Value { get; } = value;

    /// <summary>
    /// Gets the encoded size in bytes.
    /// </summary>
    public virtual int Size => 32;

    /// <summary>
    /// Encodes the fixed-size value into a 32-byte ABI word.
    /// </summary>
    /// <param name="buffer">A span containing the target ABI word bytes.</param>
    public abstract void Encode(Span<byte> buffer);
}
