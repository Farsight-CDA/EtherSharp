using EtherSharp.ABI.Types;
using EtherSharp.ABI.Encode.Interfaces;
using System.Numerics;

namespace EtherSharp.ABI;
public partial class AbiEncoder
{
    public AbiEncoder Int8Array(params sbyte[] value)
        => AddElement(new AbiTypes.PrimitiveNumberArray<sbyte>(value, 8));
    void IArrayAbiEncoder.Int8Array(params sbyte[] value)
        => Int8Array(value); 
    IDynamicTupleEncoder IDynamicTupleEncoder.Int8Array(params sbyte[] value)
        => Int8Array(value);

    public AbiEncoder UInt8Array(params byte[] value)
        => AddElement(new AbiTypes.PrimitiveNumberArray<byte>(value, 8));
    void IArrayAbiEncoder.UInt8Array(params byte[] value)
        => UInt8Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt8Array(params byte[] value)
        => UInt8Array(value);

    public AbiEncoder Int16Array(params short[] value)
        => AddElement(new AbiTypes.PrimitiveNumberArray<short>(value, 16));
    void IArrayAbiEncoder.Int16Array(params short[] value)
        => Int16Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int16Array(params short[] value)
        => Int16Array(value);

    public AbiEncoder UInt16Array(params ushort[] value)
        => AddElement(new AbiTypes.PrimitiveNumberArray<ushort>(value, 16));
    void IArrayAbiEncoder.UInt16Array(params ushort[] value)
        => UInt16Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt16Array(params ushort[] value)
        => UInt16Array(value);

    public AbiEncoder Int24Array(params int[] value)
        => AddElement(new AbiTypes.PrimitiveNumberArray<int>(value, 24));
    void IArrayAbiEncoder.Int24Array(params int[] value)
        => Int24Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int24Array(params int[] value)
        => Int24Array(value);

    public AbiEncoder UInt24Array(params uint[] value)
        => AddElement(new AbiTypes.PrimitiveNumberArray<uint>(value, 24));
    void IArrayAbiEncoder.UInt24Array(params uint[] value)
        => UInt24Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt24Array(params uint[] value)
        => UInt24Array(value);
    public AbiEncoder Int32Array(params int[] value)
        => AddElement(new AbiTypes.PrimitiveNumberArray<int>(value, 32));
    void IArrayAbiEncoder.Int32Array(params int[] value)
        => Int32Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int32Array(params int[] value)
        => Int32Array(value);

    public AbiEncoder UInt32Array(params uint[] value)
        => AddElement(new AbiTypes.PrimitiveNumberArray<uint>(value, 32));
    void IArrayAbiEncoder.UInt32Array(params uint[] value)
        => UInt32Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt32Array(params uint[] value)
        => UInt32Array(value);
    public AbiEncoder Int40Array(params long[] value)
        => AddElement(new AbiTypes.PrimitiveNumberArray<long>(value, 40));
    void IArrayAbiEncoder.Int40Array(params long[] value)
        => Int40Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int40Array(params long[] value)
        => Int40Array(value);

    public AbiEncoder UInt40Array(params ulong[] value)
        => AddElement(new AbiTypes.PrimitiveNumberArray<ulong>(value, 40));
    void IArrayAbiEncoder.UInt40Array(params ulong[] value)
        => UInt40Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt40Array(params ulong[] value)
        => UInt40Array(value);
    public AbiEncoder Int48Array(params long[] value)
        => AddElement(new AbiTypes.PrimitiveNumberArray<long>(value, 48));
    void IArrayAbiEncoder.Int48Array(params long[] value)
        => Int48Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int48Array(params long[] value)
        => Int48Array(value);

    public AbiEncoder UInt48Array(params ulong[] value)
        => AddElement(new AbiTypes.PrimitiveNumberArray<ulong>(value, 48));
    void IArrayAbiEncoder.UInt48Array(params ulong[] value)
        => UInt48Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt48Array(params ulong[] value)
        => UInt48Array(value);
    public AbiEncoder Int56Array(params long[] value)
        => AddElement(new AbiTypes.PrimitiveNumberArray<long>(value, 56));
    void IArrayAbiEncoder.Int56Array(params long[] value)
        => Int56Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int56Array(params long[] value)
        => Int56Array(value);

    public AbiEncoder UInt56Array(params ulong[] value)
        => AddElement(new AbiTypes.PrimitiveNumberArray<ulong>(value, 56));
    void IArrayAbiEncoder.UInt56Array(params ulong[] value)
        => UInt56Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt56Array(params ulong[] value)
        => UInt56Array(value);
    public AbiEncoder Int64Array(params long[] value)
        => AddElement(new AbiTypes.PrimitiveNumberArray<long>(value, 64));
    void IArrayAbiEncoder.Int64Array(params long[] value)
        => Int64Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int64Array(params long[] value)
        => Int64Array(value);

    public AbiEncoder UInt64Array(params ulong[] value)
        => AddElement(new AbiTypes.PrimitiveNumberArray<ulong>(value, 64));
    void IArrayAbiEncoder.UInt64Array(params ulong[] value)
        => UInt64Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt64Array(params ulong[] value)
        => UInt64Array(value);
    public AbiEncoder Int72Array(params BigInteger[] value)
        => AddElement(new AbiTypes.BigIntegerArray(value, false, 72));
    void IArrayAbiEncoder.Int72Array(params BigInteger[] value)
        => Int72Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int72Array(params BigInteger[] value)
        => Int72Array(value);

    public AbiEncoder UInt72Array(params BigInteger[] value)
        => AddElement(new AbiTypes.BigIntegerArray(value, true, 72));
    void IArrayAbiEncoder.UInt72Array(params BigInteger[] value)
        => UInt72Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt72Array(params BigInteger[] value)
        => UInt72Array(value);
    public AbiEncoder Int80Array(params BigInteger[] value)
        => AddElement(new AbiTypes.BigIntegerArray(value, false, 80));
    void IArrayAbiEncoder.Int80Array(params BigInteger[] value)
        => Int80Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int80Array(params BigInteger[] value)
        => Int80Array(value);

    public AbiEncoder UInt80Array(params BigInteger[] value)
        => AddElement(new AbiTypes.BigIntegerArray(value, true, 80));
    void IArrayAbiEncoder.UInt80Array(params BigInteger[] value)
        => UInt80Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt80Array(params BigInteger[] value)
        => UInt80Array(value);
    public AbiEncoder Int88Array(params BigInteger[] value)
        => AddElement(new AbiTypes.BigIntegerArray(value, false, 88));
    void IArrayAbiEncoder.Int88Array(params BigInteger[] value)
        => Int88Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int88Array(params BigInteger[] value)
        => Int88Array(value);

    public AbiEncoder UInt88Array(params BigInteger[] value)
        => AddElement(new AbiTypes.BigIntegerArray(value, true, 88));
    void IArrayAbiEncoder.UInt88Array(params BigInteger[] value)
        => UInt88Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt88Array(params BigInteger[] value)
        => UInt88Array(value);
    public AbiEncoder Int96Array(params BigInteger[] value)
        => AddElement(new AbiTypes.BigIntegerArray(value, false, 96));
    void IArrayAbiEncoder.Int96Array(params BigInteger[] value)
        => Int96Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int96Array(params BigInteger[] value)
        => Int96Array(value);

    public AbiEncoder UInt96Array(params BigInteger[] value)
        => AddElement(new AbiTypes.BigIntegerArray(value, true, 96));
    void IArrayAbiEncoder.UInt96Array(params BigInteger[] value)
        => UInt96Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt96Array(params BigInteger[] value)
        => UInt96Array(value);
    public AbiEncoder Int104Array(params BigInteger[] value)
        => AddElement(new AbiTypes.BigIntegerArray(value, false, 104));
    void IArrayAbiEncoder.Int104Array(params BigInteger[] value)
        => Int104Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int104Array(params BigInteger[] value)
        => Int104Array(value);

    public AbiEncoder UInt104Array(params BigInteger[] value)
        => AddElement(new AbiTypes.BigIntegerArray(value, true, 104));
    void IArrayAbiEncoder.UInt104Array(params BigInteger[] value)
        => UInt104Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt104Array(params BigInteger[] value)
        => UInt104Array(value);
    public AbiEncoder Int112Array(params BigInteger[] value)
        => AddElement(new AbiTypes.BigIntegerArray(value, false, 112));
    void IArrayAbiEncoder.Int112Array(params BigInteger[] value)
        => Int112Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int112Array(params BigInteger[] value)
        => Int112Array(value);

    public AbiEncoder UInt112Array(params BigInteger[] value)
        => AddElement(new AbiTypes.BigIntegerArray(value, true, 112));
    void IArrayAbiEncoder.UInt112Array(params BigInteger[] value)
        => UInt112Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt112Array(params BigInteger[] value)
        => UInt112Array(value);
    public AbiEncoder Int120Array(params BigInteger[] value)
        => AddElement(new AbiTypes.BigIntegerArray(value, false, 120));
    void IArrayAbiEncoder.Int120Array(params BigInteger[] value)
        => Int120Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int120Array(params BigInteger[] value)
        => Int120Array(value);

    public AbiEncoder UInt120Array(params BigInteger[] value)
        => AddElement(new AbiTypes.BigIntegerArray(value, true, 120));
    void IArrayAbiEncoder.UInt120Array(params BigInteger[] value)
        => UInt120Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt120Array(params BigInteger[] value)
        => UInt120Array(value);
    public AbiEncoder Int128Array(params BigInteger[] value)
        => AddElement(new AbiTypes.BigIntegerArray(value, false, 128));
    void IArrayAbiEncoder.Int128Array(params BigInteger[] value)
        => Int128Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int128Array(params BigInteger[] value)
        => Int128Array(value);

    public AbiEncoder UInt128Array(params BigInteger[] value)
        => AddElement(new AbiTypes.BigIntegerArray(value, true, 128));
    void IArrayAbiEncoder.UInt128Array(params BigInteger[] value)
        => UInt128Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt128Array(params BigInteger[] value)
        => UInt128Array(value);
    public AbiEncoder Int136Array(params BigInteger[] value)
        => AddElement(new AbiTypes.BigIntegerArray(value, false, 136));
    void IArrayAbiEncoder.Int136Array(params BigInteger[] value)
        => Int136Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int136Array(params BigInteger[] value)
        => Int136Array(value);

    public AbiEncoder UInt136Array(params BigInteger[] value)
        => AddElement(new AbiTypes.BigIntegerArray(value, true, 136));
    void IArrayAbiEncoder.UInt136Array(params BigInteger[] value)
        => UInt136Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt136Array(params BigInteger[] value)
        => UInt136Array(value);
    public AbiEncoder Int144Array(params BigInteger[] value)
        => AddElement(new AbiTypes.BigIntegerArray(value, false, 144));
    void IArrayAbiEncoder.Int144Array(params BigInteger[] value)
        => Int144Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int144Array(params BigInteger[] value)
        => Int144Array(value);

    public AbiEncoder UInt144Array(params BigInteger[] value)
        => AddElement(new AbiTypes.BigIntegerArray(value, true, 144));
    void IArrayAbiEncoder.UInt144Array(params BigInteger[] value)
        => UInt144Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt144Array(params BigInteger[] value)
        => UInt144Array(value);
    public AbiEncoder Int152Array(params BigInteger[] value)
        => AddElement(new AbiTypes.BigIntegerArray(value, false, 152));
    void IArrayAbiEncoder.Int152Array(params BigInteger[] value)
        => Int152Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int152Array(params BigInteger[] value)
        => Int152Array(value);

    public AbiEncoder UInt152Array(params BigInteger[] value)
        => AddElement(new AbiTypes.BigIntegerArray(value, true, 152));
    void IArrayAbiEncoder.UInt152Array(params BigInteger[] value)
        => UInt152Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt152Array(params BigInteger[] value)
        => UInt152Array(value);
    public AbiEncoder Int160Array(params BigInteger[] value)
        => AddElement(new AbiTypes.BigIntegerArray(value, false, 160));
    void IArrayAbiEncoder.Int160Array(params BigInteger[] value)
        => Int160Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int160Array(params BigInteger[] value)
        => Int160Array(value);

    public AbiEncoder UInt160Array(params BigInteger[] value)
        => AddElement(new AbiTypes.BigIntegerArray(value, true, 160));
    void IArrayAbiEncoder.UInt160Array(params BigInteger[] value)
        => UInt160Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt160Array(params BigInteger[] value)
        => UInt160Array(value);
    public AbiEncoder Int168Array(params BigInteger[] value)
        => AddElement(new AbiTypes.BigIntegerArray(value, false, 168));
    void IArrayAbiEncoder.Int168Array(params BigInteger[] value)
        => Int168Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int168Array(params BigInteger[] value)
        => Int168Array(value);

    public AbiEncoder UInt168Array(params BigInteger[] value)
        => AddElement(new AbiTypes.BigIntegerArray(value, true, 168));
    void IArrayAbiEncoder.UInt168Array(params BigInteger[] value)
        => UInt168Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt168Array(params BigInteger[] value)
        => UInt168Array(value);
    public AbiEncoder Int176Array(params BigInteger[] value)
        => AddElement(new AbiTypes.BigIntegerArray(value, false, 176));
    void IArrayAbiEncoder.Int176Array(params BigInteger[] value)
        => Int176Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int176Array(params BigInteger[] value)
        => Int176Array(value);

    public AbiEncoder UInt176Array(params BigInteger[] value)
        => AddElement(new AbiTypes.BigIntegerArray(value, true, 176));
    void IArrayAbiEncoder.UInt176Array(params BigInteger[] value)
        => UInt176Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt176Array(params BigInteger[] value)
        => UInt176Array(value);
    public AbiEncoder Int184Array(params BigInteger[] value)
        => AddElement(new AbiTypes.BigIntegerArray(value, false, 184));
    void IArrayAbiEncoder.Int184Array(params BigInteger[] value)
        => Int184Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int184Array(params BigInteger[] value)
        => Int184Array(value);

    public AbiEncoder UInt184Array(params BigInteger[] value)
        => AddElement(new AbiTypes.BigIntegerArray(value, true, 184));
    void IArrayAbiEncoder.UInt184Array(params BigInteger[] value)
        => UInt184Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt184Array(params BigInteger[] value)
        => UInt184Array(value);
    public AbiEncoder Int192Array(params BigInteger[] value)
        => AddElement(new AbiTypes.BigIntegerArray(value, false, 192));
    void IArrayAbiEncoder.Int192Array(params BigInteger[] value)
        => Int192Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int192Array(params BigInteger[] value)
        => Int192Array(value);

    public AbiEncoder UInt192Array(params BigInteger[] value)
        => AddElement(new AbiTypes.BigIntegerArray(value, true, 192));
    void IArrayAbiEncoder.UInt192Array(params BigInteger[] value)
        => UInt192Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt192Array(params BigInteger[] value)
        => UInt192Array(value);
    public AbiEncoder Int200Array(params BigInteger[] value)
        => AddElement(new AbiTypes.BigIntegerArray(value, false, 200));
    void IArrayAbiEncoder.Int200Array(params BigInteger[] value)
        => Int200Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int200Array(params BigInteger[] value)
        => Int200Array(value);

    public AbiEncoder UInt200Array(params BigInteger[] value)
        => AddElement(new AbiTypes.BigIntegerArray(value, true, 200));
    void IArrayAbiEncoder.UInt200Array(params BigInteger[] value)
        => UInt200Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt200Array(params BigInteger[] value)
        => UInt200Array(value);
    public AbiEncoder Int208Array(params BigInteger[] value)
        => AddElement(new AbiTypes.BigIntegerArray(value, false, 208));
    void IArrayAbiEncoder.Int208Array(params BigInteger[] value)
        => Int208Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int208Array(params BigInteger[] value)
        => Int208Array(value);

    public AbiEncoder UInt208Array(params BigInteger[] value)
        => AddElement(new AbiTypes.BigIntegerArray(value, true, 208));
    void IArrayAbiEncoder.UInt208Array(params BigInteger[] value)
        => UInt208Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt208Array(params BigInteger[] value)
        => UInt208Array(value);
    public AbiEncoder Int216Array(params BigInteger[] value)
        => AddElement(new AbiTypes.BigIntegerArray(value, false, 216));
    void IArrayAbiEncoder.Int216Array(params BigInteger[] value)
        => Int216Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int216Array(params BigInteger[] value)
        => Int216Array(value);

    public AbiEncoder UInt216Array(params BigInteger[] value)
        => AddElement(new AbiTypes.BigIntegerArray(value, true, 216));
    void IArrayAbiEncoder.UInt216Array(params BigInteger[] value)
        => UInt216Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt216Array(params BigInteger[] value)
        => UInt216Array(value);
    public AbiEncoder Int224Array(params BigInteger[] value)
        => AddElement(new AbiTypes.BigIntegerArray(value, false, 224));
    void IArrayAbiEncoder.Int224Array(params BigInteger[] value)
        => Int224Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int224Array(params BigInteger[] value)
        => Int224Array(value);

    public AbiEncoder UInt224Array(params BigInteger[] value)
        => AddElement(new AbiTypes.BigIntegerArray(value, true, 224));
    void IArrayAbiEncoder.UInt224Array(params BigInteger[] value)
        => UInt224Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt224Array(params BigInteger[] value)
        => UInt224Array(value);
    public AbiEncoder Int232Array(params BigInteger[] value)
        => AddElement(new AbiTypes.BigIntegerArray(value, false, 232));
    void IArrayAbiEncoder.Int232Array(params BigInteger[] value)
        => Int232Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int232Array(params BigInteger[] value)
        => Int232Array(value);

    public AbiEncoder UInt232Array(params BigInteger[] value)
        => AddElement(new AbiTypes.BigIntegerArray(value, true, 232));
    void IArrayAbiEncoder.UInt232Array(params BigInteger[] value)
        => UInt232Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt232Array(params BigInteger[] value)
        => UInt232Array(value);
    public AbiEncoder Int240Array(params BigInteger[] value)
        => AddElement(new AbiTypes.BigIntegerArray(value, false, 240));
    void IArrayAbiEncoder.Int240Array(params BigInteger[] value)
        => Int240Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int240Array(params BigInteger[] value)
        => Int240Array(value);

    public AbiEncoder UInt240Array(params BigInteger[] value)
        => AddElement(new AbiTypes.BigIntegerArray(value, true, 240));
    void IArrayAbiEncoder.UInt240Array(params BigInteger[] value)
        => UInt240Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt240Array(params BigInteger[] value)
        => UInt240Array(value);
    public AbiEncoder Int248Array(params BigInteger[] value)
        => AddElement(new AbiTypes.BigIntegerArray(value, false, 248));
    void IArrayAbiEncoder.Int248Array(params BigInteger[] value)
        => Int248Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int248Array(params BigInteger[] value)
        => Int248Array(value);

    public AbiEncoder UInt248Array(params BigInteger[] value)
        => AddElement(new AbiTypes.BigIntegerArray(value, true, 248));
    void IArrayAbiEncoder.UInt248Array(params BigInteger[] value)
        => UInt248Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt248Array(params BigInteger[] value)
        => UInt248Array(value);
    public AbiEncoder Int256Array(params BigInteger[] value)
        => AddElement(new AbiTypes.BigIntegerArray(value, false, 256));
    void IArrayAbiEncoder.Int256Array(params BigInteger[] value)
        => Int256Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int256Array(params BigInteger[] value)
        => Int256Array(value);

    public AbiEncoder UInt256Array(params BigInteger[] value)
        => AddElement(new AbiTypes.BigIntegerArray(value, true, 256));
    void IArrayAbiEncoder.UInt256Array(params BigInteger[] value)
        => UInt256Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt256Array(params BigInteger[] value)
        => UInt256Array(value);
}