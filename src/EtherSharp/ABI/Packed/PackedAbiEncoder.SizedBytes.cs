using EtherSharp.ABI.Types;

namespace EtherSharp.ABI.Packed;

public partial class PackedAbiEncoder
{
    /// <summary>
    /// Encodes a bytes1 value in packed format.
    /// </summary>
    /// <param name="value">The byte value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public PackedAbiEncoder Bytes1(byte value)
        => AddElement(new AbiTypes.Byte(value));
    /// <summary>
    /// Encodes a bytes2 value in packed format.
    /// </summary>
    /// <param name="value">The 2-byte value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public PackedAbiEncoder Bytes2(ReadOnlyMemory<byte> value)
        => AddElement(new AbiTypes.SizedBytes(value, 2));
    /// <summary>
    /// Encodes a bytes3 value in packed format.
    /// </summary>
    /// <param name="value">The 3-byte value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public PackedAbiEncoder Bytes3(ReadOnlyMemory<byte> value)
        => AddElement(new AbiTypes.SizedBytes(value, 3));
    /// <summary>
    /// Encodes a bytes4 value in packed format.
    /// </summary>
    /// <param name="value">The 4-byte value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public PackedAbiEncoder Bytes4(ReadOnlyMemory<byte> value)
        => AddElement(new AbiTypes.SizedBytes(value, 4));
    /// <summary>
    /// Encodes a bytes5 value in packed format.
    /// </summary>
    /// <param name="value">The 5-byte value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public PackedAbiEncoder Bytes5(ReadOnlyMemory<byte> value)
        => AddElement(new AbiTypes.SizedBytes(value, 5));
    /// <summary>
    /// Encodes a bytes6 value in packed format.
    /// </summary>
    /// <param name="value">The 6-byte value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public PackedAbiEncoder Bytes6(ReadOnlyMemory<byte> value)
        => AddElement(new AbiTypes.SizedBytes(value, 6));
    /// <summary>
    /// Encodes a bytes7 value in packed format.
    /// </summary>
    /// <param name="value">The 7-byte value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public PackedAbiEncoder Bytes7(ReadOnlyMemory<byte> value)
        => AddElement(new AbiTypes.SizedBytes(value, 7));
    /// <summary>
    /// Encodes a bytes8 value in packed format.
    /// </summary>
    /// <param name="value">The 8-byte value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public PackedAbiEncoder Bytes8(ReadOnlyMemory<byte> value)
        => AddElement(new AbiTypes.SizedBytes(value, 8));
    /// <summary>
    /// Encodes a bytes9 value in packed format.
    /// </summary>
    /// <param name="value">The 9-byte value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public PackedAbiEncoder Bytes9(ReadOnlyMemory<byte> value)
        => AddElement(new AbiTypes.SizedBytes(value, 9));
    /// <summary>
    /// Encodes a bytes10 value in packed format.
    /// </summary>
    /// <param name="value">The 10-byte value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public PackedAbiEncoder Bytes10(ReadOnlyMemory<byte> value)
        => AddElement(new AbiTypes.SizedBytes(value, 10));
    /// <summary>
    /// Encodes a bytes11 value in packed format.
    /// </summary>
    /// <param name="value">The 11-byte value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public PackedAbiEncoder Bytes11(ReadOnlyMemory<byte> value)
        => AddElement(new AbiTypes.SizedBytes(value, 11));
    /// <summary>
    /// Encodes a bytes12 value in packed format.
    /// </summary>
    /// <param name="value">The 12-byte value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public PackedAbiEncoder Bytes12(ReadOnlyMemory<byte> value)
        => AddElement(new AbiTypes.SizedBytes(value, 12));
    /// <summary>
    /// Encodes a bytes13 value in packed format.
    /// </summary>
    /// <param name="value">The 13-byte value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public PackedAbiEncoder Bytes13(ReadOnlyMemory<byte> value)
        => AddElement(new AbiTypes.SizedBytes(value, 13));
    /// <summary>
    /// Encodes a bytes14 value in packed format.
    /// </summary>
    /// <param name="value">The 14-byte value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public PackedAbiEncoder Bytes14(ReadOnlyMemory<byte> value)
        => AddElement(new AbiTypes.SizedBytes(value, 14));
    /// <summary>
    /// Encodes a bytes15 value in packed format.
    /// </summary>
    /// <param name="value">The 15-byte value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public PackedAbiEncoder Bytes15(ReadOnlyMemory<byte> value)
        => AddElement(new AbiTypes.SizedBytes(value, 15));
    /// <summary>
    /// Encodes a bytes16 value in packed format.
    /// </summary>
    /// <param name="value">The 16-byte value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public PackedAbiEncoder Bytes16(ReadOnlyMemory<byte> value)
        => AddElement(new AbiTypes.SizedBytes(value, 16));
    /// <summary>
    /// Encodes a bytes17 value in packed format.
    /// </summary>
    /// <param name="value">The 17-byte value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public PackedAbiEncoder Bytes17(ReadOnlyMemory<byte> value)
        => AddElement(new AbiTypes.SizedBytes(value, 17));
    /// <summary>
    /// Encodes a bytes18 value in packed format.
    /// </summary>
    /// <param name="value">The 18-byte value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public PackedAbiEncoder Bytes18(ReadOnlyMemory<byte> value)
        => AddElement(new AbiTypes.SizedBytes(value, 18));
    /// <summary>
    /// Encodes a bytes19 value in packed format.
    /// </summary>
    /// <param name="value">The 19-byte value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public PackedAbiEncoder Bytes19(ReadOnlyMemory<byte> value)
        => AddElement(new AbiTypes.SizedBytes(value, 19));
    /// <summary>
    /// Encodes a bytes20 value in packed format.
    /// </summary>
    /// <param name="value">The 20-byte value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public PackedAbiEncoder Bytes20(ReadOnlyMemory<byte> value)
        => AddElement(new AbiTypes.SizedBytes(value, 20));
    /// <summary>
    /// Encodes a bytes21 value in packed format.
    /// </summary>
    /// <param name="value">The 21-byte value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public PackedAbiEncoder Bytes21(ReadOnlyMemory<byte> value)
        => AddElement(new AbiTypes.SizedBytes(value, 21));
    /// <summary>
    /// Encodes a bytes22 value in packed format.
    /// </summary>
    /// <param name="value">The 22-byte value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public PackedAbiEncoder Bytes22(ReadOnlyMemory<byte> value)
        => AddElement(new AbiTypes.SizedBytes(value, 22));
    /// <summary>
    /// Encodes a bytes23 value in packed format.
    /// </summary>
    /// <param name="value">The 23-byte value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public PackedAbiEncoder Bytes23(ReadOnlyMemory<byte> value)
        => AddElement(new AbiTypes.SizedBytes(value, 23));
    /// <summary>
    /// Encodes a bytes24 value in packed format.
    /// </summary>
    /// <param name="value">The 24-byte value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public PackedAbiEncoder Bytes24(ReadOnlyMemory<byte> value)
        => AddElement(new AbiTypes.SizedBytes(value, 24));
    /// <summary>
    /// Encodes a bytes25 value in packed format.
    /// </summary>
    /// <param name="value">The 25-byte value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public PackedAbiEncoder Bytes25(ReadOnlyMemory<byte> value)
        => AddElement(new AbiTypes.SizedBytes(value, 25));
    /// <summary>
    /// Encodes a bytes26 value in packed format.
    /// </summary>
    /// <param name="value">The 26-byte value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public PackedAbiEncoder Bytes26(ReadOnlyMemory<byte> value)
        => AddElement(new AbiTypes.SizedBytes(value, 26));
    /// <summary>
    /// Encodes a bytes27 value in packed format.
    /// </summary>
    /// <param name="value">The 27-byte value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public PackedAbiEncoder Bytes27(ReadOnlyMemory<byte> value)
        => AddElement(new AbiTypes.SizedBytes(value, 27));
    /// <summary>
    /// Encodes a bytes28 value in packed format.
    /// </summary>
    /// <param name="value">The 28-byte value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public PackedAbiEncoder Bytes28(ReadOnlyMemory<byte> value)
        => AddElement(new AbiTypes.SizedBytes(value, 28));
    /// <summary>
    /// Encodes a bytes29 value in packed format.
    /// </summary>
    /// <param name="value">The 29-byte value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public PackedAbiEncoder Bytes29(ReadOnlyMemory<byte> value)
        => AddElement(new AbiTypes.SizedBytes(value, 29));
    /// <summary>
    /// Encodes a bytes30 value in packed format.
    /// </summary>
    /// <param name="value">The 30-byte value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public PackedAbiEncoder Bytes30(ReadOnlyMemory<byte> value)
        => AddElement(new AbiTypes.SizedBytes(value, 30));
    /// <summary>
    /// Encodes a bytes31 value in packed format.
    /// </summary>
    /// <param name="value">The 31-byte value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public PackedAbiEncoder Bytes31(ReadOnlyMemory<byte> value)
        => AddElement(new AbiTypes.SizedBytes(value, 31));
    /// <summary>
    /// Encodes a bytes32 value in packed format.
    /// </summary>
    /// <param name="value">The 32-byte value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public PackedAbiEncoder Bytes32(ReadOnlyMemory<byte> value)
        => AddElement(new AbiTypes.SizedBytes(value, 32));

}
