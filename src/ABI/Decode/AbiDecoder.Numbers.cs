using EtherSharp.ABI.Fixed;
using System.Numerics;
namespace EtherSharp.ABI.Decode;
public partial class AbiDecoder
{
    public AbiDecoder Int24(out short value)
    {
        value = FixedType<object>.Short.Decode(EncodedBytes);
        return ConsumeBytes(0);
    }
    public AbiDecoder UInt24(out ushort value)
    {
        value = FixedType<object>.UShort.Decode(EncodedBytes);
        return ConsumeBytes(0);
    }
    public AbiDecoder Int32(out short value)
    {
        value = FixedType<object>.Short.Decode(EncodedBytes);
        return ConsumeBytes(0);
    }
    public AbiDecoder UInt32(out ushort value)
    {
        value = FixedType<object>.UShort.Decode(EncodedBytes);
        return ConsumeBytes(0);
    }
    public AbiDecoder Int40(out long value)
    {
        value = FixedType<object>.Long.Decode(EncodedBytes);
        return ConsumeBytes(0);
    }
    public AbiDecoder UInt40(out ulong value)
    {
        value = FixedType<object>.ULong.Decode(EncodedBytes);
        return ConsumeBytes(0);
    }
    public AbiDecoder Int48(out long value)
    {
        value = FixedType<object>.Long.Decode(EncodedBytes);
        return ConsumeBytes(0);
    }
    public AbiDecoder UInt48(out ulong value)
    {
        value = FixedType<object>.ULong.Decode(EncodedBytes);
        return ConsumeBytes(0);
    }
    public AbiDecoder Int56(out long value)
    {
        value = FixedType<object>.Long.Decode(EncodedBytes);
        return ConsumeBytes(0);
    }
    public AbiDecoder UInt56(out ulong value)
    {
        value = FixedType<object>.ULong.Decode(EncodedBytes);
        return ConsumeBytes(0);
    }
    public AbiDecoder Int64(out long value)
    {
        value = FixedType<object>.Long.Decode(EncodedBytes);
        return ConsumeBytes(0);
    }
    public AbiDecoder UInt64(out ulong value)
    {
        value = FixedType<object>.ULong.Decode(EncodedBytes);
        return ConsumeBytes(0);
    }
    public AbiDecoder Int72(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(EncodedBytes, false);
        return ConsumeBytes(0);
    }
    public AbiDecoder UInt72(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(EncodedBytes, true);
        return ConsumeBytes(0);
    }
    public AbiDecoder Int80(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(EncodedBytes, false);
        return ConsumeBytes(0);
    }
    public AbiDecoder UInt80(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(EncodedBytes, true);
        return ConsumeBytes(0);
    }
    public AbiDecoder Int88(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(EncodedBytes, false);
        return ConsumeBytes(0);
    }
    public AbiDecoder UInt88(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(EncodedBytes, true);
        return ConsumeBytes(0);
    }
    public AbiDecoder Int96(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(EncodedBytes, false);
        return ConsumeBytes(0);
    }
    public AbiDecoder UInt96(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(EncodedBytes, true);
        return ConsumeBytes(0);
    }
    public AbiDecoder Int104(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(EncodedBytes, false);
        return ConsumeBytes(0);
    }
    public AbiDecoder UInt104(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(EncodedBytes, true);
        return ConsumeBytes(0);
    }
    public AbiDecoder Int112(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(EncodedBytes, false);
        return ConsumeBytes(0);
    }
    public AbiDecoder UInt112(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(EncodedBytes, true);
        return ConsumeBytes(0);
    }
    public AbiDecoder Int120(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(EncodedBytes, false);
        return ConsumeBytes(0);
    }
    public AbiDecoder UInt120(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(EncodedBytes, true);
        return ConsumeBytes(0);
    }
    public AbiDecoder Int128(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(EncodedBytes, false);
        return ConsumeBytes(0);
    }
    public AbiDecoder UInt128(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(EncodedBytes, true);
        return ConsumeBytes(0);
    }
    public AbiDecoder Int136(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(EncodedBytes, false);
        return ConsumeBytes(0);
    }
    public AbiDecoder UInt136(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(EncodedBytes, true);
        return ConsumeBytes(0);
    }
    public AbiDecoder Int144(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(EncodedBytes, false);
        return ConsumeBytes(0);
    }
    public AbiDecoder UInt144(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(EncodedBytes, true);
        return ConsumeBytes(0);
    }
    public AbiDecoder Int152(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(EncodedBytes, false);
        return ConsumeBytes(0);
    }
    public AbiDecoder UInt152(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(EncodedBytes, true);
        return ConsumeBytes(0);
    }
    public AbiDecoder Int160(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(EncodedBytes, false);
        return ConsumeBytes(0);
    }
    public AbiDecoder UInt160(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(EncodedBytes, true);
        return ConsumeBytes(0);
    }
    public AbiDecoder Int168(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(EncodedBytes, false);
        return ConsumeBytes(0);
    }
    public AbiDecoder UInt168(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(EncodedBytes, true);
        return ConsumeBytes(0);
    }
    public AbiDecoder Int176(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(EncodedBytes, false);
        return ConsumeBytes(0);
    }
    public AbiDecoder UInt176(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(EncodedBytes, true);
        return ConsumeBytes(0);
    }
    public AbiDecoder Int184(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(EncodedBytes, false);
        return ConsumeBytes(0);
    }
    public AbiDecoder UInt184(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(EncodedBytes, true);
        return ConsumeBytes(0);
    }
    public AbiDecoder Int192(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(EncodedBytes, false);
        return ConsumeBytes(0);
    }
    public AbiDecoder UInt192(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(EncodedBytes, true);
        return ConsumeBytes(0);
    }
    public AbiDecoder Int200(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(EncodedBytes, false);
        return ConsumeBytes(0);
    }
    public AbiDecoder UInt200(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(EncodedBytes, true);
        return ConsumeBytes(0);
    }
    public AbiDecoder Int208(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(EncodedBytes, false);
        return ConsumeBytes(0);
    }
    public AbiDecoder UInt208(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(EncodedBytes, true);
        return ConsumeBytes(0);
    }
    public AbiDecoder Int216(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(EncodedBytes, false);
        return ConsumeBytes(0);
    }
    public AbiDecoder UInt216(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(EncodedBytes, true);
        return ConsumeBytes(0);
    }
    public AbiDecoder Int224(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(EncodedBytes, false);
        return ConsumeBytes(0);
    }
    public AbiDecoder UInt224(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(EncodedBytes, true);
        return ConsumeBytes(0);
    }
    public AbiDecoder Int232(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(EncodedBytes, false);
        return ConsumeBytes(0);
    }
    public AbiDecoder UInt232(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(EncodedBytes, true);
        return ConsumeBytes(0);
    }
    public AbiDecoder Int240(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(EncodedBytes, false);
        return ConsumeBytes(0);
    }
    public AbiDecoder UInt240(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(EncodedBytes, true);
        return ConsumeBytes(0);
    }
    public AbiDecoder Int248(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(EncodedBytes, false);
        return ConsumeBytes(0);
    }
    public AbiDecoder UInt248(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(EncodedBytes, true);
        return ConsumeBytes(0);
    }
    public AbiDecoder Int256(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(EncodedBytes, false);
        return ConsumeBytes(0);
    }
    public AbiDecoder UInt256(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(EncodedBytes, true);
        return ConsumeBytes(0);
    }
}