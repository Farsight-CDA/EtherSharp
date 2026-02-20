using EtherSharp.ABI.Decode.Interfaces;
using EtherSharp.ABI.Types;
using EtherSharp.Numerics;

namespace EtherSharp.ABI;

public partial class AbiDecoder
{
    /// <summary>
    /// Reads an int8 array from the input.
    /// </summary>
    /// <returns>The decoded signed byte array.</returns>
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

    /// <summary>
    /// Reads a uint8 array from the input.
    /// </summary>
    /// <returns>The decoded byte array.</returns>
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

    /// <summary>
    /// Reads an int16 array from the input.
    /// </summary>
    /// <returns>The decoded short array.</returns>
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

    /// <summary>
    /// Reads a uint16 array from the input.
    /// </summary>
    /// <returns>The decoded unsigned short array.</returns>
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

    /// <summary>
    /// Reads an int24 array from the input.
    /// </summary>
    /// <returns>The decoded int array.</returns>
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

    /// <summary>
    /// Reads a uint24 array from the input.
    /// </summary>
    /// <returns>The decoded uint array.</returns>
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
    /// <summary>
    /// Reads an int32 array from the input.
    /// </summary>
    /// <returns>The decoded int array.</returns>
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

    /// <summary>
    /// Reads a uint32 array from the input.
    /// </summary>
    /// <returns>The decoded uint array.</returns>
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

    /// <summary>
    /// Reads an int40 array from the input.
    /// </summary>
    /// <returns>The decoded long array.</returns>
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

    /// <summary>
    /// Reads a uint40 array from the input.
    /// </summary>
    /// <returns>The decoded ulong array.</returns>
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
    /// <summary>
    /// Reads an int48 array from the input.
    /// </summary>
    /// <returns>The decoded long array.</returns>
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

    /// <summary>
    /// Reads a uint48 array from the input.
    /// </summary>
    /// <returns>The decoded ulong array.</returns>
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
    /// <summary>
    /// Reads an int56 array from the input.
    /// </summary>
    /// <returns>The decoded long array.</returns>
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

    /// <summary>
    /// Reads a uint56 array from the input.
    /// </summary>
    /// <returns>The decoded ulong array.</returns>
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
    /// <summary>
    /// Reads an int64 array from the input.
    /// </summary>
    /// <returns>The decoded long array.</returns>
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

    /// <summary>
    /// Reads a uint64 array from the input.
    /// </summary>
    /// <returns>The decoded ulong array.</returns>
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

    /// <summary>
    /// Reads an int72 array from the input.
    /// </summary>
    /// <returns>The decoded Int256 array.</returns>
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

    /// <summary>
    /// Reads a uint72 array from the input.
    /// </summary>
    /// <returns>The decoded UInt256 array.</returns>
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
    /// <summary>
    /// Reads an int80 array from the input.
    /// </summary>
    /// <returns>The decoded Int256 array.</returns>
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

    /// <summary>
    /// Reads a uint80 array from the input.
    /// </summary>
    /// <returns>The decoded UInt256 array.</returns>
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
    /// <summary>
    /// Reads an int88 array from the input.
    /// </summary>
    /// <returns>The decoded Int256 array.</returns>
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

    /// <summary>
    /// Reads a uint88 array from the input.
    /// </summary>
    /// <returns>The decoded UInt256 array.</returns>
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
    /// <summary>
    /// Reads an int96 array from the input.
    /// </summary>
    /// <returns>The decoded Int256 array.</returns>
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

    /// <summary>
    /// Reads a uint96 array from the input.
    /// </summary>
    /// <returns>The decoded UInt256 array.</returns>
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
    /// <summary>
    /// Reads an int104 array from the input.
    /// </summary>
    /// <returns>The decoded Int256 array.</returns>
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

    /// <summary>
    /// Reads a uint104 array from the input.
    /// </summary>
    /// <returns>The decoded UInt256 array.</returns>
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
    /// <summary>
    /// Reads an int112 array from the input.
    /// </summary>
    /// <returns>The decoded Int256 array.</returns>
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

    /// <summary>
    /// Reads a uint112 array from the input.
    /// </summary>
    /// <returns>The decoded UInt256 array.</returns>
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
    /// <summary>
    /// Reads an int120 array from the input.
    /// </summary>
    /// <returns>The decoded Int256 array.</returns>
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

    /// <summary>
    /// Reads a uint120 array from the input.
    /// </summary>
    /// <returns>The decoded UInt256 array.</returns>
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
    /// <summary>
    /// Reads an int128 array from the input.
    /// </summary>
    /// <returns>The decoded Int256 array.</returns>
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

    /// <summary>
    /// Reads a uint128 array from the input.
    /// </summary>
    /// <returns>The decoded UInt256 array.</returns>
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
    /// <summary>
    /// Reads an int136 array from the input.
    /// </summary>
    /// <returns>The decoded Int256 array.</returns>
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

    /// <summary>
    /// Reads a uint136 array from the input.
    /// </summary>
    /// <returns>The decoded UInt256 array.</returns>
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
    /// <summary>
    /// Reads an int144 array from the input.
    /// </summary>
    /// <returns>The decoded Int256 array.</returns>
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

    /// <summary>
    /// Reads a uint144 array from the input.
    /// </summary>
    /// <returns>The decoded UInt256 array.</returns>
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
    /// <summary>
    /// Reads an int152 array from the input.
    /// </summary>
    /// <returns>The decoded Int256 array.</returns>
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

    /// <summary>
    /// Reads a uint152 array from the input.
    /// </summary>
    /// <returns>The decoded UInt256 array.</returns>
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
    /// <summary>
    /// Reads an int160 array from the input.
    /// </summary>
    /// <returns>The decoded Int256 array.</returns>
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

    /// <summary>
    /// Reads a uint160 array from the input.
    /// </summary>
    /// <returns>The decoded UInt256 array.</returns>
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
    /// <summary>
    /// Reads an int168 array from the input.
    /// </summary>
    /// <returns>The decoded Int256 array.</returns>
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

    /// <summary>
    /// Reads a uint168 array from the input.
    /// </summary>
    /// <returns>The decoded UInt256 array.</returns>
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
    /// <summary>
    /// Reads an int176 array from the input.
    /// </summary>
    /// <returns>The decoded Int256 array.</returns>
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

    /// <summary>
    /// Reads a uint176 array from the input.
    /// </summary>
    /// <returns>The decoded UInt256 array.</returns>
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
    /// <summary>
    /// Reads an int184 array from the input.
    /// </summary>
    /// <returns>The decoded Int256 array.</returns>
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

    /// <summary>
    /// Reads a uint184 array from the input.
    /// </summary>
    /// <returns>The decoded UInt256 array.</returns>
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
    /// <summary>
    /// Reads an int192 array from the input.
    /// </summary>
    /// <returns>The decoded Int256 array.</returns>
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

    /// <summary>
    /// Reads a uint192 array from the input.
    /// </summary>
    /// <returns>The decoded UInt256 array.</returns>
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
    /// <summary>
    /// Reads an int200 array from the input.
    /// </summary>
    /// <returns>The decoded Int256 array.</returns>
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

    /// <summary>
    /// Reads a uint200 array from the input.
    /// </summary>
    /// <returns>The decoded UInt256 array.</returns>
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
    /// <summary>
    /// Reads an int208 array from the input.
    /// </summary>
    /// <returns>The decoded Int256 array.</returns>
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

    /// <summary>
    /// Reads a uint208 array from the input.
    /// </summary>
    /// <returns>The decoded UInt256 array.</returns>
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
    /// <summary>
    /// Reads an int216 array from the input.
    /// </summary>
    /// <returns>The decoded Int256 array.</returns>
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

    /// <summary>
    /// Reads a uint216 array from the input.
    /// </summary>
    /// <returns>The decoded UInt256 array.</returns>
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
    /// <summary>
    /// Reads an int224 array from the input.
    /// </summary>
    /// <returns>The decoded Int256 array.</returns>
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

    /// <summary>
    /// Reads a uint224 array from the input.
    /// </summary>
    /// <returns>The decoded UInt256 array.</returns>
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
    /// <summary>
    /// Reads an int232 array from the input.
    /// </summary>
    /// <returns>The decoded Int256 array.</returns>
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

    /// <summary>
    /// Reads a uint232 array from the input.
    /// </summary>
    /// <returns>The decoded UInt256 array.</returns>
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
    /// <summary>
    /// Reads an int240 array from the input.
    /// </summary>
    /// <returns>The decoded Int256 array.</returns>
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

    /// <summary>
    /// Reads a uint240 array from the input.
    /// </summary>
    /// <returns>The decoded UInt256 array.</returns>
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
    /// <summary>
    /// Reads an int248 array from the input.
    /// </summary>
    /// <returns>The decoded Int256 array.</returns>
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

    /// <summary>
    /// Reads a uint248 array from the input.
    /// </summary>
    /// <returns>The decoded UInt256 array.</returns>
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
    /// <summary>
    /// Reads an int256 array from the input.
    /// </summary>
    /// <returns>The decoded Int256 array.</returns>
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

    /// <summary>
    /// Reads a uint256 array from the input.
    /// </summary>
    /// <returns>The decoded UInt256 array.</returns>
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
