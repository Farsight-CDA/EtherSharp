using EtherSharp.ABI.Dynamic;
using EtherSharp.ABI.Fixed;

namespace EtherSharp.ABI.Decode;

public partial class AbiDecoder(Memory<byte> bytes)
{
    private readonly Memory<byte> _bytes = bytes;

    private Span<byte> Bytes => _bytes.Span[(int) _currentMetadataIndex..];

    private uint _currentMetadataIndex = 0;

    private AbiDecoder ConsumeBytes(uint payloadSize)
    {
        _currentMetadataIndex += 32;
        return this;
    }

    public AbiDecoder Int8(out sbyte value)
    {
        value = FixedType<object>.SByte.Decode(Bytes);
        return ConsumeBytes(0);
    }

    public AbiDecoder UInt8(out byte value)
    {
        value = FixedType<object>.Byte.Decode(Bytes);
        return ConsumeBytes(0);
    }

    public AbiDecoder Struct<T>(out T value, Func<StructAbiDecoder, T> func)
    {
        value = DynamicType<T>.Struct.Decode(_bytes[(int) _currentMetadataIndex..], _currentMetadataIndex, func);
        return this;
    }

    public AbiDecoder Array<T>(out T[] value, Func<ArrayAbiDecoder, T[]> func)
    {
        value = DynamicType<T>.Array.Decode(_bytes[(int) _currentMetadataIndex..], _currentMetadataIndex, func);
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
                    byte[] n = DynamicType<object>.PrimitiveNumberArray<byte>.Decode(_bytes[(int) _currentMetadataIndex..], _currentMetadataIndex, this);
                    numbers = n is TNumber[] b ? b : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(byte)}");
                }
                else
                {
                    sbyte[] n = DynamicType<object>.PrimitiveNumberArray<sbyte>.Decode(_bytes[(int) _currentMetadataIndex..], _currentMetadataIndex, this);
                    numbers = n is TNumber[] b ? b : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(sbyte)}");
                }
                break;
            }
            case 16:
            {
                if(isUnsigned)
                {
                    ushort[] n = DynamicType<object>.PrimitiveNumberArray<ushort>.Decode(_bytes[(int) _currentMetadataIndex..], _currentMetadataIndex, this);
                    numbers = n is TNumber[] b ? b : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(ushort)}");
                }
                else
                {
                    short[] n = DynamicType<object>.PrimitiveNumberArray<short>.Decode(_bytes[(int) _currentMetadataIndex..], _currentMetadataIndex, this);
                    numbers = n is TNumber[] b ? b : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(short)}");
                }
                break;
            }
            case > 16 and <= 32:
            {
                if(isUnsigned)
                {
                    uint[] n = DynamicType<object>.PrimitiveNumberArray<uint>.Decode(_bytes[(int) _currentMetadataIndex..], _currentMetadataIndex, this);
                    numbers = n is TNumber[] b ? b : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(uint)}");
                }
                else
                {
                    int[] n = DynamicType<object>.PrimitiveNumberArray<int>.Decode(_bytes[(int) _currentMetadataIndex..], _currentMetadataIndex, this);
                    numbers = n is TNumber[] b ? b : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(int)}");
                }
                break;
            }
            case > 32 and <= 64:
            {
                if(isUnsigned)
                {
                    ulong[] n = DynamicType<object>.PrimitiveNumberArray<ulong>.Decode(_bytes[(int) _currentMetadataIndex..], _currentMetadataIndex, this);
                    numbers = n is TNumber[] b ? b : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(ulong)}");
                }
                else
                {
                    long[] n = DynamicType<object>.PrimitiveNumberArray<long>.Decode(_bytes[(int) _currentMetadataIndex..], _currentMetadataIndex, this);
                    numbers = n is TNumber[] b ? b : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(long)}");
                }
                break;
            }
            case > 64 and <= 256:
            {
                var n = DynamicType<object>.BigIntegerArray.Decode(_bytes[(int) _currentMetadataIndex..], _currentMetadataIndex, bitLength, isUnsigned, this);
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
        //
        switch(bitLength)
        {
            case 8:
            {
                if(isUnsigned)
                {
                    byte n = FixedType<object>.Byte.Decode(Bytes);
                    number = n is TNumber b ? b : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(byte)}");
                }
                else
                {
                    sbyte n = FixedType<object>.SByte.Decode(Bytes);
                    number = n is TNumber b ? b : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(sbyte)}");
                }
                break;
            }
            case 16:
            {
                if(isUnsigned)
                {
                    ushort n = FixedType<object>.UShort.Decode(Bytes);
                    number = n is TNumber b ? b : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(ushort)}");
                }
                else
                {
                    short n = FixedType<object>.Short.Decode(Bytes);
                    number = n is TNumber b ? b : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(short)}");
                }
                break;
            }
            case > 16 and <= 32:
            {
                if(isUnsigned)
                {
                    uint n = FixedType<object>.UInt.Decode(Bytes);
                    number = n is TNumber b ? b : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(uint)}");
                }
                else
                {
                    int n = FixedType<object>.Int.Decode(Bytes);
                    number = n is TNumber b ? b : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(int)}");
                }
                break;
            }
            case > 32 and <= 64:
            {
                if(isUnsigned)
                {
                    ulong n = FixedType<object>.ULong.Decode(Bytes);
                    number = n is TNumber b ? b : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(ulong)}");
                }
                else
                {
                    long n = FixedType<object>.Long.Decode(Bytes);
                    number = n is TNumber b ? b : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {typeof(long)}");
                }
                break;
            }
            case > 64 and <= 256:
            {
                var n = FixedType<object>.BigInteger.Decode(Bytes, isUnsigned);
                number = n is TNumber b ? b : throw new ArgumentException($"Unexpected number type for length {bitLength}, expected {(isUnsigned ? "u-" : "")} {typeof(System.Numerics.BigInteger)}");
                break;
            }

            default:
                throw new NotImplementedException();
        }
        return this;
    }
}