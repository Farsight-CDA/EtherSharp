﻿using System.Numerics;

namespace EtherSharp.ABI.Encode.Interfaces;
public partial interface IFixedTupleEncoder 
{
    public IFixedTupleEncoder Int8(sbyte value);
    public IFixedTupleEncoder UInt8(byte value);

    public IFixedTupleEncoder Int16(short value);
    public IFixedTupleEncoder UInt16(ushort value);

    public IFixedTupleEncoder Int24(int value);
    public IFixedTupleEncoder UInt24(uint value);
    public IFixedTupleEncoder Int32(int value);
    public IFixedTupleEncoder UInt32(uint value);
    public IFixedTupleEncoder Int40(long value);
    public IFixedTupleEncoder UInt40(ulong value);
    public IFixedTupleEncoder Int48(long value);
    public IFixedTupleEncoder UInt48(ulong value);
    public IFixedTupleEncoder Int56(long value);
    public IFixedTupleEncoder UInt56(ulong value);
    public IFixedTupleEncoder Int64(long value);
    public IFixedTupleEncoder UInt64(ulong value);
    public IFixedTupleEncoder Int72(BigInteger value);
    public IFixedTupleEncoder UInt72(BigInteger value);
    public IFixedTupleEncoder Int80(BigInteger value);
    public IFixedTupleEncoder UInt80(BigInteger value);
    public IFixedTupleEncoder Int88(BigInteger value);
    public IFixedTupleEncoder UInt88(BigInteger value);
    public IFixedTupleEncoder Int96(BigInteger value);
    public IFixedTupleEncoder UInt96(BigInteger value);
    public IFixedTupleEncoder Int104(BigInteger value);
    public IFixedTupleEncoder UInt104(BigInteger value);
    public IFixedTupleEncoder Int112(BigInteger value);
    public IFixedTupleEncoder UInt112(BigInteger value);
    public IFixedTupleEncoder Int120(BigInteger value);
    public IFixedTupleEncoder UInt120(BigInteger value);
    public IFixedTupleEncoder Int128(BigInteger value);
    public IFixedTupleEncoder UInt128(BigInteger value);
    public IFixedTupleEncoder Int136(BigInteger value);
    public IFixedTupleEncoder UInt136(BigInteger value);
    public IFixedTupleEncoder Int144(BigInteger value);
    public IFixedTupleEncoder UInt144(BigInteger value);
    public IFixedTupleEncoder Int152(BigInteger value);
    public IFixedTupleEncoder UInt152(BigInteger value);
    public IFixedTupleEncoder Int160(BigInteger value);
    public IFixedTupleEncoder UInt160(BigInteger value);
    public IFixedTupleEncoder Int168(BigInteger value);
    public IFixedTupleEncoder UInt168(BigInteger value);
    public IFixedTupleEncoder Int176(BigInteger value);
    public IFixedTupleEncoder UInt176(BigInteger value);
    public IFixedTupleEncoder Int184(BigInteger value);
    public IFixedTupleEncoder UInt184(BigInteger value);
    public IFixedTupleEncoder Int192(BigInteger value);
    public IFixedTupleEncoder UInt192(BigInteger value);
    public IFixedTupleEncoder Int200(BigInteger value);
    public IFixedTupleEncoder UInt200(BigInteger value);
    public IFixedTupleEncoder Int208(BigInteger value);
    public IFixedTupleEncoder UInt208(BigInteger value);
    public IFixedTupleEncoder Int216(BigInteger value);
    public IFixedTupleEncoder UInt216(BigInteger value);
    public IFixedTupleEncoder Int224(BigInteger value);
    public IFixedTupleEncoder UInt224(BigInteger value);
    public IFixedTupleEncoder Int232(BigInteger value);
    public IFixedTupleEncoder UInt232(BigInteger value);
    public IFixedTupleEncoder Int240(BigInteger value);
    public IFixedTupleEncoder UInt240(BigInteger value);
    public IFixedTupleEncoder Int248(BigInteger value);
    public IFixedTupleEncoder UInt248(BigInteger value);
    public IFixedTupleEncoder Int256(BigInteger value);
    public IFixedTupleEncoder UInt256(BigInteger value);
}