using EtherSharp.ABI.Types;
using EtherSharp.ABI.Types.Base;
using EtherSharp.Numerics;
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
    /// Encodes a packed numeric value with the given signedness and bit width.
    /// </summary>
    /// <typeparam name="TNumber">CLR number type matching the requested ABI width.</typeparam>
    /// <param name="number">Value to encode.</param>
    /// <param name="isUnsigned"><see langword="true"/> for uintN, <see langword="false"/> for intN.</param>
    /// <param name="bitLength">ABI bit width (8..256 in steps of 8).</param>
    /// <returns>This encoder instance for fluent chaining.</returns>
    public PackedAbiEncoder Number<TNumber>(TNumber number, bool isUnsigned, int bitLength)
    {
        if(bitLength % 8 != 0 || bitLength < 8 || bitLength > 256)
        {
            throw new ArgumentException("Invalid bitLength", nameof(bitLength));
        }
        //
        return AddElement(bitLength switch
        {
            8 => isUnsigned
                ? new AbiTypes.Byte(
                    number is byte us ? us : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(byte)}"))
                : new AbiTypes.SByte(
                    number is sbyte s ? s : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(sbyte)}")),
            16 => isUnsigned
                ? new AbiTypes.UShort(
                    number is ushort us ? us : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(ushort)}"))
                : new AbiTypes.Short(
                    number is short s ? s : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(short)}")),
            > 16 and <= 32 => isUnsigned
                ? new AbiTypes.UInt(
                    number is uint us ? us : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(uint)}"), bitLength / 8)
                : new AbiTypes.Int(
                    number is int s ? s : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(int)}"), bitLength / 8),
            > 32 and <= 64 => isUnsigned
                ? new AbiTypes.ULong(
                    number is ulong us ? us : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(ulong)}"), bitLength / 8)
                : new AbiTypes.Long(
                    number is long s ? s : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(long)}"), bitLength / 8),
            > 64 and <= 256 => isUnsigned
                ? new AbiTypes.UInt256(
                    number is UInt256 us ? us : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(UInt256)}"), bitLength / 8)
                : new AbiTypes.Int256(
                    number is Int256 s ? s : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(Int256)}"), bitLength / 8),
            _ => throw new NotSupportedException()
        });
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
    public PackedAbiEncoder Address(Address value)
        => AddElement(new AbiTypes.Address(value));

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
    public bool TryWritoTo(Span<byte> outputBuffer)
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
        _ = TryWritoTo(buffer);
        return buffer;
    }
}
