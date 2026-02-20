using EtherSharp.ABI.Encode.Interfaces;
using EtherSharp.ABI.Types;
using EtherSharp.Numerics;

namespace EtherSharp.ABI;

public partial class AbiEncoder
{
    /// <summary>
    /// Encodes an int8 array value.
    /// </summary>
    /// <param name="value">The signed byte array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder Int8Array(params sbyte[] value)
        => AddElement(new AbiTypes.SizedNumberArray<sbyte>(value, 8));
    void IArrayAbiEncoder.Int8Array(params sbyte[] value)
        => Int8Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int8Array(params sbyte[] value)
        => Int8Array(value);

    /// <summary>
    /// Encodes a uint8 array value.
    /// </summary>
    /// <param name="value">The byte array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder UInt8Array(params byte[] value)
        => AddElement(new AbiTypes.SizedNumberArray<byte>(value, 8));
    void IArrayAbiEncoder.UInt8Array(params byte[] value)
        => UInt8Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt8Array(params byte[] value)
        => UInt8Array(value);

    /// <summary>
    /// Encodes an int16 array value.
    /// </summary>
    /// <param name="value">The short array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder Int16Array(params short[] value)
        => AddElement(new AbiTypes.SizedNumberArray<short>(value, 16));
    void IArrayAbiEncoder.Int16Array(params short[] value)
        => Int16Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int16Array(params short[] value)
        => Int16Array(value);

    /// <summary>
    /// Encodes a uint16 array value.
    /// </summary>
    /// <param name="value">The unsigned short array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder UInt16Array(params ushort[] value)
        => AddElement(new AbiTypes.SizedNumberArray<ushort>(value, 16));
    void IArrayAbiEncoder.UInt16Array(params ushort[] value)
        => UInt16Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt16Array(params ushort[] value)
        => UInt16Array(value);

    /// <summary>
    /// Encodes an int24 array value.
    /// </summary>
    /// <param name="value">The int array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder Int24Array(params int[] value)
        => AddElement(new AbiTypes.SizedNumberArray<int>(value, 24));
    void IArrayAbiEncoder.Int24Array(params int[] value)
        => Int24Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int24Array(params int[] value)
        => Int24Array(value);

    /// <summary>
    /// Encodes a uint24 array value.
    /// </summary>
    /// <param name="value">The uint array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder UInt24Array(params uint[] value)
        => AddElement(new AbiTypes.SizedNumberArray<uint>(value, 24));
    void IArrayAbiEncoder.UInt24Array(params uint[] value)
        => UInt24Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt24Array(params uint[] value)
        => UInt24Array(value);
    /// <summary>
    /// Encodes an int32 array value.
    /// </summary>
    /// <param name="value">The int array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder Int32Array(params int[] value)
        => AddElement(new AbiTypes.SizedNumberArray<int>(value, 32));
    void IArrayAbiEncoder.Int32Array(params int[] value)
        => Int32Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int32Array(params int[] value)
        => Int32Array(value);

    /// <summary>
    /// Encodes a uint32 array value.
    /// </summary>
    /// <param name="value">The uint array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder UInt32Array(params uint[] value)
        => AddElement(new AbiTypes.SizedNumberArray<uint>(value, 32));
    void IArrayAbiEncoder.UInt32Array(params uint[] value)
        => UInt32Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt32Array(params uint[] value)
        => UInt32Array(value);
    /// <summary>
    /// Encodes an int40 array value.
    /// </summary>
    /// <param name="value">The long array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder Int40Array(params long[] value)
        => AddElement(new AbiTypes.SizedNumberArray<long>(value, 40));
    void IArrayAbiEncoder.Int40Array(params long[] value)
        => Int40Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int40Array(params long[] value)
        => Int40Array(value);

    /// <summary>
    /// Encodes a uint40 array value.
    /// </summary>
    /// <param name="value">The ulong array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder UInt40Array(params ulong[] value)
        => AddElement(new AbiTypes.SizedNumberArray<ulong>(value, 40));
    void IArrayAbiEncoder.UInt40Array(params ulong[] value)
        => UInt40Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt40Array(params ulong[] value)
        => UInt40Array(value);
    /// <summary>
    /// Encodes an int48 array value.
    /// </summary>
    /// <param name="value">The long array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder Int48Array(params long[] value)
        => AddElement(new AbiTypes.SizedNumberArray<long>(value, 48));
    void IArrayAbiEncoder.Int48Array(params long[] value)
        => Int48Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int48Array(params long[] value)
        => Int48Array(value);

    /// <summary>
    /// Encodes a uint48 array value.
    /// </summary>
    /// <param name="value">The ulong array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder UInt48Array(params ulong[] value)
        => AddElement(new AbiTypes.SizedNumberArray<ulong>(value, 48));
    void IArrayAbiEncoder.UInt48Array(params ulong[] value)
        => UInt48Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt48Array(params ulong[] value)
        => UInt48Array(value);
    /// <summary>
    /// Encodes an int56 array value.
    /// </summary>
    /// <param name="value">The long array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder Int56Array(params long[] value)
        => AddElement(new AbiTypes.SizedNumberArray<long>(value, 56));
    void IArrayAbiEncoder.Int56Array(params long[] value)
        => Int56Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int56Array(params long[] value)
        => Int56Array(value);

    /// <summary>
    /// Encodes a uint56 array value.
    /// </summary>
    /// <param name="value">The ulong array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder UInt56Array(params ulong[] value)
        => AddElement(new AbiTypes.SizedNumberArray<ulong>(value, 56));
    void IArrayAbiEncoder.UInt56Array(params ulong[] value)
        => UInt56Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt56Array(params ulong[] value)
        => UInt56Array(value);
    /// <summary>
    /// Encodes an int64 array value.
    /// </summary>
    /// <param name="value">The long array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder Int64Array(params long[] value)
        => AddElement(new AbiTypes.SizedNumberArray<long>(value, 64));
    void IArrayAbiEncoder.Int64Array(params long[] value)
        => Int64Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int64Array(params long[] value)
        => Int64Array(value);

    /// <summary>
    /// Encodes a uint64 array value.
    /// </summary>
    /// <param name="value">The ulong array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder UInt64Array(params ulong[] value)
        => AddElement(new AbiTypes.SizedNumberArray<ulong>(value, 64));
    void IArrayAbiEncoder.UInt64Array(params ulong[] value)
        => UInt64Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt64Array(params ulong[] value)
        => UInt64Array(value);
    /// <summary>
    /// Encodes an int72 array value.
    /// </summary>
    /// <param name="value">The Int256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder Int72Array(params Int256[] value)
        => AddElement(new AbiTypes.Int256Array(value, 72));
    void IArrayAbiEncoder.Int72Array(params Int256[] value)
        => Int72Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int72Array(params Int256[] value)
        => Int72Array(value);

    /// <summary>
    /// Encodes a uint72 array value.
    /// </summary>
    /// <param name="value">The UInt256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder UInt72Array(params UInt256[] value)
        => AddElement(new AbiTypes.UInt256Array(value, 72));
    void IArrayAbiEncoder.UInt72Array(params UInt256[] value)
        => UInt72Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt72Array(params UInt256[] value)
        => UInt72Array(value);
    /// <summary>
    /// Encodes an int80 array value.
    /// </summary>
    /// <param name="value">The Int256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder Int80Array(params Int256[] value)
        => AddElement(new AbiTypes.Int256Array(value, 80));
    void IArrayAbiEncoder.Int80Array(params Int256[] value)
        => Int80Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int80Array(params Int256[] value)
        => Int80Array(value);

    /// <summary>
    /// Encodes a uint80 array value.
    /// </summary>
    /// <param name="value">The UInt256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder UInt80Array(params UInt256[] value)
        => AddElement(new AbiTypes.UInt256Array(value, 80));
    void IArrayAbiEncoder.UInt80Array(params UInt256[] value)
        => UInt80Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt80Array(params UInt256[] value)
        => UInt80Array(value);
    /// <summary>
    /// Encodes an int88 array value.
    /// </summary>
    /// <param name="value">The Int256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder Int88Array(params Int256[] value)
        => AddElement(new AbiTypes.Int256Array(value, 88));
    void IArrayAbiEncoder.Int88Array(params Int256[] value)
        => Int88Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int88Array(params Int256[] value)
        => Int88Array(value);

    /// <summary>
    /// Encodes a uint88 array value.
    /// </summary>
    /// <param name="value">The UInt256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder UInt88Array(params UInt256[] value)
        => AddElement(new AbiTypes.UInt256Array(value, 88));
    void IArrayAbiEncoder.UInt88Array(params UInt256[] value)
        => UInt88Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt88Array(params UInt256[] value)
        => UInt88Array(value);
    /// <summary>
    /// Encodes an int96 array value.
    /// </summary>
    /// <param name="value">The Int256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder Int96Array(params Int256[] value)
        => AddElement(new AbiTypes.Int256Array(value, 96));
    void IArrayAbiEncoder.Int96Array(params Int256[] value)
        => Int96Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int96Array(params Int256[] value)
        => Int96Array(value);

    /// <summary>
    /// Encodes a uint96 array value.
    /// </summary>
    /// <param name="value">The UInt256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder UInt96Array(params UInt256[] value)
        => AddElement(new AbiTypes.UInt256Array(value, 96));
    void IArrayAbiEncoder.UInt96Array(params UInt256[] value)
        => UInt96Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt96Array(params UInt256[] value)
        => UInt96Array(value);
    /// <summary>
    /// Encodes an int104 array value.
    /// </summary>
    /// <param name="value">The Int256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder Int104Array(params Int256[] value)
        => AddElement(new AbiTypes.Int256Array(value, 104));
    void IArrayAbiEncoder.Int104Array(params Int256[] value)
        => Int104Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int104Array(params Int256[] value)
        => Int104Array(value);

    /// <summary>
    /// Encodes a uint104 array value.
    /// </summary>
    /// <param name="value">The UInt256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder UInt104Array(params UInt256[] value)
        => AddElement(new AbiTypes.UInt256Array(value, 104));
    void IArrayAbiEncoder.UInt104Array(params UInt256[] value)
        => UInt104Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt104Array(params UInt256[] value)
        => UInt104Array(value);
    /// <summary>
    /// Encodes an int112 array value.
    /// </summary>
    /// <param name="value">The Int256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder Int112Array(params Int256[] value)
        => AddElement(new AbiTypes.Int256Array(value, 112));
    void IArrayAbiEncoder.Int112Array(params Int256[] value)
        => Int112Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int112Array(params Int256[] value)
        => Int112Array(value);

    /// <summary>
    /// Encodes a uint112 array value.
    /// </summary>
    /// <param name="value">The UInt256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder UInt112Array(params UInt256[] value)
        => AddElement(new AbiTypes.UInt256Array(value, 112));
    void IArrayAbiEncoder.UInt112Array(params UInt256[] value)
        => UInt112Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt112Array(params UInt256[] value)
        => UInt112Array(value);
    /// <summary>
    /// Encodes an int120 array value.
    /// </summary>
    /// <param name="value">The Int256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder Int120Array(params Int256[] value)
        => AddElement(new AbiTypes.Int256Array(value, 120));
    void IArrayAbiEncoder.Int120Array(params Int256[] value)
        => Int120Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int120Array(params Int256[] value)
        => Int120Array(value);

    /// <summary>
    /// Encodes a uint120 array value.
    /// </summary>
    /// <param name="value">The UInt256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder UInt120Array(params UInt256[] value)
        => AddElement(new AbiTypes.UInt256Array(value, 120));
    void IArrayAbiEncoder.UInt120Array(params UInt256[] value)
        => UInt120Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt120Array(params UInt256[] value)
        => UInt120Array(value);
    /// <summary>
    /// Encodes an int128 array value.
    /// </summary>
    /// <param name="value">The Int256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder Int128Array(params Int256[] value)
        => AddElement(new AbiTypes.Int256Array(value, 128));
    void IArrayAbiEncoder.Int128Array(params Int256[] value)
        => Int128Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int128Array(params Int256[] value)
        => Int128Array(value);

    /// <summary>
    /// Encodes a uint128 array value.
    /// </summary>
    /// <param name="value">The UInt256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder UInt128Array(params UInt256[] value)
        => AddElement(new AbiTypes.UInt256Array(value, 128));
    void IArrayAbiEncoder.UInt128Array(params UInt256[] value)
        => UInt128Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt128Array(params UInt256[] value)
        => UInt128Array(value);
    /// <summary>
    /// Encodes an int136 array value.
    /// </summary>
    /// <param name="value">The Int256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder Int136Array(params Int256[] value)
        => AddElement(new AbiTypes.Int256Array(value, 136));
    void IArrayAbiEncoder.Int136Array(params Int256[] value)
        => Int136Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int136Array(params Int256[] value)
        => Int136Array(value);

    /// <summary>
    /// Encodes a uint136 array value.
    /// </summary>
    /// <param name="value">The UInt256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder UInt136Array(params UInt256[] value)
        => AddElement(new AbiTypes.UInt256Array(value, 136));
    void IArrayAbiEncoder.UInt136Array(params UInt256[] value)
        => UInt136Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt136Array(params UInt256[] value)
        => UInt136Array(value);
    /// <summary>
    /// Encodes an int144 array value.
    /// </summary>
    /// <param name="value">The Int256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder Int144Array(params Int256[] value)
        => AddElement(new AbiTypes.Int256Array(value, 144));
    void IArrayAbiEncoder.Int144Array(params Int256[] value)
        => Int144Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int144Array(params Int256[] value)
        => Int144Array(value);

    /// <summary>
    /// Encodes a uint144 array value.
    /// </summary>
    /// <param name="value">The UInt256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder UInt144Array(params UInt256[] value)
        => AddElement(new AbiTypes.UInt256Array(value, 144));
    void IArrayAbiEncoder.UInt144Array(params UInt256[] value)
        => UInt144Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt144Array(params UInt256[] value)
        => UInt144Array(value);
    /// <summary>
    /// Encodes an int152 array value.
    /// </summary>
    /// <param name="value">The Int256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder Int152Array(params Int256[] value)
        => AddElement(new AbiTypes.Int256Array(value, 152));
    void IArrayAbiEncoder.Int152Array(params Int256[] value)
        => Int152Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int152Array(params Int256[] value)
        => Int152Array(value);

    /// <summary>
    /// Encodes a uint152 array value.
    /// </summary>
    /// <param name="value">The UInt256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder UInt152Array(params UInt256[] value)
        => AddElement(new AbiTypes.UInt256Array(value, 152));
    void IArrayAbiEncoder.UInt152Array(params UInt256[] value)
        => UInt152Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt152Array(params UInt256[] value)
        => UInt152Array(value);
    /// <summary>
    /// Encodes an int160 array value.
    /// </summary>
    /// <param name="value">The Int256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder Int160Array(params Int256[] value)
        => AddElement(new AbiTypes.Int256Array(value, 160));
    void IArrayAbiEncoder.Int160Array(params Int256[] value)
        => Int160Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int160Array(params Int256[] value)
        => Int160Array(value);

    /// <summary>
    /// Encodes a uint160 array value.
    /// </summary>
    /// <param name="value">The UInt256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder UInt160Array(params UInt256[] value)
        => AddElement(new AbiTypes.UInt256Array(value, 160));
    void IArrayAbiEncoder.UInt160Array(params UInt256[] value)
        => UInt160Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt160Array(params UInt256[] value)
        => UInt160Array(value);
    /// <summary>
    /// Encodes an int168 array value.
    /// </summary>
    /// <param name="value">The Int256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder Int168Array(params Int256[] value)
        => AddElement(new AbiTypes.Int256Array(value, 168));
    void IArrayAbiEncoder.Int168Array(params Int256[] value)
        => Int168Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int168Array(params Int256[] value)
        => Int168Array(value);

    /// <summary>
    /// Encodes a uint168 array value.
    /// </summary>
    /// <param name="value">The UInt256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder UInt168Array(params UInt256[] value)
        => AddElement(new AbiTypes.UInt256Array(value, 168));
    void IArrayAbiEncoder.UInt168Array(params UInt256[] value)
        => UInt168Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt168Array(params UInt256[] value)
        => UInt168Array(value);
    /// <summary>
    /// Encodes an int176 array value.
    /// </summary>
    /// <param name="value">The Int256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder Int176Array(params Int256[] value)
        => AddElement(new AbiTypes.Int256Array(value, 176));
    void IArrayAbiEncoder.Int176Array(params Int256[] value)
        => Int176Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int176Array(params Int256[] value)
        => Int176Array(value);

    /// <summary>
    /// Encodes a uint176 array value.
    /// </summary>
    /// <param name="value">The UInt256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder UInt176Array(params UInt256[] value)
        => AddElement(new AbiTypes.UInt256Array(value, 176));
    void IArrayAbiEncoder.UInt176Array(params UInt256[] value)
        => UInt176Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt176Array(params UInt256[] value)
        => UInt176Array(value);
    /// <summary>
    /// Encodes an int184 array value.
    /// </summary>
    /// <param name="value">The Int256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder Int184Array(params Int256[] value)
        => AddElement(new AbiTypes.Int256Array(value, 184));
    void IArrayAbiEncoder.Int184Array(params Int256[] value)
        => Int184Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int184Array(params Int256[] value)
        => Int184Array(value);

    /// <summary>
    /// Encodes a uint184 array value.
    /// </summary>
    /// <param name="value">The UInt256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder UInt184Array(params UInt256[] value)
        => AddElement(new AbiTypes.UInt256Array(value, 184));
    void IArrayAbiEncoder.UInt184Array(params UInt256[] value)
        => UInt184Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt184Array(params UInt256[] value)
        => UInt184Array(value);
    /// <summary>
    /// Encodes an int192 array value.
    /// </summary>
    /// <param name="value">The Int256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder Int192Array(params Int256[] value)
        => AddElement(new AbiTypes.Int256Array(value, 192));
    void IArrayAbiEncoder.Int192Array(params Int256[] value)
        => Int192Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int192Array(params Int256[] value)
        => Int192Array(value);

    /// <summary>
    /// Encodes a uint192 array value.
    /// </summary>
    /// <param name="value">The UInt256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder UInt192Array(params UInt256[] value)
        => AddElement(new AbiTypes.UInt256Array(value, 192));
    void IArrayAbiEncoder.UInt192Array(params UInt256[] value)
        => UInt192Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt192Array(params UInt256[] value)
        => UInt192Array(value);
    /// <summary>
    /// Encodes an int200 array value.
    /// </summary>
    /// <param name="value">The Int256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder Int200Array(params Int256[] value)
        => AddElement(new AbiTypes.Int256Array(value, 200));
    void IArrayAbiEncoder.Int200Array(params Int256[] value)
        => Int200Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int200Array(params Int256[] value)
        => Int200Array(value);

    /// <summary>
    /// Encodes a uint200 array value.
    /// </summary>
    /// <param name="value">The UInt256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder UInt200Array(params UInt256[] value)
        => AddElement(new AbiTypes.UInt256Array(value, 200));
    void IArrayAbiEncoder.UInt200Array(params UInt256[] value)
        => UInt200Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt200Array(params UInt256[] value)
        => UInt200Array(value);
    /// <summary>
    /// Encodes an int208 array value.
    /// </summary>
    /// <param name="value">The Int256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder Int208Array(params Int256[] value)
        => AddElement(new AbiTypes.Int256Array(value, 208));
    void IArrayAbiEncoder.Int208Array(params Int256[] value)
        => Int208Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int208Array(params Int256[] value)
        => Int208Array(value);

    /// <summary>
    /// Encodes a uint208 array value.
    /// </summary>
    /// <param name="value">The UInt256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder UInt208Array(params UInt256[] value)
        => AddElement(new AbiTypes.UInt256Array(value, 208));
    void IArrayAbiEncoder.UInt208Array(params UInt256[] value)
        => UInt208Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt208Array(params UInt256[] value)
        => UInt208Array(value);
    /// <summary>
    /// Encodes an int216 array value.
    /// </summary>
    /// <param name="value">The Int256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder Int216Array(params Int256[] value)
        => AddElement(new AbiTypes.Int256Array(value, 216));
    void IArrayAbiEncoder.Int216Array(params Int256[] value)
        => Int216Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int216Array(params Int256[] value)
        => Int216Array(value);

    /// <summary>
    /// Encodes a uint216 array value.
    /// </summary>
    /// <param name="value">The UInt256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder UInt216Array(params UInt256[] value)
        => AddElement(new AbiTypes.UInt256Array(value, 216));
    void IArrayAbiEncoder.UInt216Array(params UInt256[] value)
        => UInt216Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt216Array(params UInt256[] value)
        => UInt216Array(value);
    /// <summary>
    /// Encodes an int224 array value.
    /// </summary>
    /// <param name="value">The Int256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder Int224Array(params Int256[] value)
        => AddElement(new AbiTypes.Int256Array(value, 224));
    void IArrayAbiEncoder.Int224Array(params Int256[] value)
        => Int224Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int224Array(params Int256[] value)
        => Int224Array(value);

    /// <summary>
    /// Encodes a uint224 array value.
    /// </summary>
    /// <param name="value">The UInt256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder UInt224Array(params UInt256[] value)
        => AddElement(new AbiTypes.UInt256Array(value, 224));
    void IArrayAbiEncoder.UInt224Array(params UInt256[] value)
        => UInt224Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt224Array(params UInt256[] value)
        => UInt224Array(value);
    /// <summary>
    /// Encodes an int232 array value.
    /// </summary>
    /// <param name="value">The Int256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder Int232Array(params Int256[] value)
        => AddElement(new AbiTypes.Int256Array(value, 232));
    void IArrayAbiEncoder.Int232Array(params Int256[] value)
        => Int232Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int232Array(params Int256[] value)
        => Int232Array(value);

    /// <summary>
    /// Encodes a uint232 array value.
    /// </summary>
    /// <param name="value">The UInt256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder UInt232Array(params UInt256[] value)
        => AddElement(new AbiTypes.UInt256Array(value, 232));
    void IArrayAbiEncoder.UInt232Array(params UInt256[] value)
        => UInt232Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt232Array(params UInt256[] value)
        => UInt232Array(value);
    /// <summary>
    /// Encodes an int240 array value.
    /// </summary>
    /// <param name="value">The Int256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder Int240Array(params Int256[] value)
        => AddElement(new AbiTypes.Int256Array(value, 240));
    void IArrayAbiEncoder.Int240Array(params Int256[] value)
        => Int240Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int240Array(params Int256[] value)
        => Int240Array(value);

    /// <summary>
    /// Encodes a uint240 array value.
    /// </summary>
    /// <param name="value">The UInt256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder UInt240Array(params UInt256[] value)
        => AddElement(new AbiTypes.UInt256Array(value, 240));
    void IArrayAbiEncoder.UInt240Array(params UInt256[] value)
        => UInt240Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt240Array(params UInt256[] value)
        => UInt240Array(value);
    /// <summary>
    /// Encodes an int248 array value.
    /// </summary>
    /// <param name="value">The Int256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder Int248Array(params Int256[] value)
        => AddElement(new AbiTypes.Int256Array(value, 248));
    void IArrayAbiEncoder.Int248Array(params Int256[] value)
        => Int248Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int248Array(params Int256[] value)
        => Int248Array(value);

    /// <summary>
    /// Encodes a uint248 array value.
    /// </summary>
    /// <param name="value">The UInt256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder UInt248Array(params UInt256[] value)
        => AddElement(new AbiTypes.UInt256Array(value, 248));
    void IArrayAbiEncoder.UInt248Array(params UInt256[] value)
        => UInt248Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt248Array(params UInt256[] value)
        => UInt248Array(value);
    /// <summary>
    /// Encodes an int256 array value.
    /// </summary>
    /// <param name="value">The Int256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder Int256Array(params Int256[] value)
        => AddElement(new AbiTypes.Int256Array(value, 256));
    void IArrayAbiEncoder.Int256Array(params Int256[] value)
        => Int256Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Int256Array(params Int256[] value)
        => Int256Array(value);

    /// <summary>
    /// Encodes a uint256 array value.
    /// </summary>
    /// <param name="value">The UInt256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder UInt256Array(params UInt256[] value)
        => AddElement(new AbiTypes.UInt256Array(value, 256));
    void IArrayAbiEncoder.UInt256Array(params UInt256[] value)
        => UInt256Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.UInt256Array(params UInt256[] value)
        => UInt256Array(value);
}
