using EtherSharp.ABI.Decode.Interfaces;
using EtherSharp.ABI.Types;
using System.Numerics;

namespace EtherSharp.ABI;
public partial class AbiDecoder
{
    public sbyte[] Int8Array()
    {
        sbyte[] result = AbiTypes.PrimitiveNumberArray<sbyte>.Decode(_bytes, _bytesRead);
        ConsumeBytes();
        return result;
    }
    sbyte[] IArrayAbiDecoder.Int8Array()
        => Int8Array();
    sbyte[] IDynamicTupleDecoder.Int8Array()
        => Int8Array();

    public byte[] UInt8Array()
    {
        byte[] result = AbiTypes.PrimitiveNumberArray<byte>.Decode(_bytes, _bytesRead);
        ConsumeBytes();
        return result;
    }
    byte[] IArrayAbiDecoder.UInt8Array()
        => UInt8Array();
    byte[] IDynamicTupleDecoder.UInt8Array()
        => UInt8Array();

    public short[] Int16Array()
    {
        short[] result = AbiTypes.PrimitiveNumberArray<short>.Decode(_bytes, _bytesRead);
        ConsumeBytes();
        return result;
    }
    short[] IArrayAbiDecoder.Int16Array()
        => Int16Array();
    short[] IDynamicTupleDecoder.Int16Array()
        => Int16Array();

    public ushort[] UInt16Array()
    {
        ushort[] result = AbiTypes.PrimitiveNumberArray<ushort>.Decode(_bytes, _bytesRead);
        ConsumeBytes();
        return result;
    }
    ushort[] IArrayAbiDecoder.UInt16Array()
        => UInt16Array();
    ushort[] IDynamicTupleDecoder.UInt16Array()
        => UInt16Array();

    public int[] Int24Array()
    {
        int[] result = AbiTypes.PrimitiveNumberArray<int>.Decode(_bytes, _bytesRead);
        ConsumeBytes();
        return result;
    }
    int[] IArrayAbiDecoder.Int24Array()
        => Int24Array();
    int[] IDynamicTupleDecoder.Int24Array()
        => Int24Array();

    public uint[] UInt24Array()
    {
        uint[] result = AbiTypes.PrimitiveNumberArray<uint>.Decode(_bytes, _bytesRead);
        ConsumeBytes();
        return result;
    }
    uint[] IArrayAbiDecoder.UInt24Array()
        => UInt24Array();
    uint[] IDynamicTupleDecoder.UInt24Array()
        => UInt24Array();
    public int[] Int32Array()
    {
        int[] result = AbiTypes.PrimitiveNumberArray<int>.Decode(_bytes, _bytesRead);
        ConsumeBytes();
        return result;
    }
    int[] IArrayAbiDecoder.Int32Array()
        => Int32Array();
    int[] IDynamicTupleDecoder.Int32Array()
        => Int32Array();

    public uint[] UInt32Array()
    {
        uint[] result = AbiTypes.PrimitiveNumberArray<uint>.Decode(_bytes, _bytesRead);
        ConsumeBytes();
        return result;
    }
    uint[] IArrayAbiDecoder.UInt32Array()
        => UInt32Array();
    uint[] IDynamicTupleDecoder.UInt32Array()
        => UInt32Array();

    public long[] Int40Array()
    {
        long[] result = AbiTypes.PrimitiveNumberArray<long>.Decode(_bytes, _bytesRead);
        ConsumeBytes();
        return result;
    }
    long[] IArrayAbiDecoder.Int40Array()
        => Int40Array();
    long[] IDynamicTupleDecoder.Int40Array()
        => Int40Array();

    public ulong[] UInt40Array()
    {
        ulong[] result = AbiTypes.PrimitiveNumberArray<ulong>.Decode(_bytes, _bytesRead);
        ConsumeBytes();
        return result;
    }
    ulong[] IArrayAbiDecoder.UInt40Array()
        => UInt40Array();
    ulong[] IDynamicTupleDecoder.UInt40Array()
        => UInt40Array();
    public long[] Int48Array()
    {
        long[] result = AbiTypes.PrimitiveNumberArray<long>.Decode(_bytes, _bytesRead);
        ConsumeBytes();
        return result;
    }
    long[] IArrayAbiDecoder.Int48Array()
        => Int48Array();
    long[] IDynamicTupleDecoder.Int48Array()
        => Int48Array();

    public ulong[] UInt48Array()
    {
        ulong[] result = AbiTypes.PrimitiveNumberArray<ulong>.Decode(_bytes, _bytesRead);
        ConsumeBytes();
        return result;
    }
    ulong[] IArrayAbiDecoder.UInt48Array()
        => UInt48Array();
    ulong[] IDynamicTupleDecoder.UInt48Array()
        => UInt48Array();
    public long[] Int56Array()
    {
        long[] result = AbiTypes.PrimitiveNumberArray<long>.Decode(_bytes, _bytesRead);
        ConsumeBytes();
        return result;
    }
    long[] IArrayAbiDecoder.Int56Array()
        => Int56Array();
    long[] IDynamicTupleDecoder.Int56Array()
        => Int56Array();

    public ulong[] UInt56Array()
    {
        ulong[] result = AbiTypes.PrimitiveNumberArray<ulong>.Decode(_bytes, _bytesRead);
        ConsumeBytes();
        return result;
    }
    ulong[] IArrayAbiDecoder.UInt56Array()
        => UInt56Array();
    ulong[] IDynamicTupleDecoder.UInt56Array()
        => UInt56Array();
    public long[] Int64Array()
    {
        long[] result = AbiTypes.PrimitiveNumberArray<long>.Decode(_bytes, _bytesRead);
        ConsumeBytes();
        return result;
    }
    long[] IArrayAbiDecoder.Int64Array()
        => Int64Array();
    long[] IDynamicTupleDecoder.Int64Array()
        => Int64Array();

    public ulong[] UInt64Array()
    {
        ulong[] result = AbiTypes.PrimitiveNumberArray<ulong>.Decode(_bytes, _bytesRead);
        ConsumeBytes();
        return result;
    }
    ulong[] IArrayAbiDecoder.UInt64Array()
        => UInt64Array();
    ulong[] IDynamicTupleDecoder.UInt64Array()
        => UInt64Array();

    public BigInteger[] Int72Array()
    {
        var result = AbiTypes.BigIntegerArray.Decode(_bytes, _bytesRead, 72, false);
        ConsumeBytes();
        return result;
    }
    BigInteger[] IArrayAbiDecoder.Int72Array()
        => Int72Array();
    BigInteger[] IDynamicTupleDecoder.Int72Array()
        => Int72Array();

    public BigInteger[] UInt72Array()
    {
        var result = AbiTypes.BigIntegerArray.Decode(_bytes, _bytesRead, 72, true);
        ConsumeBytes();
        return result;
    }
    BigInteger[] IArrayAbiDecoder.UInt72Array()
        => UInt72Array();
    BigInteger[] IDynamicTupleDecoder.UInt72Array()
        => UInt72Array();
    public BigInteger[] Int80Array()
    {
        var result = AbiTypes.BigIntegerArray.Decode(_bytes, _bytesRead, 80, false);
        ConsumeBytes();
        return result;
    }
    BigInteger[] IArrayAbiDecoder.Int80Array()
        => Int80Array();
    BigInteger[] IDynamicTupleDecoder.Int80Array()
        => Int80Array();

    public BigInteger[] UInt80Array()
    {
        var result = AbiTypes.BigIntegerArray.Decode(_bytes, _bytesRead, 80, true);
        ConsumeBytes();
        return result;
    }
    BigInteger[] IArrayAbiDecoder.UInt80Array()
        => UInt80Array();
    BigInteger[] IDynamicTupleDecoder.UInt80Array()
        => UInt80Array();
    public BigInteger[] Int88Array()
    {
        var result = AbiTypes.BigIntegerArray.Decode(_bytes, _bytesRead, 88, false);
        ConsumeBytes();
        return result;
    }
    BigInteger[] IArrayAbiDecoder.Int88Array()
        => Int88Array();
    BigInteger[] IDynamicTupleDecoder.Int88Array()
        => Int88Array();

    public BigInteger[] UInt88Array()
    {
        var result = AbiTypes.BigIntegerArray.Decode(_bytes, _bytesRead, 88, true);
        ConsumeBytes();
        return result;
    }
    BigInteger[] IArrayAbiDecoder.UInt88Array()
        => UInt88Array();
    BigInteger[] IDynamicTupleDecoder.UInt88Array()
        => UInt88Array();
    public BigInteger[] Int96Array()
    {
        var result = AbiTypes.BigIntegerArray.Decode(_bytes, _bytesRead, 96, false);
        ConsumeBytes();
        return result;
    }
    BigInteger[] IArrayAbiDecoder.Int96Array()
        => Int96Array();
    BigInteger[] IDynamicTupleDecoder.Int96Array()
        => Int96Array();

    public BigInteger[] UInt96Array()
    {
        var result = AbiTypes.BigIntegerArray.Decode(_bytes, _bytesRead, 96, true);
        ConsumeBytes();
        return result;
    }
    BigInteger[] IArrayAbiDecoder.UInt96Array()
        => UInt96Array();
    BigInteger[] IDynamicTupleDecoder.UInt96Array()
        => UInt96Array();
    public BigInteger[] Int104Array()
    {
        var result = AbiTypes.BigIntegerArray.Decode(_bytes, _bytesRead, 104, false);
        ConsumeBytes();
        return result;
    }
    BigInteger[] IArrayAbiDecoder.Int104Array()
        => Int104Array();
    BigInteger[] IDynamicTupleDecoder.Int104Array()
        => Int104Array();

    public BigInteger[] UInt104Array()
    {
        var result = AbiTypes.BigIntegerArray.Decode(_bytes, _bytesRead, 104, true);
        ConsumeBytes();
        return result;
    }
    BigInteger[] IArrayAbiDecoder.UInt104Array()
        => UInt104Array();
    BigInteger[] IDynamicTupleDecoder.UInt104Array()
        => UInt104Array();
    public BigInteger[] Int112Array()
    {
        var result = AbiTypes.BigIntegerArray.Decode(_bytes, _bytesRead, 112, false);
        ConsumeBytes();
        return result;
    }
    BigInteger[] IArrayAbiDecoder.Int112Array()
        => Int112Array();
    BigInteger[] IDynamicTupleDecoder.Int112Array()
        => Int112Array();

    public BigInteger[] UInt112Array()
    {
        var result = AbiTypes.BigIntegerArray.Decode(_bytes, _bytesRead, 112, true);
        ConsumeBytes();
        return result;
    }
    BigInteger[] IArrayAbiDecoder.UInt112Array()
        => UInt112Array();
    BigInteger[] IDynamicTupleDecoder.UInt112Array()
        => UInt112Array();
    public BigInteger[] Int120Array()
    {
        var result = AbiTypes.BigIntegerArray.Decode(_bytes, _bytesRead, 120, false);
        ConsumeBytes();
        return result;
    }
    BigInteger[] IArrayAbiDecoder.Int120Array()
        => Int120Array();
    BigInteger[] IDynamicTupleDecoder.Int120Array()
        => Int120Array();

    public BigInteger[] UInt120Array()
    {
        var result = AbiTypes.BigIntegerArray.Decode(_bytes, _bytesRead, 120, true);
        ConsumeBytes();
        return result;
    }
    BigInteger[] IArrayAbiDecoder.UInt120Array()
        => UInt120Array();
    BigInteger[] IDynamicTupleDecoder.UInt120Array()
        => UInt120Array();
    public BigInteger[] Int128Array()
    {
        var result = AbiTypes.BigIntegerArray.Decode(_bytes, _bytesRead, 128, false);
        ConsumeBytes();
        return result;
    }
    BigInteger[] IArrayAbiDecoder.Int128Array()
        => Int128Array();
    BigInteger[] IDynamicTupleDecoder.Int128Array()
        => Int128Array();

    public BigInteger[] UInt128Array()
    {
        var result = AbiTypes.BigIntegerArray.Decode(_bytes, _bytesRead, 128, true);
        ConsumeBytes();
        return result;
    }
    BigInteger[] IArrayAbiDecoder.UInt128Array()
        => UInt128Array();
    BigInteger[] IDynamicTupleDecoder.UInt128Array()
        => UInt128Array();
    public BigInteger[] Int136Array()
    {
        var result = AbiTypes.BigIntegerArray.Decode(_bytes, _bytesRead, 136, false);
        ConsumeBytes();
        return result;
    }
    BigInteger[] IArrayAbiDecoder.Int136Array()
        => Int136Array();
    BigInteger[] IDynamicTupleDecoder.Int136Array()
        => Int136Array();

    public BigInteger[] UInt136Array()
    {
        var result = AbiTypes.BigIntegerArray.Decode(_bytes, _bytesRead, 136, true);
        ConsumeBytes();
        return result;
    }
    BigInteger[] IArrayAbiDecoder.UInt136Array()
        => UInt136Array();
    BigInteger[] IDynamicTupleDecoder.UInt136Array()
        => UInt136Array();
    public BigInteger[] Int144Array()
    {
        var result = AbiTypes.BigIntegerArray.Decode(_bytes, _bytesRead, 144, false);
        ConsumeBytes();
        return result;
    }
    BigInteger[] IArrayAbiDecoder.Int144Array()
        => Int144Array();
    BigInteger[] IDynamicTupleDecoder.Int144Array()
        => Int144Array();

    public BigInteger[] UInt144Array()
    {
        var result = AbiTypes.BigIntegerArray.Decode(_bytes, _bytesRead, 144, true);
        ConsumeBytes();
        return result;
    }
    BigInteger[] IArrayAbiDecoder.UInt144Array()
        => UInt144Array();
    BigInteger[] IDynamicTupleDecoder.UInt144Array()
        => UInt144Array();
    public BigInteger[] Int152Array()
    {
        var result = AbiTypes.BigIntegerArray.Decode(_bytes, _bytesRead, 152, false);
        ConsumeBytes();
        return result;
    }
    BigInteger[] IArrayAbiDecoder.Int152Array()
        => Int152Array();
    BigInteger[] IDynamicTupleDecoder.Int152Array()
        => Int152Array();

    public BigInteger[] UInt152Array()
    {
        var result = AbiTypes.BigIntegerArray.Decode(_bytes, _bytesRead, 152, true);
        ConsumeBytes();
        return result;
    }
    BigInteger[] IArrayAbiDecoder.UInt152Array()
        => UInt152Array();
    BigInteger[] IDynamicTupleDecoder.UInt152Array()
        => UInt152Array();
    public BigInteger[] Int160Array()
    {
        var result = AbiTypes.BigIntegerArray.Decode(_bytes, _bytesRead, 160, false);
        ConsumeBytes();
        return result;
    }
    BigInteger[] IArrayAbiDecoder.Int160Array()
        => Int160Array();
    BigInteger[] IDynamicTupleDecoder.Int160Array()
        => Int160Array();

    public BigInteger[] UInt160Array()
    {
        var result = AbiTypes.BigIntegerArray.Decode(_bytes, _bytesRead, 160, true);
        ConsumeBytes();
        return result;
    }
    BigInteger[] IArrayAbiDecoder.UInt160Array()
        => UInt160Array();
    BigInteger[] IDynamicTupleDecoder.UInt160Array()
        => UInt160Array();
    public BigInteger[] Int168Array()
    {
        var result = AbiTypes.BigIntegerArray.Decode(_bytes, _bytesRead, 168, false);
        ConsumeBytes();
        return result;
    }
    BigInteger[] IArrayAbiDecoder.Int168Array()
        => Int168Array();
    BigInteger[] IDynamicTupleDecoder.Int168Array()
        => Int168Array();

    public BigInteger[] UInt168Array()
    {
        var result = AbiTypes.BigIntegerArray.Decode(_bytes, _bytesRead, 168, true);
        ConsumeBytes();
        return result;
    }
    BigInteger[] IArrayAbiDecoder.UInt168Array()
        => UInt168Array();
    BigInteger[] IDynamicTupleDecoder.UInt168Array()
        => UInt168Array();
    public BigInteger[] Int176Array()
    {
        var result = AbiTypes.BigIntegerArray.Decode(_bytes, _bytesRead, 176, false);
        ConsumeBytes();
        return result;
    }
    BigInteger[] IArrayAbiDecoder.Int176Array()
        => Int176Array();
    BigInteger[] IDynamicTupleDecoder.Int176Array()
        => Int176Array();

    public BigInteger[] UInt176Array()
    {
        var result = AbiTypes.BigIntegerArray.Decode(_bytes, _bytesRead, 176, true);
        ConsumeBytes();
        return result;
    }
    BigInteger[] IArrayAbiDecoder.UInt176Array()
        => UInt176Array();
    BigInteger[] IDynamicTupleDecoder.UInt176Array()
        => UInt176Array();
    public BigInteger[] Int184Array()
    {
        var result = AbiTypes.BigIntegerArray.Decode(_bytes, _bytesRead, 184, false);
        ConsumeBytes();
        return result;
    }
    BigInteger[] IArrayAbiDecoder.Int184Array()
        => Int184Array();
    BigInteger[] IDynamicTupleDecoder.Int184Array()
        => Int184Array();

    public BigInteger[] UInt184Array()
    {
        var result = AbiTypes.BigIntegerArray.Decode(_bytes, _bytesRead, 184, true);
        ConsumeBytes();
        return result;
    }
    BigInteger[] IArrayAbiDecoder.UInt184Array()
        => UInt184Array();
    BigInteger[] IDynamicTupleDecoder.UInt184Array()
        => UInt184Array();
    public BigInteger[] Int192Array()
    {
        var result = AbiTypes.BigIntegerArray.Decode(_bytes, _bytesRead, 192, false);
        ConsumeBytes();
        return result;
    }
    BigInteger[] IArrayAbiDecoder.Int192Array()
        => Int192Array();
    BigInteger[] IDynamicTupleDecoder.Int192Array()
        => Int192Array();

    public BigInteger[] UInt192Array()
    {
        var result = AbiTypes.BigIntegerArray.Decode(_bytes, _bytesRead, 192, true);
        ConsumeBytes();
        return result;
    }
    BigInteger[] IArrayAbiDecoder.UInt192Array()
        => UInt192Array();
    BigInteger[] IDynamicTupleDecoder.UInt192Array()
        => UInt192Array();
    public BigInteger[] Int200Array()
    {
        var result = AbiTypes.BigIntegerArray.Decode(_bytes, _bytesRead, 200, false);
        ConsumeBytes();
        return result;
    }
    BigInteger[] IArrayAbiDecoder.Int200Array()
        => Int200Array();
    BigInteger[] IDynamicTupleDecoder.Int200Array()
        => Int200Array();

    public BigInteger[] UInt200Array()
    {
        var result = AbiTypes.BigIntegerArray.Decode(_bytes, _bytesRead, 200, true);
        ConsumeBytes();
        return result;
    }
    BigInteger[] IArrayAbiDecoder.UInt200Array()
        => UInt200Array();
    BigInteger[] IDynamicTupleDecoder.UInt200Array()
        => UInt200Array();
    public BigInteger[] Int208Array()
    {
        var result = AbiTypes.BigIntegerArray.Decode(_bytes, _bytesRead, 208, false);
        ConsumeBytes();
        return result;
    }
    BigInteger[] IArrayAbiDecoder.Int208Array()
        => Int208Array();
    BigInteger[] IDynamicTupleDecoder.Int208Array()
        => Int208Array();

    public BigInteger[] UInt208Array()
    {
        var result = AbiTypes.BigIntegerArray.Decode(_bytes, _bytesRead, 208, true);
        ConsumeBytes();
        return result;
    }
    BigInteger[] IArrayAbiDecoder.UInt208Array()
        => UInt208Array();
    BigInteger[] IDynamicTupleDecoder.UInt208Array()
        => UInt208Array();
    public BigInteger[] Int216Array()
    {
        var result = AbiTypes.BigIntegerArray.Decode(_bytes, _bytesRead, 216, false);
        ConsumeBytes();
        return result;
    }
    BigInteger[] IArrayAbiDecoder.Int216Array()
        => Int216Array();
    BigInteger[] IDynamicTupleDecoder.Int216Array()
        => Int216Array();

    public BigInteger[] UInt216Array()
    {
        var result = AbiTypes.BigIntegerArray.Decode(_bytes, _bytesRead, 216, true);
        ConsumeBytes();
        return result;
    }
    BigInteger[] IArrayAbiDecoder.UInt216Array()
        => UInt216Array();
    BigInteger[] IDynamicTupleDecoder.UInt216Array()
        => UInt216Array();
    public BigInteger[] Int224Array()
    {
        var result = AbiTypes.BigIntegerArray.Decode(_bytes, _bytesRead, 224, false);
        ConsumeBytes();
        return result;
    }
    BigInteger[] IArrayAbiDecoder.Int224Array()
        => Int224Array();
    BigInteger[] IDynamicTupleDecoder.Int224Array()
        => Int224Array();

    public BigInteger[] UInt224Array()
    {
        var result = AbiTypes.BigIntegerArray.Decode(_bytes, _bytesRead, 224, true);
        ConsumeBytes();
        return result;
    }
    BigInteger[] IArrayAbiDecoder.UInt224Array()
        => UInt224Array();
    BigInteger[] IDynamicTupleDecoder.UInt224Array()
        => UInt224Array();
    public BigInteger[] Int232Array()
    {
        var result = AbiTypes.BigIntegerArray.Decode(_bytes, _bytesRead, 232, false);
        ConsumeBytes();
        return result;
    }
    BigInteger[] IArrayAbiDecoder.Int232Array()
        => Int232Array();
    BigInteger[] IDynamicTupleDecoder.Int232Array()
        => Int232Array();

    public BigInteger[] UInt232Array()
    {
        var result = AbiTypes.BigIntegerArray.Decode(_bytes, _bytesRead, 232, true);
        ConsumeBytes();
        return result;
    }
    BigInteger[] IArrayAbiDecoder.UInt232Array()
        => UInt232Array();
    BigInteger[] IDynamicTupleDecoder.UInt232Array()
        => UInt232Array();
    public BigInteger[] Int240Array()
    {
        var result = AbiTypes.BigIntegerArray.Decode(_bytes, _bytesRead, 240, false);
        ConsumeBytes();
        return result;
    }
    BigInteger[] IArrayAbiDecoder.Int240Array()
        => Int240Array();
    BigInteger[] IDynamicTupleDecoder.Int240Array()
        => Int240Array();

    public BigInteger[] UInt240Array()
    {
        var result = AbiTypes.BigIntegerArray.Decode(_bytes, _bytesRead, 240, true);
        ConsumeBytes();
        return result;
    }
    BigInteger[] IArrayAbiDecoder.UInt240Array()
        => UInt240Array();
    BigInteger[] IDynamicTupleDecoder.UInt240Array()
        => UInt240Array();
    public BigInteger[] Int248Array()
    {
        var result = AbiTypes.BigIntegerArray.Decode(_bytes, _bytesRead, 248, false);
        ConsumeBytes();
        return result;
    }
    BigInteger[] IArrayAbiDecoder.Int248Array()
        => Int248Array();
    BigInteger[] IDynamicTupleDecoder.Int248Array()
        => Int248Array();

    public BigInteger[] UInt248Array()
    {
        var result = AbiTypes.BigIntegerArray.Decode(_bytes, _bytesRead, 248, true);
        ConsumeBytes();
        return result;
    }
    BigInteger[] IArrayAbiDecoder.UInt248Array()
        => UInt248Array();
    BigInteger[] IDynamicTupleDecoder.UInt248Array()
        => UInt248Array();
    public BigInteger[] Int256Array()
    {
        var result = AbiTypes.BigIntegerArray.Decode(_bytes, _bytesRead, 256, false);
        ConsumeBytes();
        return result;
    }
    BigInteger[] IArrayAbiDecoder.Int256Array()
        => Int256Array();
    BigInteger[] IDynamicTupleDecoder.Int256Array()
        => Int256Array();

    public BigInteger[] UInt256Array()
    {
        var result = AbiTypes.BigIntegerArray.Decode(_bytes, _bytesRead, 256, true);
        ConsumeBytes();
        return result;
    }
    BigInteger[] IArrayAbiDecoder.UInt256Array()
        => UInt256Array();
    BigInteger[] IDynamicTupleDecoder.UInt256Array()
        => UInt256Array();
}
