using EtherSharp.ABI.Decode.Interfaces;
using EtherSharp.ABI.Types;
using System.Numerics;

namespace EtherSharp.ABI;
public partial class AbiDecoder
{
    public sbyte Int8()
    {
        sbyte result = AbiTypes.SByte.Decode(CurrentSlot);
        ConsumeBytes();
        return result;
    }
    sbyte IFixedTupleDecoder.Int8()
        => Int8();
    sbyte IDynamicTupleDecoder.Int8()
        => Int8();

    public byte UInt8()
    {
        byte result = AbiTypes.Byte.Decode(CurrentSlot);
        ConsumeBytes();
        return result;
    }
    byte IFixedTupleDecoder.UInt8()
        => UInt8();
    byte IDynamicTupleDecoder.UInt8()
        => UInt8();

    public short Int16()
    {
        short result = AbiTypes.Short.Decode(CurrentSlot);
        ConsumeBytes();
        return result;
    }
    short IFixedTupleDecoder.Int16()
        => Int16();
    short IDynamicTupleDecoder.Int16()
        => Int16();

    public ushort UInt16()
    {
        ushort result = AbiTypes.UShort.Decode(CurrentSlot);
        ConsumeBytes();
        return result;
    }
    ushort IFixedTupleDecoder.UInt16()
        => UInt16();
    ushort IDynamicTupleDecoder.UInt16()
        => UInt16();

    public int Int24()
    {
        int result = AbiTypes.Int.Decode(CurrentSlot);
        ConsumeBytes();
        return result;
    }
    int IFixedTupleDecoder.Int24()
        => Int24();
    int IDynamicTupleDecoder.Int24()
        => Int24();

    public uint UInt24()
    {
        uint result = AbiTypes.UInt.Decode(CurrentSlot);
        ConsumeBytes();
        return result;
    }
    uint IFixedTupleDecoder.UInt24()
        => UInt24();
    uint IDynamicTupleDecoder.UInt24()
        => UInt24();
    public int Int32()
    {
        int result = AbiTypes.Int.Decode(CurrentSlot);
        ConsumeBytes();
        return result;
    }
    int IFixedTupleDecoder.Int32()
        => Int32();
    int IDynamicTupleDecoder.Int32()
        => Int32();

    public uint UInt32()
    {
        uint result = AbiTypes.UInt.Decode(CurrentSlot);
        ConsumeBytes();
        return result;
    }
    uint IFixedTupleDecoder.UInt32()
        => UInt32();
    uint IDynamicTupleDecoder.UInt32()
        => UInt32();

    public long Int40()
    {
        long result = AbiTypes.Long.Decode(CurrentSlot);
        ConsumeBytes();
        return result;
    }
    long IFixedTupleDecoder.Int40()
        => Int40();
    long IDynamicTupleDecoder.Int40()
        => Int40();

    public ulong UInt40()
    {
        ulong result = AbiTypes.ULong.Decode(CurrentSlot);
        ConsumeBytes();
        return result;
    }
    ulong IFixedTupleDecoder.UInt40()
        => UInt40();
    ulong IDynamicTupleDecoder.UInt40()
        => UInt40();
    public long Int48()
    {
        long result = AbiTypes.Long.Decode(CurrentSlot);
        ConsumeBytes();
        return result;
    }
    long IFixedTupleDecoder.Int48()
        => Int48();
    long IDynamicTupleDecoder.Int48()
        => Int48();

    public ulong UInt48()
    {
        ulong result = AbiTypes.ULong.Decode(CurrentSlot);
        ConsumeBytes();
        return result;
    }
    ulong IFixedTupleDecoder.UInt48()
        => UInt48();
    ulong IDynamicTupleDecoder.UInt48()
        => UInt48();
    public long Int56()
    {
        long result = AbiTypes.Long.Decode(CurrentSlot);
        ConsumeBytes();
        return result;
    }
    long IFixedTupleDecoder.Int56()
        => Int56();
    long IDynamicTupleDecoder.Int56()
        => Int56();

    public ulong UInt56()
    {
        ulong result = AbiTypes.ULong.Decode(CurrentSlot);
        ConsumeBytes();
        return result;
    }
    ulong IFixedTupleDecoder.UInt56()
        => UInt56();
    ulong IDynamicTupleDecoder.UInt56()
        => UInt56();
    public long Int64()
    {
        long result = AbiTypes.Long.Decode(CurrentSlot);
        ConsumeBytes();
        return result;
    }
    long IFixedTupleDecoder.Int64()
        => Int64();
    long IDynamicTupleDecoder.Int64()
        => Int64();

    public ulong UInt64()
    {
        ulong result = AbiTypes.ULong.Decode(CurrentSlot);
        ConsumeBytes();
        return result;
    }
    ulong IFixedTupleDecoder.UInt64()
        => UInt64();
    ulong IDynamicTupleDecoder.UInt64()
        => UInt64();

    public BigInteger Int72()
    {
        var result = AbiTypes.BigInteger.Decode(CurrentSlot, false);
        ConsumeBytes();
        return result;
    }
    BigInteger IFixedTupleDecoder.Int72()
        => Int72();
    BigInteger IDynamicTupleDecoder.Int72()
        => Int72();

    public BigInteger UInt72()
    {
        var result = AbiTypes.BigInteger.Decode(CurrentSlot, true);
        ConsumeBytes();
        return result;
    }
    BigInteger IFixedTupleDecoder.UInt72()
        => UInt72();
    BigInteger IDynamicTupleDecoder.UInt72()
        => UInt72();
    public BigInteger Int80()
    {
        var result = AbiTypes.BigInteger.Decode(CurrentSlot, false);
        ConsumeBytes();
        return result;
    }
    BigInteger IFixedTupleDecoder.Int80()
        => Int80();
    BigInteger IDynamicTupleDecoder.Int80()
        => Int80();

    public BigInteger UInt80()
    {
        var result = AbiTypes.BigInteger.Decode(CurrentSlot, true);
        ConsumeBytes();
        return result;
    }
    BigInteger IFixedTupleDecoder.UInt80()
        => UInt80();
    BigInteger IDynamicTupleDecoder.UInt80()
        => UInt80();
    public BigInteger Int88()
    {
        var result = AbiTypes.BigInteger.Decode(CurrentSlot, false);
        ConsumeBytes();
        return result;
    }
    BigInteger IFixedTupleDecoder.Int88()
        => Int88();
    BigInteger IDynamicTupleDecoder.Int88()
        => Int88();

    public BigInteger UInt88()
    {
        var result = AbiTypes.BigInteger.Decode(CurrentSlot, true);
        ConsumeBytes();
        return result;
    }
    BigInteger IFixedTupleDecoder.UInt88()
        => UInt88();
    BigInteger IDynamicTupleDecoder.UInt88()
        => UInt88();
    public BigInteger Int96()
    {
        var result = AbiTypes.BigInteger.Decode(CurrentSlot, false);
        ConsumeBytes();
        return result;
    }
    BigInteger IFixedTupleDecoder.Int96()
        => Int96();
    BigInteger IDynamicTupleDecoder.Int96()
        => Int96();

    public BigInteger UInt96()
    {
        var result = AbiTypes.BigInteger.Decode(CurrentSlot, true);
        ConsumeBytes();
        return result;
    }
    BigInteger IFixedTupleDecoder.UInt96()
        => UInt96();
    BigInteger IDynamicTupleDecoder.UInt96()
        => UInt96();
    public BigInteger Int104()
    {
        var result = AbiTypes.BigInteger.Decode(CurrentSlot, false);
        ConsumeBytes();
        return result;
    }
    BigInteger IFixedTupleDecoder.Int104()
        => Int104();
    BigInteger IDynamicTupleDecoder.Int104()
        => Int104();

    public BigInteger UInt104()
    {
        var result = AbiTypes.BigInteger.Decode(CurrentSlot, true);
        ConsumeBytes();
        return result;
    }
    BigInteger IFixedTupleDecoder.UInt104()
        => UInt104();
    BigInteger IDynamicTupleDecoder.UInt104()
        => UInt104();
    public BigInteger Int112()
    {
        var result = AbiTypes.BigInteger.Decode(CurrentSlot, false);
        ConsumeBytes();
        return result;
    }
    BigInteger IFixedTupleDecoder.Int112()
        => Int112();
    BigInteger IDynamicTupleDecoder.Int112()
        => Int112();

    public BigInteger UInt112()
    {
        var result = AbiTypes.BigInteger.Decode(CurrentSlot, true);
        ConsumeBytes();
        return result;
    }
    BigInteger IFixedTupleDecoder.UInt112()
        => UInt112();
    BigInteger IDynamicTupleDecoder.UInt112()
        => UInt112();
    public BigInteger Int120()
    {
        var result = AbiTypes.BigInteger.Decode(CurrentSlot, false);
        ConsumeBytes();
        return result;
    }
    BigInteger IFixedTupleDecoder.Int120()
        => Int120();
    BigInteger IDynamicTupleDecoder.Int120()
        => Int120();

    public BigInteger UInt120()
    {
        var result = AbiTypes.BigInteger.Decode(CurrentSlot, true);
        ConsumeBytes();
        return result;
    }
    BigInteger IFixedTupleDecoder.UInt120()
        => UInt120();
    BigInteger IDynamicTupleDecoder.UInt120()
        => UInt120();
    public BigInteger Int128()
    {
        var result = AbiTypes.BigInteger.Decode(CurrentSlot, false);
        ConsumeBytes();
        return result;
    }
    BigInteger IFixedTupleDecoder.Int128()
        => Int128();
    BigInteger IDynamicTupleDecoder.Int128()
        => Int128();

    public BigInteger UInt128()
    {
        var result = AbiTypes.BigInteger.Decode(CurrentSlot, true);
        ConsumeBytes();
        return result;
    }
    BigInteger IFixedTupleDecoder.UInt128()
        => UInt128();
    BigInteger IDynamicTupleDecoder.UInt128()
        => UInt128();
    public BigInteger Int136()
    {
        var result = AbiTypes.BigInteger.Decode(CurrentSlot, false);
        ConsumeBytes();
        return result;
    }
    BigInteger IFixedTupleDecoder.Int136()
        => Int136();
    BigInteger IDynamicTupleDecoder.Int136()
        => Int136();

    public BigInteger UInt136()
    {
        var result = AbiTypes.BigInteger.Decode(CurrentSlot, true);
        ConsumeBytes();
        return result;
    }
    BigInteger IFixedTupleDecoder.UInt136()
        => UInt136();
    BigInteger IDynamicTupleDecoder.UInt136()
        => UInt136();
    public BigInteger Int144()
    {
        var result = AbiTypes.BigInteger.Decode(CurrentSlot, false);
        ConsumeBytes();
        return result;
    }
    BigInteger IFixedTupleDecoder.Int144()
        => Int144();
    BigInteger IDynamicTupleDecoder.Int144()
        => Int144();

    public BigInteger UInt144()
    {
        var result = AbiTypes.BigInteger.Decode(CurrentSlot, true);
        ConsumeBytes();
        return result;
    }
    BigInteger IFixedTupleDecoder.UInt144()
        => UInt144();
    BigInteger IDynamicTupleDecoder.UInt144()
        => UInt144();
    public BigInteger Int152()
    {
        var result = AbiTypes.BigInteger.Decode(CurrentSlot, false);
        ConsumeBytes();
        return result;
    }
    BigInteger IFixedTupleDecoder.Int152()
        => Int152();
    BigInteger IDynamicTupleDecoder.Int152()
        => Int152();

    public BigInteger UInt152()
    {
        var result = AbiTypes.BigInteger.Decode(CurrentSlot, true);
        ConsumeBytes();
        return result;
    }
    BigInteger IFixedTupleDecoder.UInt152()
        => UInt152();
    BigInteger IDynamicTupleDecoder.UInt152()
        => UInt152();
    public BigInteger Int160()
    {
        var result = AbiTypes.BigInteger.Decode(CurrentSlot, false);
        ConsumeBytes();
        return result;
    }
    BigInteger IFixedTupleDecoder.Int160()
        => Int160();
    BigInteger IDynamicTupleDecoder.Int160()
        => Int160();

    public BigInteger UInt160()
    {
        var result = AbiTypes.BigInteger.Decode(CurrentSlot, true);
        ConsumeBytes();
        return result;
    }
    BigInteger IFixedTupleDecoder.UInt160()
        => UInt160();
    BigInteger IDynamicTupleDecoder.UInt160()
        => UInt160();
    public BigInteger Int168()
    {
        var result = AbiTypes.BigInteger.Decode(CurrentSlot, false);
        ConsumeBytes();
        return result;
    }
    BigInteger IFixedTupleDecoder.Int168()
        => Int168();
    BigInteger IDynamicTupleDecoder.Int168()
        => Int168();

    public BigInteger UInt168()
    {
        var result = AbiTypes.BigInteger.Decode(CurrentSlot, true);
        ConsumeBytes();
        return result;
    }
    BigInteger IFixedTupleDecoder.UInt168()
        => UInt168();
    BigInteger IDynamicTupleDecoder.UInt168()
        => UInt168();
    public BigInteger Int176()
    {
        var result = AbiTypes.BigInteger.Decode(CurrentSlot, false);
        ConsumeBytes();
        return result;
    }
    BigInteger IFixedTupleDecoder.Int176()
        => Int176();
    BigInteger IDynamicTupleDecoder.Int176()
        => Int176();

    public BigInteger UInt176()
    {
        var result = AbiTypes.BigInteger.Decode(CurrentSlot, true);
        ConsumeBytes();
        return result;
    }
    BigInteger IFixedTupleDecoder.UInt176()
        => UInt176();
    BigInteger IDynamicTupleDecoder.UInt176()
        => UInt176();
    public BigInteger Int184()
    {
        var result = AbiTypes.BigInteger.Decode(CurrentSlot, false);
        ConsumeBytes();
        return result;
    }
    BigInteger IFixedTupleDecoder.Int184()
        => Int184();
    BigInteger IDynamicTupleDecoder.Int184()
        => Int184();

    public BigInteger UInt184()
    {
        var result = AbiTypes.BigInteger.Decode(CurrentSlot, true);
        ConsumeBytes();
        return result;
    }
    BigInteger IFixedTupleDecoder.UInt184()
        => UInt184();
    BigInteger IDynamicTupleDecoder.UInt184()
        => UInt184();
    public BigInteger Int192()
    {
        var result = AbiTypes.BigInteger.Decode(CurrentSlot, false);
        ConsumeBytes();
        return result;
    }
    BigInteger IFixedTupleDecoder.Int192()
        => Int192();
    BigInteger IDynamicTupleDecoder.Int192()
        => Int192();

    public BigInteger UInt192()
    {
        var result = AbiTypes.BigInteger.Decode(CurrentSlot, true);
        ConsumeBytes();
        return result;
    }
    BigInteger IFixedTupleDecoder.UInt192()
        => UInt192();
    BigInteger IDynamicTupleDecoder.UInt192()
        => UInt192();
    public BigInteger Int200()
    {
        var result = AbiTypes.BigInteger.Decode(CurrentSlot, false);
        ConsumeBytes();
        return result;
    }
    BigInteger IFixedTupleDecoder.Int200()
        => Int200();
    BigInteger IDynamicTupleDecoder.Int200()
        => Int200();

    public BigInteger UInt200()
    {
        var result = AbiTypes.BigInteger.Decode(CurrentSlot, true);
        ConsumeBytes();
        return result;
    }
    BigInteger IFixedTupleDecoder.UInt200()
        => UInt200();
    BigInteger IDynamicTupleDecoder.UInt200()
        => UInt200();
    public BigInteger Int208()
    {
        var result = AbiTypes.BigInteger.Decode(CurrentSlot, false);
        ConsumeBytes();
        return result;
    }
    BigInteger IFixedTupleDecoder.Int208()
        => Int208();
    BigInteger IDynamicTupleDecoder.Int208()
        => Int208();

    public BigInteger UInt208()
    {
        var result = AbiTypes.BigInteger.Decode(CurrentSlot, true);
        ConsumeBytes();
        return result;
    }
    BigInteger IFixedTupleDecoder.UInt208()
        => UInt208();
    BigInteger IDynamicTupleDecoder.UInt208()
        => UInt208();
    public BigInteger Int216()
    {
        var result = AbiTypes.BigInteger.Decode(CurrentSlot, false);
        ConsumeBytes();
        return result;
    }
    BigInteger IFixedTupleDecoder.Int216()
        => Int216();
    BigInteger IDynamicTupleDecoder.Int216()
        => Int216();

    public BigInteger UInt216()
    {
        var result = AbiTypes.BigInteger.Decode(CurrentSlot, true);
        ConsumeBytes();
        return result;
    }
    BigInteger IFixedTupleDecoder.UInt216()
        => UInt216();
    BigInteger IDynamicTupleDecoder.UInt216()
        => UInt216();
    public BigInteger Int224()
    {
        var result = AbiTypes.BigInteger.Decode(CurrentSlot, false);
        ConsumeBytes();
        return result;
    }
    BigInteger IFixedTupleDecoder.Int224()
        => Int224();
    BigInteger IDynamicTupleDecoder.Int224()
        => Int224();

    public BigInteger UInt224()
    {
        var result = AbiTypes.BigInteger.Decode(CurrentSlot, true);
        ConsumeBytes();
        return result;
    }
    BigInteger IFixedTupleDecoder.UInt224()
        => UInt224();
    BigInteger IDynamicTupleDecoder.UInt224()
        => UInt224();
    public BigInteger Int232()
    {
        var result = AbiTypes.BigInteger.Decode(CurrentSlot, false);
        ConsumeBytes();
        return result;
    }
    BigInteger IFixedTupleDecoder.Int232()
        => Int232();
    BigInteger IDynamicTupleDecoder.Int232()
        => Int232();

    public BigInteger UInt232()
    {
        var result = AbiTypes.BigInteger.Decode(CurrentSlot, true);
        ConsumeBytes();
        return result;
    }
    BigInteger IFixedTupleDecoder.UInt232()
        => UInt232();
    BigInteger IDynamicTupleDecoder.UInt232()
        => UInt232();
    public BigInteger Int240()
    {
        var result = AbiTypes.BigInteger.Decode(CurrentSlot, false);
        ConsumeBytes();
        return result;
    }
    BigInteger IFixedTupleDecoder.Int240()
        => Int240();
    BigInteger IDynamicTupleDecoder.Int240()
        => Int240();

    public BigInteger UInt240()
    {
        var result = AbiTypes.BigInteger.Decode(CurrentSlot, true);
        ConsumeBytes();
        return result;
    }
    BigInteger IFixedTupleDecoder.UInt240()
        => UInt240();
    BigInteger IDynamicTupleDecoder.UInt240()
        => UInt240();
    public BigInteger Int248()
    {
        var result = AbiTypes.BigInteger.Decode(CurrentSlot, false);
        ConsumeBytes();
        return result;
    }
    BigInteger IFixedTupleDecoder.Int248()
        => Int248();
    BigInteger IDynamicTupleDecoder.Int248()
        => Int248();

    public BigInteger UInt248()
    {
        var result = AbiTypes.BigInteger.Decode(CurrentSlot, true);
        ConsumeBytes();
        return result;
    }
    BigInteger IFixedTupleDecoder.UInt248()
        => UInt248();
    BigInteger IDynamicTupleDecoder.UInt248()
        => UInt248();
    public BigInteger Int256()
    {
        var result = AbiTypes.BigInteger.Decode(CurrentSlot, false);
        ConsumeBytes();
        return result;
    }
    BigInteger IFixedTupleDecoder.Int256()
        => Int256();
    BigInteger IDynamicTupleDecoder.Int256()
        => Int256();

    public BigInteger UInt256()
    {
        var result = AbiTypes.BigInteger.Decode(CurrentSlot, true);
        ConsumeBytes();
        return result;
    }
    BigInteger IFixedTupleDecoder.UInt256()
        => UInt256();
    BigInteger IDynamicTupleDecoder.UInt256()
        => UInt256();
}
