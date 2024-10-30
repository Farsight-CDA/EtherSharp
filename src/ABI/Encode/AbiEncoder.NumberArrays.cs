using EtherSharp.ABI.Dynamic;
using EtherSharp.ABI.Encode.Interfaces;
using System.Numerics;

namespace EtherSharp.ABI;
public partial class AbiEncoder
{
    public AbiEncoder Int24Array(params int[] value)
        => AddElement(new DynamicType<object>.PrimitiveNumberArray<int>(value, 24));
    IArrayAbiEncoder IArrayAbiEncoder.Int24Array(params int[] value)
        => Int24Array(value);
    IStructAbiEncoder IStructAbiEncoder.Int24Array(params int[] value)
        => Int24Array(value);

    public AbiEncoder UInt24Array(params uint[] value)
        => AddElement(new DynamicType<object>.PrimitiveNumberArray<uint>(value, 24));
    IArrayAbiEncoder IArrayAbiEncoder.UInt24Array(params uint[] value)
        => UInt24Array(value);
    IStructAbiEncoder IStructAbiEncoder.UInt24Array(params uint[] value)
        => UInt24Array(value);
    public AbiEncoder Int32Array(params int[] value)
        => AddElement(new DynamicType<object>.PrimitiveNumberArray<int>(value, 32));
    IArrayAbiEncoder IArrayAbiEncoder.Int32Array(params int[] value)
        => Int32Array(value);
    IStructAbiEncoder IStructAbiEncoder.Int32Array(params int[] value)
        => Int32Array(value);

    public AbiEncoder UInt32Array(params uint[] value)
        => AddElement(new DynamicType<object>.PrimitiveNumberArray<uint>(value, 32));
    IArrayAbiEncoder IArrayAbiEncoder.UInt32Array(params uint[] value)
        => UInt32Array(value);
    IStructAbiEncoder IStructAbiEncoder.UInt32Array(params uint[] value)
        => UInt32Array(value);
    public AbiEncoder Int40Array(params long[] value)
        => AddElement(new DynamicType<object>.PrimitiveNumberArray<long>(value, 40));
    IArrayAbiEncoder IArrayAbiEncoder.Int40Array(params long[] value)
        => Int40Array(value);
    IStructAbiEncoder IStructAbiEncoder.Int40Array(params long[] value)
        => Int40Array(value);

    public AbiEncoder UInt40Array(params ulong[] value)
        => AddElement(new DynamicType<object>.PrimitiveNumberArray<ulong>(value, 40));
    IArrayAbiEncoder IArrayAbiEncoder.UInt40Array(params ulong[] value)
        => UInt40Array(value);
    IStructAbiEncoder IStructAbiEncoder.UInt40Array(params ulong[] value)
        => UInt40Array(value);
    public AbiEncoder Int48Array(params long[] value)
        => AddElement(new DynamicType<object>.PrimitiveNumberArray<long>(value, 48));
    IArrayAbiEncoder IArrayAbiEncoder.Int48Array(params long[] value)
        => Int48Array(value);
    IStructAbiEncoder IStructAbiEncoder.Int48Array(params long[] value)
        => Int48Array(value);

    public AbiEncoder UInt48Array(params ulong[] value)
        => AddElement(new DynamicType<object>.PrimitiveNumberArray<ulong>(value, 48));
    IArrayAbiEncoder IArrayAbiEncoder.UInt48Array(params ulong[] value)
        => UInt48Array(value);
    IStructAbiEncoder IStructAbiEncoder.UInt48Array(params ulong[] value)
        => UInt48Array(value);
    public AbiEncoder Int56Array(params long[] value)
        => AddElement(new DynamicType<object>.PrimitiveNumberArray<long>(value, 56));
    IArrayAbiEncoder IArrayAbiEncoder.Int56Array(params long[] value)
        => Int56Array(value);
    IStructAbiEncoder IStructAbiEncoder.Int56Array(params long[] value)
        => Int56Array(value);

    public AbiEncoder UInt56Array(params ulong[] value)
        => AddElement(new DynamicType<object>.PrimitiveNumberArray<ulong>(value, 56));
    IArrayAbiEncoder IArrayAbiEncoder.UInt56Array(params ulong[] value)
        => UInt56Array(value);
    IStructAbiEncoder IStructAbiEncoder.UInt56Array(params ulong[] value)
        => UInt56Array(value);
    public AbiEncoder Int64Array(params long[] value)
        => AddElement(new DynamicType<object>.PrimitiveNumberArray<long>(value, 64));
    IArrayAbiEncoder IArrayAbiEncoder.Int64Array(params long[] value)
        => Int64Array(value);
    IStructAbiEncoder IStructAbiEncoder.Int64Array(params long[] value)
        => Int64Array(value);

    public AbiEncoder UInt64Array(params ulong[] value)
        => AddElement(new DynamicType<object>.PrimitiveNumberArray<ulong>(value, 64));
    IArrayAbiEncoder IArrayAbiEncoder.UInt64Array(params ulong[] value)
        => UInt64Array(value);
    IStructAbiEncoder IStructAbiEncoder.UInt64Array(params ulong[] value)
        => UInt64Array(value);
    public AbiEncoder Int72Array(params BigInteger[] value)
        => AddElement(new DynamicType<object>.BigIntegerArray(value, false, 72));
    IArrayAbiEncoder IArrayAbiEncoder.Int72Array(params BigInteger[] value)
        => Int72Array(value);
    IStructAbiEncoder IStructAbiEncoder.Int72Array(params BigInteger[] value)
        => Int72Array(value);

    public AbiEncoder UInt72Array(params BigInteger[] value)
        => AddElement(new DynamicType<object>.BigIntegerArray(value, true, 72));
    IArrayAbiEncoder IArrayAbiEncoder.UInt72Array(params BigInteger[] value)
        => UInt72Array(value);
    IStructAbiEncoder IStructAbiEncoder.UInt72Array(params BigInteger[] value)
        => UInt72Array(value);
    public AbiEncoder Int80Array(params BigInteger[] value)
        => AddElement(new DynamicType<object>.BigIntegerArray(value, false, 80));
    IArrayAbiEncoder IArrayAbiEncoder.Int80Array(params BigInteger[] value)
        => Int80Array(value);
    IStructAbiEncoder IStructAbiEncoder.Int80Array(params BigInteger[] value)
        => Int80Array(value);

    public AbiEncoder UInt80Array(params BigInteger[] value)
        => AddElement(new DynamicType<object>.BigIntegerArray(value, true, 80));
    IArrayAbiEncoder IArrayAbiEncoder.UInt80Array(params BigInteger[] value)
        => UInt80Array(value);
    IStructAbiEncoder IStructAbiEncoder.UInt80Array(params BigInteger[] value)
        => UInt80Array(value);
    public AbiEncoder Int88Array(params BigInteger[] value)
        => AddElement(new DynamicType<object>.BigIntegerArray(value, false, 88));
    IArrayAbiEncoder IArrayAbiEncoder.Int88Array(params BigInteger[] value)
        => Int88Array(value);
    IStructAbiEncoder IStructAbiEncoder.Int88Array(params BigInteger[] value)
        => Int88Array(value);

    public AbiEncoder UInt88Array(params BigInteger[] value)
        => AddElement(new DynamicType<object>.BigIntegerArray(value, true, 88));
    IArrayAbiEncoder IArrayAbiEncoder.UInt88Array(params BigInteger[] value)
        => UInt88Array(value);
    IStructAbiEncoder IStructAbiEncoder.UInt88Array(params BigInteger[] value)
        => UInt88Array(value);
    public AbiEncoder Int96Array(params BigInteger[] value)
        => AddElement(new DynamicType<object>.BigIntegerArray(value, false, 96));
    IArrayAbiEncoder IArrayAbiEncoder.Int96Array(params BigInteger[] value)
        => Int96Array(value);
    IStructAbiEncoder IStructAbiEncoder.Int96Array(params BigInteger[] value)
        => Int96Array(value);

    public AbiEncoder UInt96Array(params BigInteger[] value)
        => AddElement(new DynamicType<object>.BigIntegerArray(value, true, 96));
    IArrayAbiEncoder IArrayAbiEncoder.UInt96Array(params BigInteger[] value)
        => UInt96Array(value);
    IStructAbiEncoder IStructAbiEncoder.UInt96Array(params BigInteger[] value)
        => UInt96Array(value);
    public AbiEncoder Int104Array(params BigInteger[] value)
        => AddElement(new DynamicType<object>.BigIntegerArray(value, false, 104));
    IArrayAbiEncoder IArrayAbiEncoder.Int104Array(params BigInteger[] value)
        => Int104Array(value);
    IStructAbiEncoder IStructAbiEncoder.Int104Array(params BigInteger[] value)
        => Int104Array(value);

    public AbiEncoder UInt104Array(params BigInteger[] value)
        => AddElement(new DynamicType<object>.BigIntegerArray(value, true, 104));
    IArrayAbiEncoder IArrayAbiEncoder.UInt104Array(params BigInteger[] value)
        => UInt104Array(value);
    IStructAbiEncoder IStructAbiEncoder.UInt104Array(params BigInteger[] value)
        => UInt104Array(value);
    public AbiEncoder Int112Array(params BigInteger[] value)
        => AddElement(new DynamicType<object>.BigIntegerArray(value, false, 112));
    IArrayAbiEncoder IArrayAbiEncoder.Int112Array(params BigInteger[] value)
        => Int112Array(value);
    IStructAbiEncoder IStructAbiEncoder.Int112Array(params BigInteger[] value)
        => Int112Array(value);

    public AbiEncoder UInt112Array(params BigInteger[] value)
        => AddElement(new DynamicType<object>.BigIntegerArray(value, true, 112));
    IArrayAbiEncoder IArrayAbiEncoder.UInt112Array(params BigInteger[] value)
        => UInt112Array(value);
    IStructAbiEncoder IStructAbiEncoder.UInt112Array(params BigInteger[] value)
        => UInt112Array(value);
    public AbiEncoder Int120Array(params BigInteger[] value)
        => AddElement(new DynamicType<object>.BigIntegerArray(value, false, 120));
    IArrayAbiEncoder IArrayAbiEncoder.Int120Array(params BigInteger[] value)
        => Int120Array(value);
    IStructAbiEncoder IStructAbiEncoder.Int120Array(params BigInteger[] value)
        => Int120Array(value);

    public AbiEncoder UInt120Array(params BigInteger[] value)
        => AddElement(new DynamicType<object>.BigIntegerArray(value, true, 120));
    IArrayAbiEncoder IArrayAbiEncoder.UInt120Array(params BigInteger[] value)
        => UInt120Array(value);
    IStructAbiEncoder IStructAbiEncoder.UInt120Array(params BigInteger[] value)
        => UInt120Array(value);
    public AbiEncoder Int128Array(params BigInteger[] value)
        => AddElement(new DynamicType<object>.BigIntegerArray(value, false, 128));
    IArrayAbiEncoder IArrayAbiEncoder.Int128Array(params BigInteger[] value)
        => Int128Array(value);
    IStructAbiEncoder IStructAbiEncoder.Int128Array(params BigInteger[] value)
        => Int128Array(value);

    public AbiEncoder UInt128Array(params BigInteger[] value)
        => AddElement(new DynamicType<object>.BigIntegerArray(value, true, 128));
    IArrayAbiEncoder IArrayAbiEncoder.UInt128Array(params BigInteger[] value)
        => UInt128Array(value);
    IStructAbiEncoder IStructAbiEncoder.UInt128Array(params BigInteger[] value)
        => UInt128Array(value);
    public AbiEncoder Int136Array(params BigInteger[] value)
        => AddElement(new DynamicType<object>.BigIntegerArray(value, false, 136));
    IArrayAbiEncoder IArrayAbiEncoder.Int136Array(params BigInteger[] value)
        => Int136Array(value);
    IStructAbiEncoder IStructAbiEncoder.Int136Array(params BigInteger[] value)
        => Int136Array(value);

    public AbiEncoder UInt136Array(params BigInteger[] value)
        => AddElement(new DynamicType<object>.BigIntegerArray(value, true, 136));
    IArrayAbiEncoder IArrayAbiEncoder.UInt136Array(params BigInteger[] value)
        => UInt136Array(value);
    IStructAbiEncoder IStructAbiEncoder.UInt136Array(params BigInteger[] value)
        => UInt136Array(value);
    public AbiEncoder Int144Array(params BigInteger[] value)
        => AddElement(new DynamicType<object>.BigIntegerArray(value, false, 144));
    IArrayAbiEncoder IArrayAbiEncoder.Int144Array(params BigInteger[] value)
        => Int144Array(value);
    IStructAbiEncoder IStructAbiEncoder.Int144Array(params BigInteger[] value)
        => Int144Array(value);

    public AbiEncoder UInt144Array(params BigInteger[] value)
        => AddElement(new DynamicType<object>.BigIntegerArray(value, true, 144));
    IArrayAbiEncoder IArrayAbiEncoder.UInt144Array(params BigInteger[] value)
        => UInt144Array(value);
    IStructAbiEncoder IStructAbiEncoder.UInt144Array(params BigInteger[] value)
        => UInt144Array(value);
    public AbiEncoder Int152Array(params BigInteger[] value)
        => AddElement(new DynamicType<object>.BigIntegerArray(value, false, 152));
    IArrayAbiEncoder IArrayAbiEncoder.Int152Array(params BigInteger[] value)
        => Int152Array(value);
    IStructAbiEncoder IStructAbiEncoder.Int152Array(params BigInteger[] value)
        => Int152Array(value);

    public AbiEncoder UInt152Array(params BigInteger[] value)
        => AddElement(new DynamicType<object>.BigIntegerArray(value, true, 152));
    IArrayAbiEncoder IArrayAbiEncoder.UInt152Array(params BigInteger[] value)
        => UInt152Array(value);
    IStructAbiEncoder IStructAbiEncoder.UInt152Array(params BigInteger[] value)
        => UInt152Array(value);
    public AbiEncoder Int160Array(params BigInteger[] value)
        => AddElement(new DynamicType<object>.BigIntegerArray(value, false, 160));
    IArrayAbiEncoder IArrayAbiEncoder.Int160Array(params BigInteger[] value)
        => Int160Array(value);
    IStructAbiEncoder IStructAbiEncoder.Int160Array(params BigInteger[] value)
        => Int160Array(value);

    public AbiEncoder UInt160Array(params BigInteger[] value)
        => AddElement(new DynamicType<object>.BigIntegerArray(value, true, 160));
    IArrayAbiEncoder IArrayAbiEncoder.UInt160Array(params BigInteger[] value)
        => UInt160Array(value);
    IStructAbiEncoder IStructAbiEncoder.UInt160Array(params BigInteger[] value)
        => UInt160Array(value);
    public AbiEncoder Int168Array(params BigInteger[] value)
        => AddElement(new DynamicType<object>.BigIntegerArray(value, false, 168));
    IArrayAbiEncoder IArrayAbiEncoder.Int168Array(params BigInteger[] value)
        => Int168Array(value);
    IStructAbiEncoder IStructAbiEncoder.Int168Array(params BigInteger[] value)
        => Int168Array(value);

    public AbiEncoder UInt168Array(params BigInteger[] value)
        => AddElement(new DynamicType<object>.BigIntegerArray(value, true, 168));
    IArrayAbiEncoder IArrayAbiEncoder.UInt168Array(params BigInteger[] value)
        => UInt168Array(value);
    IStructAbiEncoder IStructAbiEncoder.UInt168Array(params BigInteger[] value)
        => UInt168Array(value);
    public AbiEncoder Int176Array(params BigInteger[] value)
        => AddElement(new DynamicType<object>.BigIntegerArray(value, false, 176));
    IArrayAbiEncoder IArrayAbiEncoder.Int176Array(params BigInteger[] value)
        => Int176Array(value);
    IStructAbiEncoder IStructAbiEncoder.Int176Array(params BigInteger[] value)
        => Int176Array(value);

    public AbiEncoder UInt176Array(params BigInteger[] value)
        => AddElement(new DynamicType<object>.BigIntegerArray(value, true, 176));
    IArrayAbiEncoder IArrayAbiEncoder.UInt176Array(params BigInteger[] value)
        => UInt176Array(value);
    IStructAbiEncoder IStructAbiEncoder.UInt176Array(params BigInteger[] value)
        => UInt176Array(value);
    public AbiEncoder Int184Array(params BigInteger[] value)
        => AddElement(new DynamicType<object>.BigIntegerArray(value, false, 184));
    IArrayAbiEncoder IArrayAbiEncoder.Int184Array(params BigInteger[] value)
        => Int184Array(value);
    IStructAbiEncoder IStructAbiEncoder.Int184Array(params BigInteger[] value)
        => Int184Array(value);

    public AbiEncoder UInt184Array(params BigInteger[] value)
        => AddElement(new DynamicType<object>.BigIntegerArray(value, true, 184));
    IArrayAbiEncoder IArrayAbiEncoder.UInt184Array(params BigInteger[] value)
        => UInt184Array(value);
    IStructAbiEncoder IStructAbiEncoder.UInt184Array(params BigInteger[] value)
        => UInt184Array(value);
    public AbiEncoder Int192Array(params BigInteger[] value)
        => AddElement(new DynamicType<object>.BigIntegerArray(value, false, 192));
    IArrayAbiEncoder IArrayAbiEncoder.Int192Array(params BigInteger[] value)
        => Int192Array(value);
    IStructAbiEncoder IStructAbiEncoder.Int192Array(params BigInteger[] value)
        => Int192Array(value);

    public AbiEncoder UInt192Array(params BigInteger[] value)
        => AddElement(new DynamicType<object>.BigIntegerArray(value, true, 192));
    IArrayAbiEncoder IArrayAbiEncoder.UInt192Array(params BigInteger[] value)
        => UInt192Array(value);
    IStructAbiEncoder IStructAbiEncoder.UInt192Array(params BigInteger[] value)
        => UInt192Array(value);
    public AbiEncoder Int200Array(params BigInteger[] value)
        => AddElement(new DynamicType<object>.BigIntegerArray(value, false, 200));
    IArrayAbiEncoder IArrayAbiEncoder.Int200Array(params BigInteger[] value)
        => Int200Array(value);
    IStructAbiEncoder IStructAbiEncoder.Int200Array(params BigInteger[] value)
        => Int200Array(value);

    public AbiEncoder UInt200Array(params BigInteger[] value)
        => AddElement(new DynamicType<object>.BigIntegerArray(value, true, 200));
    IArrayAbiEncoder IArrayAbiEncoder.UInt200Array(params BigInteger[] value)
        => UInt200Array(value);
    IStructAbiEncoder IStructAbiEncoder.UInt200Array(params BigInteger[] value)
        => UInt200Array(value);
    public AbiEncoder Int208Array(params BigInteger[] value)
        => AddElement(new DynamicType<object>.BigIntegerArray(value, false, 208));
    IArrayAbiEncoder IArrayAbiEncoder.Int208Array(params BigInteger[] value)
        => Int208Array(value);
    IStructAbiEncoder IStructAbiEncoder.Int208Array(params BigInteger[] value)
        => Int208Array(value);

    public AbiEncoder UInt208Array(params BigInteger[] value)
        => AddElement(new DynamicType<object>.BigIntegerArray(value, true, 208));
    IArrayAbiEncoder IArrayAbiEncoder.UInt208Array(params BigInteger[] value)
        => UInt208Array(value);
    IStructAbiEncoder IStructAbiEncoder.UInt208Array(params BigInteger[] value)
        => UInt208Array(value);
    public AbiEncoder Int216Array(params BigInteger[] value)
        => AddElement(new DynamicType<object>.BigIntegerArray(value, false, 216));
    IArrayAbiEncoder IArrayAbiEncoder.Int216Array(params BigInteger[] value)
        => Int216Array(value);
    IStructAbiEncoder IStructAbiEncoder.Int216Array(params BigInteger[] value)
        => Int216Array(value);

    public AbiEncoder UInt216Array(params BigInteger[] value)
        => AddElement(new DynamicType<object>.BigIntegerArray(value, true, 216));
    IArrayAbiEncoder IArrayAbiEncoder.UInt216Array(params BigInteger[] value)
        => UInt216Array(value);
    IStructAbiEncoder IStructAbiEncoder.UInt216Array(params BigInteger[] value)
        => UInt216Array(value);
    public AbiEncoder Int224Array(params BigInteger[] value)
        => AddElement(new DynamicType<object>.BigIntegerArray(value, false, 224));
    IArrayAbiEncoder IArrayAbiEncoder.Int224Array(params BigInteger[] value)
        => Int224Array(value);
    IStructAbiEncoder IStructAbiEncoder.Int224Array(params BigInteger[] value)
        => Int224Array(value);

    public AbiEncoder UInt224Array(params BigInteger[] value)
        => AddElement(new DynamicType<object>.BigIntegerArray(value, true, 224));
    IArrayAbiEncoder IArrayAbiEncoder.UInt224Array(params BigInteger[] value)
        => UInt224Array(value);
    IStructAbiEncoder IStructAbiEncoder.UInt224Array(params BigInteger[] value)
        => UInt224Array(value);
    public AbiEncoder Int232Array(params BigInteger[] value)
        => AddElement(new DynamicType<object>.BigIntegerArray(value, false, 232));
    IArrayAbiEncoder IArrayAbiEncoder.Int232Array(params BigInteger[] value)
        => Int232Array(value);
    IStructAbiEncoder IStructAbiEncoder.Int232Array(params BigInteger[] value)
        => Int232Array(value);

    public AbiEncoder UInt232Array(params BigInteger[] value)
        => AddElement(new DynamicType<object>.BigIntegerArray(value, true, 232));
    IArrayAbiEncoder IArrayAbiEncoder.UInt232Array(params BigInteger[] value)
        => UInt232Array(value);
    IStructAbiEncoder IStructAbiEncoder.UInt232Array(params BigInteger[] value)
        => UInt232Array(value);
    public AbiEncoder Int240Array(params BigInteger[] value)
        => AddElement(new DynamicType<object>.BigIntegerArray(value, false, 240));
    IArrayAbiEncoder IArrayAbiEncoder.Int240Array(params BigInteger[] value)
        => Int240Array(value);
    IStructAbiEncoder IStructAbiEncoder.Int240Array(params BigInteger[] value)
        => Int240Array(value);

    public AbiEncoder UInt240Array(params BigInteger[] value)
        => AddElement(new DynamicType<object>.BigIntegerArray(value, true, 240));
    IArrayAbiEncoder IArrayAbiEncoder.UInt240Array(params BigInteger[] value)
        => UInt240Array(value);
    IStructAbiEncoder IStructAbiEncoder.UInt240Array(params BigInteger[] value)
        => UInt240Array(value);
    public AbiEncoder Int248Array(params BigInteger[] value)
        => AddElement(new DynamicType<object>.BigIntegerArray(value, false, 248));
    IArrayAbiEncoder IArrayAbiEncoder.Int248Array(params BigInteger[] value)
        => Int248Array(value);
    IStructAbiEncoder IStructAbiEncoder.Int248Array(params BigInteger[] value)
        => Int248Array(value);

    public AbiEncoder UInt248Array(params BigInteger[] value)
        => AddElement(new DynamicType<object>.BigIntegerArray(value, true, 248));
    IArrayAbiEncoder IArrayAbiEncoder.UInt248Array(params BigInteger[] value)
        => UInt248Array(value);
    IStructAbiEncoder IStructAbiEncoder.UInt248Array(params BigInteger[] value)
        => UInt248Array(value);
    public AbiEncoder Int256Array(params BigInteger[] value)
        => AddElement(new DynamicType<object>.BigIntegerArray(value, false, 256));
    IArrayAbiEncoder IArrayAbiEncoder.Int256Array(params BigInteger[] value)
        => Int256Array(value);
    IStructAbiEncoder IStructAbiEncoder.Int256Array(params BigInteger[] value)
        => Int256Array(value);

    public AbiEncoder UInt256Array(params BigInteger[] value)
        => AddElement(new DynamicType<object>.BigIntegerArray(value, true, 256));
    IArrayAbiEncoder IArrayAbiEncoder.UInt256Array(params BigInteger[] value)
        => UInt256Array(value);
    IStructAbiEncoder IStructAbiEncoder.UInt256Array(params BigInteger[] value)
        => UInt256Array(value);
}