
using System.Numerics;

namespace EtherSharp.ABI;
public partial interface IAbiEncoder
{

    public AbiDecoder Int24(int value);

    public AbiDecoder UInt24(uint value);

    public AbiDecoder Int32(int value);

    public AbiDecoder UInt32(uint value);

    public AbiDecoder Int40(long value);

    public AbiDecoder UInt40(ulong value);

    public AbiDecoder Int48(long value);

    public AbiDecoder UInt48(ulong value);

    public AbiDecoder Int56(long value);

    public AbiDecoder UInt56(ulong value);

    public AbiDecoder Int64(long value);

    public AbiDecoder UInt64(ulong value);

    public AbiDecoder Int72(BigInteger value);

    public AbiDecoder UInt72(BigInteger value);

    public AbiDecoder Int80(BigInteger value);

    public AbiDecoder UInt80(BigInteger value);

    public AbiDecoder Int88(BigInteger value);

    public AbiDecoder UInt88(BigInteger value);

    public AbiDecoder Int96(BigInteger value);

    public AbiDecoder UInt96(BigInteger value);

    public AbiDecoder Int104(BigInteger value);

    public AbiDecoder UInt104(BigInteger value);

    public AbiDecoder Int112(BigInteger value);

    public AbiDecoder UInt112(BigInteger value);

    public AbiDecoder Int120(BigInteger value);

    public AbiDecoder UInt120(BigInteger value);

    public AbiDecoder Int128(BigInteger value);

    public AbiDecoder UInt128(BigInteger value);

    public AbiDecoder Int136(BigInteger value);

    public AbiDecoder UInt136(BigInteger value);

    public AbiDecoder Int144(BigInteger value);

    public AbiDecoder UInt144(BigInteger value);

    public AbiDecoder Int152(BigInteger value);

    public AbiDecoder UInt152(BigInteger value);

    public AbiDecoder Int160(BigInteger value);

    public AbiDecoder UInt160(BigInteger value);

    public AbiDecoder Int168(BigInteger value);

    public AbiDecoder UInt168(BigInteger value);

    public AbiDecoder Int176(BigInteger value);

    public AbiDecoder UInt176(BigInteger value);

    public AbiDecoder Int184(BigInteger value);

    public AbiDecoder UInt184(BigInteger value);

    public AbiDecoder Int192(BigInteger value);

    public AbiDecoder UInt192(BigInteger value);

    public AbiDecoder Int200(BigInteger value);

    public AbiDecoder UInt200(BigInteger value);

    public AbiDecoder Int208(BigInteger value);

    public AbiDecoder UInt208(BigInteger value);

    public AbiDecoder Int216(BigInteger value);

    public AbiDecoder UInt216(BigInteger value);

    public AbiDecoder Int224(BigInteger value);

    public AbiDecoder UInt224(BigInteger value);

    public AbiDecoder Int232(BigInteger value);

    public AbiDecoder UInt232(BigInteger value);

    public AbiDecoder Int240(BigInteger value);

    public AbiDecoder UInt240(BigInteger value);

    public AbiDecoder Int248(BigInteger value);

    public AbiDecoder UInt248(BigInteger value);

    public AbiDecoder Int256(BigInteger value);

    public AbiDecoder UInt256(BigInteger value);
}