using EtherSharp.ABI.Encode.Interfaces;
using EtherSharp.ABI.Fixed;
using System.Numerics;

namespace EtherSharp.ABI;
public partial class AbiEncoder
{
    public AbiEncoder UInt8(byte value)
        => AddElement(new FixedType<object>.Byte(value));
    IFixedTupleEncoder IFixedTupleEncoder.UInt8(byte value)
        => UInt8(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt8(byte value)
        => UInt8(value);

    public AbiEncoder Int8(sbyte value)
        => AddElement(new FixedType<object>.SByte(value));
    IFixedTupleEncoder IFixedTupleEncoder.Int8(sbyte value)
        => Int8(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int8(sbyte value)
        => Int8(value);

    public AbiEncoder UInt16(ushort value)
        => AddElement(new FixedType<object>.UShort(value));
    IFixedTupleEncoder IFixedTupleEncoder.UInt16(ushort value)
        => UInt16(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt16(ushort value)
        => UInt16(value);

    public AbiEncoder Int16(short value)
        => AddElement(new FixedType<object>.Short(value));
    IFixedTupleEncoder IFixedTupleEncoder.Int16(short value)
        => Int16(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int16(short value)
        => Int16(value);

    public AbiEncoder Int24(int value)
        => AddElement(new FixedType<object>.Int(value, 24));
    IFixedTupleEncoder IFixedTupleEncoder.Int24(int value)
        => Int24(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int24(int value)
        => Int24(value);

    public AbiEncoder UInt24(uint value)
        => AddElement(new FixedType<object>.UInt(value, 24));
    IFixedTupleEncoder IFixedTupleEncoder.UInt24(uint value)
        => UInt24(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt24(uint value)
        => UInt24(value);
    public AbiEncoder Int32(int value)
        => AddElement(new FixedType<object>.Int(value, 32));
    IFixedTupleEncoder IFixedTupleEncoder.Int32(int value)
        => Int32(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int32(int value)
        => Int32(value);

    public AbiEncoder UInt32(uint value)
        => AddElement(new FixedType<object>.UInt(value, 32));
    IFixedTupleEncoder IFixedTupleEncoder.UInt32(uint value)
        => UInt32(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt32(uint value)
        => UInt32(value);
    public AbiEncoder Int40(long value)
        => AddElement(new FixedType<object>.Long(value, 40));
    IFixedTupleEncoder IFixedTupleEncoder.Int40(long value)
        => Int40(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int40(long value)
        => Int40(value);

    public AbiEncoder UInt40(ulong value)
        => AddElement(new FixedType<object>.ULong(value, 40));
    IFixedTupleEncoder IFixedTupleEncoder.UInt40(ulong value)
        => UInt40(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt40(ulong value)
        => UInt40(value);
    public AbiEncoder Int48(long value)
        => AddElement(new FixedType<object>.Long(value, 48));
    IFixedTupleEncoder IFixedTupleEncoder.Int48(long value)
        => Int48(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int48(long value)
        => Int48(value);

    public AbiEncoder UInt48(ulong value)
        => AddElement(new FixedType<object>.ULong(value, 48));
    IFixedTupleEncoder IFixedTupleEncoder.UInt48(ulong value)
        => UInt48(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt48(ulong value)
        => UInt48(value);
    public AbiEncoder Int56(long value)
        => AddElement(new FixedType<object>.Long(value, 56));
    IFixedTupleEncoder IFixedTupleEncoder.Int56(long value)
        => Int56(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int56(long value)
        => Int56(value);

    public AbiEncoder UInt56(ulong value)
        => AddElement(new FixedType<object>.ULong(value, 56));
    IFixedTupleEncoder IFixedTupleEncoder.UInt56(ulong value)
        => UInt56(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt56(ulong value)
        => UInt56(value);
    public AbiEncoder Int64(long value)
        => AddElement(new FixedType<object>.Long(value, 64));
    IFixedTupleEncoder IFixedTupleEncoder.Int64(long value)
        => Int64(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int64(long value)
        => Int64(value);

    public AbiEncoder UInt64(ulong value)
        => AddElement(new FixedType<object>.ULong(value, 64));
    IFixedTupleEncoder IFixedTupleEncoder.UInt64(ulong value)
        => UInt64(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt64(ulong value)
        => UInt64(value);
    public AbiEncoder Int72(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, false, 72));
    IFixedTupleEncoder IFixedTupleEncoder.Int72(BigInteger value)
        => Int72(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int72(BigInteger value)
        => Int72(value);

    public AbiEncoder UInt72(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, true, 72));
    IFixedTupleEncoder IFixedTupleEncoder.UInt72(BigInteger value)
        => UInt72(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt72(BigInteger value)
        => UInt72(value);
    public AbiEncoder Int80(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, false, 80));
    IFixedTupleEncoder IFixedTupleEncoder.Int80(BigInteger value)
        => Int80(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int80(BigInteger value)
        => Int80(value);

    public AbiEncoder UInt80(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, true, 80));
    IFixedTupleEncoder IFixedTupleEncoder.UInt80(BigInteger value)
        => UInt80(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt80(BigInteger value)
        => UInt80(value);
    public AbiEncoder Int88(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, false, 88));
    IFixedTupleEncoder IFixedTupleEncoder.Int88(BigInteger value)
        => Int88(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int88(BigInteger value)
        => Int88(value);

    public AbiEncoder UInt88(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, true, 88));
    IFixedTupleEncoder IFixedTupleEncoder.UInt88(BigInteger value)
        => UInt88(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt88(BigInteger value)
        => UInt88(value);
    public AbiEncoder Int96(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, false, 96));
    IFixedTupleEncoder IFixedTupleEncoder.Int96(BigInteger value)
        => Int96(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int96(BigInteger value)
        => Int96(value);

    public AbiEncoder UInt96(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, true, 96));
    IFixedTupleEncoder IFixedTupleEncoder.UInt96(BigInteger value)
        => UInt96(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt96(BigInteger value)
        => UInt96(value);
    public AbiEncoder Int104(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, false, 104));
    IFixedTupleEncoder IFixedTupleEncoder.Int104(BigInteger value)
        => Int104(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int104(BigInteger value)
        => Int104(value);

    public AbiEncoder UInt104(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, true, 104));
    IFixedTupleEncoder IFixedTupleEncoder.UInt104(BigInteger value)
        => UInt104(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt104(BigInteger value)
        => UInt104(value);
    public AbiEncoder Int112(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, false, 112));
    IFixedTupleEncoder IFixedTupleEncoder.Int112(BigInteger value)
        => Int112(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int112(BigInteger value)
        => Int112(value);

    public AbiEncoder UInt112(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, true, 112));
    IFixedTupleEncoder IFixedTupleEncoder.UInt112(BigInteger value)
        => UInt112(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt112(BigInteger value)
        => UInt112(value);
    public AbiEncoder Int120(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, false, 120));
    IFixedTupleEncoder IFixedTupleEncoder.Int120(BigInteger value)
        => Int120(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int120(BigInteger value)
        => Int120(value);

    public AbiEncoder UInt120(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, true, 120));
    IFixedTupleEncoder IFixedTupleEncoder.UInt120(BigInteger value)
        => UInt120(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt120(BigInteger value)
        => UInt120(value);
    public AbiEncoder Int128(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, false, 128));
    IFixedTupleEncoder IFixedTupleEncoder.Int128(BigInteger value)
        => Int128(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int128(BigInteger value)
        => Int128(value);

    public AbiEncoder UInt128(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, true, 128));
    IFixedTupleEncoder IFixedTupleEncoder.UInt128(BigInteger value)
        => UInt128(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt128(BigInteger value)
        => UInt128(value);
    public AbiEncoder Int136(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, false, 136));
    IFixedTupleEncoder IFixedTupleEncoder.Int136(BigInteger value)
        => Int136(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int136(BigInteger value)
        => Int136(value);

    public AbiEncoder UInt136(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, true, 136));
    IFixedTupleEncoder IFixedTupleEncoder.UInt136(BigInteger value)
        => UInt136(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt136(BigInteger value)
        => UInt136(value);
    public AbiEncoder Int144(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, false, 144));
    IFixedTupleEncoder IFixedTupleEncoder.Int144(BigInteger value)
        => Int144(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int144(BigInteger value)
        => Int144(value);

    public AbiEncoder UInt144(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, true, 144));
    IFixedTupleEncoder IFixedTupleEncoder.UInt144(BigInteger value)
        => UInt144(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt144(BigInteger value)
        => UInt144(value);
    public AbiEncoder Int152(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, false, 152));
    IFixedTupleEncoder IFixedTupleEncoder.Int152(BigInteger value)
        => Int152(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int152(BigInteger value)
        => Int152(value);

    public AbiEncoder UInt152(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, true, 152));
    IFixedTupleEncoder IFixedTupleEncoder.UInt152(BigInteger value)
        => UInt152(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt152(BigInteger value)
        => UInt152(value);
    public AbiEncoder Int160(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, false, 160));
    IFixedTupleEncoder IFixedTupleEncoder.Int160(BigInteger value)
        => Int160(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int160(BigInteger value)
        => Int160(value);

    public AbiEncoder UInt160(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, true, 160));
    IFixedTupleEncoder IFixedTupleEncoder.UInt160(BigInteger value)
        => UInt160(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt160(BigInteger value)
        => UInt160(value);
    public AbiEncoder Int168(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, false, 168));
    IFixedTupleEncoder IFixedTupleEncoder.Int168(BigInteger value)
        => Int168(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int168(BigInteger value)
        => Int168(value);

    public AbiEncoder UInt168(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, true, 168));
    IFixedTupleEncoder IFixedTupleEncoder.UInt168(BigInteger value)
        => UInt168(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt168(BigInteger value)
        => UInt168(value);
    public AbiEncoder Int176(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, false, 176));
    IFixedTupleEncoder IFixedTupleEncoder.Int176(BigInteger value)
        => Int176(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int176(BigInteger value)
        => Int176(value);

    public AbiEncoder UInt176(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, true, 176));
    IFixedTupleEncoder IFixedTupleEncoder.UInt176(BigInteger value)
        => UInt176(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt176(BigInteger value)
        => UInt176(value);
    public AbiEncoder Int184(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, false, 184));
    IFixedTupleEncoder IFixedTupleEncoder.Int184(BigInteger value)
        => Int184(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int184(BigInteger value)
        => Int184(value);

    public AbiEncoder UInt184(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, true, 184));
    IFixedTupleEncoder IFixedTupleEncoder.UInt184(BigInteger value)
        => UInt184(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt184(BigInteger value)
        => UInt184(value);
    public AbiEncoder Int192(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, false, 192));
    IFixedTupleEncoder IFixedTupleEncoder.Int192(BigInteger value)
        => Int192(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int192(BigInteger value)
        => Int192(value);

    public AbiEncoder UInt192(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, true, 192));
    IFixedTupleEncoder IFixedTupleEncoder.UInt192(BigInteger value)
        => UInt192(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt192(BigInteger value)
        => UInt192(value);
    public AbiEncoder Int200(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, false, 200));
    IFixedTupleEncoder IFixedTupleEncoder.Int200(BigInteger value)
        => Int200(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int200(BigInteger value)
        => Int200(value);

    public AbiEncoder UInt200(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, true, 200));
    IFixedTupleEncoder IFixedTupleEncoder.UInt200(BigInteger value)
        => UInt200(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt200(BigInteger value)
        => UInt200(value);
    public AbiEncoder Int208(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, false, 208));
    IFixedTupleEncoder IFixedTupleEncoder.Int208(BigInteger value)
        => Int208(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int208(BigInteger value)
        => Int208(value);

    public AbiEncoder UInt208(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, true, 208));
    IFixedTupleEncoder IFixedTupleEncoder.UInt208(BigInteger value)
        => UInt208(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt208(BigInteger value)
        => UInt208(value);
    public AbiEncoder Int216(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, false, 216));
    IFixedTupleEncoder IFixedTupleEncoder.Int216(BigInteger value)
        => Int216(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int216(BigInteger value)
        => Int216(value);

    public AbiEncoder UInt216(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, true, 216));
    IFixedTupleEncoder IFixedTupleEncoder.UInt216(BigInteger value)
        => UInt216(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt216(BigInteger value)
        => UInt216(value);
    public AbiEncoder Int224(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, false, 224));
    IFixedTupleEncoder IFixedTupleEncoder.Int224(BigInteger value)
        => Int224(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int224(BigInteger value)
        => Int224(value);

    public AbiEncoder UInt224(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, true, 224));
    IFixedTupleEncoder IFixedTupleEncoder.UInt224(BigInteger value)
        => UInt224(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt224(BigInteger value)
        => UInt224(value);
    public AbiEncoder Int232(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, false, 232));
    IFixedTupleEncoder IFixedTupleEncoder.Int232(BigInteger value)
        => Int232(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int232(BigInteger value)
        => Int232(value);

    public AbiEncoder UInt232(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, true, 232));
    IFixedTupleEncoder IFixedTupleEncoder.UInt232(BigInteger value)
        => UInt232(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt232(BigInteger value)
        => UInt232(value);
    public AbiEncoder Int240(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, false, 240));
    IFixedTupleEncoder IFixedTupleEncoder.Int240(BigInteger value)
        => Int240(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int240(BigInteger value)
        => Int240(value);

    public AbiEncoder UInt240(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, true, 240));
    IFixedTupleEncoder IFixedTupleEncoder.UInt240(BigInteger value)
        => UInt240(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt240(BigInteger value)
        => UInt240(value);
    public AbiEncoder Int248(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, false, 248));
    IFixedTupleEncoder IFixedTupleEncoder.Int248(BigInteger value)
        => Int248(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int248(BigInteger value)
        => Int248(value);

    public AbiEncoder UInt248(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, true, 248));
    IFixedTupleEncoder IFixedTupleEncoder.UInt248(BigInteger value)
        => UInt248(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt248(BigInteger value)
        => UInt248(value);
    public AbiEncoder Int256(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, false, 256));
    IFixedTupleEncoder IFixedTupleEncoder.Int256(BigInteger value)
        => Int256(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int256(BigInteger value)
        => Int256(value);

    public AbiEncoder UInt256(BigInteger value)
        => AddElement(new FixedType<object>.BigInteger(value, true, 256));
    IFixedTupleEncoder IFixedTupleEncoder.UInt256(BigInteger value)
        => UInt256(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt256(BigInteger value)
        => UInt256(value);
}