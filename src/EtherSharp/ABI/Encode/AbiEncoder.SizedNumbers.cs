using EtherSharp.ABI.Types;
using EtherSharp.ABI.Encode.Interfaces;
using EtherSharp.Numerics;

namespace EtherSharp.ABI;
public partial class AbiEncoder
{
    public AbiEncoder UInt8(byte value)
        => AddElement(new AbiTypes.Byte(value));
    IFixedTupleEncoder IFixedTupleEncoder.UInt8(byte value)
        => UInt8(value);    
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt8(byte value)
        => UInt8(value);

    public AbiEncoder Int8(sbyte value)
        => AddElement(new AbiTypes.SByte(value));
    IFixedTupleEncoder IFixedTupleEncoder.Int8(sbyte value)
        => Int8(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int8(sbyte value)
        => Int8(value);

    public AbiEncoder UInt16(ushort value)
        => AddElement(new AbiTypes.UShort(value));
    IFixedTupleEncoder IFixedTupleEncoder.UInt16(ushort value)
        => UInt16(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt16(ushort value)
        => UInt16(value);

    public AbiEncoder Int16(short value)
        => AddElement(new AbiTypes.Short(value));
    IFixedTupleEncoder IFixedTupleEncoder.Int16(short value)
        => Int16(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int16(short value)
        => Int16(value);

    public AbiEncoder Int24(int value)
        => AddElement(new AbiTypes.Int(value, 3));
    IFixedTupleEncoder IFixedTupleEncoder.Int24(int value)
        => Int24(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int24(int value)
        => Int24(value);

    public AbiEncoder UInt24(uint value)
        => AddElement(new AbiTypes.UInt(value, 3));
    IFixedTupleEncoder IFixedTupleEncoder.UInt24(uint value)
        => UInt24(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt24(uint value)
        => UInt24(value);
    public AbiEncoder Int32(int value)
        => AddElement(new AbiTypes.Int(value, 4));
    IFixedTupleEncoder IFixedTupleEncoder.Int32(int value)
        => Int32(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int32(int value)
        => Int32(value);

    public AbiEncoder UInt32(uint value)
        => AddElement(new AbiTypes.UInt(value, 4));
    IFixedTupleEncoder IFixedTupleEncoder.UInt32(uint value)
        => UInt32(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt32(uint value)
        => UInt32(value);
    public AbiEncoder Int40(long value)
        => AddElement(new AbiTypes.Long(value, 5));
    IFixedTupleEncoder IFixedTupleEncoder.Int40(long value)
        => Int40(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int40(long value)
        => Int40(value);

    public AbiEncoder UInt40(ulong value)
        => AddElement(new AbiTypes.ULong(value, 5));
    IFixedTupleEncoder IFixedTupleEncoder.UInt40(ulong value)
        => UInt40(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt40(ulong value)
        => UInt40(value);
    public AbiEncoder Int48(long value)
        => AddElement(new AbiTypes.Long(value, 6));
    IFixedTupleEncoder IFixedTupleEncoder.Int48(long value)
        => Int48(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int48(long value)
        => Int48(value);

    public AbiEncoder UInt48(ulong value)
        => AddElement(new AbiTypes.ULong(value, 6));
    IFixedTupleEncoder IFixedTupleEncoder.UInt48(ulong value)
        => UInt48(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt48(ulong value)
        => UInt48(value);
    public AbiEncoder Int56(long value)
        => AddElement(new AbiTypes.Long(value, 7));
    IFixedTupleEncoder IFixedTupleEncoder.Int56(long value)
        => Int56(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int56(long value)
        => Int56(value);

    public AbiEncoder UInt56(ulong value)
        => AddElement(new AbiTypes.ULong(value, 7));
    IFixedTupleEncoder IFixedTupleEncoder.UInt56(ulong value)
        => UInt56(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt56(ulong value)
        => UInt56(value);
    public AbiEncoder Int64(long value)
        => AddElement(new AbiTypes.Long(value, 8));
    IFixedTupleEncoder IFixedTupleEncoder.Int64(long value)
        => Int64(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int64(long value)
        => Int64(value);

    public AbiEncoder UInt64(ulong value)
        => AddElement(new AbiTypes.ULong(value, 8));
    IFixedTupleEncoder IFixedTupleEncoder.UInt64(ulong value)
        => UInt64(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt64(ulong value)
        => UInt64(value);
    public AbiEncoder Int72(Int256 value)
        => AddElement(new AbiTypes.Int256(value, 9));
    IFixedTupleEncoder IFixedTupleEncoder.Int72(Int256 value)
        => Int72(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int72(Int256 value)
        => Int72(value);

    public AbiEncoder UInt72(UInt256 value)
        => AddElement(new AbiTypes.UInt256(value, 9));
    IFixedTupleEncoder IFixedTupleEncoder.UInt72(UInt256 value)
        => UInt72(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt72(UInt256 value)
        => UInt72(value);
    public AbiEncoder Int80(Int256 value)
        => AddElement(new AbiTypes.Int256(value, 10));
    IFixedTupleEncoder IFixedTupleEncoder.Int80(Int256 value)
        => Int80(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int80(Int256 value)
        => Int80(value);

    public AbiEncoder UInt80(UInt256 value)
        => AddElement(new AbiTypes.UInt256(value, 10));
    IFixedTupleEncoder IFixedTupleEncoder.UInt80(UInt256 value)
        => UInt80(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt80(UInt256 value)
        => UInt80(value);
    public AbiEncoder Int88(Int256 value)
        => AddElement(new AbiTypes.Int256(value, 11));
    IFixedTupleEncoder IFixedTupleEncoder.Int88(Int256 value)
        => Int88(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int88(Int256 value)
        => Int88(value);

    public AbiEncoder UInt88(UInt256 value)
        => AddElement(new AbiTypes.UInt256(value, 11));
    IFixedTupleEncoder IFixedTupleEncoder.UInt88(UInt256 value)
        => UInt88(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt88(UInt256 value)
        => UInt88(value);
    public AbiEncoder Int96(Int256 value)
        => AddElement(new AbiTypes.Int256(value, 12));
    IFixedTupleEncoder IFixedTupleEncoder.Int96(Int256 value)
        => Int96(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int96(Int256 value)
        => Int96(value);

    public AbiEncoder UInt96(UInt256 value)
        => AddElement(new AbiTypes.UInt256(value, 12));
    IFixedTupleEncoder IFixedTupleEncoder.UInt96(UInt256 value)
        => UInt96(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt96(UInt256 value)
        => UInt96(value);
    public AbiEncoder Int104(Int256 value)
        => AddElement(new AbiTypes.Int256(value, 13));
    IFixedTupleEncoder IFixedTupleEncoder.Int104(Int256 value)
        => Int104(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int104(Int256 value)
        => Int104(value);

    public AbiEncoder UInt104(UInt256 value)
        => AddElement(new AbiTypes.UInt256(value, 13));
    IFixedTupleEncoder IFixedTupleEncoder.UInt104(UInt256 value)
        => UInt104(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt104(UInt256 value)
        => UInt104(value);
    public AbiEncoder Int112(Int256 value)
        => AddElement(new AbiTypes.Int256(value, 14));
    IFixedTupleEncoder IFixedTupleEncoder.Int112(Int256 value)
        => Int112(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int112(Int256 value)
        => Int112(value);

    public AbiEncoder UInt112(UInt256 value)
        => AddElement(new AbiTypes.UInt256(value, 14));
    IFixedTupleEncoder IFixedTupleEncoder.UInt112(UInt256 value)
        => UInt112(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt112(UInt256 value)
        => UInt112(value);
    public AbiEncoder Int120(Int256 value)
        => AddElement(new AbiTypes.Int256(value, 15));
    IFixedTupleEncoder IFixedTupleEncoder.Int120(Int256 value)
        => Int120(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int120(Int256 value)
        => Int120(value);

    public AbiEncoder UInt120(UInt256 value)
        => AddElement(new AbiTypes.UInt256(value, 15));
    IFixedTupleEncoder IFixedTupleEncoder.UInt120(UInt256 value)
        => UInt120(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt120(UInt256 value)
        => UInt120(value);
    public AbiEncoder Int128(Int256 value)
        => AddElement(new AbiTypes.Int256(value, 16));
    IFixedTupleEncoder IFixedTupleEncoder.Int128(Int256 value)
        => Int128(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int128(Int256 value)
        => Int128(value);

    public AbiEncoder UInt128(UInt256 value)
        => AddElement(new AbiTypes.UInt256(value, 16));
    IFixedTupleEncoder IFixedTupleEncoder.UInt128(UInt256 value)
        => UInt128(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt128(UInt256 value)
        => UInt128(value);
    public AbiEncoder Int136(Int256 value)
        => AddElement(new AbiTypes.Int256(value, 17));
    IFixedTupleEncoder IFixedTupleEncoder.Int136(Int256 value)
        => Int136(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int136(Int256 value)
        => Int136(value);

    public AbiEncoder UInt136(UInt256 value)
        => AddElement(new AbiTypes.UInt256(value, 17));
    IFixedTupleEncoder IFixedTupleEncoder.UInt136(UInt256 value)
        => UInt136(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt136(UInt256 value)
        => UInt136(value);
    public AbiEncoder Int144(Int256 value)
        => AddElement(new AbiTypes.Int256(value, 18));
    IFixedTupleEncoder IFixedTupleEncoder.Int144(Int256 value)
        => Int144(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int144(Int256 value)
        => Int144(value);

    public AbiEncoder UInt144(UInt256 value)
        => AddElement(new AbiTypes.UInt256(value, 18));
    IFixedTupleEncoder IFixedTupleEncoder.UInt144(UInt256 value)
        => UInt144(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt144(UInt256 value)
        => UInt144(value);
    public AbiEncoder Int152(Int256 value)
        => AddElement(new AbiTypes.Int256(value, 19));
    IFixedTupleEncoder IFixedTupleEncoder.Int152(Int256 value)
        => Int152(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int152(Int256 value)
        => Int152(value);

    public AbiEncoder UInt152(UInt256 value)
        => AddElement(new AbiTypes.UInt256(value, 19));
    IFixedTupleEncoder IFixedTupleEncoder.UInt152(UInt256 value)
        => UInt152(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt152(UInt256 value)
        => UInt152(value);
    public AbiEncoder Int160(Int256 value)
        => AddElement(new AbiTypes.Int256(value, 20));
    IFixedTupleEncoder IFixedTupleEncoder.Int160(Int256 value)
        => Int160(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int160(Int256 value)
        => Int160(value);

    public AbiEncoder UInt160(UInt256 value)
        => AddElement(new AbiTypes.UInt256(value, 20));
    IFixedTupleEncoder IFixedTupleEncoder.UInt160(UInt256 value)
        => UInt160(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt160(UInt256 value)
        => UInt160(value);
    public AbiEncoder Int168(Int256 value)
        => AddElement(new AbiTypes.Int256(value, 21));
    IFixedTupleEncoder IFixedTupleEncoder.Int168(Int256 value)
        => Int168(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int168(Int256 value)
        => Int168(value);

    public AbiEncoder UInt168(UInt256 value)
        => AddElement(new AbiTypes.UInt256(value, 21));
    IFixedTupleEncoder IFixedTupleEncoder.UInt168(UInt256 value)
        => UInt168(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt168(UInt256 value)
        => UInt168(value);
    public AbiEncoder Int176(Int256 value)
        => AddElement(new AbiTypes.Int256(value, 22));
    IFixedTupleEncoder IFixedTupleEncoder.Int176(Int256 value)
        => Int176(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int176(Int256 value)
        => Int176(value);

    public AbiEncoder UInt176(UInt256 value)
        => AddElement(new AbiTypes.UInt256(value, 22));
    IFixedTupleEncoder IFixedTupleEncoder.UInt176(UInt256 value)
        => UInt176(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt176(UInt256 value)
        => UInt176(value);
    public AbiEncoder Int184(Int256 value)
        => AddElement(new AbiTypes.Int256(value, 23));
    IFixedTupleEncoder IFixedTupleEncoder.Int184(Int256 value)
        => Int184(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int184(Int256 value)
        => Int184(value);

    public AbiEncoder UInt184(UInt256 value)
        => AddElement(new AbiTypes.UInt256(value, 23));
    IFixedTupleEncoder IFixedTupleEncoder.UInt184(UInt256 value)
        => UInt184(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt184(UInt256 value)
        => UInt184(value);
    public AbiEncoder Int192(Int256 value)
        => AddElement(new AbiTypes.Int256(value, 24));
    IFixedTupleEncoder IFixedTupleEncoder.Int192(Int256 value)
        => Int192(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int192(Int256 value)
        => Int192(value);

    public AbiEncoder UInt192(UInt256 value)
        => AddElement(new AbiTypes.UInt256(value, 24));
    IFixedTupleEncoder IFixedTupleEncoder.UInt192(UInt256 value)
        => UInt192(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt192(UInt256 value)
        => UInt192(value);
    public AbiEncoder Int200(Int256 value)
        => AddElement(new AbiTypes.Int256(value, 25));
    IFixedTupleEncoder IFixedTupleEncoder.Int200(Int256 value)
        => Int200(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int200(Int256 value)
        => Int200(value);

    public AbiEncoder UInt200(UInt256 value)
        => AddElement(new AbiTypes.UInt256(value, 25));
    IFixedTupleEncoder IFixedTupleEncoder.UInt200(UInt256 value)
        => UInt200(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt200(UInt256 value)
        => UInt200(value);
    public AbiEncoder Int208(Int256 value)
        => AddElement(new AbiTypes.Int256(value, 26));
    IFixedTupleEncoder IFixedTupleEncoder.Int208(Int256 value)
        => Int208(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int208(Int256 value)
        => Int208(value);

    public AbiEncoder UInt208(UInt256 value)
        => AddElement(new AbiTypes.UInt256(value, 26));
    IFixedTupleEncoder IFixedTupleEncoder.UInt208(UInt256 value)
        => UInt208(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt208(UInt256 value)
        => UInt208(value);
    public AbiEncoder Int216(Int256 value)
        => AddElement(new AbiTypes.Int256(value, 27));
    IFixedTupleEncoder IFixedTupleEncoder.Int216(Int256 value)
        => Int216(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int216(Int256 value)
        => Int216(value);

    public AbiEncoder UInt216(UInt256 value)
        => AddElement(new AbiTypes.UInt256(value, 27));
    IFixedTupleEncoder IFixedTupleEncoder.UInt216(UInt256 value)
        => UInt216(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt216(UInt256 value)
        => UInt216(value);
    public AbiEncoder Int224(Int256 value)
        => AddElement(new AbiTypes.Int256(value, 28));
    IFixedTupleEncoder IFixedTupleEncoder.Int224(Int256 value)
        => Int224(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int224(Int256 value)
        => Int224(value);

    public AbiEncoder UInt224(UInt256 value)
        => AddElement(new AbiTypes.UInt256(value, 28));
    IFixedTupleEncoder IFixedTupleEncoder.UInt224(UInt256 value)
        => UInt224(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt224(UInt256 value)
        => UInt224(value);
    public AbiEncoder Int232(Int256 value)
        => AddElement(new AbiTypes.Int256(value, 29));
    IFixedTupleEncoder IFixedTupleEncoder.Int232(Int256 value)
        => Int232(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int232(Int256 value)
        => Int232(value);

    public AbiEncoder UInt232(UInt256 value)
        => AddElement(new AbiTypes.UInt256(value, 29));
    IFixedTupleEncoder IFixedTupleEncoder.UInt232(UInt256 value)
        => UInt232(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt232(UInt256 value)
        => UInt232(value);
    public AbiEncoder Int240(Int256 value)
        => AddElement(new AbiTypes.Int256(value, 30));
    IFixedTupleEncoder IFixedTupleEncoder.Int240(Int256 value)
        => Int240(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int240(Int256 value)
        => Int240(value);

    public AbiEncoder UInt240(UInt256 value)
        => AddElement(new AbiTypes.UInt256(value, 30));
    IFixedTupleEncoder IFixedTupleEncoder.UInt240(UInt256 value)
        => UInt240(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt240(UInt256 value)
        => UInt240(value);
    public AbiEncoder Int248(Int256 value)
        => AddElement(new AbiTypes.Int256(value, 31));
    IFixedTupleEncoder IFixedTupleEncoder.Int248(Int256 value)
        => Int248(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int248(Int256 value)
        => Int248(value);

    public AbiEncoder UInt248(UInt256 value)
        => AddElement(new AbiTypes.UInt256(value, 31));
    IFixedTupleEncoder IFixedTupleEncoder.UInt248(UInt256 value)
        => UInt248(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt248(UInt256 value)
        => UInt248(value);
    public AbiEncoder Int256(Int256 value)
        => AddElement(new AbiTypes.Int256(value, 32));
    IFixedTupleEncoder IFixedTupleEncoder.Int256(Int256 value)
        => Int256(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int256(Int256 value)
        => Int256(value);

    public AbiEncoder UInt256(UInt256 value)
        => AddElement(new AbiTypes.UInt256(value, 32));
    IFixedTupleEncoder IFixedTupleEncoder.UInt256(UInt256 value)
        => UInt256(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt256(UInt256 value)
        => UInt256(value);
}