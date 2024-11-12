using EtherSharp.ABI.Fixed;
using System.Numerics;
namespace EtherSharp.ABI.Decode;
public partial class AbiDecoder
{
    public AbiDecoder Int8(out sbyte value)
    {
        value = FixedType<object>.SByte.Decode(CurrentSlot);
        return ConsumeBytes();
    }
    public AbiDecoder UInt8(out byte value)
    {
        value = FixedType<object>.Byte.Decode(CurrentSlot);
        return ConsumeBytes();
    }
    public AbiDecoder Int16(out short value)
    {
        value = FixedType<object>.Byte.Decode(CurrentSlot);
        return ConsumeBytes();
    }
    public AbiDecoder UInt16(out ushort value)
    {
        value = FixedType<object>.Byte.Decode(CurrentSlot);
        return ConsumeBytes();
    }
    public AbiDecoder Int24(out int value)
    {
        value = FixedType<object>.Int.Decode(CurrentSlot);
        return ConsumeBytes();
    }
    public AbiDecoder UInt24(out uint value)
    {
        value = FixedType<object>.UInt.Decode(CurrentSlot);
        return ConsumeBytes();
    }
    public AbiDecoder Int32(out int value)
    {
        value = FixedType<object>.Int.Decode(CurrentSlot);
        return ConsumeBytes();
    }
    public AbiDecoder UInt32(out uint value)
    {
        value = FixedType<object>.UInt.Decode(CurrentSlot);
        return ConsumeBytes();
    }
    public AbiDecoder Int40(out long value)
    {
        value = FixedType<object>.Long.Decode(CurrentSlot);
        return ConsumeBytes();
    }
    public AbiDecoder UInt40(out ulong value)
    {
        value = FixedType<object>.ULong.Decode(CurrentSlot);
        return ConsumeBytes();
    }
    public AbiDecoder Int48(out long value)
    {
        value = FixedType<object>.Long.Decode(CurrentSlot);
        return ConsumeBytes();
    }
    public AbiDecoder UInt48(out ulong value)
    {
        value = FixedType<object>.ULong.Decode(CurrentSlot);
        return ConsumeBytes();
    }
    public AbiDecoder Int56(out long value)
    {
        value = FixedType<object>.Long.Decode(CurrentSlot);
        return ConsumeBytes();
    }
    public AbiDecoder UInt56(out ulong value)
    {
        value = FixedType<object>.ULong.Decode(CurrentSlot);
        return ConsumeBytes();
    }
    public AbiDecoder Int64(out long value)
    {
        value = FixedType<object>.Long.Decode(CurrentSlot);
        return ConsumeBytes();
    }
    public AbiDecoder UInt64(out ulong value)
    {
        value = FixedType<object>.ULong.Decode(CurrentSlot);
        return ConsumeBytes();
    }
    public AbiDecoder Int72(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, false);
        return ConsumeBytes();
    }
    public AbiDecoder UInt72(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, true);
        return ConsumeBytes();
    }
    public AbiDecoder Int80(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, false);
        return ConsumeBytes();
    }
    public AbiDecoder UInt80(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, true);
        return ConsumeBytes();
    }
    public AbiDecoder Int88(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, false);
        return ConsumeBytes();
    }
    public AbiDecoder UInt88(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, true);
        return ConsumeBytes();
    }
    public AbiDecoder Int96(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, false);
        return ConsumeBytes();
    }
    public AbiDecoder UInt96(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, true);
        return ConsumeBytes();
    }
    public AbiDecoder Int104(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, false);
        return ConsumeBytes();
    }
    public AbiDecoder UInt104(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, true);
        return ConsumeBytes();
    }
    public AbiDecoder Int112(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, false);
        return ConsumeBytes();
    }
    public AbiDecoder UInt112(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, true);
        return ConsumeBytes();
    }
    public AbiDecoder Int120(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, false);
        return ConsumeBytes();
    }
    public AbiDecoder UInt120(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, true);
        return ConsumeBytes();
    }
    public AbiDecoder Int128(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, false);
        return ConsumeBytes();
    }
    public AbiDecoder UInt128(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, true);
        return ConsumeBytes();
    }
    public AbiDecoder Int136(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, false);
        return ConsumeBytes();
    }
    public AbiDecoder UInt136(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, true);
        return ConsumeBytes();
    }
    public AbiDecoder Int144(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, false);
        return ConsumeBytes();
    }
    public AbiDecoder UInt144(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, true);
        return ConsumeBytes();
    }
    public AbiDecoder Int152(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, false);
        return ConsumeBytes();
    }
    public AbiDecoder UInt152(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, true);
        return ConsumeBytes();
    }
    public AbiDecoder Int160(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, false);
        return ConsumeBytes();
    }
    public AbiDecoder UInt160(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, true);
        return ConsumeBytes();
    }
    public AbiDecoder Int168(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, false);
        return ConsumeBytes();
    }
    public AbiDecoder UInt168(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, true);
        return ConsumeBytes();
    }
    public AbiDecoder Int176(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, false);
        return ConsumeBytes();
    }
    public AbiDecoder UInt176(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, true);
        return ConsumeBytes();
    }
    public AbiDecoder Int184(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, false);
        return ConsumeBytes();
    }
    public AbiDecoder UInt184(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, true);
        return ConsumeBytes();
    }
    public AbiDecoder Int192(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, false);
        return ConsumeBytes();
    }
    public AbiDecoder UInt192(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, true);
        return ConsumeBytes();
    }
    public AbiDecoder Int200(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, false);
        return ConsumeBytes();
    }
    public AbiDecoder UInt200(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, true);
        return ConsumeBytes();
    }
    public AbiDecoder Int208(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, false);
        return ConsumeBytes();
    }
    public AbiDecoder UInt208(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, true);
        return ConsumeBytes();
    }
    public AbiDecoder Int216(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, false);
        return ConsumeBytes();
    }
    public AbiDecoder UInt216(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, true);
        return ConsumeBytes();
    }
    public AbiDecoder Int224(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, false);
        return ConsumeBytes();
    }
    public AbiDecoder UInt224(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, true);
        return ConsumeBytes();
    }
    public AbiDecoder Int232(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, false);
        return ConsumeBytes();
    }
    public AbiDecoder UInt232(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, true);
        return ConsumeBytes();
    }
    public AbiDecoder Int240(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, false);
        return ConsumeBytes();
    }
    public AbiDecoder UInt240(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, true);
        return ConsumeBytes();
    }
    public AbiDecoder Int248(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, false);
        return ConsumeBytes();
    }
    public AbiDecoder UInt248(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, true);
        return ConsumeBytes();
    }
    public AbiDecoder Int256(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, false);
        return ConsumeBytes();
    }
    public AbiDecoder UInt256(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, true);
        return ConsumeBytes();
    }
}