using EtherSharp.ABI.Types;
using EtherSharp.ABI.Decode.Interfaces;
using EtherSharp.Numerics;

namespace EtherSharp.ABI;
public partial class AbiDecoder
{
    public sbyte Int8()
    {
        var result = AbiTypes.SByte.Decode(CurrentSlot);
        ConsumeBytes();
        return result;
    }
    sbyte IFixedTupleDecoder.Int8() 
        => Int8();
    sbyte IDynamicTupleDecoder.Int8() 
        => Int8();

    public byte UInt8()
    {
        var result = AbiTypes.Byte.Decode(CurrentSlot);
        ConsumeBytes();
        return result;
    }
    byte IFixedTupleDecoder.UInt8()     
        => UInt8();
    byte IDynamicTupleDecoder.UInt8() 
        => UInt8();

    public short Int16()
    {
        var result = AbiTypes.Short.Decode(CurrentSlot);
        ConsumeBytes();
        return result;
    }
    short IFixedTupleDecoder.Int16() 
        => Int16();
    short IDynamicTupleDecoder.Int16() 
        => Int16();

    public ushort UInt16()
    {
        var result = AbiTypes.UShort.Decode(CurrentSlot);
        ConsumeBytes();
        return result;
    }
    ushort IFixedTupleDecoder.UInt16()
        => UInt16();
    ushort IDynamicTupleDecoder.UInt16() 
        => UInt16();

    public int Int24()
    {
        var result = AbiTypes.Int.Decode(CurrentSlot);
        ConsumeBytes();
        return result;
    }
    int IFixedTupleDecoder.Int24() 
        => Int24();
    int IDynamicTupleDecoder.Int24() 
        => Int24();

    public uint UInt24()
    {
        var result = AbiTypes.UInt.Decode(CurrentSlot);
        ConsumeBytes();
        return result;
    }
    uint IFixedTupleDecoder.UInt24() 
        => UInt24();
    uint IDynamicTupleDecoder.UInt24() 
        => UInt24();
    public int Int32()
    {
        var result = AbiTypes.Int.Decode(CurrentSlot);
        ConsumeBytes();
        return result;
    }
    int IFixedTupleDecoder.Int32() 
        => Int32();
    int IDynamicTupleDecoder.Int32() 
        => Int32();

    public uint UInt32()
    {
        var result = AbiTypes.UInt.Decode(CurrentSlot);
        ConsumeBytes();
        return result;
    }
    uint IFixedTupleDecoder.UInt32() 
        => UInt32();
    uint IDynamicTupleDecoder.UInt32() 
        => UInt32();

    public long Int40()
    {
        var result = AbiTypes.Long.Decode(CurrentSlot);
        ConsumeBytes();
        return result;
    }
    long IFixedTupleDecoder.Int40() 
        => Int40();
    long IDynamicTupleDecoder.Int40() 
        => Int40();

    public ulong UInt40()
    {
        var result = AbiTypes.ULong.Decode(CurrentSlot);
        ConsumeBytes();
        return result;
    }
    ulong IFixedTupleDecoder.UInt40() 
        => UInt40();
    ulong IDynamicTupleDecoder.UInt40() 
        => UInt40();
    public long Int48()
    {
        var result = AbiTypes.Long.Decode(CurrentSlot);
        ConsumeBytes();
        return result;
    }
    long IFixedTupleDecoder.Int48() 
        => Int48();
    long IDynamicTupleDecoder.Int48() 
        => Int48();

    public ulong UInt48()
    {
        var result = AbiTypes.ULong.Decode(CurrentSlot);
        ConsumeBytes();
        return result;
    }
    ulong IFixedTupleDecoder.UInt48() 
        => UInt48();
    ulong IDynamicTupleDecoder.UInt48() 
        => UInt48();
    public long Int56()
    {
        var result = AbiTypes.Long.Decode(CurrentSlot);
        ConsumeBytes();
        return result;
    }
    long IFixedTupleDecoder.Int56() 
        => Int56();
    long IDynamicTupleDecoder.Int56() 
        => Int56();

    public ulong UInt56()
    {
        var result = AbiTypes.ULong.Decode(CurrentSlot);
        ConsumeBytes();
        return result;
    }
    ulong IFixedTupleDecoder.UInt56() 
        => UInt56();
    ulong IDynamicTupleDecoder.UInt56() 
        => UInt56();
    public long Int64()
    {
        var result = AbiTypes.Long.Decode(CurrentSlot);
        ConsumeBytes();
        return result;
    }
    long IFixedTupleDecoder.Int64() 
        => Int64();
    long IDynamicTupleDecoder.Int64() 
        => Int64();

    public ulong UInt64()
    {
        var result = AbiTypes.ULong.Decode(CurrentSlot);
        ConsumeBytes();
        return result;
    }
    ulong IFixedTupleDecoder.UInt64() 
        => UInt64();
    ulong IDynamicTupleDecoder.UInt64() 
        => UInt64();

    public Int256 Int72()
    {
        var result = AbiTypes.Int256.Decode(CurrentSlot);
        ConsumeBytes();
        return result;
    }
    Int256 IFixedTupleDecoder.Int72() 
        => Int72();
    Int256 IDynamicTupleDecoder.Int72() 
        => Int72();

    public UInt256 UInt72()
    {
        var result = AbiTypes.UInt256.Decode(CurrentSlot);
        ConsumeBytes();
        return result;
    }
    UInt256 IFixedTupleDecoder.UInt72() 
        => UInt72();
    UInt256 IDynamicTupleDecoder.UInt72() 
        => UInt72();
    public Int256 Int80()
    {
        var result = AbiTypes.Int256.Decode(CurrentSlot);
        ConsumeBytes();
        return result;
    }
    Int256 IFixedTupleDecoder.Int80() 
        => Int80();
    Int256 IDynamicTupleDecoder.Int80() 
        => Int80();

    public UInt256 UInt80()
    {
        var result = AbiTypes.UInt256.Decode(CurrentSlot);
        ConsumeBytes();
        return result;
    }
    UInt256 IFixedTupleDecoder.UInt80() 
        => UInt80();
    UInt256 IDynamicTupleDecoder.UInt80() 
        => UInt80();
    public Int256 Int88()
    {
        var result = AbiTypes.Int256.Decode(CurrentSlot);
        ConsumeBytes();
        return result;
    }
    Int256 IFixedTupleDecoder.Int88() 
        => Int88();
    Int256 IDynamicTupleDecoder.Int88() 
        => Int88();

    public UInt256 UInt88()
    {
        var result = AbiTypes.UInt256.Decode(CurrentSlot);
        ConsumeBytes();
        return result;
    }
    UInt256 IFixedTupleDecoder.UInt88() 
        => UInt88();
    UInt256 IDynamicTupleDecoder.UInt88() 
        => UInt88();
    public Int256 Int96()
    {
        var result = AbiTypes.Int256.Decode(CurrentSlot);
        ConsumeBytes();
        return result;
    }
    Int256 IFixedTupleDecoder.Int96() 
        => Int96();
    Int256 IDynamicTupleDecoder.Int96() 
        => Int96();

    public UInt256 UInt96()
    {
        var result = AbiTypes.UInt256.Decode(CurrentSlot);
        ConsumeBytes();
        return result;
    }
    UInt256 IFixedTupleDecoder.UInt96() 
        => UInt96();
    UInt256 IDynamicTupleDecoder.UInt96() 
        => UInt96();
    public Int256 Int104()
    {
        var result = AbiTypes.Int256.Decode(CurrentSlot);
        ConsumeBytes();
        return result;
    }
    Int256 IFixedTupleDecoder.Int104() 
        => Int104();
    Int256 IDynamicTupleDecoder.Int104() 
        => Int104();

    public UInt256 UInt104()
    {
        var result = AbiTypes.UInt256.Decode(CurrentSlot);
        ConsumeBytes();
        return result;
    }
    UInt256 IFixedTupleDecoder.UInt104() 
        => UInt104();
    UInt256 IDynamicTupleDecoder.UInt104() 
        => UInt104();
    public Int256 Int112()
    {
        var result = AbiTypes.Int256.Decode(CurrentSlot);
        ConsumeBytes();
        return result;
    }
    Int256 IFixedTupleDecoder.Int112() 
        => Int112();
    Int256 IDynamicTupleDecoder.Int112() 
        => Int112();

    public UInt256 UInt112()
    {
        var result = AbiTypes.UInt256.Decode(CurrentSlot);
        ConsumeBytes();
        return result;
    }
    UInt256 IFixedTupleDecoder.UInt112() 
        => UInt112();
    UInt256 IDynamicTupleDecoder.UInt112() 
        => UInt112();
    public Int256 Int120()
    {
        var result = AbiTypes.Int256.Decode(CurrentSlot);
        ConsumeBytes();
        return result;
    }
    Int256 IFixedTupleDecoder.Int120() 
        => Int120();
    Int256 IDynamicTupleDecoder.Int120() 
        => Int120();

    public UInt256 UInt120()
    {
        var result = AbiTypes.UInt256.Decode(CurrentSlot);
        ConsumeBytes();
        return result;
    }
    UInt256 IFixedTupleDecoder.UInt120() 
        => UInt120();
    UInt256 IDynamicTupleDecoder.UInt120() 
        => UInt120();
    public Int256 Int128()
    {
        var result = AbiTypes.Int256.Decode(CurrentSlot);
        ConsumeBytes();
        return result;
    }
    Int256 IFixedTupleDecoder.Int128() 
        => Int128();
    Int256 IDynamicTupleDecoder.Int128() 
        => Int128();

    public UInt256 UInt128()
    {
        var result = AbiTypes.UInt256.Decode(CurrentSlot);
        ConsumeBytes();
        return result;
    }
    UInt256 IFixedTupleDecoder.UInt128() 
        => UInt128();
    UInt256 IDynamicTupleDecoder.UInt128() 
        => UInt128();
    public Int256 Int136()
    {
        var result = AbiTypes.Int256.Decode(CurrentSlot);
        ConsumeBytes();
        return result;
    }
    Int256 IFixedTupleDecoder.Int136() 
        => Int136();
    Int256 IDynamicTupleDecoder.Int136() 
        => Int136();

    public UInt256 UInt136()
    {
        var result = AbiTypes.UInt256.Decode(CurrentSlot);
        ConsumeBytes();
        return result;
    }
    UInt256 IFixedTupleDecoder.UInt136() 
        => UInt136();
    UInt256 IDynamicTupleDecoder.UInt136() 
        => UInt136();
    public Int256 Int144()
    {
        var result = AbiTypes.Int256.Decode(CurrentSlot);
        ConsumeBytes();
        return result;
    }
    Int256 IFixedTupleDecoder.Int144() 
        => Int144();
    Int256 IDynamicTupleDecoder.Int144() 
        => Int144();

    public UInt256 UInt144()
    {
        var result = AbiTypes.UInt256.Decode(CurrentSlot);
        ConsumeBytes();
        return result;
    }
    UInt256 IFixedTupleDecoder.UInt144() 
        => UInt144();
    UInt256 IDynamicTupleDecoder.UInt144() 
        => UInt144();
    public Int256 Int152()
    {
        var result = AbiTypes.Int256.Decode(CurrentSlot);
        ConsumeBytes();
        return result;
    }
    Int256 IFixedTupleDecoder.Int152() 
        => Int152();
    Int256 IDynamicTupleDecoder.Int152() 
        => Int152();

    public UInt256 UInt152()
    {
        var result = AbiTypes.UInt256.Decode(CurrentSlot);
        ConsumeBytes();
        return result;
    }
    UInt256 IFixedTupleDecoder.UInt152() 
        => UInt152();
    UInt256 IDynamicTupleDecoder.UInt152() 
        => UInt152();
    public Int256 Int160()
    {
        var result = AbiTypes.Int256.Decode(CurrentSlot);
        ConsumeBytes();
        return result;
    }
    Int256 IFixedTupleDecoder.Int160() 
        => Int160();
    Int256 IDynamicTupleDecoder.Int160() 
        => Int160();

    public UInt256 UInt160()
    {
        var result = AbiTypes.UInt256.Decode(CurrentSlot);
        ConsumeBytes();
        return result;
    }
    UInt256 IFixedTupleDecoder.UInt160() 
        => UInt160();
    UInt256 IDynamicTupleDecoder.UInt160() 
        => UInt160();
    public Int256 Int168()
    {
        var result = AbiTypes.Int256.Decode(CurrentSlot);
        ConsumeBytes();
        return result;
    }
    Int256 IFixedTupleDecoder.Int168() 
        => Int168();
    Int256 IDynamicTupleDecoder.Int168() 
        => Int168();

    public UInt256 UInt168()
    {
        var result = AbiTypes.UInt256.Decode(CurrentSlot);
        ConsumeBytes();
        return result;
    }
    UInt256 IFixedTupleDecoder.UInt168() 
        => UInt168();
    UInt256 IDynamicTupleDecoder.UInt168() 
        => UInt168();
    public Int256 Int176()
    {
        var result = AbiTypes.Int256.Decode(CurrentSlot);
        ConsumeBytes();
        return result;
    }
    Int256 IFixedTupleDecoder.Int176() 
        => Int176();
    Int256 IDynamicTupleDecoder.Int176() 
        => Int176();

    public UInt256 UInt176()
    {
        var result = AbiTypes.UInt256.Decode(CurrentSlot);
        ConsumeBytes();
        return result;
    }
    UInt256 IFixedTupleDecoder.UInt176() 
        => UInt176();
    UInt256 IDynamicTupleDecoder.UInt176() 
        => UInt176();
    public Int256 Int184()
    {
        var result = AbiTypes.Int256.Decode(CurrentSlot);
        ConsumeBytes();
        return result;
    }
    Int256 IFixedTupleDecoder.Int184() 
        => Int184();
    Int256 IDynamicTupleDecoder.Int184() 
        => Int184();

    public UInt256 UInt184()
    {
        var result = AbiTypes.UInt256.Decode(CurrentSlot);
        ConsumeBytes();
        return result;
    }
    UInt256 IFixedTupleDecoder.UInt184() 
        => UInt184();
    UInt256 IDynamicTupleDecoder.UInt184() 
        => UInt184();
    public Int256 Int192()
    {
        var result = AbiTypes.Int256.Decode(CurrentSlot);
        ConsumeBytes();
        return result;
    }
    Int256 IFixedTupleDecoder.Int192() 
        => Int192();
    Int256 IDynamicTupleDecoder.Int192() 
        => Int192();

    public UInt256 UInt192()
    {
        var result = AbiTypes.UInt256.Decode(CurrentSlot);
        ConsumeBytes();
        return result;
    }
    UInt256 IFixedTupleDecoder.UInt192() 
        => UInt192();
    UInt256 IDynamicTupleDecoder.UInt192() 
        => UInt192();
    public Int256 Int200()
    {
        var result = AbiTypes.Int256.Decode(CurrentSlot);
        ConsumeBytes();
        return result;
    }
    Int256 IFixedTupleDecoder.Int200() 
        => Int200();
    Int256 IDynamicTupleDecoder.Int200() 
        => Int200();

    public UInt256 UInt200()
    {
        var result = AbiTypes.UInt256.Decode(CurrentSlot);
        ConsumeBytes();
        return result;
    }
    UInt256 IFixedTupleDecoder.UInt200() 
        => UInt200();
    UInt256 IDynamicTupleDecoder.UInt200() 
        => UInt200();
    public Int256 Int208()
    {
        var result = AbiTypes.Int256.Decode(CurrentSlot);
        ConsumeBytes();
        return result;
    }
    Int256 IFixedTupleDecoder.Int208() 
        => Int208();
    Int256 IDynamicTupleDecoder.Int208() 
        => Int208();

    public UInt256 UInt208()
    {
        var result = AbiTypes.UInt256.Decode(CurrentSlot);
        ConsumeBytes();
        return result;
    }
    UInt256 IFixedTupleDecoder.UInt208() 
        => UInt208();
    UInt256 IDynamicTupleDecoder.UInt208() 
        => UInt208();
    public Int256 Int216()
    {
        var result = AbiTypes.Int256.Decode(CurrentSlot);
        ConsumeBytes();
        return result;
    }
    Int256 IFixedTupleDecoder.Int216() 
        => Int216();
    Int256 IDynamicTupleDecoder.Int216() 
        => Int216();

    public UInt256 UInt216()
    {
        var result = AbiTypes.UInt256.Decode(CurrentSlot);
        ConsumeBytes();
        return result;
    }
    UInt256 IFixedTupleDecoder.UInt216() 
        => UInt216();
    UInt256 IDynamicTupleDecoder.UInt216() 
        => UInt216();
    public Int256 Int224()
    {
        var result = AbiTypes.Int256.Decode(CurrentSlot);
        ConsumeBytes();
        return result;
    }
    Int256 IFixedTupleDecoder.Int224() 
        => Int224();
    Int256 IDynamicTupleDecoder.Int224() 
        => Int224();

    public UInt256 UInt224()
    {
        var result = AbiTypes.UInt256.Decode(CurrentSlot);
        ConsumeBytes();
        return result;
    }
    UInt256 IFixedTupleDecoder.UInt224() 
        => UInt224();
    UInt256 IDynamicTupleDecoder.UInt224() 
        => UInt224();
    public Int256 Int232()
    {
        var result = AbiTypes.Int256.Decode(CurrentSlot);
        ConsumeBytes();
        return result;
    }
    Int256 IFixedTupleDecoder.Int232() 
        => Int232();
    Int256 IDynamicTupleDecoder.Int232() 
        => Int232();

    public UInt256 UInt232()
    {
        var result = AbiTypes.UInt256.Decode(CurrentSlot);
        ConsumeBytes();
        return result;
    }
    UInt256 IFixedTupleDecoder.UInt232() 
        => UInt232();
    UInt256 IDynamicTupleDecoder.UInt232() 
        => UInt232();
    public Int256 Int240()
    {
        var result = AbiTypes.Int256.Decode(CurrentSlot);
        ConsumeBytes();
        return result;
    }
    Int256 IFixedTupleDecoder.Int240() 
        => Int240();
    Int256 IDynamicTupleDecoder.Int240() 
        => Int240();

    public UInt256 UInt240()
    {
        var result = AbiTypes.UInt256.Decode(CurrentSlot);
        ConsumeBytes();
        return result;
    }
    UInt256 IFixedTupleDecoder.UInt240() 
        => UInt240();
    UInt256 IDynamicTupleDecoder.UInt240() 
        => UInt240();
    public Int256 Int248()
    {
        var result = AbiTypes.Int256.Decode(CurrentSlot);
        ConsumeBytes();
        return result;
    }
    Int256 IFixedTupleDecoder.Int248() 
        => Int248();
    Int256 IDynamicTupleDecoder.Int248() 
        => Int248();

    public UInt256 UInt248()
    {
        var result = AbiTypes.UInt256.Decode(CurrentSlot);
        ConsumeBytes();
        return result;
    }
    UInt256 IFixedTupleDecoder.UInt248() 
        => UInt248();
    UInt256 IDynamicTupleDecoder.UInt248() 
        => UInt248();
    public Int256 Int256()
    {
        var result = AbiTypes.Int256.Decode(CurrentSlot);
        ConsumeBytes();
        return result;
    }
    Int256 IFixedTupleDecoder.Int256() 
        => Int256();
    Int256 IDynamicTupleDecoder.Int256() 
        => Int256();

    public UInt256 UInt256()
    {
        var result = AbiTypes.UInt256.Decode(CurrentSlot);
        ConsumeBytes();
        return result;
    }
    UInt256 IFixedTupleDecoder.UInt256() 
        => UInt256();
    UInt256 IDynamicTupleDecoder.UInt256() 
        => UInt256();
}
