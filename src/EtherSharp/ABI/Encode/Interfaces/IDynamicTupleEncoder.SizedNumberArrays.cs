using EtherSharp.Numerics;

namespace EtherSharp.ABI.Encode.Interfaces;
public partial interface IDynamicTupleEncoder 
{
    /// <summary>
    /// Encodes an int8 array value.
    /// </summary>
    /// <param name="value">The signed byte array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IDynamicTupleEncoder Int8Array(params sbyte[] value);
    /// <summary>
    /// Encodes a uint8 array value.
    /// </summary>
    /// <param name="value">The byte array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IDynamicTupleEncoder UInt8Array(params byte[] value);

    /// <summary>
    /// Encodes an int16 array value.
    /// </summary>
    /// <param name="value">The short array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IDynamicTupleEncoder Int16Array(params short[] value);
    /// <summary>
    /// Encodes a uint16 array value.
    /// </summary>
    /// <param name="value">The unsigned short array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IDynamicTupleEncoder UInt16Array(params ushort[] value);

    /// <summary>
    /// Encodes an int24 array value.
    /// </summary>
    /// <param name="value">The int array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IDynamicTupleEncoder Int24Array(params int[] value);
    /// <summary>
    /// Encodes a uint24 array value.
    /// </summary>
    /// <param name="value">The uint array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IDynamicTupleEncoder UInt24Array(params uint[] value);
    /// <summary>
    /// Encodes an int32 array value.
    /// </summary>
    /// <param name="value">The int array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IDynamicTupleEncoder Int32Array(params int[] value);
    /// <summary>
    /// Encodes a uint32 array value.
    /// </summary>
    /// <param name="value">The uint array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IDynamicTupleEncoder UInt32Array(params uint[] value);
    /// <summary>
    /// Encodes an int40 array value.
    /// </summary>
    /// <param name="value">The long array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IDynamicTupleEncoder Int40Array(params long[] value);
    /// <summary>
    /// Encodes a uint40 array value.
    /// </summary>
    /// <param name="value">The ulong array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IDynamicTupleEncoder UInt40Array(params ulong[] value);
    /// <summary>
    /// Encodes an int48 array value.
    /// </summary>
    /// <param name="value">The long array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IDynamicTupleEncoder Int48Array(params long[] value);
    /// <summary>
    /// Encodes a uint48 array value.
    /// </summary>
    /// <param name="value">The ulong array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IDynamicTupleEncoder UInt48Array(params ulong[] value);
    /// <summary>
    /// Encodes an int56 array value.
    /// </summary>
    /// <param name="value">The long array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IDynamicTupleEncoder Int56Array(params long[] value);
    /// <summary>
    /// Encodes a uint56 array value.
    /// </summary>
    /// <param name="value">The ulong array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IDynamicTupleEncoder UInt56Array(params ulong[] value);
    /// <summary>
    /// Encodes an int64 array value.
    /// </summary>
    /// <param name="value">The long array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IDynamicTupleEncoder Int64Array(params long[] value);
    /// <summary>
    /// Encodes a uint64 array value.
    /// </summary>
    /// <param name="value">The ulong array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IDynamicTupleEncoder UInt64Array(params ulong[] value);
    /// <summary>
    /// Encodes an int72 array value.
    /// </summary>
    /// <param name="value">The Int256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IDynamicTupleEncoder Int72Array(params Int256[] value);
    /// <summary>
    /// Encodes a uint72 array value.
    /// </summary>
    /// <param name="value">The UInt256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IDynamicTupleEncoder UInt72Array(params UInt256[] value);
    /// <summary>
    /// Encodes an int80 array value.
    /// </summary>
    /// <param name="value">The Int256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IDynamicTupleEncoder Int80Array(params Int256[] value);
    /// <summary>
    /// Encodes a uint80 array value.
    /// </summary>
    /// <param name="value">The UInt256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IDynamicTupleEncoder UInt80Array(params UInt256[] value);
    /// <summary>
    /// Encodes an int88 array value.
    /// </summary>
    /// <param name="value">The Int256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IDynamicTupleEncoder Int88Array(params Int256[] value);
    /// <summary>
    /// Encodes a uint88 array value.
    /// </summary>
    /// <param name="value">The UInt256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IDynamicTupleEncoder UInt88Array(params UInt256[] value);
    /// <summary>
    /// Encodes an int96 array value.
    /// </summary>
    /// <param name="value">The Int256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IDynamicTupleEncoder Int96Array(params Int256[] value);
    /// <summary>
    /// Encodes a uint96 array value.
    /// </summary>
    /// <param name="value">The UInt256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IDynamicTupleEncoder UInt96Array(params UInt256[] value);
    /// <summary>
    /// Encodes an int104 array value.
    /// </summary>
    /// <param name="value">The Int256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IDynamicTupleEncoder Int104Array(params Int256[] value);
    /// <summary>
    /// Encodes a uint104 array value.
    /// </summary>
    /// <param name="value">The UInt256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IDynamicTupleEncoder UInt104Array(params UInt256[] value);
    /// <summary>
    /// Encodes an int112 array value.
    /// </summary>
    /// <param name="value">The Int256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IDynamicTupleEncoder Int112Array(params Int256[] value);
    /// <summary>
    /// Encodes a uint112 array value.
    /// </summary>
    /// <param name="value">The UInt256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IDynamicTupleEncoder UInt112Array(params UInt256[] value);
    /// <summary>
    /// Encodes an int120 array value.
    /// </summary>
    /// <param name="value">The Int256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IDynamicTupleEncoder Int120Array(params Int256[] value);
    /// <summary>
    /// Encodes a uint120 array value.
    /// </summary>
    /// <param name="value">The UInt256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IDynamicTupleEncoder UInt120Array(params UInt256[] value);
    /// <summary>
    /// Encodes an int128 array value.
    /// </summary>
    /// <param name="value">The Int256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IDynamicTupleEncoder Int128Array(params Int256[] value);
    /// <summary>
    /// Encodes a uint128 array value.
    /// </summary>
    /// <param name="value">The UInt256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IDynamicTupleEncoder UInt128Array(params UInt256[] value);
    /// <summary>
    /// Encodes an int136 array value.
    /// </summary>
    /// <param name="value">The Int256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IDynamicTupleEncoder Int136Array(params Int256[] value);
    /// <summary>
    /// Encodes a uint136 array value.
    /// </summary>
    /// <param name="value">The UInt256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IDynamicTupleEncoder UInt136Array(params UInt256[] value);
    /// <summary>
    /// Encodes an int144 array value.
    /// </summary>
    /// <param name="value">The Int256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IDynamicTupleEncoder Int144Array(params Int256[] value);
    /// <summary>
    /// Encodes a uint144 array value.
    /// </summary>
    /// <param name="value">The UInt256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IDynamicTupleEncoder UInt144Array(params UInt256[] value);
    /// <summary>
    /// Encodes an int152 array value.
    /// </summary>
    /// <param name="value">The Int256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IDynamicTupleEncoder Int152Array(params Int256[] value);
    /// <summary>
    /// Encodes a uint152 array value.
    /// </summary>
    /// <param name="value">The UInt256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IDynamicTupleEncoder UInt152Array(params UInt256[] value);
    /// <summary>
    /// Encodes an int160 array value.
    /// </summary>
    /// <param name="value">The Int256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IDynamicTupleEncoder Int160Array(params Int256[] value);
    /// <summary>
    /// Encodes a uint160 array value.
    /// </summary>
    /// <param name="value">The UInt256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IDynamicTupleEncoder UInt160Array(params UInt256[] value);
    /// <summary>
    /// Encodes an int168 array value.
    /// </summary>
    /// <param name="value">The Int256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IDynamicTupleEncoder Int168Array(params Int256[] value);
    /// <summary>
    /// Encodes a uint168 array value.
    /// </summary>
    /// <param name="value">The UInt256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IDynamicTupleEncoder UInt168Array(params UInt256[] value);
    /// <summary>
    /// Encodes an int176 array value.
    /// </summary>
    /// <param name="value">The Int256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IDynamicTupleEncoder Int176Array(params Int256[] value);
    /// <summary>
    /// Encodes a uint176 array value.
    /// </summary>
    /// <param name="value">The UInt256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IDynamicTupleEncoder UInt176Array(params UInt256[] value);
    /// <summary>
    /// Encodes an int184 array value.
    /// </summary>
    /// <param name="value">The Int256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IDynamicTupleEncoder Int184Array(params Int256[] value);
    /// <summary>
    /// Encodes a uint184 array value.
    /// </summary>
    /// <param name="value">The UInt256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IDynamicTupleEncoder UInt184Array(params UInt256[] value);
    /// <summary>
    /// Encodes an int192 array value.
    /// </summary>
    /// <param name="value">The Int256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IDynamicTupleEncoder Int192Array(params Int256[] value);
    /// <summary>
    /// Encodes a uint192 array value.
    /// </summary>
    /// <param name="value">The UInt256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IDynamicTupleEncoder UInt192Array(params UInt256[] value);
    /// <summary>
    /// Encodes an int200 array value.
    /// </summary>
    /// <param name="value">The Int256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IDynamicTupleEncoder Int200Array(params Int256[] value);
    /// <summary>
    /// Encodes a uint200 array value.
    /// </summary>
    /// <param name="value">The UInt256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IDynamicTupleEncoder UInt200Array(params UInt256[] value);
    /// <summary>
    /// Encodes an int208 array value.
    /// </summary>
    /// <param name="value">The Int256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IDynamicTupleEncoder Int208Array(params Int256[] value);
    /// <summary>
    /// Encodes a uint208 array value.
    /// </summary>
    /// <param name="value">The UInt256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IDynamicTupleEncoder UInt208Array(params UInt256[] value);
    /// <summary>
    /// Encodes an int216 array value.
    /// </summary>
    /// <param name="value">The Int256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IDynamicTupleEncoder Int216Array(params Int256[] value);
    /// <summary>
    /// Encodes a uint216 array value.
    /// </summary>
    /// <param name="value">The UInt256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IDynamicTupleEncoder UInt216Array(params UInt256[] value);
    /// <summary>
    /// Encodes an int224 array value.
    /// </summary>
    /// <param name="value">The Int256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IDynamicTupleEncoder Int224Array(params Int256[] value);
    /// <summary>
    /// Encodes a uint224 array value.
    /// </summary>
    /// <param name="value">The UInt256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IDynamicTupleEncoder UInt224Array(params UInt256[] value);
    /// <summary>
    /// Encodes an int232 array value.
    /// </summary>
    /// <param name="value">The Int256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IDynamicTupleEncoder Int232Array(params Int256[] value);
    /// <summary>
    /// Encodes a uint232 array value.
    /// </summary>
    /// <param name="value">The UInt256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IDynamicTupleEncoder UInt232Array(params UInt256[] value);
    /// <summary>
    /// Encodes an int240 array value.
    /// </summary>
    /// <param name="value">The Int256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IDynamicTupleEncoder Int240Array(params Int256[] value);
    /// <summary>
    /// Encodes a uint240 array value.
    /// </summary>
    /// <param name="value">The UInt256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IDynamicTupleEncoder UInt240Array(params UInt256[] value);
    /// <summary>
    /// Encodes an int248 array value.
    /// </summary>
    /// <param name="value">The Int256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IDynamicTupleEncoder Int248Array(params Int256[] value);
    /// <summary>
    /// Encodes a uint248 array value.
    /// </summary>
    /// <param name="value">The UInt256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IDynamicTupleEncoder UInt248Array(params UInt256[] value);
    /// <summary>
    /// Encodes an int256 array value.
    /// </summary>
    /// <param name="value">The Int256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IDynamicTupleEncoder Int256Array(params Int256[] value);
    /// <summary>
    /// Encodes a uint256 array value.
    /// </summary>
    /// <param name="value">The UInt256 array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IDynamicTupleEncoder UInt256Array(params UInt256[] value);
}
