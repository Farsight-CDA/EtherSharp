using EtherSharp.ABI.Encode.Interfaces;
using EtherSharp.ABI.Fixed;
using System.Numerics;

namespace EtherSharp.ABI;
public partial class AbiEncoder
{
    public AbiEncoder Int24(int value)
        => AddElement(new FixedType<object>.Int(value, 24));
    IStructAbiEncoder IStructAbiEncoder.Int24(int value)
        => Int24(value);

    public AbiEncoder UInt24(uint value)
        => AddElement(new FixedType<object>.UInt(value, 24));
    IStructAbiEncoder IStructAbiEncoder.UInt24(uint value)
        => UInt24(value);

    public AbiEncoder Int32(int value)
        => AddElement(new FixedType<object>.Int(value, 32));
    IStructAbiEncoder IStructAbiEncoder.Int32(int value)
        => Int32(value);

    public AbiEncoder UInt32(uint value)
        => AddElement(new FixedType<object>.UInt(value, 32));
    IStructAbiEncoder IStructAbiEncoder.UInt32(uint value)
        => UInt32(value);

    public AbiEncoder Int40(long value)
        => AddElement(new FixedType<object>.Long(value, 40));
    IStructAbiEncoder IStructAbiEncoder.Int40(long value)
        => Int40(value);

    public AbiEncoder UInt40(ulong value)
        => AddElement(new FixedType<object>.ULong(value, 40));
    IStructAbiEncoder IStructAbiEncoder.UInt40(ulong value)
        => UInt40(value);

    public AbiEncoder Int48(long value)
        => AddElement(new FixedType<object>.Long(value, 48));
    IStructAbiEncoder IStructAbiEncoder.Int48(long value)
        => Int48(value);

    public AbiEncoder UInt48(ulong value)
        => AddElement(new FixedType<object>.ULong(value, 48));
    IStructAbiEncoder IStructAbiEncoder.UInt48(ulong value)
        => UInt48(value);

    public AbiEncoder Int56(long value)
        => AddElement(new FixedType<object>.Long(value, 56));
    IStructAbiEncoder IStructAbiEncoder.Int56(long value)
        => Int56(value);

    public AbiEncoder UInt56(ulong value)
        => AddElement(new FixedType<object>.ULong(value, 56));
    IStructAbiEncoder IStructAbiEncoder.UInt56(ulong value)
        => UInt56(value);

    public AbiEncoder Int64(long value)
        => AddElement(new FixedType<object>.Long(value, 64));
    IStructAbiEncoder IStructAbiEncoder.Int64(long value)
        => Int64(value);

    public AbiEncoder UInt64(ulong value)
        => AddElement(new FixedType<object>.ULong(value, 64));
    IStructAbiEncoder IStructAbiEncoder.UInt64(ulong value)
        => UInt64(value);

    public AbiEncoder Int72(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, false, 72));
    IStructAbiEncoder IStructAbiEncoder.Int72(BigInteger value)
        => Int72(value);

    public AbiEncoder UInt72(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, true, 72));
    IStructAbiEncoder IStructAbiEncoder.UInt72(BigInteger value)
        => UInt72(value);
    public AbiEncoder Int80(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, false, 80));
    IStructAbiEncoder IStructAbiEncoder.Int80(BigInteger value)
        => Int80(value);

    public AbiEncoder UInt80(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, true, 80));
    IStructAbiEncoder IStructAbiEncoder.UInt80(BigInteger value)
        => UInt80(value);
    public AbiEncoder Int88(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, false, 88));
    IStructAbiEncoder IStructAbiEncoder.Int88(BigInteger value)
        => Int88(value);

    public AbiEncoder UInt88(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, true, 88));
    IStructAbiEncoder IStructAbiEncoder.UInt88(BigInteger value)
        => UInt88(value);
    public AbiEncoder Int96(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, false, 96));
    IStructAbiEncoder IStructAbiEncoder.Int96(BigInteger value)
        => Int96(value);

    public AbiEncoder UInt96(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, true, 96));
    IStructAbiEncoder IStructAbiEncoder.UInt96(BigInteger value)
        => UInt96(value);
    public AbiEncoder Int104(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, false, 104));
    IStructAbiEncoder IStructAbiEncoder.Int104(BigInteger value)
        => Int104(value);

    public AbiEncoder UInt104(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, true, 104));
    IStructAbiEncoder IStructAbiEncoder.UInt104(BigInteger value)
        => UInt104(value);
    public AbiEncoder Int112(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, false, 112));
    IStructAbiEncoder IStructAbiEncoder.Int112(BigInteger value)
        => Int112(value);

    public AbiEncoder UInt112(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, true, 112));
    IStructAbiEncoder IStructAbiEncoder.UInt112(BigInteger value)
        => UInt112(value);
    public AbiEncoder Int120(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, false, 120));
    IStructAbiEncoder IStructAbiEncoder.Int120(BigInteger value)
        => Int120(value);

    public AbiEncoder UInt120(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, true, 120));
    IStructAbiEncoder IStructAbiEncoder.UInt120(BigInteger value)
        => UInt120(value);
    public AbiEncoder Int128(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, false, 128));
    IStructAbiEncoder IStructAbiEncoder.Int128(BigInteger value)
        => Int128(value);

    public AbiEncoder UInt128(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, true, 128));
    IStructAbiEncoder IStructAbiEncoder.UInt128(BigInteger value)
        => UInt128(value);
    public AbiEncoder Int136(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, false, 136));
    IStructAbiEncoder IStructAbiEncoder.Int136(BigInteger value)
        => Int136(value);

    public AbiEncoder UInt136(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, true, 136));
    IStructAbiEncoder IStructAbiEncoder.UInt136(BigInteger value)
        => UInt136(value);
    public AbiEncoder Int144(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, false, 144));
    IStructAbiEncoder IStructAbiEncoder.Int144(BigInteger value)
        => Int144(value);

    public AbiEncoder UInt144(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, true, 144));
    IStructAbiEncoder IStructAbiEncoder.UInt144(BigInteger value)
        => UInt144(value);
    public AbiEncoder Int152(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, false, 152));
    IStructAbiEncoder IStructAbiEncoder.Int152(BigInteger value)
        => Int152(value);

    public AbiEncoder UInt152(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, true, 152));
    IStructAbiEncoder IStructAbiEncoder.UInt152(BigInteger value)
        => UInt152(value);
    public AbiEncoder Int160(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, false, 160));
    IStructAbiEncoder IStructAbiEncoder.Int160(BigInteger value)
        => Int160(value);

    public AbiEncoder UInt160(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, true, 160));
    IStructAbiEncoder IStructAbiEncoder.UInt160(BigInteger value)
        => UInt160(value);
    public AbiEncoder Int168(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, false, 168));
    IStructAbiEncoder IStructAbiEncoder.Int168(BigInteger value)
        => Int168(value);

    public AbiEncoder UInt168(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, true, 168));
    IStructAbiEncoder IStructAbiEncoder.UInt168(BigInteger value)
        => UInt168(value);
    public AbiEncoder Int176(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, false, 176));
    IStructAbiEncoder IStructAbiEncoder.Int176(BigInteger value)
        => Int176(value);

    public AbiEncoder UInt176(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, true, 176));
    IStructAbiEncoder IStructAbiEncoder.UInt176(BigInteger value)
        => UInt176(value);
    public AbiEncoder Int184(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, false, 184));
    IStructAbiEncoder IStructAbiEncoder.Int184(BigInteger value)
        => Int184(value);

    public AbiEncoder UInt184(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, true, 184));
    IStructAbiEncoder IStructAbiEncoder.UInt184(BigInteger value)
        => UInt184(value);
    public AbiEncoder Int192(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, false, 192));
    IStructAbiEncoder IStructAbiEncoder.Int192(BigInteger value)
        => Int192(value);

    public AbiEncoder UInt192(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, true, 192));
    IStructAbiEncoder IStructAbiEncoder.UInt192(BigInteger value)
        => UInt192(value);
    public AbiEncoder Int200(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, false, 200));
    IStructAbiEncoder IStructAbiEncoder.Int200(BigInteger value)
        => Int200(value);

    public AbiEncoder UInt200(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, true, 200));
    IStructAbiEncoder IStructAbiEncoder.UInt200(BigInteger value)
        => UInt200(value);
    public AbiEncoder Int208(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, false, 208));
    IStructAbiEncoder IStructAbiEncoder.Int208(BigInteger value)
        => Int208(value);

    public AbiEncoder UInt208(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, true, 208));
    IStructAbiEncoder IStructAbiEncoder.UInt208(BigInteger value)
        => UInt208(value);
    public AbiEncoder Int216(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, false, 216));
    IStructAbiEncoder IStructAbiEncoder.Int216(BigInteger value)
        => Int216(value);

    public AbiEncoder UInt216(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, true, 216));
    IStructAbiEncoder IStructAbiEncoder.UInt216(BigInteger value)
        => UInt216(value);
    public AbiEncoder Int224(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, false, 224));
    IStructAbiEncoder IStructAbiEncoder.Int224(BigInteger value)
        => Int224(value);

    public AbiEncoder UInt224(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, true, 224));
    IStructAbiEncoder IStructAbiEncoder.UInt224(BigInteger value)
        => UInt224(value);
    public AbiEncoder Int232(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, false, 232));
    IStructAbiEncoder IStructAbiEncoder.Int232(BigInteger value)
        => Int232(value);

    public AbiEncoder UInt232(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, true, 232));
    IStructAbiEncoder IStructAbiEncoder.UInt232(BigInteger value)
        => UInt232(value);
    public AbiEncoder Int240(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, false, 240));
    IStructAbiEncoder IStructAbiEncoder.Int240(BigInteger value)
        => Int240(value);

    public AbiEncoder UInt240(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, true, 240));
    IStructAbiEncoder IStructAbiEncoder.UInt240(BigInteger value)
        => UInt240(value);
    public AbiEncoder Int248(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, false, 248));
    IStructAbiEncoder IStructAbiEncoder.Int248(BigInteger value)
        => Int248(value);

    public AbiEncoder UInt248(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, true, 248));
    IStructAbiEncoder IStructAbiEncoder.UInt248(BigInteger value)
        => UInt248(value);
    public AbiEncoder Int256(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, false, 256));
    IStructAbiEncoder IStructAbiEncoder.Int256(BigInteger value)
        => Int256(value);

    public AbiEncoder UInt256(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, true, 256));
    IStructAbiEncoder IStructAbiEncoder.UInt256(BigInteger value)
        => UInt256(value);
}