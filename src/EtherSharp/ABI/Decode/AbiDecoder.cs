﻿using EtherSharp.ABI.Decode.Interfaces;
using EtherSharp.ABI.Dynamic;
using EtherSharp.ABI.Fixed;
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
            8 when isUnsigned => FixedType<object>.Byte.Decode(CurrentSlot) is not TNumber b
                ? throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(byte)}")
                : b,
            8 when !isUnsigned => FixedType<object>.SByte.Decode(CurrentSlot) is not TNumber b
                ? throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(sbyte)}")
                : b,
            16 when isUnsigned => FixedType<object>.UShort.Decode(CurrentSlot) is not TNumber b
                ? throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(ushort)}")
                : b,
            16 when !isUnsigned => FixedType<object>.Short.Decode(CurrentSlot) is not TNumber b
                ? throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(short)}")
                : b,
            > 16 and <= 32 when isUnsigned => FixedType<object>.UInt.Decode(CurrentSlot) is not TNumber b
                ? throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(uint)}")
                : b,
            > 16 and <= 32 when !isUnsigned => FixedType<object>.Int.Decode(CurrentSlot) is not TNumber b
                ? throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(int)}")
                : b,
            > 32 and <= 64 when isUnsigned => FixedType<object>.ULong.Decode(CurrentSlot) is not TNumber b
                ? throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(ulong)}")
                : b,
            > 32 and <= 64 when !isUnsigned => FixedType<object>.Long.Decode(CurrentSlot) is not TNumber b
                ? throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(long)}")
                : b,
            > 64 and <= 256 => FixedType<object>.BigInteger.Decode(CurrentSlot, isUnsigned) is not TNumber b
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
        value = FixedType<object>.Bool.Decode(CurrentSlot);
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
        value = FixedType<object>.Address.Decode(CurrentSlot);
        return ConsumeBytes();
    }

    public AbiDecoder String(out string str)
    {
        str = DynamicType<object>.String.Decode(_bytes, _bytesRead);
        return ConsumeBytes();
    }
    string IDynamicTupleDecoder.String()
    {
        _ = String(out string? str);
        return str;
    }

    public AbiDecoder Bytes(out ReadOnlySpan<byte> value)
    {
        value = DynamicType<object>.Bytes.Decode(_bytes.Span, _bytesRead);
        return ConsumeBytes();
    }
    ReadOnlySpan<byte> IDynamicTupleDecoder.Bytes()
    {
        _ = Bytes(out var value);
        return value;
    }

    public AbiDecoder Array<T>(out T[] value, Func<IArrayAbiDecoder, T> func)
    {
        value = DynamicType<T>.Array.Decode(_bytes, _bytesRead, func);
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
        value = FixedType<T>.Tuple.Decode(this, func);
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
        value = DynamicType<T>.Tuple.Decode(_bytes, _bytesRead, func);
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
            8 when isUnsigned => DynamicType<object>.PrimitiveNumberArray<byte>.Decode(_bytes, _bytesRead) is not TNumber[] b
                ? throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(byte)}")
                : b,
            8 when !isUnsigned => DynamicType<object>.PrimitiveNumberArray<sbyte>.Decode(_bytes, _bytesRead) is not TNumber[] b
                ? throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(sbyte)}")
                : b,
            16 when isUnsigned => DynamicType<object>.PrimitiveNumberArray<ushort>.Decode(_bytes, _bytesRead) is not TNumber[] b
                ? throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(ushort)}")
                : b,
            16 when !isUnsigned => DynamicType<object>.PrimitiveNumberArray<short>.Decode(_bytes, _bytesRead) is not TNumber[] b
                ? throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(short)}")
                : b,
            > 16 and <= 32 when isUnsigned => DynamicType<object>.PrimitiveNumberArray<uint>.Decode(_bytes, _bytesRead) is not TNumber[] b
                ? throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(uint)}")
                : b,
            > 16 and <= 32 when !isUnsigned => DynamicType<object>.PrimitiveNumberArray<int>.Decode(_bytes, _bytesRead) is not TNumber[] b
                ? throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(int)}")
                : b,
            > 32 and <= 64 when isUnsigned => DynamicType<object>.PrimitiveNumberArray<ulong>.Decode(_bytes, _bytesRead) is not TNumber[] b
                ? throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(ulong)}")
                : b,
            > 32 and <= 64 when !isUnsigned => DynamicType<object>.PrimitiveNumberArray<long>.Decode(_bytes, _bytesRead) is not TNumber[] b
                ? throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(long)}")
                : b,
            > 64 and <= 256 => DynamicType<object>.BigIntegerArray.Decode(_bytes, _bytesRead, bitLength, isUnsigned) is not TNumber[] b
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