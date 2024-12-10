﻿using System.Numerics;

namespace EtherSharp.ABI.Decode.Interfaces;
public partial interface IDynamicTupleDecoder
{
    public sbyte Int8();
    public byte UInt8();

    public short Int16();
    public ushort UInt16();

    public int Int24();
    public uint UInt24();
    public int Int32();
    public uint UInt32();
    public long Int40();
    public ulong UInt40();
    public long Int48();
    public ulong UInt48();
    public long Int56();
    public ulong UInt56();
    public long Int64();
    public ulong UInt64();
    public BigInteger Int72();
    public BigInteger UInt72();
    public BigInteger Int80();
    public BigInteger UInt80();
    public BigInteger Int88();
    public BigInteger UInt88();
    public BigInteger Int96();
    public BigInteger UInt96();
    public BigInteger Int104();
    public BigInteger UInt104();
    public BigInteger Int112();
    public BigInteger UInt112();
    public BigInteger Int120();
    public BigInteger UInt120();
    public BigInteger Int128();
    public BigInteger UInt128();
    public BigInteger Int136();
    public BigInteger UInt136();
    public BigInteger Int144();
    public BigInteger UInt144();
    public BigInteger Int152();
    public BigInteger UInt152();
    public BigInteger Int160();
    public BigInteger UInt160();
    public BigInteger Int168();
    public BigInteger UInt168();
    public BigInteger Int176();
    public BigInteger UInt176();
    public BigInteger Int184();
    public BigInteger UInt184();
    public BigInteger Int192();
    public BigInteger UInt192();
    public BigInteger Int200();
    public BigInteger UInt200();
    public BigInteger Int208();
    public BigInteger UInt208();
    public BigInteger Int216();
    public BigInteger UInt216();
    public BigInteger Int224();
    public BigInteger UInt224();
    public BigInteger Int232();
    public BigInteger UInt232();
    public BigInteger Int240();
    public BigInteger UInt240();
    public BigInteger Int248();
    public BigInteger UInt248();
    public BigInteger Int256();
    public BigInteger UInt256();
}