using System.Numerics;

namespace EtherSharp.ABI.Encode.Interfaces;
public partial interface IArrayAbiEncoder
{
    public void Int8Array(params sbyte[] value);
    public void UInt8Array(params byte[] value);

    public void Int16Array(params short[] value);
    public void UInt16Array(params ushort[] value);

    public void Int24Array(params int[] value);
    public void UInt24Array(params uint[] value);
    public void Int32Array(params int[] value);
    public void UInt32Array(params uint[] value);
    public void Int40Array(params long[] value);
    public void UInt40Array(params ulong[] value);
    public void Int48Array(params long[] value);
    public void UInt48Array(params ulong[] value);
    public void Int56Array(params long[] value);
    public void UInt56Array(params ulong[] value);
    public void Int64Array(params long[] value);
    public void UInt64Array(params ulong[] value);
    public void Int72Array(params BigInteger[] value);
    public void UInt72Array(params BigInteger[] value);
    public void Int80Array(params BigInteger[] value);
    public void UInt80Array(params BigInteger[] value);
    public void Int88Array(params BigInteger[] value);
    public void UInt88Array(params BigInteger[] value);
    public void Int96Array(params BigInteger[] value);
    public void UInt96Array(params BigInteger[] value);
    public void Int104Array(params BigInteger[] value);
    public void UInt104Array(params BigInteger[] value);
    public void Int112Array(params BigInteger[] value);
    public void UInt112Array(params BigInteger[] value);
    public void Int120Array(params BigInteger[] value);
    public void UInt120Array(params BigInteger[] value);
    public void Int128Array(params BigInteger[] value);
    public void UInt128Array(params BigInteger[] value);
    public void Int136Array(params BigInteger[] value);
    public void UInt136Array(params BigInteger[] value);
    public void Int144Array(params BigInteger[] value);
    public void UInt144Array(params BigInteger[] value);
    public void Int152Array(params BigInteger[] value);
    public void UInt152Array(params BigInteger[] value);
    public void Int160Array(params BigInteger[] value);
    public void UInt160Array(params BigInteger[] value);
    public void Int168Array(params BigInteger[] value);
    public void UInt168Array(params BigInteger[] value);
    public void Int176Array(params BigInteger[] value);
    public void UInt176Array(params BigInteger[] value);
    public void Int184Array(params BigInteger[] value);
    public void UInt184Array(params BigInteger[] value);
    public void Int192Array(params BigInteger[] value);
    public void UInt192Array(params BigInteger[] value);
    public void Int200Array(params BigInteger[] value);
    public void UInt200Array(params BigInteger[] value);
    public void Int208Array(params BigInteger[] value);
    public void UInt208Array(params BigInteger[] value);
    public void Int216Array(params BigInteger[] value);
    public void UInt216Array(params BigInteger[] value);
    public void Int224Array(params BigInteger[] value);
    public void UInt224Array(params BigInteger[] value);
    public void Int232Array(params BigInteger[] value);
    public void UInt232Array(params BigInteger[] value);
    public void Int240Array(params BigInteger[] value);
    public void UInt240Array(params BigInteger[] value);
    public void Int248Array(params BigInteger[] value);
    public void UInt248Array(params BigInteger[] value);
    public void Int256Array(params BigInteger[] value);
    public void UInt256Array(params BigInteger[] value);
}

