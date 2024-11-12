using EtherSharp.ABI.Dynamic;
namespace EtherSharp.ABI.Decode;
public partial class AbiDecoder
{
    public AbiDecoder Int8Array(out byte[] bytes)
    {
        bytes = DynamicType<byte>.PrimitiveNumberArray<byte>.Decode(_bytes, _bytesRead);
        return ConsumeBytes();
    }

    public AbiDecoder UInt8Array(out sbyte[] bytes)
    {
        bytes = DynamicType<sbyte>.PrimitiveNumberArray<sbyte>.Decode(_bytes, _bytesRead);
        return ConsumeBytes();
    }
    public AbiDecoder Int16Array(out short[] shorts)
    {
        shorts = DynamicType<short>.PrimitiveNumberArray<short>.Decode(_bytes, _bytesRead);
        return ConsumeBytes();
    }

    public AbiDecoder UInt16Array(out ushort[] shorts)
    {
        shorts = DynamicType<ushort>.PrimitiveNumberArray<ushort>.Decode(_bytes, _bytesRead);
        return ConsumeBytes();
    }
    public AbiDecoder Int24Array(out int[] ints)
    {
        ints = DynamicType<int>.PrimitiveNumberArray<int>.Decode(_bytes, _bytesRead);
        return ConsumeBytes();
    }

    public AbiDecoder UInt24Array(out uint[] ints)
    {
        ints = DynamicType<uint>.PrimitiveNumberArray<uint>.Decode(_bytes, _bytesRead);
        return ConsumeBytes();
    }
    public AbiDecoder Int32Array(out int[] ints)
    {
        ints = DynamicType<int>.PrimitiveNumberArray<int>.Decode(_bytes, _bytesRead);
        return ConsumeBytes();
    }

    public AbiDecoder UInt32Array(out uint[] ints)
    {
        ints = DynamicType<uint>.PrimitiveNumberArray<uint>.Decode(_bytes, _bytesRead);
        return ConsumeBytes();
    }
    public AbiDecoder Int40Array(out long[] longs)
    {
        longs = DynamicType<long>.PrimitiveNumberArray<long>.Decode(_bytes, _bytesRead);
        return ConsumeBytes();
    }

    public AbiDecoder Int40Array(out ulong[] longs)
    {
        longs = DynamicType<ulong>.PrimitiveNumberArray<ulong>.Decode(_bytes, _bytesRead);
        return ConsumeBytes();
    }
    public AbiDecoder Int48Array(out long[] longs)
    {
        longs = DynamicType<long>.PrimitiveNumberArray<long>.Decode(_bytes, _bytesRead);
        return ConsumeBytes();
    }

    public AbiDecoder Int48Array(out ulong[] longs)
    {
        longs = DynamicType<ulong>.PrimitiveNumberArray<ulong>.Decode(_bytes, _bytesRead);
        return ConsumeBytes();
    }
    public AbiDecoder Int56Array(out long[] longs)
    {
        longs = DynamicType<long>.PrimitiveNumberArray<long>.Decode(_bytes, _bytesRead);
        return ConsumeBytes();
    }

    public AbiDecoder Int56Array(out ulong[] longs)
    {
        longs = DynamicType<ulong>.PrimitiveNumberArray<ulong>.Decode(_bytes, _bytesRead);
        return ConsumeBytes();
    }
    public AbiDecoder Int64Array(out long[] longs)
    {
        longs = DynamicType<long>.PrimitiveNumberArray<long>.Decode(_bytes, _bytesRead);
        return ConsumeBytes();
    }

    public AbiDecoder Int64Array(out ulong[] longs)
    {
        longs = DynamicType<ulong>.PrimitiveNumberArray<ulong>.Decode(_bytes, _bytesRead);
        return ConsumeBytes();
    }
    public AbiDecoder Int72Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 72, false);
        return ConsumeBytes();
    }
    public AbiDecoder UInt72Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 72, true);
        return ConsumeBytes();
    }
    public AbiDecoder Int80Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 80, false);
        return ConsumeBytes();
    }
    public AbiDecoder UInt80Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 80, true);
        return ConsumeBytes();
    }
    public AbiDecoder Int88Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 88, false);
        return ConsumeBytes();
    }
    public AbiDecoder UInt88Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 88, true);
        return ConsumeBytes();
    }
    public AbiDecoder Int96Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 96, false);
        return ConsumeBytes();
    }
    public AbiDecoder UInt96Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 96, true);
        return ConsumeBytes();
    }
    public AbiDecoder Int104Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 104, false);
        return ConsumeBytes();
    }
    public AbiDecoder UInt104Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 104, true);
        return ConsumeBytes();
    }
    public AbiDecoder Int112Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 112, false);
        return ConsumeBytes();
    }
    public AbiDecoder UInt112Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 112, true);
        return ConsumeBytes();
    }
    public AbiDecoder Int120Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 120, false);
        return ConsumeBytes();
    }
    public AbiDecoder UInt120Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 120, true);
        return ConsumeBytes();
    }
    public AbiDecoder Int128Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 128, false);
        return ConsumeBytes();
    }
    public AbiDecoder UInt128Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 128, true);
        return ConsumeBytes();
    }
    public AbiDecoder Int136Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 136, false);
        return ConsumeBytes();
    }
    public AbiDecoder UInt136Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 136, true);
        return ConsumeBytes();
    }
    public AbiDecoder Int144Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 144, false);
        return ConsumeBytes();
    }
    public AbiDecoder UInt144Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 144, true);
        return ConsumeBytes();
    }
    public AbiDecoder Int152Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 152, false);
        return ConsumeBytes();
    }
    public AbiDecoder UInt152Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 152, true);
        return ConsumeBytes();
    }
    public AbiDecoder Int160Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 160, false);
        return ConsumeBytes();
    }
    public AbiDecoder UInt160Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 160, true);
        return ConsumeBytes();
    }
    public AbiDecoder Int168Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 168, false);
        return ConsumeBytes();
    }
    public AbiDecoder UInt168Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 168, true);
        return ConsumeBytes();
    }
    public AbiDecoder Int176Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 176, false);
        return ConsumeBytes();
    }
    public AbiDecoder UInt176Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 176, true);
        return ConsumeBytes();
    }
    public AbiDecoder Int184Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 184, false);
        return ConsumeBytes();
    }
    public AbiDecoder UInt184Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 184, true);
        return ConsumeBytes();
    }
    public AbiDecoder Int192Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 192, false);
        return ConsumeBytes();
    }
    public AbiDecoder UInt192Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 192, true);
        return ConsumeBytes();
    }
    public AbiDecoder Int200Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 200, false);
        return ConsumeBytes();
    }
    public AbiDecoder UInt200Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 200, true);
        return ConsumeBytes();
    }
    public AbiDecoder Int208Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 208, false);
        return ConsumeBytes();
    }
    public AbiDecoder UInt208Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 208, true);
        return ConsumeBytes();
    }
    public AbiDecoder Int216Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 216, false);
        return ConsumeBytes();
    }
    public AbiDecoder UInt216Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 216, true);
        return ConsumeBytes();
    }
    public AbiDecoder Int224Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 224, false);
        return ConsumeBytes();
    }
    public AbiDecoder UInt224Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 224, true);
        return ConsumeBytes();
    }
    public AbiDecoder Int232Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 232, false);
        return ConsumeBytes();
    }
    public AbiDecoder UInt232Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 232, true);
        return ConsumeBytes();
    }
    public AbiDecoder Int240Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 240, false);
        return ConsumeBytes();
    }
    public AbiDecoder UInt240Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 240, true);
        return ConsumeBytes();
    }
    public AbiDecoder Int248Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 248, false);
        return ConsumeBytes();
    }
    public AbiDecoder UInt248Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 248, true);
        return ConsumeBytes();
    }
    public AbiDecoder Int256Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 256, false);
        return ConsumeBytes();
    }
    public AbiDecoder UInt256Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 256, true);
        return ConsumeBytes();
    }
}