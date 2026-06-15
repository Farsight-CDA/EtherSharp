using EtherSharp.ABI.Types;
using EtherSharp.ABI.Types.Base;
using EtherSharp.Types;

namespace EtherSharp.ABI.Packed;

/// <summary>
/// Builds Ethereum ABI-packed (tightly packed) byte payloads.
/// </summary>
public partial class PackedAbiEncoder
{
    private readonly List<IPackedEncodeType> _entries = [];

    /// <summary>
    /// Gets the total packed size in bytes.
    /// </summary>
    public int Size { get; private set; }

    private PackedAbiEncoder AddElement(IPackedEncodeType type)
    {
        Size += type.PackedSize;
        _entries.Add(type);
        return this;
    }

    /// <summary>
    /// Encodes a boolean value in packed format.
    /// </summary>
    /// <param name="value">Boolean to encode.</param>
    /// <returns>This encoder instance for fluent chaining.</returns>
    public PackedAbiEncoder Bool(bool value)
        => AddElement(new AbiTypes.Bool(value));

    /// <summary>
    /// Encodes an Ethereum address in packed format.
    /// </summary>
    /// <param name="value">Address to encode.</param>
    /// <returns>This encoder instance for fluent chaining.</returns>
    public PackedAbiEncoder Address(in Address value)
        => AddElement(new AbiTypes.Address(in value));

    /// <summary>
    /// Encodes a UTF-8 string in packed format.
    /// </summary>
    /// <param name="value">String to encode.</param>
    /// <returns>This encoder instance for fluent chaining.</returns>
    public PackedAbiEncoder String(string value)
            => AddElement(new AbiTypes.String(value));

    /// <summary>
    /// Encodes a dynamic byte sequence in packed format.
    /// </summary>
    /// <param name="arr">Bytes to encode.</param>
    /// <returns>This encoder instance for fluent chaining.</returns>
    public PackedAbiEncoder Bytes(ReadOnlyMemory<byte> arr)
        => AddElement(new AbiTypes.Bytes(arr));

    /// <summary>
    /// Writes the packed payload into a caller-provided buffer.
    /// </summary>
    /// <param name="outputBuffer">Destination buffer. Must be at least <see cref="Size"/> bytes long.</param>
    /// <returns><see langword="true"/> when writing succeeds.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="outputBuffer"/> is too small.</exception>
    public bool TryWriteTo(Span<byte> outputBuffer)
    {
        if(outputBuffer.Length < Size)
        {
            throw new ArgumentException("Output buffer too small", nameof(outputBuffer));
        }

        int offset = 0;

        foreach(var entry in _entries)
        {
            entry.EncodePacked(
                outputBuffer.Slice(
                    offset,
                    entry.PackedSize
                )
            );
            offset += entry.PackedSize;
        }

        return true;
    }

    /// <summary>
    /// Materializes the current packed payload as a new byte array.
    /// </summary>
    /// <returns>The encoded packed payload.</returns>
    public byte[] Build()
    {
        byte[] buffer = new byte[Size];
        _ = TryWriteTo(buffer);
        return buffer;
    }
}
