﻿using System.Numerics;

namespace EtherSharp.ABI.Encode.Interfaces;
public partial interface IArrayAbiEncoder
{
    public IArrayAbiEncoder Int8Array(params sbyte[] value);
    public IArrayAbiEncoder UInt8Array(params byte[] value);

    public IArrayAbiEncoder Int16Array(params short[] value);
    public IArrayAbiEncoder UInt16Array(params ushort[] value);

    public IArrayAbiEncoder Int24Array(params int[] value);
    public IArrayAbiEncoder UInt24Array(params uint[] value);
    public IArrayAbiEncoder Int32Array(params int[] value);
    public IArrayAbiEncoder UInt32Array(params uint[] value);
    public IArrayAbiEncoder Int40Array(params long[] value);
    public IArrayAbiEncoder UInt40Array(params ulong[] value);
    public IArrayAbiEncoder Int48Array(params long[] value);
    public IArrayAbiEncoder UInt48Array(params ulong[] value);
    public IArrayAbiEncoder Int56Array(params long[] value);
    public IArrayAbiEncoder UInt56Array(params ulong[] value);
    public IArrayAbiEncoder Int64Array(params long[] value);
    public IArrayAbiEncoder UInt64Array(params ulong[] value);
    public IArrayAbiEncoder Int72Array(params BigInteger[] value);
    public IArrayAbiEncoder UInt72Array(params BigInteger[] value);
    public IArrayAbiEncoder Int80Array(params BigInteger[] value);
    public IArrayAbiEncoder UInt80Array(params BigInteger[] value);
    public IArrayAbiEncoder Int88Array(params BigInteger[] value);
    public IArrayAbiEncoder UInt88Array(params BigInteger[] value);
    public IArrayAbiEncoder Int96Array(params BigInteger[] value);
    public IArrayAbiEncoder UInt96Array(params BigInteger[] value);
    public IArrayAbiEncoder Int104Array(params BigInteger[] value);
    public IArrayAbiEncoder UInt104Array(params BigInteger[] value);
    public IArrayAbiEncoder Int112Array(params BigInteger[] value);
    public IArrayAbiEncoder UInt112Array(params BigInteger[] value);
    public IArrayAbiEncoder Int120Array(params BigInteger[] value);
    public IArrayAbiEncoder UInt120Array(params BigInteger[] value);
    public IArrayAbiEncoder Int128Array(params BigInteger[] value);
    public IArrayAbiEncoder UInt128Array(params BigInteger[] value);
    public IArrayAbiEncoder Int136Array(params BigInteger[] value);
    public IArrayAbiEncoder UInt136Array(params BigInteger[] value);
    public IArrayAbiEncoder Int144Array(params BigInteger[] value);
    public IArrayAbiEncoder UInt144Array(params BigInteger[] value);
    public IArrayAbiEncoder Int152Array(params BigInteger[] value);
    public IArrayAbiEncoder UInt152Array(params BigInteger[] value);
    public IArrayAbiEncoder Int160Array(params BigInteger[] value);
    public IArrayAbiEncoder UInt160Array(params BigInteger[] value);
    public IArrayAbiEncoder Int168Array(params BigInteger[] value);
    public IArrayAbiEncoder UInt168Array(params BigInteger[] value);
    public IArrayAbiEncoder Int176Array(params BigInteger[] value);
    public IArrayAbiEncoder UInt176Array(params BigInteger[] value);
    public IArrayAbiEncoder Int184Array(params BigInteger[] value);
    public IArrayAbiEncoder UInt184Array(params BigInteger[] value);
    public IArrayAbiEncoder Int192Array(params BigInteger[] value);
    public IArrayAbiEncoder UInt192Array(params BigInteger[] value);
    public IArrayAbiEncoder Int200Array(params BigInteger[] value);
    public IArrayAbiEncoder UInt200Array(params BigInteger[] value);
    public IArrayAbiEncoder Int208Array(params BigInteger[] value);
    public IArrayAbiEncoder UInt208Array(params BigInteger[] value);
    public IArrayAbiEncoder Int216Array(params BigInteger[] value);
    public IArrayAbiEncoder UInt216Array(params BigInteger[] value);
    public IArrayAbiEncoder Int224Array(params BigInteger[] value);
    public IArrayAbiEncoder UInt224Array(params BigInteger[] value);
    public IArrayAbiEncoder Int232Array(params BigInteger[] value);
    public IArrayAbiEncoder UInt232Array(params BigInteger[] value);
    public IArrayAbiEncoder Int240Array(params BigInteger[] value);
    public IArrayAbiEncoder UInt240Array(params BigInteger[] value);
    public IArrayAbiEncoder Int248Array(params BigInteger[] value);
    public IArrayAbiEncoder UInt248Array(params BigInteger[] value);
    public IArrayAbiEncoder Int256Array(params BigInteger[] value);
    public IArrayAbiEncoder UInt256Array(params BigInteger[] value);
}

