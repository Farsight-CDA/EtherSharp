using EtherSharp.ABI.Decode.Interfaces;
using EtherSharp.ABI.Types;
using EtherSharp.Numerics;
using EtherSharp.Types;
using System.Buffers.Binary;

namespace EtherSharp.ABI;

/// <summary>
/// Reads Ethereum ABI-encoded data sequentially from a byte buffer.
/// </summary>
/// <param name="bytes">ABI-encoded payload to decode.</param>
public partial class AbiDecoder(ReadOnlyMemory<byte> bytes) : IFixedTupleDecoder, IDynamicTupleDecoder, IArrayAbiDecoder
{
    private readonly ReadOnlyMemory<byte> _bytes = bytes;

    private ReadOnlySpan<byte> CurrentSlot => _bytes.Span.Slice(BytesRead, 32);

    /// <summary>
    /// Number of bytes read from the head section of the input so far.
    /// </summary>
    public int BytesRead { get; private set; }

    private void ConsumeBytes()
        => BytesRead += 32;

    /// <summary>
    /// Decodes the next ABI slot as a boolean value.
    /// </summary>
    /// <returns>The decoded boolean.</returns>
    public bool Bool()
    {
        bool result = AbiTypes.Bool.Decode(CurrentSlot);
        ConsumeBytes();
        return result;
    }

    /// <summary>
    /// Decodes the next ABI slot as an Ethereum address.
    /// </summary>
    /// <returns>The decoded address.</returns>
    public Address Address()
    {
        var result = AbiTypes.Address.Decode(CurrentSlot);
        ConsumeBytes();
        return result;
    }

    /// <summary>
    /// Decodes the next ABI slot as a dynamic UTF-8 string.
    /// </summary>
    /// <returns>The decoded string.</returns>
    public string String()
    {
        string result = AbiTypes.String.Decode(_bytes.Span, BytesRead);
        ConsumeBytes();
        return result;
    }

    /// <summary>
    /// Decodes the next ABI slot as a dynamic byte array.
    /// </summary>
    /// <returns>The decoded bytes.</returns>
    public ReadOnlyMemory<byte> Bytes()
    {
        var result = AbiTypes.Bytes.Decode(_bytes, BytesRead);
        ConsumeBytes();
        return result;
    }

    /// <summary>
    /// Decodes the next ABI slot as a fixed-size byte value (<c>bytesN</c>).
    /// </summary>
    /// <param name="bitLength">Bit width of the target value (8..256 in steps of 8).</param>
    /// <returns>The decoded bytes.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="bitLength"/> is outside ABI bounds.</exception>
    public ReadOnlyMemory<byte> SizedBytes(int bitLength)
    {
        if(bitLength % 8 != 0 || bitLength < 8 || bitLength > 256)
        {
            throw new ArgumentException("Invalid bitLength", nameof(bitLength));
        }

        var result = _bytes.Slice(BytesRead, bitLength / 8);
        ConsumeBytes();
        return result;
    }

    /// <summary>
    /// Decodes the next ABI slot as a numeric value (<c>intN</c> or <c>uintN</c>).
    /// </summary>
    /// <typeparam name="TNumber">Expected CLR result type for the requested ABI width.</typeparam>
    /// <param name="isUnsigned"><see langword="true"/> for uintN, <see langword="false"/> for intN.</param>
    /// <param name="bitLength">ABI bit width (8..256 in steps of 8).</param>
    /// <returns>The decoded numeric value.</returns>
    /// <exception cref="ArgumentException">Thrown for invalid bit widths or mismatched CLR target type.</exception>
    /// <exception cref="NotSupportedException">Thrown when the requested configuration is unsupported.</exception>
    public TNumber Number<TNumber>(bool isUnsigned, int bitLength)
    {
        if(bitLength % 8 != 0 || bitLength < 8 || bitLength > 256)
        {
            throw new ArgumentException("Invalid bitLength", nameof(bitLength));
        }

        var result = bitLength switch
        {
            8 when isUnsigned => AbiTypes.Byte.Decode(CurrentSlot) is not TNumber b
                ? throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(byte)}")
                : b,
            8 when !isUnsigned => AbiTypes.SByte.Decode(CurrentSlot) is not TNumber b
                ? throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(sbyte)}")
                : b,
            16 when isUnsigned => AbiTypes.UShort.Decode(CurrentSlot) is not TNumber b
                ? throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(ushort)}")
                : b,
            16 when !isUnsigned => AbiTypes.Short.Decode(CurrentSlot) is not TNumber b
                ? throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(short)}")
                : b,
            > 16 and <= 32 when isUnsigned => AbiTypes.UInt.Decode(CurrentSlot) is not TNumber b
                ? throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(uint)}")
                : b,
            > 16 and <= 32 when !isUnsigned => AbiTypes.Int.Decode(CurrentSlot) is not TNumber b
                ? throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(int)}")
                : b,
            > 32 and <= 64 when isUnsigned => AbiTypes.ULong.Decode(CurrentSlot) is not TNumber b
                ? throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(ulong)}")
                : b,
            > 32 and <= 64 when !isUnsigned => AbiTypes.Long.Decode(CurrentSlot) is not TNumber b
                ? throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(long)}")
                : b,
            > 64 and <= 256 when isUnsigned => AbiTypes.UInt256.Decode(CurrentSlot) is not TNumber b
                ? throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(UInt256)}")
                : b,
            > 64 and <= 256 when !isUnsigned => AbiTypes.Int256.Decode(CurrentSlot) is not TNumber b
                ? throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(Int256)}")
                : b,
            _ => throw new NotSupportedException()
        };

        ConsumeBytes();
        return result;
    }

    /// <summary>
    /// Decodes the next ABI slot as a dynamic array of booleans.
    /// </summary>
    /// <returns>The decoded boolean array.</returns>
    /// <exception cref="IndexOutOfRangeException">Thrown when the encoded offsets/lengths point outside the payload.</exception>
    public bool[] BoolArray()
    {
        int payloadOffset = (int) BinaryPrimitives.ReadUInt32BigEndian(CurrentSlot[28..32]);

        var payload = _bytes.Span[payloadOffset..];

        uint arrayLength = BinaryPrimitives.ReadUInt32BigEndian(payload[28..32]);

        bool[] values = new bool[arrayLength];

        int offset = 32;
        for(uint i = 0; i < arrayLength; i++)
        {
            values[i] = AbiTypes.Bool.Decode(payload[offset..(offset + 32)]);
            offset += 32;
        }

        ConsumeBytes();
        return values;
    }

    /// <summary>
    /// Decodes the next ABI slot as a dynamic array of addresses.
    /// </summary>
    /// <returns>The decoded address array.</returns>
    /// <exception cref="IndexOutOfRangeException">Thrown when the encoded offsets/lengths point outside the payload.</exception>
    public Address[] AddressArray()
    {
        int payloadOffset = (int) BinaryPrimitives.ReadUInt32BigEndian(CurrentSlot[28..32]);

        var payload = _bytes.Span[payloadOffset..];

        uint arrayLength = BinaryPrimitives.ReadUInt32BigEndian(payload[28..32]);

        var addresses = new Address[arrayLength];

        int offset = 32;
        for(uint i = 0; i < arrayLength; i++)
        {
            addresses[i] = AbiTypes.Address.Decode(payload[offset..(offset + 32)]);
            offset += 32;
        }

        ConsumeBytes();
        return addresses;
    }

    /// <summary>
    /// Decodes the next ABI slot as a numeric array (<c>intN[]</c> or <c>uintN[]</c>).
    /// </summary>
    /// <typeparam name="TNumber">Expected CLR element type for the requested ABI width.</typeparam>
    /// <param name="isUnsigned"><see langword="true"/> for uintN[], <see langword="false"/> for intN[].</param>
    /// <param name="bitLength">ABI bit width (8..256 in steps of 8).</param>
    /// <returns>The decoded numeric array.</returns>
    /// <exception cref="ArgumentException">Thrown for invalid bit widths or mismatched CLR element type.</exception>
    /// <exception cref="NotSupportedException">Thrown when the requested configuration is unsupported.</exception>
    public TNumber[] NumberArray<TNumber>(bool isUnsigned, uint bitLength)
    {
        if(bitLength % 8 != 0 || bitLength < 8 || bitLength > 256)
        {
            throw new ArgumentException("Invalid bitLength", nameof(bitLength));
        }

        var result = bitLength switch
        {
            8 when isUnsigned => AbiTypes.SizedNumberArray<byte>.Decode(_bytes, BytesRead) is not TNumber[] b
                ? throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(byte)}")
                : b,
            8 when !isUnsigned => AbiTypes.SizedNumberArray<sbyte>.Decode(_bytes, BytesRead) is not TNumber[] b
                ? throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(sbyte)}")
                : b,
            16 when isUnsigned => AbiTypes.SizedNumberArray<ushort>.Decode(_bytes, BytesRead) is not TNumber[] b
                ? throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(ushort)}")
                : b,
            16 when !isUnsigned => AbiTypes.SizedNumberArray<short>.Decode(_bytes, BytesRead) is not TNumber[] b
                ? throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(short)}")
                : b,
            > 16 and <= 32 when isUnsigned => AbiTypes.SizedNumberArray<uint>.Decode(_bytes, BytesRead) is not TNumber[] b
                ? throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(uint)}")
                : b,
            > 16 and <= 32 when !isUnsigned => AbiTypes.SizedNumberArray<int>.Decode(_bytes, BytesRead) is not TNumber[] b
                ? throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(int)}")
                : b,
            > 32 and <= 64 when isUnsigned => AbiTypes.SizedNumberArray<ulong>.Decode(_bytes, BytesRead) is not TNumber[] b
                ? throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(ulong)}")
                : b,
            > 32 and <= 64 when !isUnsigned => AbiTypes.SizedNumberArray<long>.Decode(_bytes, BytesRead) is not TNumber[] b
                ? throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(long)}")
                : b,
            > 64 and <= 256 when isUnsigned => AbiTypes.UInt256Array.Decode(_bytes, BytesRead, bitLength) is not TNumber[] b
                ? throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(UInt256)}")
                : b,
            > 64 and <= 256 when !isUnsigned => AbiTypes.Int256Array.Decode(_bytes, BytesRead, bitLength) is not TNumber[] b
                ? throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(Int256)}")
                : b,
            _ => throw new NotSupportedException()
        };

        ConsumeBytes();
        return result;
    }

    /// <summary>
    /// Decodes the next ABI slot as a dynamic array of strings.
    /// </summary>
    /// <returns>The decoded string array.</returns>
    /// <exception cref="IndexOutOfRangeException">Thrown when the encoded offsets/lengths point outside the payload.</exception>
    public string[] StringArray()
    {
        int payloadOffset = (int) BinaryPrimitives.ReadUInt32BigEndian(CurrentSlot[28..32]);

        var payload = _bytes[payloadOffset..];

        uint arrayLength = BinaryPrimitives.ReadUInt32BigEndian(payload.Span[28..32]);

        string[] values = new string[arrayLength];

        var decoder = new AbiDecoder(payload[32..]);

        for(int i = 0; i < arrayLength; i++)
        {
            values[i] = decoder.String();
        }

        ConsumeBytes();
        return values;
    }

    /// <summary>
    /// Decodes the next ABI slot as a dynamic array of byte arrays.
    /// </summary>
    /// <returns>The decoded array of byte buffers.</returns>
    /// <exception cref="IndexOutOfRangeException">Thrown when the encoded offsets/lengths point outside the payload.</exception>
    public ReadOnlyMemory<byte>[] BytesArray()
    {
        int payloadOffset = (int) BinaryPrimitives.ReadUInt32BigEndian(CurrentSlot[28..32]);

        var payload = _bytes[payloadOffset..];

        uint arrayLength = BinaryPrimitives.ReadUInt32BigEndian(payload.Span[28..32]);

        var values = new ReadOnlyMemory<byte>[arrayLength];

        var decoder = new AbiDecoder(payload[32..]);

        for(int i = 0; i < arrayLength; i++)
        {
            values[i] = decoder.Bytes();
        }

        ConsumeBytes();
        return values;
    }

    /// <summary>
    /// Decodes the next ABI slot as an array using a custom element decoder.
    /// </summary>
    /// <typeparam name="T">Element result type.</typeparam>
    /// <param name="func">Element decoder that reads one element from an array decoder.</param>
    /// <returns>The decoded array.</returns>
    public T[] Array<T>(Func<IArrayAbiDecoder, T> func)
    {
        var result = AbiTypes.Array.Decode(_bytes, BytesRead, func);
        ConsumeBytes();
        return result;
    }

    /// <summary>
    /// Decodes a fixed (static) tuple at the current decoder position.
    /// </summary>
    /// <typeparam name="T">Tuple projection result type.</typeparam>
    /// <param name="func">Projection that reads tuple fields from a fixed-tuple decoder.</param>
    /// <returns>The projected tuple result.</returns>
    public T FixedTuple<T>(Func<IFixedTupleDecoder, T> func)
    {
        var result = AbiTypes.FixedTuple.Decode(this, func);
        return result;
    }

    /// <summary>
    /// Decodes the next ABI slot as a dynamic tuple.
    /// </summary>
    /// <typeparam name="T">Tuple projection result type.</typeparam>
    /// <param name="func">Projection that reads tuple fields from a dynamic-tuple decoder.</param>
    /// <returns>The projected tuple result.</returns>
    public T DynamicTuple<T>(Func<IDynamicTupleDecoder, T> func)
    {
        var result = AbiTypes.DynamicTuple.Decode(_bytes, BytesRead, func);
        ConsumeBytes();
        return result;
    }
}
