using EtherSharp.ABI.Decode.Interfaces;
using EtherSharp.ABI.Types;
using System.Buffers.Binary;
using System.Numerics;

namespace EtherSharp.ABI;
public partial class AbiDecoder(ReadOnlyMemory<byte> bytes) : IFixedTupleDecoder, IDynamicTupleDecoder, IArrayAbiDecoder
{
    private ReadOnlyMemory<byte> _bytes = bytes;
    private uint _bytesRead = 0;

    private ReadOnlySpan<byte> CurrentSlot => _bytes.Span[..32];

    private AbiDecoder ConsumeBytes()
    {
        _bytes = _bytes[32..];
        _bytesRead += 32;
        return this;
    }

    public AbiDecoder Number<TNumber>(out TNumber number, bool isUnsigned, int bitLength)
    {
        if(bitLength % 8 != 0 || bitLength < 8 || bitLength > 256)
        {
            throw new ArgumentException("Invalid bitLength", nameof(bitLength));
        }

        number = bitLength switch
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
            > 64 and <= 256 => AbiTypes.BigInteger.Decode(CurrentSlot, isUnsigned) is not TNumber b
                ? throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(BigInteger)}")
                : b,
            _ => throw new NotSupportedException()
        };

        return ConsumeBytes();
    }
    TNumber IFixedTupleDecoder.Number<TNumber>(bool isUnsigned, int bitLength)
    {
        _ = Number<TNumber>(out var number, isUnsigned, bitLength);
        return number;
    }
    TNumber IDynamicTupleDecoder.Number<TNumber>(bool isUnsigned, int bitLength)
    {
        _ = Number<TNumber>(out var number, isUnsigned, bitLength);
        return number;
    }

    public AbiDecoder Bool(out bool value)
    {
        value = AbiTypes.Bool.Decode(CurrentSlot);
        return ConsumeBytes();
    }
    bool IFixedTupleDecoder.Bool()
    {
        _ = Bool(out bool value);
        return value;
    }
    bool IDynamicTupleDecoder.Bool()
    {
        _ = Bool(out bool value);
        return value;
    }

    public AbiDecoder Address(out string value)
    {
        value = AbiTypes.Address.Decode(CurrentSlot);
        return ConsumeBytes();
    }

    public AbiDecoder String(out string str)
    {
        str = AbiTypes.String.Decode(_bytes, _bytesRead);
        return ConsumeBytes();
    }
    string IDynamicTupleDecoder.String()
    {
        _ = String(out string? str);
        return str;
    }

    public AbiDecoder Bytes(out ReadOnlySpan<byte> value)
    {
        value = AbiTypes.Bytes.Decode(_bytes.Span, _bytesRead);
        return ConsumeBytes();
    }
    ReadOnlySpan<byte> IDynamicTupleDecoder.Bytes()
    {
        _ = Bytes(out var value);
        return value;
    }

    public AbiDecoder AddressArray(out string[] addresses)
    {
        uint payloadOffset = BinaryPrimitives.ReadUInt32BigEndian(_bytes[28..32].Span);

        if(payloadOffset < _bytesRead)
        {
            throw new IndexOutOfRangeException("Index out of range");
        }

        long relativePayloadOffset = payloadOffset - _bytesRead;
        var payload = _bytes[(int) relativePayloadOffset..].Span;

        uint arrayLength = BinaryPrimitives.ReadUInt32BigEndian(payload[28..32]);

        addresses = new string[arrayLength];

        int offset = 32;
        for(uint i = 0; i < arrayLength; i++)
        {
            addresses[i] = AbiTypes.Address.Decode(payload[offset..(offset + 32)]);
            offset += 32;
        }

        return ConsumeBytes();
    }
    string[] IDynamicTupleDecoder.AddressArray()
    {
        _ = AddressArray(out string[]? addresses);
        return addresses;
    }

    public AbiDecoder Array<T>(out T[] value, Func<IArrayAbiDecoder, T> func)
    {
        value = AbiTypes.Array.Decode(_bytes, _bytesRead, func);
        return ConsumeBytes();
    }
    T[] IArrayAbiDecoder.Array<T>(Func<IArrayAbiDecoder, T> func)
    {
        _ = Array(out var value, func);
        return value;
    }
    T[] IDynamicTupleDecoder.Array<T>(out T[] value, Func<IArrayAbiDecoder, T> func)
    {
        _ = Array(out value, func);
        return value;
    }

    public AbiDecoder FixedTuple<T>(out T value, Func<IFixedTupleDecoder, T> func)
    {
        value = AbiTypes.FixedTuple.Decode(this, func);
        return ConsumeBytes();
    }
    T IFixedTupleDecoder.FixedTuple<T>(Func<IFixedTupleDecoder, T> func)
    {
        _ = FixedTuple(out var value, func);
        return value;
    }
    T IDynamicTupleDecoder.FixedTuple<T>(Func<IFixedTupleDecoder, T> func)
    {
        _ = FixedTuple(out var value, func);
        return value;
    }

    public AbiDecoder DynamicTuple<T>(out T value, Func<IDynamicTupleDecoder, T> func)
    {
        value = AbiTypes.DynamicTuple.Decode<T>(_bytes, _bytesRead, func);
        return ConsumeBytes();
    }
    T IDynamicTupleDecoder.DynamicTuple<T>(Func<IDynamicTupleDecoder, T> func)
    {
        _ = DynamicTuple(out var value, func);
        return value;
    }

    public AbiDecoder NumberArray<TNumber>(bool isUnsigned, uint bitLength, out TNumber[] numbers)
    {
        if(bitLength % 8 != 0 || bitLength < 8 || bitLength > 256)
        {
            throw new ArgumentException("Invalid bitLength", nameof(bitLength));
        }

        numbers = bitLength switch
        {
            8 when isUnsigned => AbiTypes.PrimitiveNumberArray<byte>.Decode(_bytes, _bytesRead) is not TNumber[] b
                ? throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(byte)}")
                : b,
            8 when !isUnsigned => AbiTypes.PrimitiveNumberArray<sbyte>.Decode(_bytes, _bytesRead) is not TNumber[] b
                ? throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(sbyte)}")
                : b,
            16 when isUnsigned => AbiTypes.PrimitiveNumberArray<ushort>.Decode(_bytes, _bytesRead) is not TNumber[] b
                ? throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(ushort)}")
                : b,
            16 when !isUnsigned => AbiTypes.PrimitiveNumberArray<short>.Decode(_bytes, _bytesRead) is not TNumber[] b
                ? throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(short)}")
                : b,
            > 16 and <= 32 when isUnsigned => AbiTypes.PrimitiveNumberArray<uint>.Decode(_bytes, _bytesRead) is not TNumber[] b
                ? throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(uint)}")
                : b,
            > 16 and <= 32 when !isUnsigned => AbiTypes.PrimitiveNumberArray<int>.Decode(_bytes, _bytesRead) is not TNumber[] b
                ? throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(int)}")
                : b,
            > 32 and <= 64 when isUnsigned => AbiTypes.PrimitiveNumberArray<ulong>.Decode(_bytes, _bytesRead) is not TNumber[] b
                ? throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(ulong)}")
                : b,
            > 32 and <= 64 when !isUnsigned => AbiTypes.PrimitiveNumberArray<long>.Decode(_bytes, _bytesRead) is not TNumber[] b
                ? throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(long)}")
                : b,
            > 64 and <= 256 => AbiTypes.BigIntegerArray.Decode(_bytes, _bytesRead, bitLength, isUnsigned) is not TNumber[] b
                ? throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {(isUnsigned ? "u-" : "")} {typeof(System.Numerics.BigInteger)}")
                : b,
            _ => throw new NotSupportedException()
        };

        return ConsumeBytes();
    }
    TNumber[] IArrayAbiDecoder.NumberArray<TNumber>(bool isUnsigned, uint bitLength)
    {
        _ = NumberArray<TNumber>(isUnsigned, bitLength, out var numbers);
        return numbers;
    }
    TNumber[] IDynamicTupleDecoder.NumberArray<TNumber>(bool isUnsigned, uint bitLength)
    {
        _ = NumberArray<TNumber>(isUnsigned, bitLength, out var numbers);
        return numbers;
    }
}