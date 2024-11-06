using EtherSharp.ABI.Dynamic;
namespace EtherSharp.ABI.Decode;
public partial class AbiDecoder
{
    public AbiDecoder Int8Array(out byte[] bytes)
    {
        bytes = DynamicType<byte>.PrimitiveNumberArray<byte>.Decode(_bytes[(int) _currentMetadataIndex..], _currentMetadataIndex, this);
        return this;
    }

    public AbiDecoder UInt8Array(out sbyte[] bytes)
    {
        bytes = DynamicType<sbyte>.PrimitiveNumberArray<sbyte>.Decode(_bytes[(int) _currentMetadataIndex..], _currentMetadataIndex, this);
        return this;
    }
    public AbiDecoder Int16Array(out short[] shorts)
    {
        shorts = DynamicType<short>.PrimitiveNumberArray<short>.Decode(_bytes[(int) _currentMetadataIndex..], _currentMetadataIndex, this);
        return this;
    }

    public AbiDecoder UInt16Array(out ushort[] shorts)
    {
        shorts = DynamicType<ushort>.PrimitiveNumberArray<ushort>.Decode(_bytes[(int) _currentMetadataIndex..], _currentMetadataIndex, this);
        return this;
    }
    public AbiDecoder Int24Array(out int[] ints)
    {
        ints = DynamicType<int>.PrimitiveNumberArray<int>.Decode(_bytes[(int) _currentMetadataIndex..], _currentMetadataIndex, this);
        return this;
    }

    public AbiDecoder UInt24Array(out uint[] ints)
    {
        ints = DynamicType<uint>.PrimitiveNumberArray<uint>.Decode(_bytes[(int) _currentMetadataIndex..], _currentMetadataIndex, this);
        return this;
    }
    public AbiDecoder Int32Array(out int[] ints)
    {
        ints = DynamicType<int>.PrimitiveNumberArray<int>.Decode(_bytes[(int) _currentMetadataIndex..], _currentMetadataIndex, this);
        return this;
    }

    public AbiDecoder UInt32Array(out uint[] ints)
    {
        ints = DynamicType<uint>.PrimitiveNumberArray<uint>.Decode(_bytes[(int) _currentMetadataIndex..], _currentMetadataIndex, this);
        return this;
    }
    public AbiDecoder Int40Array(out long[] longs)
    {
        longs = DynamicType<long>.PrimitiveNumberArray<long>.Decode(_bytes[(int) _currentMetadataIndex..], _currentMetadataIndex, this);
        return this;
    }

    public AbiDecoder Int40Array(out ulong[] longs)
    {
        longs = DynamicType<ulong>.PrimitiveNumberArray<ulong>.Decode(_bytes[(int) _currentMetadataIndex..], _currentMetadataIndex, this);
        return this;
    }
    public AbiDecoder Int48Array(out long[] longs)
    {
        longs = DynamicType<long>.PrimitiveNumberArray<long>.Decode(_bytes[(int) _currentMetadataIndex..], _currentMetadataIndex, this);
        return this;
    }

    public AbiDecoder Int48Array(out ulong[] longs)
    {
        longs = DynamicType<ulong>.PrimitiveNumberArray<ulong>.Decode(_bytes[(int) _currentMetadataIndex..], _currentMetadataIndex, this);
        return this;
    }
    public AbiDecoder Int56Array(out long[] longs)
    {
        longs = DynamicType<long>.PrimitiveNumberArray<long>.Decode(_bytes[(int) _currentMetadataIndex..], _currentMetadataIndex, this);
        return this;
    }

    public AbiDecoder Int56Array(out ulong[] longs)
    {
        longs = DynamicType<ulong>.PrimitiveNumberArray<ulong>.Decode(_bytes[(int) _currentMetadataIndex..], _currentMetadataIndex, this);
        return this;
    }
    public AbiDecoder Int64Array(out long[] longs)
    {
        longs = DynamicType<long>.PrimitiveNumberArray<long>.Decode(_bytes[(int) _currentMetadataIndex..], _currentMetadataIndex, this);
        return this;
    }

    public AbiDecoder Int64Array(out ulong[] longs)
    {
        longs = DynamicType<ulong>.PrimitiveNumberArray<ulong>.Decode(_bytes[(int) _currentMetadataIndex..], _currentMetadataIndex, this);
        return this;
    }
    public AbiDecoder Int72Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes[(int) _currentMetadataIndex..], _currentMetadataIndex, 72, false, this);
        return this;
    }
    public AbiDecoder UInt72Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes[(int) _currentMetadataIndex..], _currentMetadataIndex, 72, true, this);
        return this;
    }
    public AbiDecoder Int80Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes[(int) _currentMetadataIndex..], _currentMetadataIndex, 80, false, this);
        return this;
    }
    public AbiDecoder UInt80Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes[(int) _currentMetadataIndex..], _currentMetadataIndex, 80, true, this);
        return this;
    }
    public AbiDecoder Int88Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes[(int) _currentMetadataIndex..], _currentMetadataIndex, 88, false, this);
        return this;
    }
    public AbiDecoder UInt88Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes[(int) _currentMetadataIndex..], _currentMetadataIndex, 88, true, this);
        return this;
    }
    public AbiDecoder Int96Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes[(int) _currentMetadataIndex..], _currentMetadataIndex, 96, false, this);
        return this;
    }
    public AbiDecoder UInt96Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes[(int) _currentMetadataIndex..], _currentMetadataIndex, 96, true, this);
        return this;
    }
    public AbiDecoder Int104Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes[(int) _currentMetadataIndex..], _currentMetadataIndex, 104, false, this);
        return this;
    }
    public AbiDecoder UInt104Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes[(int) _currentMetadataIndex..], _currentMetadataIndex, 104, true, this);
        return this;
    }
    public AbiDecoder Int112Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes[(int) _currentMetadataIndex..], _currentMetadataIndex, 112, false, this);
        return this;
    }
    public AbiDecoder UInt112Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes[(int) _currentMetadataIndex..], _currentMetadataIndex, 112, true, this);
        return this;
    }
    public AbiDecoder Int120Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes[(int) _currentMetadataIndex..], _currentMetadataIndex, 120, false, this);
        return this;
    }
    public AbiDecoder UInt120Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes[(int) _currentMetadataIndex..], _currentMetadataIndex, 120, true, this);
        return this;
    }
    public AbiDecoder Int128Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes[(int) _currentMetadataIndex..], _currentMetadataIndex, 128, false, this);
        return this;
    }
    public AbiDecoder UInt128Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes[(int) _currentMetadataIndex..], _currentMetadataIndex, 128, true, this);
        return this;
    }
    public AbiDecoder Int136Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes[(int) _currentMetadataIndex..], _currentMetadataIndex, 136, false, this);
        return this;
    }
    public AbiDecoder UInt136Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes[(int) _currentMetadataIndex..], _currentMetadataIndex, 136, true, this);
        return this;
    }
    public AbiDecoder Int144Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes[(int) _currentMetadataIndex..], _currentMetadataIndex, 144, false, this);
        return this;
    }
    public AbiDecoder UInt144Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes[(int) _currentMetadataIndex..], _currentMetadataIndex, 144, true, this);
        return this;
    }
    public AbiDecoder Int152Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes[(int) _currentMetadataIndex..], _currentMetadataIndex, 152, false, this);
        return this;
    }
    public AbiDecoder UInt152Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes[(int) _currentMetadataIndex..], _currentMetadataIndex, 152, true, this);
        return this;
    }
    public AbiDecoder Int160Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes[(int) _currentMetadataIndex..], _currentMetadataIndex, 160, false, this);
        return this;
    }
    public AbiDecoder UInt160Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes[(int) _currentMetadataIndex..], _currentMetadataIndex, 160, true, this);
        return this;
    }
    public AbiDecoder Int168Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes[(int) _currentMetadataIndex..], _currentMetadataIndex, 168, false, this);
        return this;
    }
    public AbiDecoder UInt168Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes[(int) _currentMetadataIndex..], _currentMetadataIndex, 168, true, this);
        return this;
    }
    public AbiDecoder Int176Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes[(int) _currentMetadataIndex..], _currentMetadataIndex, 176, false, this);
        return this;
    }
    public AbiDecoder UInt176Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes[(int) _currentMetadataIndex..], _currentMetadataIndex, 176, true, this);
        return this;
    }
    public AbiDecoder Int184Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes[(int) _currentMetadataIndex..], _currentMetadataIndex, 184, false, this);
        return this;
    }
    public AbiDecoder UInt184Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes[(int) _currentMetadataIndex..], _currentMetadataIndex, 184, true, this);
        return this;
    }
    public AbiDecoder Int192Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes[(int) _currentMetadataIndex..], _currentMetadataIndex, 192, false, this);
        return this;
    }
    public AbiDecoder UInt192Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes[(int) _currentMetadataIndex..], _currentMetadataIndex, 192, true, this);
        return this;
    }
    public AbiDecoder Int200Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes[(int) _currentMetadataIndex..], _currentMetadataIndex, 200, false, this);
        return this;
    }
    public AbiDecoder UInt200Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes[(int) _currentMetadataIndex..], _currentMetadataIndex, 200, true, this);
        return this;
    }
    public AbiDecoder Int208Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes[(int) _currentMetadataIndex..], _currentMetadataIndex, 208, false, this);
        return this;
    }
    public AbiDecoder UInt208Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes[(int) _currentMetadataIndex..], _currentMetadataIndex, 208, true, this);
        return this;
    }
    public AbiDecoder Int216Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes[(int) _currentMetadataIndex..], _currentMetadataIndex, 216, false, this);
        return this;
    }
    public AbiDecoder UInt216Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes[(int) _currentMetadataIndex..], _currentMetadataIndex, 216, true, this);
        return this;
    }
    public AbiDecoder Int224Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes[(int) _currentMetadataIndex..], _currentMetadataIndex, 224, false, this);
        return this;
    }
    public AbiDecoder UInt224Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes[(int) _currentMetadataIndex..], _currentMetadataIndex, 224, true, this);
        return this;
    }
    public AbiDecoder Int232Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes[(int) _currentMetadataIndex..], _currentMetadataIndex, 232, false, this);
        return this;
    }
    public AbiDecoder UInt232Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes[(int) _currentMetadataIndex..], _currentMetadataIndex, 232, true, this);
        return this;
    }
    public AbiDecoder Int240Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes[(int) _currentMetadataIndex..], _currentMetadataIndex, 240, false, this);
        return this;
    }
    public AbiDecoder UInt240Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes[(int) _currentMetadataIndex..], _currentMetadataIndex, 240, true, this);
        return this;
    }
    public AbiDecoder Int248Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes[(int) _currentMetadataIndex..], _currentMetadataIndex, 248, false, this);
        return this;
    }
    public AbiDecoder UInt248Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes[(int) _currentMetadataIndex..], _currentMetadataIndex, 248, true, this);
        return this;
    }
    public AbiDecoder Int256Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes[(int) _currentMetadataIndex..], _currentMetadataIndex, 256, false, this);
        return this;
    }
    public AbiDecoder UInt256Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes[(int) _currentMetadataIndex..], _currentMetadataIndex, 256, true, this);
        return this;
    }
}