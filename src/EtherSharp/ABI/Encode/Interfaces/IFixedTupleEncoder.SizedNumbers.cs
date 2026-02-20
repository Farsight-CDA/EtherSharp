using EtherSharp.Numerics;

namespace EtherSharp.ABI.Encode.Interfaces;

public partial interface IFixedTupleEncoder
{
    /// <summary>
    /// Encodes an int8 value.
    /// </summary>
    /// <param name="value">The signed byte value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IFixedTupleEncoder Int8(sbyte value);
    /// <summary>
    /// Encodes a uint8 value.
    /// </summary>
    /// <param name="value">The byte value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IFixedTupleEncoder UInt8(byte value);

    /// <summary>
    /// Encodes an int16 value.
    /// </summary>
    /// <param name="value">The short value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IFixedTupleEncoder Int16(short value);
    /// <summary>
    /// Encodes a uint16 value.
    /// </summary>
    /// <param name="value">The unsigned short value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IFixedTupleEncoder UInt16(ushort value);

    /// <summary>
    /// Encodes an int24 value.
    /// </summary>
    /// <param name="value">The int value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IFixedTupleEncoder Int24(int value);
    /// <summary>
    /// Encodes a uint24 value.
    /// </summary>
    /// <param name="value">The uint value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IFixedTupleEncoder UInt24(uint value);
    /// <summary>
    /// Encodes an int32 value.
    /// </summary>
    /// <param name="value">The int value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IFixedTupleEncoder Int32(int value);
    /// <summary>
    /// Encodes a uint32 value.
    /// </summary>
    /// <param name="value">The uint value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IFixedTupleEncoder UInt32(uint value);
    /// <summary>
    /// Encodes an int40 value.
    /// </summary>
    /// <param name="value">The long value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IFixedTupleEncoder Int40(long value);
    /// <summary>
    /// Encodes a uint40 value.
    /// </summary>
    /// <param name="value">The ulong value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IFixedTupleEncoder UInt40(ulong value);
    /// <summary>
    /// Encodes an int48 value.
    /// </summary>
    /// <param name="value">The long value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IFixedTupleEncoder Int48(long value);
    /// <summary>
    /// Encodes a uint48 value.
    /// </summary>
    /// <param name="value">The ulong value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IFixedTupleEncoder UInt48(ulong value);
    /// <summary>
    /// Encodes an int56 value.
    /// </summary>
    /// <param name="value">The long value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IFixedTupleEncoder Int56(long value);
    /// <summary>
    /// Encodes a uint56 value.
    /// </summary>
    /// <param name="value">The ulong value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IFixedTupleEncoder UInt56(ulong value);
    /// <summary>
    /// Encodes an int64 value.
    /// </summary>
    /// <param name="value">The long value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IFixedTupleEncoder Int64(long value);
    /// <summary>
    /// Encodes a uint64 value.
    /// </summary>
    /// <param name="value">The ulong value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IFixedTupleEncoder UInt64(ulong value);
    /// <summary>
    /// Encodes an int72 value.
    /// </summary>
    /// <param name="value">The Int256 value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IFixedTupleEncoder Int72(Int256 value);
    /// <summary>
    /// Encodes a uint72 value.
    /// </summary>
    /// <param name="value">The UInt256 value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IFixedTupleEncoder UInt72(UInt256 value);
    /// <summary>
    /// Encodes an int80 value.
    /// </summary>
    /// <param name="value">The Int256 value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IFixedTupleEncoder Int80(Int256 value);
    /// <summary>
    /// Encodes a uint80 value.
    /// </summary>
    /// <param name="value">The UInt256 value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IFixedTupleEncoder UInt80(UInt256 value);
    /// <summary>
    /// Encodes an int88 value.
    /// </summary>
    /// <param name="value">The Int256 value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IFixedTupleEncoder Int88(Int256 value);
    /// <summary>
    /// Encodes a uint88 value.
    /// </summary>
    /// <param name="value">The UInt256 value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IFixedTupleEncoder UInt88(UInt256 value);
    /// <summary>
    /// Encodes an int96 value.
    /// </summary>
    /// <param name="value">The Int256 value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IFixedTupleEncoder Int96(Int256 value);
    /// <summary>
    /// Encodes a uint96 value.
    /// </summary>
    /// <param name="value">The UInt256 value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IFixedTupleEncoder UInt96(UInt256 value);
    /// <summary>
    /// Encodes an int104 value.
    /// </summary>
    /// <param name="value">The Int256 value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IFixedTupleEncoder Int104(Int256 value);
    /// <summary>
    /// Encodes a uint104 value.
    /// </summary>
    /// <param name="value">The UInt256 value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IFixedTupleEncoder UInt104(UInt256 value);
    /// <summary>
    /// Encodes an int112 value.
    /// </summary>
    /// <param name="value">The Int256 value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IFixedTupleEncoder Int112(Int256 value);
    /// <summary>
    /// Encodes a uint112 value.
    /// </summary>
    /// <param name="value">The UInt256 value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IFixedTupleEncoder UInt112(UInt256 value);
    /// <summary>
    /// Encodes an int120 value.
    /// </summary>
    /// <param name="value">The Int256 value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IFixedTupleEncoder Int120(Int256 value);
    /// <summary>
    /// Encodes a uint120 value.
    /// </summary>
    /// <param name="value">The UInt256 value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IFixedTupleEncoder UInt120(UInt256 value);
    /// <summary>
    /// Encodes an int128 value.
    /// </summary>
    /// <param name="value">The Int256 value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IFixedTupleEncoder Int128(Int256 value);
    /// <summary>
    /// Encodes a uint128 value.
    /// </summary>
    /// <param name="value">The UInt256 value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IFixedTupleEncoder UInt128(UInt256 value);
    /// <summary>
    /// Encodes an int136 value.
    /// </summary>
    /// <param name="value">The Int256 value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IFixedTupleEncoder Int136(Int256 value);
    /// <summary>
    /// Encodes a uint136 value.
    /// </summary>
    /// <param name="value">The UInt256 value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IFixedTupleEncoder UInt136(UInt256 value);
    /// <summary>
    /// Encodes an int144 value.
    /// </summary>
    /// <param name="value">The Int256 value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IFixedTupleEncoder Int144(Int256 value);
    /// <summary>
    /// Encodes a uint144 value.
    /// </summary>
    /// <param name="value">The UInt256 value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IFixedTupleEncoder UInt144(UInt256 value);
    /// <summary>
    /// Encodes an int152 value.
    /// </summary>
    /// <param name="value">The Int256 value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IFixedTupleEncoder Int152(Int256 value);
    /// <summary>
    /// Encodes a uint152 value.
    /// </summary>
    /// <param name="value">The UInt256 value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IFixedTupleEncoder UInt152(UInt256 value);
    /// <summary>
    /// Encodes an int160 value.
    /// </summary>
    /// <param name="value">The Int256 value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IFixedTupleEncoder Int160(Int256 value);
    /// <summary>
    /// Encodes a uint160 value.
    /// </summary>
    /// <param name="value">The UInt256 value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IFixedTupleEncoder UInt160(UInt256 value);
    /// <summary>
    /// Encodes an int168 value.
    /// </summary>
    /// <param name="value">The Int256 value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IFixedTupleEncoder Int168(Int256 value);
    /// <summary>
    /// Encodes a uint168 value.
    /// </summary>
    /// <param name="value">The UInt256 value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IFixedTupleEncoder UInt168(UInt256 value);
    /// <summary>
    /// Encodes an int176 value.
    /// </summary>
    /// <param name="value">The Int256 value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IFixedTupleEncoder Int176(Int256 value);
    /// <summary>
    /// Encodes a uint176 value.
    /// </summary>
    /// <param name="value">The UInt256 value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IFixedTupleEncoder UInt176(UInt256 value);
    /// <summary>
    /// Encodes an int184 value.
    /// </summary>
    /// <param name="value">The Int256 value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IFixedTupleEncoder Int184(Int256 value);
    /// <summary>
    /// Encodes a uint184 value.
    /// </summary>
    /// <param name="value">The UInt256 value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IFixedTupleEncoder UInt184(UInt256 value);
    /// <summary>
    /// Encodes an int192 value.
    /// </summary>
    /// <param name="value">The Int256 value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IFixedTupleEncoder Int192(Int256 value);
    /// <summary>
    /// Encodes a uint192 value.
    /// </summary>
    /// <param name="value">The UInt256 value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IFixedTupleEncoder UInt192(UInt256 value);
    /// <summary>
    /// Encodes an int200 value.
    /// </summary>
    /// <param name="value">The Int256 value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IFixedTupleEncoder Int200(Int256 value);
    /// <summary>
    /// Encodes a uint200 value.
    /// </summary>
    /// <param name="value">The UInt256 value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IFixedTupleEncoder UInt200(UInt256 value);
    /// <summary>
    /// Encodes an int208 value.
    /// </summary>
    /// <param name="value">The Int256 value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IFixedTupleEncoder Int208(Int256 value);
    /// <summary>
    /// Encodes a uint208 value.
    /// </summary>
    /// <param name="value">The UInt256 value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IFixedTupleEncoder UInt208(UInt256 value);
    /// <summary>
    /// Encodes an int216 value.
    /// </summary>
    /// <param name="value">The Int256 value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IFixedTupleEncoder Int216(Int256 value);
    /// <summary>
    /// Encodes a uint216 value.
    /// </summary>
    /// <param name="value">The UInt256 value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IFixedTupleEncoder UInt216(UInt256 value);
    /// <summary>
    /// Encodes an int224 value.
    /// </summary>
    /// <param name="value">The Int256 value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IFixedTupleEncoder Int224(Int256 value);
    /// <summary>
    /// Encodes a uint224 value.
    /// </summary>
    /// <param name="value">The UInt256 value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IFixedTupleEncoder UInt224(UInt256 value);
    /// <summary>
    /// Encodes an int232 value.
    /// </summary>
    /// <param name="value">The Int256 value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IFixedTupleEncoder Int232(Int256 value);
    /// <summary>
    /// Encodes a uint232 value.
    /// </summary>
    /// <param name="value">The UInt256 value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IFixedTupleEncoder UInt232(UInt256 value);
    /// <summary>
    /// Encodes an int240 value.
    /// </summary>
    /// <param name="value">The Int256 value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IFixedTupleEncoder Int240(Int256 value);
    /// <summary>
    /// Encodes a uint240 value.
    /// </summary>
    /// <param name="value">The UInt256 value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IFixedTupleEncoder UInt240(UInt256 value);
    /// <summary>
    /// Encodes an int248 value.
    /// </summary>
    /// <param name="value">The Int256 value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IFixedTupleEncoder Int248(Int256 value);
    /// <summary>
    /// Encodes a uint248 value.
    /// </summary>
    /// <param name="value">The UInt256 value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IFixedTupleEncoder UInt248(UInt256 value);
    /// <summary>
    /// Encodes an int256 value.
    /// </summary>
    /// <param name="value">The Int256 value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IFixedTupleEncoder Int256(Int256 value);
    /// <summary>
    /// Encodes a uint256 value.
    /// </summary>
    /// <param name="value">The UInt256 value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IFixedTupleEncoder UInt256(UInt256 value);
}
