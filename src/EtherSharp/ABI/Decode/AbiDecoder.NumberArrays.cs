using EtherSharp.ABI.Decode.Interfaces;
using EtherSharp.ABI.Dynamic;
using System.Numerics;
namespace EtherSharp.ABI.Decode;
public partial class AbiDecoder
{

    public AbiDecoder Int8Array(out sbyte[] bytes)
    {
        bytes = DynamicType<object>.PrimitiveNumberArray<sbyte>.Decode(_bytes, _bytesRead);
        return ConsumeBytes();
    }
    sbyte[] IArrayAbiDecoder.Int8Array()
    {
        _ = Int8Array(out sbyte[] output);
        return output;
    }
    sbyte[] IStructAbiDecoder.Int8Array()
    {
        _ = Int8Array(out sbyte[] output);
        return output;
    }
    public AbiDecoder UInt8Array(out byte[] bytes)
    {
        bytes = DynamicType<object>.PrimitiveNumberArray<byte>.Decode(_bytes, _bytesRead);
        return ConsumeBytes();
    }
    byte[] IArrayAbiDecoder.UInt8Array()
    {
        _ = UInt8Array(out byte[] output);
        return output;
    }
    byte[] IStructAbiDecoder.UInt8Array()
    {
        _ = UInt8Array(out byte[] output);
        return output;
    }

    public AbiDecoder Int16Array(out short[] shorts)
    {
        shorts = DynamicType<short>.PrimitiveNumberArray<short>.Decode(_bytes, _bytesRead);
        return ConsumeBytes();
    }
    short[] IArrayAbiDecoder.Int16Array()
    {
        _ = Int16Array(out short[] output);
        return output;
    }
    short[] IStructAbiDecoder.Int16Array()
    {
        _ = Int16Array(out short[] output);
        return output;
    }
    public AbiDecoder UInt16Array(out ushort[] shorts)
    {
        shorts = DynamicType<ushort>.PrimitiveNumberArray<ushort>.Decode(_bytes, _bytesRead);
        return ConsumeBytes();
    }
    ushort[] IArrayAbiDecoder.UInt16Array()
    {
        _ = UInt16Array(out ushort[] output);
        return output;
    }
    ushort[] IStructAbiDecoder.UInt16Array()
    {
        _ = UInt16Array(out ushort[] output);
        return output;
    }

    public AbiDecoder Int24Array(out int[] ints)
    {
        ints = DynamicType<int>.PrimitiveNumberArray<int>.Decode(_bytes, _bytesRead);
        return ConsumeBytes();
    }
    int[] IArrayAbiDecoder.Int24Array()
    {
        _ = Int24Array(out int[] output);
        return output;
    }
    int[] IStructAbiDecoder.Int24Array()
    {
        _ = Int24Array(out int[] output);
        return output;
    }
    public AbiDecoder UInt24Array(out uint[] ints)
    {
        ints = DynamicType<uint>.PrimitiveNumberArray<uint>.Decode(_bytes, _bytesRead);
        return ConsumeBytes();
    }
    uint[] IArrayAbiDecoder.UInt24Array()
    {
        _ = UInt24Array(out uint[] output);
        return output;
    }
    uint[] IStructAbiDecoder.UInt24Array()
    {
        _ = UInt24Array(out uint[] output);
        return output;
    }
    public AbiDecoder Int32Array(out int[] ints)
    {
        ints = DynamicType<int>.PrimitiveNumberArray<int>.Decode(_bytes, _bytesRead);
        return ConsumeBytes();
    }
    int[] IArrayAbiDecoder.Int32Array()
    {
        _ = Int32Array(out int[] output);
        return output;
    }
    int[] IStructAbiDecoder.Int32Array()
    {
        _ = Int32Array(out int[] output);
        return output;
    }
    public AbiDecoder UInt32Array(out uint[] ints)
    {
        ints = DynamicType<uint>.PrimitiveNumberArray<uint>.Decode(_bytes, _bytesRead);
        return ConsumeBytes();
    }
    uint[] IArrayAbiDecoder.UInt32Array()
    {
        _ = UInt32Array(out uint[] output);
        return output;
    }
    uint[] IStructAbiDecoder.UInt32Array()
    {
        _ = UInt32Array(out uint[] output);
        return output;
    }
    public AbiDecoder Int40Array(out long[] longs)
    {
        longs = DynamicType<object>.PrimitiveNumberArray<long>.Decode(_bytes, _bytesRead);
        return ConsumeBytes();
    }
    long[] IArrayAbiDecoder.Int40Array()
    {
        _ = Int40Array(out long[] output);
        return output;
    }
    long[] IStructAbiDecoder.Int40Array()
    {
        _ = Int40Array(out long[] output);
        return output;
    }
    public AbiDecoder UInt40Array(out ulong[] longs)
    {
        longs = DynamicType<ulong>.PrimitiveNumberArray<ulong>.Decode(_bytes, _bytesRead);
        return ConsumeBytes();
    }
    ulong[] IArrayAbiDecoder.UInt40Array()
    {
        _ = UInt40Array(out ulong[] output);
        return output;
    }
    ulong[] IStructAbiDecoder.UInt40Array()
    {
        _ = UInt40Array(out ulong[] output);
        return output;
    }
    public AbiDecoder Int48Array(out long[] longs)
    {
        longs = DynamicType<object>.PrimitiveNumberArray<long>.Decode(_bytes, _bytesRead);
        return ConsumeBytes();
    }
    long[] IArrayAbiDecoder.Int48Array()
    {
        _ = Int48Array(out long[] output);
        return output;
    }
    long[] IStructAbiDecoder.Int48Array()
    {
        _ = Int48Array(out long[] output);
        return output;
    }
    public AbiDecoder UInt48Array(out ulong[] longs)
    {
        longs = DynamicType<ulong>.PrimitiveNumberArray<ulong>.Decode(_bytes, _bytesRead);
        return ConsumeBytes();
    }
    ulong[] IArrayAbiDecoder.UInt48Array()
    {
        _ = UInt48Array(out ulong[] output);
        return output;
    }
    ulong[] IStructAbiDecoder.UInt48Array()
    {
        _ = UInt48Array(out ulong[] output);
        return output;
    }
    public AbiDecoder Int56Array(out long[] longs)
    {
        longs = DynamicType<object>.PrimitiveNumberArray<long>.Decode(_bytes, _bytesRead);
        return ConsumeBytes();
    }
    long[] IArrayAbiDecoder.Int56Array()
    {
        _ = Int56Array(out long[] output);
        return output;
    }
    long[] IStructAbiDecoder.Int56Array()
    {
        _ = Int56Array(out long[] output);
        return output;
    }
    public AbiDecoder UInt56Array(out ulong[] longs)
    {
        longs = DynamicType<ulong>.PrimitiveNumberArray<ulong>.Decode(_bytes, _bytesRead);
        return ConsumeBytes();
    }
    ulong[] IArrayAbiDecoder.UInt56Array()
    {
        _ = UInt56Array(out ulong[] output);
        return output;
    }
    ulong[] IStructAbiDecoder.UInt56Array()
    {
        _ = UInt56Array(out ulong[] output);
        return output;
    }
    public AbiDecoder Int64Array(out long[] longs)
    {
        longs = DynamicType<object>.PrimitiveNumberArray<long>.Decode(_bytes, _bytesRead);
        return ConsumeBytes();
    }
    long[] IArrayAbiDecoder.Int64Array()
    {
        _ = Int64Array(out long[] output);
        return output;
    }
    long[] IStructAbiDecoder.Int64Array()
    {
        _ = Int64Array(out long[] output);
        return output;
    }
    public AbiDecoder UInt64Array(out ulong[] longs)
    {
        longs = DynamicType<ulong>.PrimitiveNumberArray<ulong>.Decode(_bytes, _bytesRead);
        return ConsumeBytes();
    }
    ulong[] IArrayAbiDecoder.UInt64Array()
    {
        _ = UInt64Array(out ulong[] output);
        return output;
    }
    ulong[] IStructAbiDecoder.UInt64Array()
    {
        _ = UInt64Array(out ulong[] output);
        return output;
    }
    public AbiDecoder Int72Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 72, false);
        return ConsumeBytes();
    }
    BigInteger[] IArrayAbiDecoder.Int72Array()
    {
        _ = UInt72Array(out var output);
        return output;
    }
    BigInteger[] IStructAbiDecoder.Int72Array()
    {
        _ = UInt72Array(out var output);
        return output;
    }
    public AbiDecoder UInt72Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 72, true);
        return ConsumeBytes();
    }
    BigInteger[] IArrayAbiDecoder.UInt72Array()
    {
        _ = UInt72Array(out var output);
        return output;
    }
    BigInteger[] IStructAbiDecoder.UInt72Array()
    {
        _ = UInt72Array(out var output);
        return output;
    }
    public AbiDecoder Int80Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 80, false);
        return ConsumeBytes();
    }
    BigInteger[] IArrayAbiDecoder.Int80Array()
    {
        _ = UInt80Array(out var output);
        return output;
    }
    BigInteger[] IStructAbiDecoder.Int80Array()
    {
        _ = UInt80Array(out var output);
        return output;
    }
    public AbiDecoder UInt80Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 80, true);
        return ConsumeBytes();
    }
    BigInteger[] IArrayAbiDecoder.UInt80Array()
    {
        _ = UInt80Array(out var output);
        return output;
    }
    BigInteger[] IStructAbiDecoder.UInt80Array()
    {
        _ = UInt80Array(out var output);
        return output;
    }
    public AbiDecoder Int88Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 88, false);
        return ConsumeBytes();
    }
    BigInteger[] IArrayAbiDecoder.Int88Array()
    {
        _ = UInt88Array(out var output);
        return output;
    }
    BigInteger[] IStructAbiDecoder.Int88Array()
    {
        _ = UInt88Array(out var output);
        return output;
    }
    public AbiDecoder UInt88Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 88, true);
        return ConsumeBytes();
    }
    BigInteger[] IArrayAbiDecoder.UInt88Array()
    {
        _ = UInt88Array(out var output);
        return output;
    }
    BigInteger[] IStructAbiDecoder.UInt88Array()
    {
        _ = UInt88Array(out var output);
        return output;
    }
    public AbiDecoder Int96Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 96, false);
        return ConsumeBytes();
    }
    BigInteger[] IArrayAbiDecoder.Int96Array()
    {
        _ = UInt96Array(out var output);
        return output;
    }
    BigInteger[] IStructAbiDecoder.Int96Array()
    {
        _ = UInt96Array(out var output);
        return output;
    }
    public AbiDecoder UInt96Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 96, true);
        return ConsumeBytes();
    }
    BigInteger[] IArrayAbiDecoder.UInt96Array()
    {
        _ = UInt96Array(out var output);
        return output;
    }
    BigInteger[] IStructAbiDecoder.UInt96Array()
    {
        _ = UInt96Array(out var output);
        return output;
    }
    public AbiDecoder Int104Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 104, false);
        return ConsumeBytes();
    }
    BigInteger[] IArrayAbiDecoder.Int104Array()
    {
        _ = UInt104Array(out var output);
        return output;
    }
    BigInteger[] IStructAbiDecoder.Int104Array()
    {
        _ = UInt104Array(out var output);
        return output;
    }
    public AbiDecoder UInt104Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 104, true);
        return ConsumeBytes();
    }
    BigInteger[] IArrayAbiDecoder.UInt104Array()
    {
        _ = UInt104Array(out var output);
        return output;
    }
    BigInteger[] IStructAbiDecoder.UInt104Array()
    {
        _ = UInt104Array(out var output);
        return output;
    }
    public AbiDecoder Int112Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 112, false);
        return ConsumeBytes();
    }
    BigInteger[] IArrayAbiDecoder.Int112Array()
    {
        _ = UInt112Array(out var output);
        return output;
    }
    BigInteger[] IStructAbiDecoder.Int112Array()
    {
        _ = UInt112Array(out var output);
        return output;
    }
    public AbiDecoder UInt112Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 112, true);
        return ConsumeBytes();
    }
    BigInteger[] IArrayAbiDecoder.UInt112Array()
    {
        _ = UInt112Array(out var output);
        return output;
    }
    BigInteger[] IStructAbiDecoder.UInt112Array()
    {
        _ = UInt112Array(out var output);
        return output;
    }
    public AbiDecoder Int120Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 120, false);
        return ConsumeBytes();
    }
    BigInteger[] IArrayAbiDecoder.Int120Array()
    {
        _ = UInt120Array(out var output);
        return output;
    }
    BigInteger[] IStructAbiDecoder.Int120Array()
    {
        _ = UInt120Array(out var output);
        return output;
    }
    public AbiDecoder UInt120Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 120, true);
        return ConsumeBytes();
    }
    BigInteger[] IArrayAbiDecoder.UInt120Array()
    {
        _ = UInt120Array(out var output);
        return output;
    }
    BigInteger[] IStructAbiDecoder.UInt120Array()
    {
        _ = UInt120Array(out var output);
        return output;
    }
    public AbiDecoder Int128Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 128, false);
        return ConsumeBytes();
    }
    BigInteger[] IArrayAbiDecoder.Int128Array()
    {
        _ = UInt128Array(out var output);
        return output;
    }
    BigInteger[] IStructAbiDecoder.Int128Array()
    {
        _ = UInt128Array(out var output);
        return output;
    }
    public AbiDecoder UInt128Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 128, true);
        return ConsumeBytes();
    }
    BigInteger[] IArrayAbiDecoder.UInt128Array()
    {
        _ = UInt128Array(out var output);
        return output;
    }
    BigInteger[] IStructAbiDecoder.UInt128Array()
    {
        _ = UInt128Array(out var output);
        return output;
    }
    public AbiDecoder Int136Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 136, false);
        return ConsumeBytes();
    }
    BigInteger[] IArrayAbiDecoder.Int136Array()
    {
        _ = UInt136Array(out var output);
        return output;
    }
    BigInteger[] IStructAbiDecoder.Int136Array()
    {
        _ = UInt136Array(out var output);
        return output;
    }
    public AbiDecoder UInt136Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 136, true);
        return ConsumeBytes();
    }
    BigInteger[] IArrayAbiDecoder.UInt136Array()
    {
        _ = UInt136Array(out var output);
        return output;
    }
    BigInteger[] IStructAbiDecoder.UInt136Array()
    {
        _ = UInt136Array(out var output);
        return output;
    }
    public AbiDecoder Int144Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 144, false);
        return ConsumeBytes();
    }
    BigInteger[] IArrayAbiDecoder.Int144Array()
    {
        _ = UInt144Array(out var output);
        return output;
    }
    BigInteger[] IStructAbiDecoder.Int144Array()
    {
        _ = UInt144Array(out var output);
        return output;
    }
    public AbiDecoder UInt144Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 144, true);
        return ConsumeBytes();
    }
    BigInteger[] IArrayAbiDecoder.UInt144Array()
    {
        _ = UInt144Array(out var output);
        return output;
    }
    BigInteger[] IStructAbiDecoder.UInt144Array()
    {
        _ = UInt144Array(out var output);
        return output;
    }
    public AbiDecoder Int152Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 152, false);
        return ConsumeBytes();
    }
    BigInteger[] IArrayAbiDecoder.Int152Array()
    {
        _ = UInt152Array(out var output);
        return output;
    }
    BigInteger[] IStructAbiDecoder.Int152Array()
    {
        _ = UInt152Array(out var output);
        return output;
    }
    public AbiDecoder UInt152Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 152, true);
        return ConsumeBytes();
    }
    BigInteger[] IArrayAbiDecoder.UInt152Array()
    {
        _ = UInt152Array(out var output);
        return output;
    }
    BigInteger[] IStructAbiDecoder.UInt152Array()
    {
        _ = UInt152Array(out var output);
        return output;
    }
    public AbiDecoder Int160Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 160, false);
        return ConsumeBytes();
    }
    BigInteger[] IArrayAbiDecoder.Int160Array()
    {
        _ = UInt160Array(out var output);
        return output;
    }
    BigInteger[] IStructAbiDecoder.Int160Array()
    {
        _ = UInt160Array(out var output);
        return output;
    }
    public AbiDecoder UInt160Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 160, true);
        return ConsumeBytes();
    }
    BigInteger[] IArrayAbiDecoder.UInt160Array()
    {
        _ = UInt160Array(out var output);
        return output;
    }
    BigInteger[] IStructAbiDecoder.UInt160Array()
    {
        _ = UInt160Array(out var output);
        return output;
    }
    public AbiDecoder Int168Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 168, false);
        return ConsumeBytes();
    }
    BigInteger[] IArrayAbiDecoder.Int168Array()
    {
        _ = UInt168Array(out var output);
        return output;
    }
    BigInteger[] IStructAbiDecoder.Int168Array()
    {
        _ = UInt168Array(out var output);
        return output;
    }
    public AbiDecoder UInt168Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 168, true);
        return ConsumeBytes();
    }
    BigInteger[] IArrayAbiDecoder.UInt168Array()
    {
        _ = UInt168Array(out var output);
        return output;
    }
    BigInteger[] IStructAbiDecoder.UInt168Array()
    {
        _ = UInt168Array(out var output);
        return output;
    }
    public AbiDecoder Int176Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 176, false);
        return ConsumeBytes();
    }
    BigInteger[] IArrayAbiDecoder.Int176Array()
    {
        _ = UInt176Array(out var output);
        return output;
    }
    BigInteger[] IStructAbiDecoder.Int176Array()
    {
        _ = UInt176Array(out var output);
        return output;
    }
    public AbiDecoder UInt176Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 176, true);
        return ConsumeBytes();
    }
    BigInteger[] IArrayAbiDecoder.UInt176Array()
    {
        _ = UInt176Array(out var output);
        return output;
    }
    BigInteger[] IStructAbiDecoder.UInt176Array()
    {
        _ = UInt176Array(out var output);
        return output;
    }
    public AbiDecoder Int184Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 184, false);
        return ConsumeBytes();
    }
    BigInteger[] IArrayAbiDecoder.Int184Array()
    {
        _ = UInt184Array(out var output);
        return output;
    }
    BigInteger[] IStructAbiDecoder.Int184Array()
    {
        _ = UInt184Array(out var output);
        return output;
    }
    public AbiDecoder UInt184Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 184, true);
        return ConsumeBytes();
    }
    BigInteger[] IArrayAbiDecoder.UInt184Array()
    {
        _ = UInt184Array(out var output);
        return output;
    }
    BigInteger[] IStructAbiDecoder.UInt184Array()
    {
        _ = UInt184Array(out var output);
        return output;
    }
    public AbiDecoder Int192Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 192, false);
        return ConsumeBytes();
    }
    BigInteger[] IArrayAbiDecoder.Int192Array()
    {
        _ = UInt192Array(out var output);
        return output;
    }
    BigInteger[] IStructAbiDecoder.Int192Array()
    {
        _ = UInt192Array(out var output);
        return output;
    }
    public AbiDecoder UInt192Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 192, true);
        return ConsumeBytes();
    }
    BigInteger[] IArrayAbiDecoder.UInt192Array()
    {
        _ = UInt192Array(out var output);
        return output;
    }
    BigInteger[] IStructAbiDecoder.UInt192Array()
    {
        _ = UInt192Array(out var output);
        return output;
    }
    public AbiDecoder Int200Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 200, false);
        return ConsumeBytes();
    }
    BigInteger[] IArrayAbiDecoder.Int200Array()
    {
        _ = UInt200Array(out var output);
        return output;
    }
    BigInteger[] IStructAbiDecoder.Int200Array()
    {
        _ = UInt200Array(out var output);
        return output;
    }
    public AbiDecoder UInt200Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 200, true);
        return ConsumeBytes();
    }
    BigInteger[] IArrayAbiDecoder.UInt200Array()
    {
        _ = UInt200Array(out var output);
        return output;
    }
    BigInteger[] IStructAbiDecoder.UInt200Array()
    {
        _ = UInt200Array(out var output);
        return output;
    }
    public AbiDecoder Int208Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 208, false);
        return ConsumeBytes();
    }
    BigInteger[] IArrayAbiDecoder.Int208Array()
    {
        _ = UInt208Array(out var output);
        return output;
    }
    BigInteger[] IStructAbiDecoder.Int208Array()
    {
        _ = UInt208Array(out var output);
        return output;
    }
    public AbiDecoder UInt208Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 208, true);
        return ConsumeBytes();
    }
    BigInteger[] IArrayAbiDecoder.UInt208Array()
    {
        _ = UInt208Array(out var output);
        return output;
    }
    BigInteger[] IStructAbiDecoder.UInt208Array()
    {
        _ = UInt208Array(out var output);
        return output;
    }
    public AbiDecoder Int216Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 216, false);
        return ConsumeBytes();
    }
    BigInteger[] IArrayAbiDecoder.Int216Array()
    {
        _ = UInt216Array(out var output);
        return output;
    }
    BigInteger[] IStructAbiDecoder.Int216Array()
    {
        _ = UInt216Array(out var output);
        return output;
    }
    public AbiDecoder UInt216Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 216, true);
        return ConsumeBytes();
    }
    BigInteger[] IArrayAbiDecoder.UInt216Array()
    {
        _ = UInt216Array(out var output);
        return output;
    }
    BigInteger[] IStructAbiDecoder.UInt216Array()
    {
        _ = UInt216Array(out var output);
        return output;
    }
    public AbiDecoder Int224Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 224, false);
        return ConsumeBytes();
    }
    BigInteger[] IArrayAbiDecoder.Int224Array()
    {
        _ = UInt224Array(out var output);
        return output;
    }
    BigInteger[] IStructAbiDecoder.Int224Array()
    {
        _ = UInt224Array(out var output);
        return output;
    }
    public AbiDecoder UInt224Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 224, true);
        return ConsumeBytes();
    }
    BigInteger[] IArrayAbiDecoder.UInt224Array()
    {
        _ = UInt224Array(out var output);
        return output;
    }
    BigInteger[] IStructAbiDecoder.UInt224Array()
    {
        _ = UInt224Array(out var output);
        return output;
    }
    public AbiDecoder Int232Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 232, false);
        return ConsumeBytes();
    }
    BigInteger[] IArrayAbiDecoder.Int232Array()
    {
        _ = UInt232Array(out var output);
        return output;
    }
    BigInteger[] IStructAbiDecoder.Int232Array()
    {
        _ = UInt232Array(out var output);
        return output;
    }
    public AbiDecoder UInt232Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 232, true);
        return ConsumeBytes();
    }
    BigInteger[] IArrayAbiDecoder.UInt232Array()
    {
        _ = UInt232Array(out var output);
        return output;
    }
    BigInteger[] IStructAbiDecoder.UInt232Array()
    {
        _ = UInt232Array(out var output);
        return output;
    }
    public AbiDecoder Int240Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 240, false);
        return ConsumeBytes();
    }
    BigInteger[] IArrayAbiDecoder.Int240Array()
    {
        _ = UInt240Array(out var output);
        return output;
    }
    BigInteger[] IStructAbiDecoder.Int240Array()
    {
        _ = UInt240Array(out var output);
        return output;
    }
    public AbiDecoder UInt240Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 240, true);
        return ConsumeBytes();
    }
    BigInteger[] IArrayAbiDecoder.UInt240Array()
    {
        _ = UInt240Array(out var output);
        return output;
    }
    BigInteger[] IStructAbiDecoder.UInt240Array()
    {
        _ = UInt240Array(out var output);
        return output;
    }
    public AbiDecoder Int248Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 248, false);
        return ConsumeBytes();
    }
    BigInteger[] IArrayAbiDecoder.Int248Array()
    {
        _ = UInt248Array(out var output);
        return output;
    }
    BigInteger[] IStructAbiDecoder.Int248Array()
    {
        _ = UInt248Array(out var output);
        return output;
    }
    public AbiDecoder UInt248Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 248, true);
        return ConsumeBytes();
    }
    BigInteger[] IArrayAbiDecoder.UInt248Array()
    {
        _ = UInt248Array(out var output);
        return output;
    }
    BigInteger[] IStructAbiDecoder.UInt248Array()
    {
        _ = UInt248Array(out var output);
        return output;
    }
    public AbiDecoder Int256Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 256, false);
        return ConsumeBytes();
    }
    BigInteger[] IArrayAbiDecoder.Int256Array()
    {
        _ = UInt256Array(out var output);
        return output;
    }
    BigInteger[] IStructAbiDecoder.Int256Array()
    {
        _ = UInt256Array(out var output);
        return output;
    }
    public AbiDecoder UInt256Array(out System.Numerics.BigInteger[] bytes)
    {
        bytes = DynamicType<System.Numerics.BigInteger>.BigIntegerArray.Decode(_bytes, _bytesRead, 256, true);
        return ConsumeBytes();
    }
    BigInteger[] IArrayAbiDecoder.UInt256Array()
    {
        _ = UInt256Array(out var output);
        return output;
    }
    BigInteger[] IStructAbiDecoder.UInt256Array()
    {
        _ = UInt256Array(out var output);
        return output;
    }
}