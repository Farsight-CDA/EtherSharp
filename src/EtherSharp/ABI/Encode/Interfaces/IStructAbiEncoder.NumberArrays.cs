﻿using System.Numerics;

namespace EtherSharp.ABI.Encode.Interfaces;
public partial interface IStructAbiEncoder
{
    public IStructAbiEncoder Int8Array(params sbyte[] value);
    public IStructAbiEncoder UInt8Array(params byte[] value);

    public IStructAbiEncoder Int16Array(params short[] value);
    public IStructAbiEncoder UInt16Array(params ushort[] value);

    public IStructAbiEncoder Int24Array(params int[] value);
    public IStructAbiEncoder UInt24Array(params uint[] value);
    public IStructAbiEncoder Int32Array(params int[] value);
    public IStructAbiEncoder UInt32Array(params uint[] value);
    public IStructAbiEncoder Int40Array(params long[] value);
    public IStructAbiEncoder UInt40Array(params ulong[] value);
    public IStructAbiEncoder Int48Array(params long[] value);
    public IStructAbiEncoder UInt48Array(params ulong[] value);
    public IStructAbiEncoder Int56Array(params long[] value);
    public IStructAbiEncoder UInt56Array(params ulong[] value);
    public IStructAbiEncoder Int64Array(params long[] value);
    public IStructAbiEncoder UInt64Array(params ulong[] value);
    public IStructAbiEncoder Int72Array(params BigInteger[] value);
    public IStructAbiEncoder UInt72Array(params BigInteger[] value);
    public IStructAbiEncoder Int80Array(params BigInteger[] value);
    public IStructAbiEncoder UInt80Array(params BigInteger[] value);
    public IStructAbiEncoder Int88Array(params BigInteger[] value);
    public IStructAbiEncoder UInt88Array(params BigInteger[] value);
    public IStructAbiEncoder Int96Array(params BigInteger[] value);
    public IStructAbiEncoder UInt96Array(params BigInteger[] value);
    public IStructAbiEncoder Int104Array(params BigInteger[] value);
    public IStructAbiEncoder UInt104Array(params BigInteger[] value);
    public IStructAbiEncoder Int112Array(params BigInteger[] value);
    public IStructAbiEncoder UInt112Array(params BigInteger[] value);
    public IStructAbiEncoder Int120Array(params BigInteger[] value);
    public IStructAbiEncoder UInt120Array(params BigInteger[] value);
    public IStructAbiEncoder Int128Array(params BigInteger[] value);
    public IStructAbiEncoder UInt128Array(params BigInteger[] value);
    public IStructAbiEncoder Int136Array(params BigInteger[] value);
    public IStructAbiEncoder UInt136Array(params BigInteger[] value);
    public IStructAbiEncoder Int144Array(params BigInteger[] value);
    public IStructAbiEncoder UInt144Array(params BigInteger[] value);
    public IStructAbiEncoder Int152Array(params BigInteger[] value);
    public IStructAbiEncoder UInt152Array(params BigInteger[] value);
    public IStructAbiEncoder Int160Array(params BigInteger[] value);
    public IStructAbiEncoder UInt160Array(params BigInteger[] value);
    public IStructAbiEncoder Int168Array(params BigInteger[] value);
    public IStructAbiEncoder UInt168Array(params BigInteger[] value);
    public IStructAbiEncoder Int176Array(params BigInteger[] value);
    public IStructAbiEncoder UInt176Array(params BigInteger[] value);
    public IStructAbiEncoder Int184Array(params BigInteger[] value);
    public IStructAbiEncoder UInt184Array(params BigInteger[] value);
    public IStructAbiEncoder Int192Array(params BigInteger[] value);
    public IStructAbiEncoder UInt192Array(params BigInteger[] value);
    public IStructAbiEncoder Int200Array(params BigInteger[] value);
    public IStructAbiEncoder UInt200Array(params BigInteger[] value);
    public IStructAbiEncoder Int208Array(params BigInteger[] value);
    public IStructAbiEncoder UInt208Array(params BigInteger[] value);
    public IStructAbiEncoder Int216Array(params BigInteger[] value);
    public IStructAbiEncoder UInt216Array(params BigInteger[] value);
    public IStructAbiEncoder Int224Array(params BigInteger[] value);
    public IStructAbiEncoder UInt224Array(params BigInteger[] value);
    public IStructAbiEncoder Int232Array(params BigInteger[] value);
    public IStructAbiEncoder UInt232Array(params BigInteger[] value);
    public IStructAbiEncoder Int240Array(params BigInteger[] value);
    public IStructAbiEncoder UInt240Array(params BigInteger[] value);
    public IStructAbiEncoder Int248Array(params BigInteger[] value);
    public IStructAbiEncoder UInt248Array(params BigInteger[] value);
    public IStructAbiEncoder Int256Array(params BigInteger[] value);
    public IStructAbiEncoder UInt256Array(params BigInteger[] value);
}

