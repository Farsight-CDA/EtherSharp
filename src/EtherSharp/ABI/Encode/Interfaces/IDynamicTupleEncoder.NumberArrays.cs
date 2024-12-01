using System.Numerics;

namespace EtherSharp.ABI.Encode.Interfaces;
public partial interface IDynamicTupleEncoder 
{
    public IDynamicTupleEncoder Int8Array(params sbyte[] value);
    public IDynamicTupleEncoder UInt8Array(params byte[] value);

    public IDynamicTupleEncoder Int16Array(params short[] value);
    public IDynamicTupleEncoder UInt16Array(params ushort[] value);

    public IDynamicTupleEncoder Int24Array(params int[] value);
    public IDynamicTupleEncoder UInt24Array(params uint[] value);
    public IDynamicTupleEncoder Int32Array(params int[] value);
    public IDynamicTupleEncoder UInt32Array(params uint[] value);
    public IDynamicTupleEncoder Int40Array(params long[] value);
    public IDynamicTupleEncoder UInt40Array(params ulong[] value);
    public IDynamicTupleEncoder Int48Array(params long[] value);
    public IDynamicTupleEncoder UInt48Array(params ulong[] value);
    public IDynamicTupleEncoder Int56Array(params long[] value);
    public IDynamicTupleEncoder UInt56Array(params ulong[] value);
    public IDynamicTupleEncoder Int64Array(params long[] value);
    public IDynamicTupleEncoder UInt64Array(params ulong[] value);
    public IDynamicTupleEncoder Int72Array(params BigInteger[] value);
    public IDynamicTupleEncoder UInt72Array(params BigInteger[] value);
    public IDynamicTupleEncoder Int80Array(params BigInteger[] value);
    public IDynamicTupleEncoder UInt80Array(params BigInteger[] value);
    public IDynamicTupleEncoder Int88Array(params BigInteger[] value);
    public IDynamicTupleEncoder UInt88Array(params BigInteger[] value);
    public IDynamicTupleEncoder Int96Array(params BigInteger[] value);
    public IDynamicTupleEncoder UInt96Array(params BigInteger[] value);
    public IDynamicTupleEncoder Int104Array(params BigInteger[] value);
    public IDynamicTupleEncoder UInt104Array(params BigInteger[] value);
    public IDynamicTupleEncoder Int112Array(params BigInteger[] value);
    public IDynamicTupleEncoder UInt112Array(params BigInteger[] value);
    public IDynamicTupleEncoder Int120Array(params BigInteger[] value);
    public IDynamicTupleEncoder UInt120Array(params BigInteger[] value);
    public IDynamicTupleEncoder Int128Array(params BigInteger[] value);
    public IDynamicTupleEncoder UInt128Array(params BigInteger[] value);
    public IDynamicTupleEncoder Int136Array(params BigInteger[] value);
    public IDynamicTupleEncoder UInt136Array(params BigInteger[] value);
    public IDynamicTupleEncoder Int144Array(params BigInteger[] value);
    public IDynamicTupleEncoder UInt144Array(params BigInteger[] value);
    public IDynamicTupleEncoder Int152Array(params BigInteger[] value);
    public IDynamicTupleEncoder UInt152Array(params BigInteger[] value);
    public IDynamicTupleEncoder Int160Array(params BigInteger[] value);
    public IDynamicTupleEncoder UInt160Array(params BigInteger[] value);
    public IDynamicTupleEncoder Int168Array(params BigInteger[] value);
    public IDynamicTupleEncoder UInt168Array(params BigInteger[] value);
    public IDynamicTupleEncoder Int176Array(params BigInteger[] value);
    public IDynamicTupleEncoder UInt176Array(params BigInteger[] value);
    public IDynamicTupleEncoder Int184Array(params BigInteger[] value);
    public IDynamicTupleEncoder UInt184Array(params BigInteger[] value);
    public IDynamicTupleEncoder Int192Array(params BigInteger[] value);
    public IDynamicTupleEncoder UInt192Array(params BigInteger[] value);
    public IDynamicTupleEncoder Int200Array(params BigInteger[] value);
    public IDynamicTupleEncoder UInt200Array(params BigInteger[] value);
    public IDynamicTupleEncoder Int208Array(params BigInteger[] value);
    public IDynamicTupleEncoder UInt208Array(params BigInteger[] value);
    public IDynamicTupleEncoder Int216Array(params BigInteger[] value);
    public IDynamicTupleEncoder UInt216Array(params BigInteger[] value);
    public IDynamicTupleEncoder Int224Array(params BigInteger[] value);
    public IDynamicTupleEncoder UInt224Array(params BigInteger[] value);
    public IDynamicTupleEncoder Int232Array(params BigInteger[] value);
    public IDynamicTupleEncoder UInt232Array(params BigInteger[] value);
    public IDynamicTupleEncoder Int240Array(params BigInteger[] value);
    public IDynamicTupleEncoder UInt240Array(params BigInteger[] value);
    public IDynamicTupleEncoder Int248Array(params BigInteger[] value);
    public IDynamicTupleEncoder UInt248Array(params BigInteger[] value);
    public IDynamicTupleEncoder Int256Array(params BigInteger[] value);
    public IDynamicTupleEncoder UInt256Array(params BigInteger[] value);
}

