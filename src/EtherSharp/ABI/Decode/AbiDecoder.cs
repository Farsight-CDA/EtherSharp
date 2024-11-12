using EtherSharp.ABI.Dynamic;
using EtherSharp.ABI.Fixed;
using System.Numerics;

namespace EtherSharp.ABI.Decode;

public partial class AbiDecoder(ReadOnlyMemory<byte> bytes) : IStructAbiDecoder, IArrayAbiDecoder
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

    public AbiDecoder Address(out string value)
    {
        value = FixedType<object>.Address.Decode(CurrentSlot);
        return ConsumeBytes();
    }

    public AbiDecoder Bytes(out ReadOnlyMemory<byte> value)
    {
        value = DynamicType<object>.Bytes.Decode(_bytes, _bytesRead);
        return ConsumeBytes();
    }

    public AbiDecoder String(out string str)
    {
        str = DynamicType<object>.String.Decode(_bytes, _bytesRead);
        return ConsumeBytes();
    }

    public AbiDecoder Struct<T>(out T value, Func<IStructAbiDecoder, T> func)
    {
        value = DynamicType<T>.Struct.Decode(_bytes, _bytesRead, func);
        return ConsumeBytes();
    }

    public AbiDecoder Array<T>(out T[] value, Func<IArrayAbiDecoder, T[]> func)
    {
        value = DynamicType<T>.Array.Decode(_bytes, _bytesRead, func);
        return this;
    }

    public AbiDecoder NumberArray<TNumber>(bool isUnsigned, uint bitLength, out TNumber[] numbers)
    {

        if(bitLength % 8 != 0 || bitLength < 8 || bitLength > 256)
        {
            throw new ArgumentException("Invalid bitLength", nameof(bitLength));
        }
        //
        switch(bitLength)
        {
            case 8:
            {
                if(isUnsigned)
                {
                    byte[] n = DynamicType<object>.PrimitiveNumberArray<byte>.Decode(_bytes, _bytesRead);
                    numbers = n is TNumber[] b ? b : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(byte)}");
                }
                else
                {
                    sbyte[] n = DynamicType<object>.PrimitiveNumberArray<sbyte>.Decode(_bytes, _bytesRead);
                    numbers = n is TNumber[] b ? b : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(sbyte)}");
                }
                break;
            }
            case 16:
            {
                if(isUnsigned)
                {
                    ushort[] n = DynamicType<object>.PrimitiveNumberArray<ushort>.Decode(_bytes, _bytesRead);
                    numbers = n is TNumber[] b ? b : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(ushort)}");
                }
                else
                {
                    short[] n = DynamicType<object>.PrimitiveNumberArray<short>.Decode(_bytes, _bytesRead);
                    numbers = n is TNumber[] b ? b : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(short)}");
                }
                break;
            }
            case > 16 and <= 32:
            {
                if(isUnsigned)
                {
                    uint[] n = DynamicType<object>.PrimitiveNumberArray<uint>.Decode(_bytes, _bytesRead);
                    numbers = n is TNumber[] b ? b : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(uint)}");
                }
                else
                {
                    int[] n = DynamicType<object>.PrimitiveNumberArray<int>.Decode(_bytes, _bytesRead);
                    numbers = n is TNumber[] b ? b : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(int)}");
                }
                break;
            }
            case > 32 and <= 64:
            {
                if(isUnsigned)
                {
                    ulong[] n = DynamicType<object>.PrimitiveNumberArray<ulong>.Decode(_bytes, _bytesRead);
                    numbers = n is TNumber[] b ? b : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(ulong)}");
                }
                else
                {
                    long[] n = DynamicType<object>.PrimitiveNumberArray<long>.Decode(_bytes, _bytesRead);
                    numbers = n is TNumber[] b ? b : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(long)}");
                }
                break;
            }
            case > 64 and <= 256:
            {
                var n = DynamicType<object>.BigIntegerArray.Decode(_bytes, _bytesRead, bitLength, isUnsigned);
                numbers = n is TNumber[] b ? b : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {(isUnsigned ? "u-" : "")} {typeof(System.Numerics.BigInteger)}");
                break;
            }

            default:
                throw new NotImplementedException();
        }
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
            > 16 and <= 32 when isUnsigned => FixedType<object>.Int.Decode(CurrentSlot) is not TNumber b
                ? throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(int)}")
                : b,
            > 32 and <= 64 when isUnsigned => FixedType<object>.ULong.Decode(CurrentSlot) is not TNumber b
                ? throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(ulong)}")
                : b,
            > 32 and <= 64 when isUnsigned => FixedType<object>.Long.Decode(CurrentSlot) is not TNumber b
                ? throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(long)}")
                : b,
            > 64 and <= 256 => FixedType<object>.BigInteger.Decode(CurrentSlot, isUnsigned) is not TNumber b
                ? throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(BigInteger)}")
                : b,
            _ => throw new ArgumentException("Bitlength must be between 8 and 256", nameof(bitLength))
        };

        return this;
    }

    ReadOnlyMemory<byte> IStructAbiDecoder.Bytes()
    {
        _ = Bytes(out var value);
        return value;
    }
    string IStructAbiDecoder.String()
    {
        _ = String(out string? str);
        return str;
    }
    T IStructAbiDecoder.Struct<T>(Func<IStructAbiDecoder, T> func)
    {
        _ = Struct(out var value, func);
        return value;
    }
    T[] IStructAbiDecoder.Array<T>(out T[] value, Func<IArrayAbiDecoder, T[]> func)
    {
        _ = Array(out value, func);
        return value;
    }
    TNumber[] IStructAbiDecoder.NumberArray<TNumber>(bool isUnsigned, uint bitLength)
    {
        _ = NumberArray<TNumber>(isUnsigned, bitLength, out var numbers);
        return numbers;
    }
    TNumber IStructAbiDecoder.Number<TNumber>(bool isUnsigned, int bitLength)
    {
        _ = Number<TNumber>(out var number, isUnsigned, bitLength);
        return number;
    }

    T IArrayAbiDecoder.Struct<T>(Func<IStructAbiDecoder, T> func)
    {
        _ = Struct(out var value, func);
        return value;
    }
    T[] IArrayAbiDecoder.Array<T>(Func<IArrayAbiDecoder, T[]> func)
    {
        _ = Array(out var value, func);
        return value;
    }
}