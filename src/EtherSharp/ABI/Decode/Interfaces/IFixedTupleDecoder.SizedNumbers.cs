using EtherSharp.Numerics;

namespace EtherSharp.ABI.Decode.Interfaces;
public partial interface IFixedTupleDecoder
{
    /// <summary>
    /// Reads an int8 value from the input.
    /// </summary>
    /// <returns>The decoded signed byte value.</returns>
    public sbyte Int8();
    /// <summary>
    /// Reads a uint8 value from the input.
    /// </summary>
    /// <returns>The decoded byte value.</returns>
    public byte UInt8();

    /// <summary>
    /// Reads an int16 value from the input.
    /// </summary>
    /// <returns>The decoded short value.</returns>
    public short Int16();
    /// <summary>
    /// Reads a uint16 value from the input.
    /// </summary>
    /// <returns>The decoded unsigned short value.</returns>
    public ushort UInt16();

    /// <summary>
    /// Reads an int24 value from the input.
    /// </summary>
    /// <returns>The decoded int value.</returns>
    public int Int24();
    /// <summary>
    /// Reads a uint24 value from the input.
    /// </summary>
    /// <returns>The decoded uint value.</returns>
    public uint UInt24();
    /// <summary>
    /// Reads an int32 value from the input.
    /// </summary>
    /// <returns>The decoded int value.</returns>
    public int Int32();
    /// <summary>
    /// Reads a uint32 value from the input.
    /// </summary>
    /// <returns>The decoded uint value.</returns>
    public uint UInt32();
    /// <summary>
    /// Reads an int40 value from the input.
    /// </summary>
    /// <returns>The decoded long value.</returns>
    public long Int40();
    /// <summary>
    /// Reads a uint40 value from the input.
    /// </summary>
    /// <returns>The decoded ulong value.</returns>
    public ulong UInt40();
    /// <summary>
    /// Reads an int48 value from the input.
    /// </summary>
    /// <returns>The decoded long value.</returns>
    public long Int48();
    /// <summary>
    /// Reads a uint48 value from the input.
    /// </summary>
    /// <returns>The decoded ulong value.</returns>
    public ulong UInt48();
    /// <summary>
    /// Reads an int56 value from the input.
    /// </summary>
    /// <returns>The decoded long value.</returns>
    public long Int56();
    /// <summary>
    /// Reads a uint56 value from the input.
    /// </summary>
    /// <returns>The decoded ulong value.</returns>
    public ulong UInt56();
    /// <summary>
    /// Reads an int64 value from the input.
    /// </summary>
    /// <returns>The decoded long value.</returns>
    public long Int64();
    /// <summary>
    /// Reads a uint64 value from the input.
    /// </summary>
    /// <returns>The decoded ulong value.</returns>
    public ulong UInt64();
    /// <summary>
    /// Reads an int72 value from the input.
    /// </summary>
    /// <returns>The decoded Int256 value.</returns>
    public Int256 Int72();
    /// <summary>
    /// Reads a uint72 value from the input.
    /// </summary>
    /// <returns>The decoded UInt256 value.</returns>
    public UInt256 UInt72();
    /// <summary>
    /// Reads an int80 value from the input.
    /// </summary>
    /// <returns>The decoded Int256 value.</returns>
    public Int256 Int80();
    /// <summary>
    /// Reads a uint80 value from the input.
    /// </summary>
    /// <returns>The decoded UInt256 value.</returns>
    public UInt256 UInt80();
    /// <summary>
    /// Reads an int88 value from the input.
    /// </summary>
    /// <returns>The decoded Int256 value.</returns>
    public Int256 Int88();
    /// <summary>
    /// Reads a uint88 value from the input.
    /// </summary>
    /// <returns>The decoded UInt256 value.</returns>
    public UInt256 UInt88();
    /// <summary>
    /// Reads an int96 value from the input.
    /// </summary>
    /// <returns>The decoded Int256 value.</returns>
    public Int256 Int96();
    /// <summary>
    /// Reads a uint96 value from the input.
    /// </summary>
    /// <returns>The decoded UInt256 value.</returns>
    public UInt256 UInt96();
    /// <summary>
    /// Reads an int104 value from the input.
    /// </summary>
    /// <returns>The decoded Int256 value.</returns>
    public Int256 Int104();
    /// <summary>
    /// Reads a uint104 value from the input.
    /// </summary>
    /// <returns>The decoded UInt256 value.</returns>
    public UInt256 UInt104();
    /// <summary>
    /// Reads an int112 value from the input.
    /// </summary>
    /// <returns>The decoded Int256 value.</returns>
    public Int256 Int112();
    /// <summary>
    /// Reads a uint112 value from the input.
    /// </summary>
    /// <returns>The decoded UInt256 value.</returns>
    public UInt256 UInt112();
    /// <summary>
    /// Reads an int120 value from the input.
    /// </summary>
    /// <returns>The decoded Int256 value.</returns>
    public Int256 Int120();
    /// <summary>
    /// Reads a uint120 value from the input.
    /// </summary>
    /// <returns>The decoded UInt256 value.</returns>
    public UInt256 UInt120();
    /// <summary>
    /// Reads an int128 value from the input.
    /// </summary>
    /// <returns>The decoded Int256 value.</returns>
    public Int256 Int128();
    /// <summary>
    /// Reads a uint128 value from the input.
    /// </summary>
    /// <returns>The decoded UInt256 value.</returns>
    public UInt256 UInt128();
    /// <summary>
    /// Reads an int136 value from the input.
    /// </summary>
    /// <returns>The decoded Int256 value.</returns>
    public Int256 Int136();
    /// <summary>
    /// Reads a uint136 value from the input.
    /// </summary>
    /// <returns>The decoded UInt256 value.</returns>
    public UInt256 UInt136();
    /// <summary>
    /// Reads an int144 value from the input.
    /// </summary>
    /// <returns>The decoded Int256 value.</returns>
    public Int256 Int144();
    /// <summary>
    /// Reads a uint144 value from the input.
    /// </summary>
    /// <returns>The decoded UInt256 value.</returns>
    public UInt256 UInt144();
    /// <summary>
    /// Reads an int152 value from the input.
    /// </summary>
    /// <returns>The decoded Int256 value.</returns>
    public Int256 Int152();
    /// <summary>
    /// Reads a uint152 value from the input.
    /// </summary>
    /// <returns>The decoded UInt256 value.</returns>
    public UInt256 UInt152();
    /// <summary>
    /// Reads an int160 value from the input.
    /// </summary>
    /// <returns>The decoded Int256 value.</returns>
    public Int256 Int160();
    /// <summary>
    /// Reads a uint160 value from the input.
    /// </summary>
    /// <returns>The decoded UInt256 value.</returns>
    public UInt256 UInt160();
    /// <summary>
    /// Reads an int168 value from the input.
    /// </summary>
    /// <returns>The decoded Int256 value.</returns>
    public Int256 Int168();
    /// <summary>
    /// Reads a uint168 value from the input.
    /// </summary>
    /// <returns>The decoded UInt256 value.</returns>
    public UInt256 UInt168();
    /// <summary>
    /// Reads an int176 value from the input.
    /// </summary>
    /// <returns>The decoded Int256 value.</returns>
    public Int256 Int176();
    /// <summary>
    /// Reads a uint176 value from the input.
    /// </summary>
    /// <returns>The decoded UInt256 value.</returns>
    public UInt256 UInt176();
    /// <summary>
    /// Reads an int184 value from the input.
    /// </summary>
    /// <returns>The decoded Int256 value.</returns>
    public Int256 Int184();
    /// <summary>
    /// Reads a uint184 value from the input.
    /// </summary>
    /// <returns>The decoded UInt256 value.</returns>
    public UInt256 UInt184();
    /// <summary>
    /// Reads an int192 value from the input.
    /// </summary>
    /// <returns>The decoded Int256 value.</returns>
    public Int256 Int192();
    /// <summary>
    /// Reads a uint192 value from the input.
    /// </summary>
    /// <returns>The decoded UInt256 value.</returns>
    public UInt256 UInt192();
    /// <summary>
    /// Reads an int200 value from the input.
    /// </summary>
    /// <returns>The decoded Int256 value.</returns>
    public Int256 Int200();
    /// <summary>
    /// Reads a uint200 value from the input.
    /// </summary>
    /// <returns>The decoded UInt256 value.</returns>
    public UInt256 UInt200();
    /// <summary>
    /// Reads an int208 value from the input.
    /// </summary>
    /// <returns>The decoded Int256 value.</returns>
    public Int256 Int208();
    /// <summary>
    /// Reads a uint208 value from the input.
    /// </summary>
    /// <returns>The decoded UInt256 value.</returns>
    public UInt256 UInt208();
    /// <summary>
    /// Reads an int216 value from the input.
    /// </summary>
    /// <returns>The decoded Int256 value.</returns>
    public Int256 Int216();
    /// <summary>
    /// Reads a uint216 value from the input.
    /// </summary>
    /// <returns>The decoded UInt256 value.</returns>
    public UInt256 UInt216();
    /// <summary>
    /// Reads an int224 value from the input.
    /// </summary>
    /// <returns>The decoded Int256 value.</returns>
    public Int256 Int224();
    /// <summary>
    /// Reads a uint224 value from the input.
    /// </summary>
    /// <returns>The decoded UInt256 value.</returns>
    public UInt256 UInt224();
    /// <summary>
    /// Reads an int232 value from the input.
    /// </summary>
    /// <returns>The decoded Int256 value.</returns>
    public Int256 Int232();
    /// <summary>
    /// Reads a uint232 value from the input.
    /// </summary>
    /// <returns>The decoded UInt256 value.</returns>
    public UInt256 UInt232();
    /// <summary>
    /// Reads an int240 value from the input.
    /// </summary>
    /// <returns>The decoded Int256 value.</returns>
    public Int256 Int240();
    /// <summary>
    /// Reads a uint240 value from the input.
    /// </summary>
    /// <returns>The decoded UInt256 value.</returns>
    public UInt256 UInt240();
    /// <summary>
    /// Reads an int248 value from the input.
    /// </summary>
    /// <returns>The decoded Int256 value.</returns>
    public Int256 Int248();
    /// <summary>
    /// Reads a uint248 value from the input.
    /// </summary>
    /// <returns>The decoded UInt256 value.</returns>
    public UInt256 UInt248();
    /// <summary>
    /// Reads an int256 value from the input.
    /// </summary>
    /// <returns>The decoded Int256 value.</returns>
    public Int256 Int256();
    /// <summary>
    /// Reads a uint256 value from the input.
    /// </summary>
    /// <returns>The decoded UInt256 value.</returns>
    public UInt256 UInt256();
}
