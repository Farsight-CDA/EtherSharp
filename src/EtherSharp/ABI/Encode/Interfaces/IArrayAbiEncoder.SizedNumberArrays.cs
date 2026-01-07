using EtherSharp.Numerics;

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
    public void Int72Array(params Int256[] value);
    public void UInt72Array(params UInt256[] value);
    public void Int80Array(params Int256[] value);
    public void UInt80Array(params UInt256[] value);
    public void Int88Array(params Int256[] value);
    public void UInt88Array(params UInt256[] value);
    public void Int96Array(params Int256[] value);
    public void UInt96Array(params UInt256[] value);
    public void Int104Array(params Int256[] value);
    public void UInt104Array(params UInt256[] value);
    public void Int112Array(params Int256[] value);
    public void UInt112Array(params UInt256[] value);
    public void Int120Array(params Int256[] value);
    public void UInt120Array(params UInt256[] value);
    public void Int128Array(params Int256[] value);
    public void UInt128Array(params UInt256[] value);
    public void Int136Array(params Int256[] value);
    public void UInt136Array(params UInt256[] value);
    public void Int144Array(params Int256[] value);
    public void UInt144Array(params UInt256[] value);
    public void Int152Array(params Int256[] value);
    public void UInt152Array(params UInt256[] value);
    public void Int160Array(params Int256[] value);
    public void UInt160Array(params UInt256[] value);
    public void Int168Array(params Int256[] value);
    public void UInt168Array(params UInt256[] value);
    public void Int176Array(params Int256[] value);
    public void UInt176Array(params UInt256[] value);
    public void Int184Array(params Int256[] value);
    public void UInt184Array(params UInt256[] value);
    public void Int192Array(params Int256[] value);
    public void UInt192Array(params UInt256[] value);
    public void Int200Array(params Int256[] value);
    public void UInt200Array(params UInt256[] value);
    public void Int208Array(params Int256[] value);
    public void UInt208Array(params UInt256[] value);
    public void Int216Array(params Int256[] value);
    public void UInt216Array(params UInt256[] value);
    public void Int224Array(params Int256[] value);
    public void UInt224Array(params UInt256[] value);
    public void Int232Array(params Int256[] value);
    public void UInt232Array(params UInt256[] value);
    public void Int240Array(params Int256[] value);
    public void UInt240Array(params UInt256[] value);
    public void Int248Array(params Int256[] value);
    public void UInt248Array(params UInt256[] value);
    public void Int256Array(params Int256[] value);
    public void UInt256Array(params UInt256[] value);
}

