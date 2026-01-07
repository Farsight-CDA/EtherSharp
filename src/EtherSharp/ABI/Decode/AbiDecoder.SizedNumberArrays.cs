using EtherSharp.ABI.Types;
using EtherSharp.ABI.Decode.Interfaces;
using EtherSharp.Numerics;

namespace EtherSharp.ABI;
public partial class AbiDecoder
{
    public sbyte[] Int8Array()
    {
        sbyte[] result = AbiTypes.SizedNumberArray<sbyte>.Decode(_bytes, BytesRead);
        ConsumeBytes();
        return result;
    }
    sbyte[] IArrayAbiDecoder.Int8Array()
        => Int8Array();
    sbyte[] IDynamicTupleDecoder.Int8Array()
        => Int8Array();

    public byte[] UInt8Array()
    {
        byte[] result = AbiTypes.SizedNumberArray<byte>.Decode(_bytes, BytesRead);
        ConsumeBytes();
        return result;
    }
    byte[] IArrayAbiDecoder.UInt8Array()
        => UInt8Array();
    byte[] IDynamicTupleDecoder.UInt8Array()
        => UInt8Array();

    public short[] Int16Array()
    {
        short[] result = AbiTypes.SizedNumberArray<short>.Decode(_bytes, BytesRead);
        ConsumeBytes();
        return result;
    }
    short[] IArrayAbiDecoder.Int16Array()
        => Int16Array();
    short[] IDynamicTupleDecoder.Int16Array()
        => Int16Array();

    public ushort[] UInt16Array()
    {
        ushort[] result = AbiTypes.SizedNumberArray<ushort>.Decode(_bytes, BytesRead);
        ConsumeBytes();
        return result;
    }
    ushort[] IArrayAbiDecoder.UInt16Array()
        => UInt16Array();
    ushort[] IDynamicTupleDecoder.UInt16Array()
        => UInt16Array();

    public int[] Int24Array()
    {
        int[] result = AbiTypes.SizedNumberArray<int>.Decode(_bytes, BytesRead);
        ConsumeBytes();
        return result;
    }
    int[] IArrayAbiDecoder.Int24Array()
        => Int24Array();
    int[] IDynamicTupleDecoder.Int24Array()
        => Int24Array();

    public uint[] UInt24Array()
    {
        uint[] result = AbiTypes.SizedNumberArray<uint>.Decode(_bytes, BytesRead);
        ConsumeBytes();
        return result;
    }
    uint[] IArrayAbiDecoder.UInt24Array()
        => UInt24Array();
    uint[] IDynamicTupleDecoder.UInt24Array()
        => UInt24Array();
    public int[] Int32Array()
    {
        int[] result = AbiTypes.SizedNumberArray<int>.Decode(_bytes, BytesRead);
        ConsumeBytes();
        return result;
    }
    int[] IArrayAbiDecoder.Int32Array()
        => Int32Array();
    int[] IDynamicTupleDecoder.Int32Array()
        => Int32Array();

    public uint[] UInt32Array()
    {
        uint[] result = AbiTypes.SizedNumberArray<uint>.Decode(_bytes, BytesRead);
        ConsumeBytes();
        return result;
    }
    uint[] IArrayAbiDecoder.UInt32Array()
        => UInt32Array();
    uint[] IDynamicTupleDecoder.UInt32Array()
        => UInt32Array();

    public long[] Int40Array()
    {
        long[] result = AbiTypes.SizedNumberArray<long>.Decode(_bytes, BytesRead);
        ConsumeBytes();
        return result;
    }
    long[] IArrayAbiDecoder.Int40Array()
        => Int40Array();
    long[] IDynamicTupleDecoder.Int40Array()
        => Int40Array();

    public ulong[] UInt40Array()
    {
        ulong[] result = AbiTypes.SizedNumberArray<ulong>.Decode(_bytes, BytesRead);
        ConsumeBytes();
        return result;
    }
    ulong[] IArrayAbiDecoder.UInt40Array()
        => UInt40Array();
    ulong[] IDynamicTupleDecoder.UInt40Array()
        => UInt40Array();
    public long[] Int48Array()
    {
        long[] result = AbiTypes.SizedNumberArray<long>.Decode(_bytes, BytesRead);
        ConsumeBytes();
        return result;
    }
    long[] IArrayAbiDecoder.Int48Array()
        => Int48Array();
    long[] IDynamicTupleDecoder.Int48Array()
        => Int48Array();

    public ulong[] UInt48Array()
    {
        ulong[] result = AbiTypes.SizedNumberArray<ulong>.Decode(_bytes, BytesRead);
        ConsumeBytes();
        return result;
    }
    ulong[] IArrayAbiDecoder.UInt48Array()
        => UInt48Array();
    ulong[] IDynamicTupleDecoder.UInt48Array()
        => UInt48Array();
    public long[] Int56Array()
    {
        long[] result = AbiTypes.SizedNumberArray<long>.Decode(_bytes, BytesRead);
        ConsumeBytes();
        return result;
    }
    long[] IArrayAbiDecoder.Int56Array()
        => Int56Array();
    long[] IDynamicTupleDecoder.Int56Array()
        => Int56Array();

    public ulong[] UInt56Array()
    {
        ulong[] result = AbiTypes.SizedNumberArray<ulong>.Decode(_bytes, BytesRead);
        ConsumeBytes();
        return result;
    }
    ulong[] IArrayAbiDecoder.UInt56Array()
        => UInt56Array();
    ulong[] IDynamicTupleDecoder.UInt56Array()
        => UInt56Array();
    public long[] Int64Array()
    {
        long[] result = AbiTypes.SizedNumberArray<long>.Decode(_bytes, BytesRead);
        ConsumeBytes();
        return result;
    }
    long[] IArrayAbiDecoder.Int64Array()
        => Int64Array();
    long[] IDynamicTupleDecoder.Int64Array()
        => Int64Array();

    public ulong[] UInt64Array()
    {
        ulong[] result = AbiTypes.SizedNumberArray<ulong>.Decode(_bytes, BytesRead);
        ConsumeBytes();
        return result;
    }
    ulong[] IArrayAbiDecoder.UInt64Array()
        => UInt64Array();
    ulong[] IDynamicTupleDecoder.UInt64Array()
        => UInt64Array();

    public Int256[] Int72Array()
    {
        var result = AbiTypes.Int256Array.Decode(_bytes, BytesRead, 72);
        ConsumeBytes();
        return result;
    }
    Int256[] IArrayAbiDecoder.Int72Array()
        => Int72Array();
    Int256[] IDynamicTupleDecoder.Int72Array()
        => Int72Array();

    public UInt256[] UInt72Array()
    {
        var result = AbiTypes.UInt256Array.Decode(_bytes, BytesRead, 72);
        ConsumeBytes();
        return result;
    }
    UInt256[] IArrayAbiDecoder.UInt72Array()
        => UInt72Array();
    UInt256[] IDynamicTupleDecoder.UInt72Array()
        => UInt72Array();
    public Int256[] Int80Array()
    {
        var result = AbiTypes.Int256Array.Decode(_bytes, BytesRead, 80);
        ConsumeBytes();
        return result;
    }
    Int256[] IArrayAbiDecoder.Int80Array()
        => Int80Array();
    Int256[] IDynamicTupleDecoder.Int80Array()
        => Int80Array();

    public UInt256[] UInt80Array()
    {
        var result = AbiTypes.UInt256Array.Decode(_bytes, BytesRead, 80);
        ConsumeBytes();
        return result;
    }
    UInt256[] IArrayAbiDecoder.UInt80Array()
        => UInt80Array();
    UInt256[] IDynamicTupleDecoder.UInt80Array()
        => UInt80Array();
    public Int256[] Int88Array()
    {
        var result = AbiTypes.Int256Array.Decode(_bytes, BytesRead, 88);
        ConsumeBytes();
        return result;
    }
    Int256[] IArrayAbiDecoder.Int88Array()
        => Int88Array();
    Int256[] IDynamicTupleDecoder.Int88Array()
        => Int88Array();

    public UInt256[] UInt88Array()
    {
        var result = AbiTypes.UInt256Array.Decode(_bytes, BytesRead, 88);
        ConsumeBytes();
        return result;
    }
    UInt256[] IArrayAbiDecoder.UInt88Array()
        => UInt88Array();
    UInt256[] IDynamicTupleDecoder.UInt88Array()
        => UInt88Array();
    public Int256[] Int96Array()
    {
        var result = AbiTypes.Int256Array.Decode(_bytes, BytesRead, 96);
        ConsumeBytes();
        return result;
    }
    Int256[] IArrayAbiDecoder.Int96Array()
        => Int96Array();
    Int256[] IDynamicTupleDecoder.Int96Array()
        => Int96Array();

    public UInt256[] UInt96Array()
    {
        var result = AbiTypes.UInt256Array.Decode(_bytes, BytesRead, 96);
        ConsumeBytes();
        return result;
    }
    UInt256[] IArrayAbiDecoder.UInt96Array()
        => UInt96Array();
    UInt256[] IDynamicTupleDecoder.UInt96Array()
        => UInt96Array();
    public Int256[] Int104Array()
    {
        var result = AbiTypes.Int256Array.Decode(_bytes, BytesRead, 104);
        ConsumeBytes();
        return result;
    }
    Int256[] IArrayAbiDecoder.Int104Array()
        => Int104Array();
    Int256[] IDynamicTupleDecoder.Int104Array()
        => Int104Array();

    public UInt256[] UInt104Array()
    {
        var result = AbiTypes.UInt256Array.Decode(_bytes, BytesRead, 104);
        ConsumeBytes();
        return result;
    }
    UInt256[] IArrayAbiDecoder.UInt104Array()
        => UInt104Array();
    UInt256[] IDynamicTupleDecoder.UInt104Array()
        => UInt104Array();
    public Int256[] Int112Array()
    {
        var result = AbiTypes.Int256Array.Decode(_bytes, BytesRead, 112);
        ConsumeBytes();
        return result;
    }
    Int256[] IArrayAbiDecoder.Int112Array()
        => Int112Array();
    Int256[] IDynamicTupleDecoder.Int112Array()
        => Int112Array();

    public UInt256[] UInt112Array()
    {
        var result = AbiTypes.UInt256Array.Decode(_bytes, BytesRead, 112);
        ConsumeBytes();
        return result;
    }
    UInt256[] IArrayAbiDecoder.UInt112Array()
        => UInt112Array();
    UInt256[] IDynamicTupleDecoder.UInt112Array()
        => UInt112Array();
    public Int256[] Int120Array()
    {
        var result = AbiTypes.Int256Array.Decode(_bytes, BytesRead, 120);
        ConsumeBytes();
        return result;
    }
    Int256[] IArrayAbiDecoder.Int120Array()
        => Int120Array();
    Int256[] IDynamicTupleDecoder.Int120Array()
        => Int120Array();

    public UInt256[] UInt120Array()
    {
        var result = AbiTypes.UInt256Array.Decode(_bytes, BytesRead, 120);
        ConsumeBytes();
        return result;
    }
    UInt256[] IArrayAbiDecoder.UInt120Array()
        => UInt120Array();
    UInt256[] IDynamicTupleDecoder.UInt120Array()
        => UInt120Array();
    public Int256[] Int128Array()
    {
        var result = AbiTypes.Int256Array.Decode(_bytes, BytesRead, 128);
        ConsumeBytes();
        return result;
    }
    Int256[] IArrayAbiDecoder.Int128Array()
        => Int128Array();
    Int256[] IDynamicTupleDecoder.Int128Array()
        => Int128Array();

    public UInt256[] UInt128Array()
    {
        var result = AbiTypes.UInt256Array.Decode(_bytes, BytesRead, 128);
        ConsumeBytes();
        return result;
    }
    UInt256[] IArrayAbiDecoder.UInt128Array()
        => UInt128Array();
    UInt256[] IDynamicTupleDecoder.UInt128Array()
        => UInt128Array();
    public Int256[] Int136Array()
    {
        var result = AbiTypes.Int256Array.Decode(_bytes, BytesRead, 136);
        ConsumeBytes();
        return result;
    }
    Int256[] IArrayAbiDecoder.Int136Array()
        => Int136Array();
    Int256[] IDynamicTupleDecoder.Int136Array()
        => Int136Array();

    public UInt256[] UInt136Array()
    {
        var result = AbiTypes.UInt256Array.Decode(_bytes, BytesRead, 136);
        ConsumeBytes();
        return result;
    }
    UInt256[] IArrayAbiDecoder.UInt136Array()
        => UInt136Array();
    UInt256[] IDynamicTupleDecoder.UInt136Array()
        => UInt136Array();
    public Int256[] Int144Array()
    {
        var result = AbiTypes.Int256Array.Decode(_bytes, BytesRead, 144);
        ConsumeBytes();
        return result;
    }
    Int256[] IArrayAbiDecoder.Int144Array()
        => Int144Array();
    Int256[] IDynamicTupleDecoder.Int144Array()
        => Int144Array();

    public UInt256[] UInt144Array()
    {
        var result = AbiTypes.UInt256Array.Decode(_bytes, BytesRead, 144);
        ConsumeBytes();
        return result;
    }
    UInt256[] IArrayAbiDecoder.UInt144Array()
        => UInt144Array();
    UInt256[] IDynamicTupleDecoder.UInt144Array()
        => UInt144Array();
    public Int256[] Int152Array()
    {
        var result = AbiTypes.Int256Array.Decode(_bytes, BytesRead, 152);
        ConsumeBytes();
        return result;
    }
    Int256[] IArrayAbiDecoder.Int152Array()
        => Int152Array();
    Int256[] IDynamicTupleDecoder.Int152Array()
        => Int152Array();

    public UInt256[] UInt152Array()
    {
        var result = AbiTypes.UInt256Array.Decode(_bytes, BytesRead, 152);
        ConsumeBytes();
        return result;
    }
    UInt256[] IArrayAbiDecoder.UInt152Array()
        => UInt152Array();
    UInt256[] IDynamicTupleDecoder.UInt152Array()
        => UInt152Array();
    public Int256[] Int160Array()
    {
        var result = AbiTypes.Int256Array.Decode(_bytes, BytesRead, 160);
        ConsumeBytes();
        return result;
    }
    Int256[] IArrayAbiDecoder.Int160Array()
        => Int160Array();
    Int256[] IDynamicTupleDecoder.Int160Array()
        => Int160Array();

    public UInt256[] UInt160Array()
    {
        var result = AbiTypes.UInt256Array.Decode(_bytes, BytesRead, 160);
        ConsumeBytes();
        return result;
    }
    UInt256[] IArrayAbiDecoder.UInt160Array()
        => UInt160Array();
    UInt256[] IDynamicTupleDecoder.UInt160Array()
        => UInt160Array();
    public Int256[] Int168Array()
    {
        var result = AbiTypes.Int256Array.Decode(_bytes, BytesRead, 168);
        ConsumeBytes();
        return result;
    }
    Int256[] IArrayAbiDecoder.Int168Array()
        => Int168Array();
    Int256[] IDynamicTupleDecoder.Int168Array()
        => Int168Array();

    public UInt256[] UInt168Array()
    {
        var result = AbiTypes.UInt256Array.Decode(_bytes, BytesRead, 168);
        ConsumeBytes();
        return result;
    }
    UInt256[] IArrayAbiDecoder.UInt168Array()
        => UInt168Array();
    UInt256[] IDynamicTupleDecoder.UInt168Array()
        => UInt168Array();
    public Int256[] Int176Array()
    {
        var result = AbiTypes.Int256Array.Decode(_bytes, BytesRead, 176);
        ConsumeBytes();
        return result;
    }
    Int256[] IArrayAbiDecoder.Int176Array()
        => Int176Array();
    Int256[] IDynamicTupleDecoder.Int176Array()
        => Int176Array();

    public UInt256[] UInt176Array()
    {
        var result = AbiTypes.UInt256Array.Decode(_bytes, BytesRead, 176);
        ConsumeBytes();
        return result;
    }
    UInt256[] IArrayAbiDecoder.UInt176Array()
        => UInt176Array();
    UInt256[] IDynamicTupleDecoder.UInt176Array()
        => UInt176Array();
    public Int256[] Int184Array()
    {
        var result = AbiTypes.Int256Array.Decode(_bytes, BytesRead, 184);
        ConsumeBytes();
        return result;
    }
    Int256[] IArrayAbiDecoder.Int184Array()
        => Int184Array();
    Int256[] IDynamicTupleDecoder.Int184Array()
        => Int184Array();

    public UInt256[] UInt184Array()
    {
        var result = AbiTypes.UInt256Array.Decode(_bytes, BytesRead, 184);
        ConsumeBytes();
        return result;
    }
    UInt256[] IArrayAbiDecoder.UInt184Array()
        => UInt184Array();
    UInt256[] IDynamicTupleDecoder.UInt184Array()
        => UInt184Array();
    public Int256[] Int192Array()
    {
        var result = AbiTypes.Int256Array.Decode(_bytes, BytesRead, 192);
        ConsumeBytes();
        return result;
    }
    Int256[] IArrayAbiDecoder.Int192Array()
        => Int192Array();
    Int256[] IDynamicTupleDecoder.Int192Array()
        => Int192Array();

    public UInt256[] UInt192Array()
    {
        var result = AbiTypes.UInt256Array.Decode(_bytes, BytesRead, 192);
        ConsumeBytes();
        return result;
    }
    UInt256[] IArrayAbiDecoder.UInt192Array()
        => UInt192Array();
    UInt256[] IDynamicTupleDecoder.UInt192Array()
        => UInt192Array();
    public Int256[] Int200Array()
    {
        var result = AbiTypes.Int256Array.Decode(_bytes, BytesRead, 200);
        ConsumeBytes();
        return result;
    }
    Int256[] IArrayAbiDecoder.Int200Array()
        => Int200Array();
    Int256[] IDynamicTupleDecoder.Int200Array()
        => Int200Array();

    public UInt256[] UInt200Array()
    {
        var result = AbiTypes.UInt256Array.Decode(_bytes, BytesRead, 200);
        ConsumeBytes();
        return result;
    }
    UInt256[] IArrayAbiDecoder.UInt200Array()
        => UInt200Array();
    UInt256[] IDynamicTupleDecoder.UInt200Array()
        => UInt200Array();
    public Int256[] Int208Array()
    {
        var result = AbiTypes.Int256Array.Decode(_bytes, BytesRead, 208);
        ConsumeBytes();
        return result;
    }
    Int256[] IArrayAbiDecoder.Int208Array()
        => Int208Array();
    Int256[] IDynamicTupleDecoder.Int208Array()
        => Int208Array();

    public UInt256[] UInt208Array()
    {
        var result = AbiTypes.UInt256Array.Decode(_bytes, BytesRead, 208);
        ConsumeBytes();
        return result;
    }
    UInt256[] IArrayAbiDecoder.UInt208Array()
        => UInt208Array();
    UInt256[] IDynamicTupleDecoder.UInt208Array()
        => UInt208Array();
    public Int256[] Int216Array()
    {
        var result = AbiTypes.Int256Array.Decode(_bytes, BytesRead, 216);
        ConsumeBytes();
        return result;
    }
    Int256[] IArrayAbiDecoder.Int216Array()
        => Int216Array();
    Int256[] IDynamicTupleDecoder.Int216Array()
        => Int216Array();

    public UInt256[] UInt216Array()
    {
        var result = AbiTypes.UInt256Array.Decode(_bytes, BytesRead, 216);
        ConsumeBytes();
        return result;
    }
    UInt256[] IArrayAbiDecoder.UInt216Array()
        => UInt216Array();
    UInt256[] IDynamicTupleDecoder.UInt216Array()
        => UInt216Array();
    public Int256[] Int224Array()
    {
        var result = AbiTypes.Int256Array.Decode(_bytes, BytesRead, 224);
        ConsumeBytes();
        return result;
    }
    Int256[] IArrayAbiDecoder.Int224Array()
        => Int224Array();
    Int256[] IDynamicTupleDecoder.Int224Array()
        => Int224Array();

    public UInt256[] UInt224Array()
    {
        var result = AbiTypes.UInt256Array.Decode(_bytes, BytesRead, 224);
        ConsumeBytes();
        return result;
    }
    UInt256[] IArrayAbiDecoder.UInt224Array()
        => UInt224Array();
    UInt256[] IDynamicTupleDecoder.UInt224Array()
        => UInt224Array();
    public Int256[] Int232Array()
    {
        var result = AbiTypes.Int256Array.Decode(_bytes, BytesRead, 232);
        ConsumeBytes();
        return result;
    }
    Int256[] IArrayAbiDecoder.Int232Array()
        => Int232Array();
    Int256[] IDynamicTupleDecoder.Int232Array()
        => Int232Array();

    public UInt256[] UInt232Array()
    {
        var result = AbiTypes.UInt256Array.Decode(_bytes, BytesRead, 232);
        ConsumeBytes();
        return result;
    }
    UInt256[] IArrayAbiDecoder.UInt232Array()
        => UInt232Array();
    UInt256[] IDynamicTupleDecoder.UInt232Array()
        => UInt232Array();
    public Int256[] Int240Array()
    {
        var result = AbiTypes.Int256Array.Decode(_bytes, BytesRead, 240);
        ConsumeBytes();
        return result;
    }
    Int256[] IArrayAbiDecoder.Int240Array()
        => Int240Array();
    Int256[] IDynamicTupleDecoder.Int240Array()
        => Int240Array();

    public UInt256[] UInt240Array()
    {
        var result = AbiTypes.UInt256Array.Decode(_bytes, BytesRead, 240);
        ConsumeBytes();
        return result;
    }
    UInt256[] IArrayAbiDecoder.UInt240Array()
        => UInt240Array();
    UInt256[] IDynamicTupleDecoder.UInt240Array()
        => UInt240Array();
    public Int256[] Int248Array()
    {
        var result = AbiTypes.Int256Array.Decode(_bytes, BytesRead, 248);
        ConsumeBytes();
        return result;
    }
    Int256[] IArrayAbiDecoder.Int248Array()
        => Int248Array();
    Int256[] IDynamicTupleDecoder.Int248Array()
        => Int248Array();

    public UInt256[] UInt248Array()
    {
        var result = AbiTypes.UInt256Array.Decode(_bytes, BytesRead, 248);
        ConsumeBytes();
        return result;
    }
    UInt256[] IArrayAbiDecoder.UInt248Array()
        => UInt248Array();
    UInt256[] IDynamicTupleDecoder.UInt248Array()
        => UInt248Array();
    public Int256[] Int256Array()
    {
        var result = AbiTypes.Int256Array.Decode(_bytes, BytesRead, 256);
        ConsumeBytes();
        return result;
    }
    Int256[] IArrayAbiDecoder.Int256Array()
        => Int256Array();
    Int256[] IDynamicTupleDecoder.Int256Array()
        => Int256Array();

    public UInt256[] UInt256Array()
    {
        var result = AbiTypes.UInt256Array.Decode(_bytes, BytesRead, 256);
        ConsumeBytes();
        return result;
    }
    UInt256[] IArrayAbiDecoder.UInt256Array()
        => UInt256Array();
    UInt256[] IDynamicTupleDecoder.UInt256Array()
        => UInt256Array();
}
