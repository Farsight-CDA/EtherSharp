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

    private void ConsumeBytes()
    {
        _bytes = _bytes[32..];
        _bytesRead += 32;
    }

    public bool Bool()
    {
        bool result = AbiTypes.Bool.Decode(CurrentSlot);
        ConsumeBytes();
        return result;
    }

    public string Address()
    {
        string result = AbiTypes.Address.Decode(CurrentSlot);
        ConsumeBytes();
        return result;
    }

    public string String()
    {
        string result = AbiTypes.String.Decode(_bytes, _bytesRead);
        ConsumeBytes();
        return result;
    }

    public ReadOnlySpan<byte> Bytes()
    {
        var result = AbiTypes.Bytes.Decode(_bytes.Span, _bytesRead);
        ConsumeBytes();
        return result;
    }

    public ReadOnlySpan<byte> SizedBytes(int bitLength)
    {
        if(bitLength % 8 != 0 || bitLength < 8 || bitLength > 256)
        {
            throw new ArgumentException("Invalid bitLength", nameof(bitLength));
        }

        var result = AbiTypes.SizedBytes.Decode(CurrentSlot, bitLength / 8);
        ConsumeBytes();
        return result;
    }

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
            > 64 and <= 256 => AbiTypes.BigInteger.Decode(CurrentSlot, isUnsigned) is not TNumber b
                ? throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(BigInteger)}")
                : b,
            _ => throw new NotSupportedException()
        };

        ConsumeBytes();
        return result;
    }

    public string[] AddressArray()
    {
        uint payloadOffset = BinaryPrimitives.ReadUInt32BigEndian(_bytes[28..32].Span);

        if(payloadOffset < _bytesRead)
        {
            throw new IndexOutOfRangeException("Index out of range");
        }

        long relativePayloadOffset = payloadOffset - _bytesRead;
        var payload = _bytes[(int) relativePayloadOffset..].Span;

        uint arrayLength = BinaryPrimitives.ReadUInt32BigEndian(payload[28..32]);

        string[] addresses = new string[arrayLength];

        int offset = 32;
        for(uint i = 0; i < arrayLength; i++)
        {
            addresses[i] = AbiTypes.Address.Decode(payload[offset..(offset + 32)]);
            offset += 32;
        }

        ConsumeBytes();
        return addresses;
    }

    public TNumber[] NumberArray<TNumber>(bool isUnsigned, uint bitLength)
    {
        if(bitLength % 8 != 0 || bitLength < 8 || bitLength > 256)
        {
            throw new ArgumentException("Invalid bitLength", nameof(bitLength));
        }

        var result = bitLength switch
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

        ConsumeBytes();
        return result;
    }

    public T[] Array<T>(Func<IArrayAbiDecoder, T> func)
    {
        var result = AbiTypes.Array.Decode(_bytes, _bytesRead, func);
        ConsumeBytes();
        return result;
    }

    public T FixedTuple<T>(Func<IFixedTupleDecoder, T> func)
    {
        var result = AbiTypes.FixedTuple.Decode(this, func);
        return result;
    }

    public T DynamicTuple<T>(Func<IDynamicTupleDecoder, T> func)
    {
        var result = AbiTypes.DynamicTuple.Decode(_bytes, _bytesRead, func);
        ConsumeBytes();
        return result;
    }
}