using System.Numerics;

namespace EtherSharp.ABI.Decode.Interfaces;
public partial interface IStructAbiDecoder
{
    public sbyte[] Int8Array();
    public byte[] UInt8Array();

    public short[] Int16Array();
    public ushort[] UInt16Array();

    public int[] Int24Array();
    public uint[] UInt24Array();
    public int[] Int32Array();
    public uint[] UInt32Array();
    public long[] Int40Array();
    public ulong[] UInt40Array();
    public long[] Int48Array();
    public ulong[] UInt48Array();
    public long[] Int56Array();
    public ulong[] UInt56Array();
    public long[] Int64Array();
    public ulong[] UInt64Array();
    public BigInteger[] Int72Array();
    public BigInteger[] UInt72Array();
    public BigInteger[] Int80Array();
    public BigInteger[] UInt80Array();
    public BigInteger[] Int88Array();
    public BigInteger[] UInt88Array();
    public BigInteger[] Int96Array();
    public BigInteger[] UInt96Array();
    public BigInteger[] Int104Array();
    public BigInteger[] UInt104Array();
    public BigInteger[] Int112Array();
    public BigInteger[] UInt112Array();
    public BigInteger[] Int120Array();
    public BigInteger[] UInt120Array();
    public BigInteger[] Int128Array();
    public BigInteger[] UInt128Array();
    public BigInteger[] Int136Array();
    public BigInteger[] UInt136Array();
    public BigInteger[] Int144Array();
    public BigInteger[] UInt144Array();
    public BigInteger[] Int152Array();
    public BigInteger[] UInt152Array();
    public BigInteger[] Int160Array();
    public BigInteger[] UInt160Array();
    public BigInteger[] Int168Array();
    public BigInteger[] UInt168Array();
    public BigInteger[] Int176Array();
    public BigInteger[] UInt176Array();
    public BigInteger[] Int184Array();
    public BigInteger[] UInt184Array();
    public BigInteger[] Int192Array();
    public BigInteger[] UInt192Array();
    public BigInteger[] Int200Array();
    public BigInteger[] UInt200Array();
    public BigInteger[] Int208Array();
    public BigInteger[] UInt208Array();
    public BigInteger[] Int216Array();
    public BigInteger[] UInt216Array();
    public BigInteger[] Int224Array();
    public BigInteger[] UInt224Array();
    public BigInteger[] Int232Array();
    public BigInteger[] UInt232Array();
    public BigInteger[] Int240Array();
    public BigInteger[] UInt240Array();
    public BigInteger[] Int248Array();
    public BigInteger[] UInt248Array();
    public BigInteger[] Int256Array();
    public BigInteger[] UInt256Array();
}

