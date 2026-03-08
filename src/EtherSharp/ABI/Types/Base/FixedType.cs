#pragma warning disable IDE0290

namespace EtherSharp.ABI.Types.Base;

/// <summary>
/// Represents an ABI type whose value is encoded in the fixed head section.
/// </summary>
/// <typeparam name="T">The CLR type wrapped by this ABI type.</typeparam>
public abstract class FixedType<T> : IFixedType
{
    /// <summary>
    /// Initializes the fixed ABI wrapper with the provided CLR value.
    /// </summary>
    /// <param name="value">The value represented by this ABI type.</param>
    protected FixedType(in T value)
    {
        Value = value;
    }

    /// <summary>
    /// Gets the underlying value being encoded.
    /// </summary>
    public T Value { get; }

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

#pragma warning restore IDE0290
