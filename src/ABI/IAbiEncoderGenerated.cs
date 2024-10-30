
using EtherSharp.ABI.Types;
using System.Numerics;

namespace EtherSharp.ABI;
public partial interface IAbiEncoder
{

    public AbiEncoder Int24(int value);

    public AbiEncoder UInt24(uint value);

    public AbiEncoder Int32(int value);

    public AbiEncoder UInt32(uint value);

    public AbiEncoder Int40(long value);

    public AbiEncoder UInt40(ulong value);

    public AbiEncoder Int48(long value);

    public AbiEncoder UInt48(ulong value);

    public AbiEncoder Int56(long value);

    public AbiEncoder UInt56(ulong value);

    public AbiEncoder Int64(long value);

    public AbiEncoder UInt64(ulong value);

    public AbiEncoder Int72(BigInteger value);

    public AbiEncoder UInt72(BigInteger value);

    public AbiEncoder Int80(BigInteger value);

    public AbiEncoder UInt80(BigInteger value);

    public AbiEncoder Int88(BigInteger value);

    public AbiEncoder UInt88(BigInteger value);

    public AbiEncoder Int96(BigInteger value);

    public AbiEncoder UInt96(BigInteger value);

    public AbiEncoder Int104(BigInteger value);

    public AbiEncoder UInt104(BigInteger value);

    public AbiEncoder Int112(BigInteger value);

    public AbiEncoder UInt112(BigInteger value);

    public AbiEncoder Int120(BigInteger value);

    public AbiEncoder UInt120(BigInteger value);

    public AbiEncoder Int128(BigInteger value);

    public AbiEncoder UInt128(BigInteger value);

    public AbiEncoder Int136(BigInteger value);

    public AbiEncoder UInt136(BigInteger value);

    public AbiEncoder Int144(BigInteger value);

    public AbiEncoder UInt144(BigInteger value);

    public AbiEncoder Int152(BigInteger value);

    public AbiEncoder UInt152(BigInteger value);

    public AbiEncoder Int160(BigInteger value);

    public AbiEncoder UInt160(BigInteger value);

    public AbiEncoder Int168(BigInteger value);

    public AbiEncoder UInt168(BigInteger value);

    public AbiEncoder Int176(BigInteger value);

    public AbiEncoder UInt176(BigInteger value);

    public AbiEncoder Int184(BigInteger value);

    public AbiEncoder UInt184(BigInteger value);

    public AbiEncoder Int192(BigInteger value);

    public AbiEncoder UInt192(BigInteger value);

    public AbiEncoder Int200(BigInteger value);

    public AbiEncoder UInt200(BigInteger value);

    public AbiEncoder Int208(BigInteger value);

    public AbiEncoder UInt208(BigInteger value);

    public AbiEncoder Int216(BigInteger value);

    public AbiEncoder UInt216(BigInteger value);

    public AbiEncoder Int224(BigInteger value);

    public AbiEncoder UInt224(BigInteger value);

    public AbiEncoder Int232(BigInteger value);

    public AbiEncoder UInt232(BigInteger value);

    public AbiEncoder Int240(BigInteger value);

    public AbiEncoder UInt240(BigInteger value);

    public AbiEncoder Int248(BigInteger value);

    public AbiEncoder UInt248(BigInteger value);

    public AbiEncoder Int256(BigInteger value);

    public AbiEncoder UInt256(BigInteger value);
}