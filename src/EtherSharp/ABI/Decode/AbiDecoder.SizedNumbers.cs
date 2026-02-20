using EtherSharp.ABI.Decode.Interfaces;
using EtherSharp.ABI.Types;
using EtherSharp.Numerics;

namespace EtherSharp.ABI;

public partial class AbiDecoder
{
    /// <summary>
    /// Reads an int8 value from the input.
    /// </summary>
    /// <returns>The decoded signed byte value.</returns>
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

    /// <summary>
    /// Reads a uint8 value from the input.
    /// </summary>
    /// <returns>The decoded byte value.</returns>
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

    /// <summary>
    /// Reads an int16 value from the input.
    /// </summary>
    /// <returns>The decoded short value.</returns>
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

    /// <summary>
    /// Reads a uint16 value from the input.
    /// </summary>
    /// <returns>The decoded unsigned short value.</returns>
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

    /// <summary>
    /// Reads an int24 value from the input.
    /// </summary>
    /// <returns>The decoded int value.</returns>
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

    /// <summary>
    /// Reads a uint24 value from the input.
    /// </summary>
    /// <returns>The decoded uint value.</returns>
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
    /// <summary>
    /// Reads an int32 value from the input.
    /// </summary>
    /// <returns>The decoded int value.</returns>
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

    /// <summary>
    /// Reads a uint32 value from the input.
    /// </summary>
    /// <returns>The decoded uint value.</returns>
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

    /// <summary>
    /// Reads an int40 value from the input.
    /// </summary>
    /// <returns>The decoded long value.</returns>
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

    /// <summary>
    /// Reads a uint40 value from the input.
    /// </summary>
    /// <returns>The decoded ulong value.</returns>
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
    /// <summary>
    /// Reads an int48 value from the input.
    /// </summary>
    /// <returns>The decoded long value.</returns>
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

    /// <summary>
    /// Reads a uint48 value from the input.
    /// </summary>
    /// <returns>The decoded ulong value.</returns>
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
    /// <summary>
    /// Reads an int56 value from the input.
    /// </summary>
    /// <returns>The decoded long value.</returns>
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

    /// <summary>
    /// Reads a uint56 value from the input.
    /// </summary>
    /// <returns>The decoded ulong value.</returns>
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
    /// <summary>
    /// Reads an int64 value from the input.
    /// </summary>
    /// <returns>The decoded long value.</returns>
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

    /// <summary>
    /// Reads a uint64 value from the input.
    /// </summary>
    /// <returns>The decoded ulong value.</returns>
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

    /// <summary>
    /// Reads an int72 value from the input.
    /// </summary>
    /// <returns>The decoded Int256 value.</returns>
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

    /// <summary>
    /// Reads a uint72 value from the input.
    /// </summary>
    /// <returns>The decoded UInt256 value.</returns>
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
    /// <summary>
    /// Reads an int80 value from the input.
    /// </summary>
    /// <returns>The decoded Int256 value.</returns>
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

    /// <summary>
    /// Reads a uint80 value from the input.
    /// </summary>
    /// <returns>The decoded UInt256 value.</returns>
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
    /// <summary>
    /// Reads an int88 value from the input.
    /// </summary>
    /// <returns>The decoded Int256 value.</returns>
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

    /// <summary>
    /// Reads a uint88 value from the input.
    /// </summary>
    /// <returns>The decoded UInt256 value.</returns>
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
    /// <summary>
    /// Reads an int96 value from the input.
    /// </summary>
    /// <returns>The decoded Int256 value.</returns>
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

    /// <summary>
    /// Reads a uint96 value from the input.
    /// </summary>
    /// <returns>The decoded UInt256 value.</returns>
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
    /// <summary>
    /// Reads an int104 value from the input.
    /// </summary>
    /// <returns>The decoded Int256 value.</returns>
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

    /// <summary>
    /// Reads a uint104 value from the input.
    /// </summary>
    /// <returns>The decoded UInt256 value.</returns>
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
    /// <summary>
    /// Reads an int112 value from the input.
    /// </summary>
    /// <returns>The decoded Int256 value.</returns>
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

    /// <summary>
    /// Reads a uint112 value from the input.
    /// </summary>
    /// <returns>The decoded UInt256 value.</returns>
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
    /// <summary>
    /// Reads an int120 value from the input.
    /// </summary>
    /// <returns>The decoded Int256 value.</returns>
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

    /// <summary>
    /// Reads a uint120 value from the input.
    /// </summary>
    /// <returns>The decoded UInt256 value.</returns>
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
    /// <summary>
    /// Reads an int128 value from the input.
    /// </summary>
    /// <returns>The decoded Int256 value.</returns>
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

    /// <summary>
    /// Reads a uint128 value from the input.
    /// </summary>
    /// <returns>The decoded UInt256 value.</returns>
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
    /// <summary>
    /// Reads an int136 value from the input.
    /// </summary>
    /// <returns>The decoded Int256 value.</returns>
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

    /// <summary>
    /// Reads a uint136 value from the input.
    /// </summary>
    /// <returns>The decoded UInt256 value.</returns>
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
    /// <summary>
    /// Reads an int144 value from the input.
    /// </summary>
    /// <returns>The decoded Int256 value.</returns>
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

    /// <summary>
    /// Reads a uint144 value from the input.
    /// </summary>
    /// <returns>The decoded UInt256 value.</returns>
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
    /// <summary>
    /// Reads an int152 value from the input.
    /// </summary>
    /// <returns>The decoded Int256 value.</returns>
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

    /// <summary>
    /// Reads a uint152 value from the input.
    /// </summary>
    /// <returns>The decoded UInt256 value.</returns>
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
    /// <summary>
    /// Reads an int160 value from the input.
    /// </summary>
    /// <returns>The decoded Int256 value.</returns>
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

    /// <summary>
    /// Reads a uint160 value from the input.
    /// </summary>
    /// <returns>The decoded UInt256 value.</returns>
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
    /// <summary>
    /// Reads an int168 value from the input.
    /// </summary>
    /// <returns>The decoded Int256 value.</returns>
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

    /// <summary>
    /// Reads a uint168 value from the input.
    /// </summary>
    /// <returns>The decoded UInt256 value.</returns>
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
    /// <summary>
    /// Reads an int176 value from the input.
    /// </summary>
    /// <returns>The decoded Int256 value.</returns>
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

    /// <summary>
    /// Reads a uint176 value from the input.
    /// </summary>
    /// <returns>The decoded UInt256 value.</returns>
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
    /// <summary>
    /// Reads an int184 value from the input.
    /// </summary>
    /// <returns>The decoded Int256 value.</returns>
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

    /// <summary>
    /// Reads a uint184 value from the input.
    /// </summary>
    /// <returns>The decoded UInt256 value.</returns>
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
    /// <summary>
    /// Reads an int192 value from the input.
    /// </summary>
    /// <returns>The decoded Int256 value.</returns>
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

    /// <summary>
    /// Reads a uint192 value from the input.
    /// </summary>
    /// <returns>The decoded UInt256 value.</returns>
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
    /// <summary>
    /// Reads an int200 value from the input.
    /// </summary>
    /// <returns>The decoded Int256 value.</returns>
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

    /// <summary>
    /// Reads a uint200 value from the input.
    /// </summary>
    /// <returns>The decoded UInt256 value.</returns>
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
    /// <summary>
    /// Reads an int208 value from the input.
    /// </summary>
    /// <returns>The decoded Int256 value.</returns>
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

    /// <summary>
    /// Reads a uint208 value from the input.
    /// </summary>
    /// <returns>The decoded UInt256 value.</returns>
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
    /// <summary>
    /// Reads an int216 value from the input.
    /// </summary>
    /// <returns>The decoded Int256 value.</returns>
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

    /// <summary>
    /// Reads a uint216 value from the input.
    /// </summary>
    /// <returns>The decoded UInt256 value.</returns>
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
    /// <summary>
    /// Reads an int224 value from the input.
    /// </summary>
    /// <returns>The decoded Int256 value.</returns>
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

    /// <summary>
    /// Reads a uint224 value from the input.
    /// </summary>
    /// <returns>The decoded UInt256 value.</returns>
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
    /// <summary>
    /// Reads an int232 value from the input.
    /// </summary>
    /// <returns>The decoded Int256 value.</returns>
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

    /// <summary>
    /// Reads a uint232 value from the input.
    /// </summary>
    /// <returns>The decoded UInt256 value.</returns>
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
    /// <summary>
    /// Reads an int240 value from the input.
    /// </summary>
    /// <returns>The decoded Int256 value.</returns>
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

    /// <summary>
    /// Reads a uint240 value from the input.
    /// </summary>
    /// <returns>The decoded UInt256 value.</returns>
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
    /// <summary>
    /// Reads an int248 value from the input.
    /// </summary>
    /// <returns>The decoded Int256 value.</returns>
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

    /// <summary>
    /// Reads a uint248 value from the input.
    /// </summary>
    /// <returns>The decoded UInt256 value.</returns>
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
    /// <summary>
    /// Reads an int256 value from the input.
    /// </summary>
    /// <returns>The decoded Int256 value.</returns>
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

    /// <summary>
    /// Reads a uint256 value from the input.
    /// </summary>
    /// <returns>The decoded UInt256 value.</returns>
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
