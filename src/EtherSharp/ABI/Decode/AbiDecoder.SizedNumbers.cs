using EtherSharp.ABI.Fixed;
using EtherSharp.ABI.Decode.Interfaces;
using System.Numerics;

namespace EtherSharp.ABI;
public partial class AbiDecoder
{
    public AbiDecoder Int8(out sbyte value)
    {
        value = FixedType<object>.SByte.Decode(CurrentSlot);
        return ConsumeBytes();
    }
    sbyte IFixedTupleDecoder.Int8() 
    {
        _ = Int8(out sbyte output);
        return output;
    }    
    sbyte IDynamicTupleDecoder.Int8() 
    {
        _ = Int8(out sbyte output);
        return output;
    }
    public AbiDecoder UInt8(out byte value)
    {
        value = FixedType<object>.Byte.Decode(CurrentSlot);
        return ConsumeBytes();
    }
    byte IFixedTupleDecoder.UInt8() 
    {
        _ = UInt8(out byte output);
        return output;
    }
    byte IDynamicTupleDecoder.UInt8() 
    {
        _ = UInt8(out byte output);
        return output;
    }

    public AbiDecoder Int16(out short value)
    {
        value = FixedType<object>.Short.Decode(CurrentSlot);
        return ConsumeBytes();
    }
    short IFixedTupleDecoder.Int16() 
    {
        _ = Int16(out short output);
        return output;
    }
    short IDynamicTupleDecoder.Int16() 
    {
        _ = Int16(out short output);
        return output;
    }
    public AbiDecoder UInt16(out ushort value)
    {
        value = FixedType<object>.UShort.Decode(CurrentSlot);
        return ConsumeBytes();
    }
    ushort IFixedTupleDecoder.UInt16() 
    {
        _ = UInt16(out ushort output);
        return output;
    }
    ushort IDynamicTupleDecoder.UInt16() 
    {
        _ = UInt16(out ushort output);
        return output;
    }
    public AbiDecoder Int24(out int value)
    {
        value = FixedType<object>.Int.Decode(CurrentSlot);
        return ConsumeBytes();
    }
    int IFixedTupleDecoder.Int24()
    {
        _ = Int24(out int output);
        return output;
    }
    int IDynamicTupleDecoder.Int24()
    {
        _ = Int24(out int output);
        return output;
    }
    public AbiDecoder UInt24(out uint value)
    {
        value = FixedType<object>.UInt.Decode(CurrentSlot);
        return ConsumeBytes();
    }
    uint IFixedTupleDecoder.UInt24()
    {
        _ = UInt24(out uint output);
        return output;
    }
    uint IDynamicTupleDecoder.UInt24()
    {
        _ = UInt24(out uint output);
        return output;
    }
    public AbiDecoder Int32(out int value)
    {
        value = FixedType<object>.Int.Decode(CurrentSlot);
        return ConsumeBytes();
    }
    int IFixedTupleDecoder.Int32()
    {
        _ = Int32(out int output);
        return output;
    }
    int IDynamicTupleDecoder.Int32()
    {
        _ = Int32(out int output);
        return output;
    }
    public AbiDecoder UInt32(out uint value)
    {
        value = FixedType<object>.UInt.Decode(CurrentSlot);
        return ConsumeBytes();
    }
    uint IFixedTupleDecoder.UInt32()
    {
        _ = UInt32(out uint output);
        return output;
    }
    uint IDynamicTupleDecoder.UInt32()
    {
        _ = UInt32(out uint output);
        return output;
    }
    public AbiDecoder Int40(out long value)
    {
        value = FixedType<object>.Long.Decode(CurrentSlot);
        return ConsumeBytes();
    }
    long IFixedTupleDecoder.Int40()
    {
        _ = Int40(out long output);
        return output;
    }
    long IDynamicTupleDecoder.Int40()
    {
        _ = Int40(out long output);
        return output;
    }
    public AbiDecoder UInt40(out ulong value)
    {
        value = FixedType<object>.ULong.Decode(CurrentSlot);
        return ConsumeBytes();
    }
    ulong IFixedTupleDecoder.UInt40()
    {
        _ = UInt40(out ulong output);
        return output;
    }
    ulong IDynamicTupleDecoder.UInt40()
    {
        _ = UInt40(out ulong output);
        return output;
    }
    public AbiDecoder Int48(out long value)
    {
        value = FixedType<object>.Long.Decode(CurrentSlot);
        return ConsumeBytes();
    }
    long IFixedTupleDecoder.Int48()
    {
        _ = Int48(out long output);
        return output;
    }
    long IDynamicTupleDecoder.Int48()
    {
        _ = Int48(out long output);
        return output;
    }
    public AbiDecoder UInt48(out ulong value)
    {
        value = FixedType<object>.ULong.Decode(CurrentSlot);
        return ConsumeBytes();
    }
    ulong IFixedTupleDecoder.UInt48()
    {
        _ = UInt48(out ulong output);
        return output;
    }
    ulong IDynamicTupleDecoder.UInt48()
    {
        _ = UInt48(out ulong output);
        return output;
    }
    public AbiDecoder Int56(out long value)
    {
        value = FixedType<object>.Long.Decode(CurrentSlot);
        return ConsumeBytes();
    }
    long IFixedTupleDecoder.Int56()
    {
        _ = Int56(out long output);
        return output;
    }
    long IDynamicTupleDecoder.Int56()
    {
        _ = Int56(out long output);
        return output;
    }
    public AbiDecoder UInt56(out ulong value)
    {
        value = FixedType<object>.ULong.Decode(CurrentSlot);
        return ConsumeBytes();
    }
    ulong IFixedTupleDecoder.UInt56()
    {
        _ = UInt56(out ulong output);
        return output;
    }
    ulong IDynamicTupleDecoder.UInt56()
    {
        _ = UInt56(out ulong output);
        return output;
    }
    public AbiDecoder Int64(out long value)
    {
        value = FixedType<object>.Long.Decode(CurrentSlot);
        return ConsumeBytes();
    }
    long IFixedTupleDecoder.Int64()
    {
        _ = Int64(out long output);
        return output;
    }
    long IDynamicTupleDecoder.Int64()
    {
        _ = Int64(out long output);
        return output;
    }
    public AbiDecoder UInt64(out ulong value)
    {
        value = FixedType<object>.ULong.Decode(CurrentSlot);
        return ConsumeBytes();
    }
    ulong IFixedTupleDecoder.UInt64()
    {
        _ = UInt64(out ulong output);
        return output;
    }
    ulong IDynamicTupleDecoder.UInt64()
    {
        _ = UInt64(out ulong output);
        return output;
    }
    public AbiDecoder Int72(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, false);
        return ConsumeBytes();
    }
    BigInteger IFixedTupleDecoder.Int72()
    {
        _ = Int72(out var output);
        return output;
    }
    BigInteger IDynamicTupleDecoder.Int72()
    {
        _ = Int72(out var output);
        return output;
    }
    public AbiDecoder UInt72(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, true);
        return ConsumeBytes();
    }
    BigInteger IFixedTupleDecoder.UInt72()
    {
        _ = UInt72(out var output);
        return output;
    }
    BigInteger IDynamicTupleDecoder.UInt72()
    {
        _ = UInt72(out var output);
        return output;
    }
    public AbiDecoder Int80(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, false);
        return ConsumeBytes();
    }
    BigInteger IFixedTupleDecoder.Int80()
    {
        _ = Int80(out var output);
        return output;
    }
    BigInteger IDynamicTupleDecoder.Int80()
    {
        _ = Int80(out var output);
        return output;
    }
    public AbiDecoder UInt80(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, true);
        return ConsumeBytes();
    }
    BigInteger IFixedTupleDecoder.UInt80()
    {
        _ = UInt80(out var output);
        return output;
    }
    BigInteger IDynamicTupleDecoder.UInt80()
    {
        _ = UInt80(out var output);
        return output;
    }
    public AbiDecoder Int88(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, false);
        return ConsumeBytes();
    }
    BigInteger IFixedTupleDecoder.Int88()
    {
        _ = Int88(out var output);
        return output;
    }
    BigInteger IDynamicTupleDecoder.Int88()
    {
        _ = Int88(out var output);
        return output;
    }
    public AbiDecoder UInt88(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, true);
        return ConsumeBytes();
    }
    BigInteger IFixedTupleDecoder.UInt88()
    {
        _ = UInt88(out var output);
        return output;
    }
    BigInteger IDynamicTupleDecoder.UInt88()
    {
        _ = UInt88(out var output);
        return output;
    }
    public AbiDecoder Int96(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, false);
        return ConsumeBytes();
    }
    BigInteger IFixedTupleDecoder.Int96()
    {
        _ = Int96(out var output);
        return output;
    }
    BigInteger IDynamicTupleDecoder.Int96()
    {
        _ = Int96(out var output);
        return output;
    }
    public AbiDecoder UInt96(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, true);
        return ConsumeBytes();
    }
    BigInteger IFixedTupleDecoder.UInt96()
    {
        _ = UInt96(out var output);
        return output;
    }
    BigInteger IDynamicTupleDecoder.UInt96()
    {
        _ = UInt96(out var output);
        return output;
    }
    public AbiDecoder Int104(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, false);
        return ConsumeBytes();
    }
    BigInteger IFixedTupleDecoder.Int104()
    {
        _ = Int104(out var output);
        return output;
    }
    BigInteger IDynamicTupleDecoder.Int104()
    {
        _ = Int104(out var output);
        return output;
    }
    public AbiDecoder UInt104(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, true);
        return ConsumeBytes();
    }
    BigInteger IFixedTupleDecoder.UInt104()
    {
        _ = UInt104(out var output);
        return output;
    }
    BigInteger IDynamicTupleDecoder.UInt104()
    {
        _ = UInt104(out var output);
        return output;
    }
    public AbiDecoder Int112(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, false);
        return ConsumeBytes();
    }
    BigInteger IFixedTupleDecoder.Int112()
    {
        _ = Int112(out var output);
        return output;
    }
    BigInteger IDynamicTupleDecoder.Int112()
    {
        _ = Int112(out var output);
        return output;
    }
    public AbiDecoder UInt112(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, true);
        return ConsumeBytes();
    }
    BigInteger IFixedTupleDecoder.UInt112()
    {
        _ = UInt112(out var output);
        return output;
    }
    BigInteger IDynamicTupleDecoder.UInt112()
    {
        _ = UInt112(out var output);
        return output;
    }
    public AbiDecoder Int120(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, false);
        return ConsumeBytes();
    }
    BigInteger IFixedTupleDecoder.Int120()
    {
        _ = Int120(out var output);
        return output;
    }
    BigInteger IDynamicTupleDecoder.Int120()
    {
        _ = Int120(out var output);
        return output;
    }
    public AbiDecoder UInt120(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, true);
        return ConsumeBytes();
    }
    BigInteger IFixedTupleDecoder.UInt120()
    {
        _ = UInt120(out var output);
        return output;
    }
    BigInteger IDynamicTupleDecoder.UInt120()
    {
        _ = UInt120(out var output);
        return output;
    }
    public AbiDecoder Int128(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, false);
        return ConsumeBytes();
    }
    BigInteger IFixedTupleDecoder.Int128()
    {
        _ = Int128(out var output);
        return output;
    }
    BigInteger IDynamicTupleDecoder.Int128()
    {
        _ = Int128(out var output);
        return output;
    }
    public AbiDecoder UInt128(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, true);
        return ConsumeBytes();
    }
    BigInteger IFixedTupleDecoder.UInt128()
    {
        _ = UInt128(out var output);
        return output;
    }
    BigInteger IDynamicTupleDecoder.UInt128()
    {
        _ = UInt128(out var output);
        return output;
    }
    public AbiDecoder Int136(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, false);
        return ConsumeBytes();
    }
    BigInteger IFixedTupleDecoder.Int136()
    {
        _ = Int136(out var output);
        return output;
    }
    BigInteger IDynamicTupleDecoder.Int136()
    {
        _ = Int136(out var output);
        return output;
    }
    public AbiDecoder UInt136(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, true);
        return ConsumeBytes();
    }
    BigInteger IFixedTupleDecoder.UInt136()
    {
        _ = UInt136(out var output);
        return output;
    }
    BigInteger IDynamicTupleDecoder.UInt136()
    {
        _ = UInt136(out var output);
        return output;
    }
    public AbiDecoder Int144(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, false);
        return ConsumeBytes();
    }
    BigInteger IFixedTupleDecoder.Int144()
    {
        _ = Int144(out var output);
        return output;
    }
    BigInteger IDynamicTupleDecoder.Int144()
    {
        _ = Int144(out var output);
        return output;
    }
    public AbiDecoder UInt144(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, true);
        return ConsumeBytes();
    }
    BigInteger IFixedTupleDecoder.UInt144()
    {
        _ = UInt144(out var output);
        return output;
    }
    BigInteger IDynamicTupleDecoder.UInt144()
    {
        _ = UInt144(out var output);
        return output;
    }
    public AbiDecoder Int152(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, false);
        return ConsumeBytes();
    }
    BigInteger IFixedTupleDecoder.Int152()
    {
        _ = Int152(out var output);
        return output;
    }
    BigInteger IDynamicTupleDecoder.Int152()
    {
        _ = Int152(out var output);
        return output;
    }
    public AbiDecoder UInt152(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, true);
        return ConsumeBytes();
    }
    BigInteger IFixedTupleDecoder.UInt152()
    {
        _ = UInt152(out var output);
        return output;
    }
    BigInteger IDynamicTupleDecoder.UInt152()
    {
        _ = UInt152(out var output);
        return output;
    }
    public AbiDecoder Int160(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, false);
        return ConsumeBytes();
    }
    BigInteger IFixedTupleDecoder.Int160()
    {
        _ = Int160(out var output);
        return output;
    }
    BigInteger IDynamicTupleDecoder.Int160()
    {
        _ = Int160(out var output);
        return output;
    }
    public AbiDecoder UInt160(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, true);
        return ConsumeBytes();
    }
    BigInteger IFixedTupleDecoder.UInt160()
    {
        _ = UInt160(out var output);
        return output;
    }
    BigInteger IDynamicTupleDecoder.UInt160()
    {
        _ = UInt160(out var output);
        return output;
    }
    public AbiDecoder Int168(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, false);
        return ConsumeBytes();
    }
    BigInteger IFixedTupleDecoder.Int168()
    {
        _ = Int168(out var output);
        return output;
    }
    BigInteger IDynamicTupleDecoder.Int168()
    {
        _ = Int168(out var output);
        return output;
    }
    public AbiDecoder UInt168(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, true);
        return ConsumeBytes();
    }
    BigInteger IFixedTupleDecoder.UInt168()
    {
        _ = UInt168(out var output);
        return output;
    }
    BigInteger IDynamicTupleDecoder.UInt168()
    {
        _ = UInt168(out var output);
        return output;
    }
    public AbiDecoder Int176(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, false);
        return ConsumeBytes();
    }
    BigInteger IFixedTupleDecoder.Int176()
    {
        _ = Int176(out var output);
        return output;
    }
    BigInteger IDynamicTupleDecoder.Int176()
    {
        _ = Int176(out var output);
        return output;
    }
    public AbiDecoder UInt176(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, true);
        return ConsumeBytes();
    }
    BigInteger IFixedTupleDecoder.UInt176()
    {
        _ = UInt176(out var output);
        return output;
    }
    BigInteger IDynamicTupleDecoder.UInt176()
    {
        _ = UInt176(out var output);
        return output;
    }
    public AbiDecoder Int184(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, false);
        return ConsumeBytes();
    }
    BigInteger IFixedTupleDecoder.Int184()
    {
        _ = Int184(out var output);
        return output;
    }
    BigInteger IDynamicTupleDecoder.Int184()
    {
        _ = Int184(out var output);
        return output;
    }
    public AbiDecoder UInt184(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, true);
        return ConsumeBytes();
    }
    BigInteger IFixedTupleDecoder.UInt184()
    {
        _ = UInt184(out var output);
        return output;
    }
    BigInteger IDynamicTupleDecoder.UInt184()
    {
        _ = UInt184(out var output);
        return output;
    }
    public AbiDecoder Int192(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, false);
        return ConsumeBytes();
    }
    BigInteger IFixedTupleDecoder.Int192()
    {
        _ = Int192(out var output);
        return output;
    }
    BigInteger IDynamicTupleDecoder.Int192()
    {
        _ = Int192(out var output);
        return output;
    }
    public AbiDecoder UInt192(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, true);
        return ConsumeBytes();
    }
    BigInteger IFixedTupleDecoder.UInt192()
    {
        _ = UInt192(out var output);
        return output;
    }
    BigInteger IDynamicTupleDecoder.UInt192()
    {
        _ = UInt192(out var output);
        return output;
    }
    public AbiDecoder Int200(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, false);
        return ConsumeBytes();
    }
    BigInteger IFixedTupleDecoder.Int200()
    {
        _ = Int200(out var output);
        return output;
    }
    BigInteger IDynamicTupleDecoder.Int200()
    {
        _ = Int200(out var output);
        return output;
    }
    public AbiDecoder UInt200(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, true);
        return ConsumeBytes();
    }
    BigInteger IFixedTupleDecoder.UInt200()
    {
        _ = UInt200(out var output);
        return output;
    }
    BigInteger IDynamicTupleDecoder.UInt200()
    {
        _ = UInt200(out var output);
        return output;
    }
    public AbiDecoder Int208(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, false);
        return ConsumeBytes();
    }
    BigInteger IFixedTupleDecoder.Int208()
    {
        _ = Int208(out var output);
        return output;
    }
    BigInteger IDynamicTupleDecoder.Int208()
    {
        _ = Int208(out var output);
        return output;
    }
    public AbiDecoder UInt208(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, true);
        return ConsumeBytes();
    }
    BigInteger IFixedTupleDecoder.UInt208()
    {
        _ = UInt208(out var output);
        return output;
    }
    BigInteger IDynamicTupleDecoder.UInt208()
    {
        _ = UInt208(out var output);
        return output;
    }
    public AbiDecoder Int216(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, false);
        return ConsumeBytes();
    }
    BigInteger IFixedTupleDecoder.Int216()
    {
        _ = Int216(out var output);
        return output;
    }
    BigInteger IDynamicTupleDecoder.Int216()
    {
        _ = Int216(out var output);
        return output;
    }
    public AbiDecoder UInt216(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, true);
        return ConsumeBytes();
    }
    BigInteger IFixedTupleDecoder.UInt216()
    {
        _ = UInt216(out var output);
        return output;
    }
    BigInteger IDynamicTupleDecoder.UInt216()
    {
        _ = UInt216(out var output);
        return output;
    }
    public AbiDecoder Int224(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, false);
        return ConsumeBytes();
    }
    BigInteger IFixedTupleDecoder.Int224()
    {
        _ = Int224(out var output);
        return output;
    }
    BigInteger IDynamicTupleDecoder.Int224()
    {
        _ = Int224(out var output);
        return output;
    }
    public AbiDecoder UInt224(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, true);
        return ConsumeBytes();
    }
    BigInteger IFixedTupleDecoder.UInt224()
    {
        _ = UInt224(out var output);
        return output;
    }
    BigInteger IDynamicTupleDecoder.UInt224()
    {
        _ = UInt224(out var output);
        return output;
    }
    public AbiDecoder Int232(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, false);
        return ConsumeBytes();
    }
    BigInteger IFixedTupleDecoder.Int232()
    {
        _ = Int232(out var output);
        return output;
    }
    BigInteger IDynamicTupleDecoder.Int232()
    {
        _ = Int232(out var output);
        return output;
    }
    public AbiDecoder UInt232(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, true);
        return ConsumeBytes();
    }
    BigInteger IFixedTupleDecoder.UInt232()
    {
        _ = UInt232(out var output);
        return output;
    }
    BigInteger IDynamicTupleDecoder.UInt232()
    {
        _ = UInt232(out var output);
        return output;
    }
    public AbiDecoder Int240(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, false);
        return ConsumeBytes();
    }
    BigInteger IFixedTupleDecoder.Int240()
    {
        _ = Int240(out var output);
        return output;
    }
    BigInteger IDynamicTupleDecoder.Int240()
    {
        _ = Int240(out var output);
        return output;
    }
    public AbiDecoder UInt240(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, true);
        return ConsumeBytes();
    }
    BigInteger IFixedTupleDecoder.UInt240()
    {
        _ = UInt240(out var output);
        return output;
    }
    BigInteger IDynamicTupleDecoder.UInt240()
    {
        _ = UInt240(out var output);
        return output;
    }
    public AbiDecoder Int248(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, false);
        return ConsumeBytes();
    }
    BigInteger IFixedTupleDecoder.Int248()
    {
        _ = Int248(out var output);
        return output;
    }
    BigInteger IDynamicTupleDecoder.Int248()
    {
        _ = Int248(out var output);
        return output;
    }
    public AbiDecoder UInt248(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, true);
        return ConsumeBytes();
    }
    BigInteger IFixedTupleDecoder.UInt248()
    {
        _ = UInt248(out var output);
        return output;
    }
    BigInteger IDynamicTupleDecoder.UInt248()
    {
        _ = UInt248(out var output);
        return output;
    }
    public AbiDecoder Int256(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, false);
        return ConsumeBytes();
    }
    BigInteger IFixedTupleDecoder.Int256()
    {
        _ = Int256(out var output);
        return output;
    }
    BigInteger IDynamicTupleDecoder.Int256()
    {
        _ = Int256(out var output);
        return output;
    }
    public AbiDecoder UInt256(out BigInteger value)
    {
        value = FixedType<object>.BigInteger.Decode(CurrentSlot, true);
        return ConsumeBytes();
    }
    BigInteger IFixedTupleDecoder.UInt256()
    {
        _ = UInt256(out var output);
        return output;
    }
    BigInteger IDynamicTupleDecoder.UInt256()
    {
        _ = UInt256(out var output);
        return output;
    }
}