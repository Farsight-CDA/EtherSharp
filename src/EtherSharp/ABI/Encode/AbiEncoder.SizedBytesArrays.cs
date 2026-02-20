using EtherSharp.ABI.Encode.Interfaces;
using EtherSharp.ABI.Types;

namespace EtherSharp.ABI;

public partial class AbiEncoder
{
    /// <summary>
    /// Encodes a bytes1 array value.
    /// </summary>
    /// <param name="value">The byte array to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder Bytes1Array(params byte[] value)
        => AddElement(new AbiTypes.SizedBytesArray([value], 1));
    IArrayAbiEncoder IArrayAbiEncoder.Bytes1Array(params byte[] value)
        => Bytes1Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes1Array(params byte[] value)
        => Bytes1Array(value);
    /// <summary>
    /// Encodes a bytes2 array value.
    /// </summary>
    /// <param name="values">The array of 2-byte values to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder Bytes2Array(params ReadOnlyMemory<byte>[] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 2));
    IArrayAbiEncoder IArrayAbiEncoder.Bytes2Array(params ReadOnlyMemory<byte>[] values)
        => Bytes2Array(values);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes2Array(params ReadOnlyMemory<byte>[] values)
        => Bytes2Array(values);
    /// <summary>
    /// Encodes a bytes3 array value.
    /// </summary>
    /// <param name="values">The array of 3-byte values to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder Bytes3Array(params ReadOnlyMemory<byte>[] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 3));
    IArrayAbiEncoder IArrayAbiEncoder.Bytes3Array(params ReadOnlyMemory<byte>[] values)
        => Bytes3Array(values);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes3Array(params ReadOnlyMemory<byte>[] values)
        => Bytes3Array(values);
    /// <summary>
    /// Encodes a bytes4 array value.
    /// </summary>
    /// <param name="values">The array of 4-byte values to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder Bytes4Array(params ReadOnlyMemory<byte>[] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 4));
    IArrayAbiEncoder IArrayAbiEncoder.Bytes4Array(params ReadOnlyMemory<byte>[] values)
        => Bytes4Array(values);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes4Array(params ReadOnlyMemory<byte>[] values)
        => Bytes4Array(values);
    /// <summary>
    /// Encodes a bytes5 array value.
    /// </summary>
    /// <param name="values">The array of 5-byte values to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder Bytes5Array(params ReadOnlyMemory<byte>[] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 5));
    IArrayAbiEncoder IArrayAbiEncoder.Bytes5Array(params ReadOnlyMemory<byte>[] values)
        => Bytes5Array(values);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes5Array(params ReadOnlyMemory<byte>[] values)
        => Bytes5Array(values);
    /// <summary>
    /// Encodes a bytes6 array value.
    /// </summary>
    /// <param name="values">The array of 6-byte values to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder Bytes6Array(params ReadOnlyMemory<byte>[] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 6));
    IArrayAbiEncoder IArrayAbiEncoder.Bytes6Array(params ReadOnlyMemory<byte>[] values)
        => Bytes6Array(values);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes6Array(params ReadOnlyMemory<byte>[] values)
        => Bytes6Array(values);
    /// <summary>
    /// Encodes a bytes7 array value.
    /// </summary>
    /// <param name="values">The array of 7-byte values to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder Bytes7Array(params ReadOnlyMemory<byte>[] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 7));
    IArrayAbiEncoder IArrayAbiEncoder.Bytes7Array(params ReadOnlyMemory<byte>[] values)
        => Bytes7Array(values);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes7Array(params ReadOnlyMemory<byte>[] values)
        => Bytes7Array(values);
    /// <summary>
    /// Encodes a bytes8 array value.
    /// </summary>
    /// <param name="values">The array of 8-byte values to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder Bytes8Array(params ReadOnlyMemory<byte>[] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 8));
    IArrayAbiEncoder IArrayAbiEncoder.Bytes8Array(params ReadOnlyMemory<byte>[] values)
        => Bytes8Array(values);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes8Array(params ReadOnlyMemory<byte>[] values)
        => Bytes8Array(values);
    /// <summary>
    /// Encodes a bytes9 array value.
    /// </summary>
    /// <param name="values">The array of 9-byte values to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder Bytes9Array(params ReadOnlyMemory<byte>[] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 9));
    IArrayAbiEncoder IArrayAbiEncoder.Bytes9Array(params ReadOnlyMemory<byte>[] values)
        => Bytes9Array(values);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes9Array(params ReadOnlyMemory<byte>[] values)
        => Bytes9Array(values);
    /// <summary>
    /// Encodes a bytes10 array value.
    /// </summary>
    /// <param name="values">The array of 10-byte values to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder Bytes10Array(params ReadOnlyMemory<byte>[] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 10));
    IArrayAbiEncoder IArrayAbiEncoder.Bytes10Array(params ReadOnlyMemory<byte>[] values)
        => Bytes10Array(values);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes10Array(params ReadOnlyMemory<byte>[] values)
        => Bytes10Array(values);
    /// <summary>
    /// Encodes a bytes11 array value.
    /// </summary>
    /// <param name="values">The array of 11-byte values to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder Bytes11Array(params ReadOnlyMemory<byte>[] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 11));
    IArrayAbiEncoder IArrayAbiEncoder.Bytes11Array(params ReadOnlyMemory<byte>[] values)
        => Bytes11Array(values);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes11Array(params ReadOnlyMemory<byte>[] values)
        => Bytes11Array(values);
    /// <summary>
    /// Encodes a bytes12 array value.
    /// </summary>
    /// <param name="values">The array of 12-byte values to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder Bytes12Array(params ReadOnlyMemory<byte>[] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 12));
    IArrayAbiEncoder IArrayAbiEncoder.Bytes12Array(params ReadOnlyMemory<byte>[] values)
        => Bytes12Array(values);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes12Array(params ReadOnlyMemory<byte>[] values)
        => Bytes12Array(values);
    /// <summary>
    /// Encodes a bytes13 array value.
    /// </summary>
    /// <param name="values">The array of 13-byte values to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder Bytes13Array(params ReadOnlyMemory<byte>[] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 13));
    IArrayAbiEncoder IArrayAbiEncoder.Bytes13Array(params ReadOnlyMemory<byte>[] values)
        => Bytes13Array(values);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes13Array(params ReadOnlyMemory<byte>[] values)
        => Bytes13Array(values);
    /// <summary>
    /// Encodes a bytes14 array value.
    /// </summary>
    /// <param name="values">The array of 14-byte values to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder Bytes14Array(params ReadOnlyMemory<byte>[] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 14));
    IArrayAbiEncoder IArrayAbiEncoder.Bytes14Array(params ReadOnlyMemory<byte>[] values)
        => Bytes14Array(values);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes14Array(params ReadOnlyMemory<byte>[] values)
        => Bytes14Array(values);
    /// <summary>
    /// Encodes a bytes15 array value.
    /// </summary>
    /// <param name="values">The array of 15-byte values to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder Bytes15Array(params ReadOnlyMemory<byte>[] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 15));
    IArrayAbiEncoder IArrayAbiEncoder.Bytes15Array(params ReadOnlyMemory<byte>[] values)
        => Bytes15Array(values);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes15Array(params ReadOnlyMemory<byte>[] values)
        => Bytes15Array(values);
    /// <summary>
    /// Encodes a bytes16 array value.
    /// </summary>
    /// <param name="values">The array of 16-byte values to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder Bytes16Array(params ReadOnlyMemory<byte>[] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 16));
    IArrayAbiEncoder IArrayAbiEncoder.Bytes16Array(params ReadOnlyMemory<byte>[] values)
        => Bytes16Array(values);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes16Array(params ReadOnlyMemory<byte>[] values)
        => Bytes16Array(values);
    /// <summary>
    /// Encodes a bytes17 array value.
    /// </summary>
    /// <param name="values">The array of 17-byte values to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder Bytes17Array(params ReadOnlyMemory<byte>[] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 17));
    IArrayAbiEncoder IArrayAbiEncoder.Bytes17Array(params ReadOnlyMemory<byte>[] values)
        => Bytes17Array(values);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes17Array(params ReadOnlyMemory<byte>[] values)
        => Bytes17Array(values);
    /// <summary>
    /// Encodes a bytes18 array value.
    /// </summary>
    /// <param name="values">The array of 18-byte values to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder Bytes18Array(params ReadOnlyMemory<byte>[] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 18));
    IArrayAbiEncoder IArrayAbiEncoder.Bytes18Array(params ReadOnlyMemory<byte>[] values)
        => Bytes18Array(values);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes18Array(params ReadOnlyMemory<byte>[] values)
        => Bytes18Array(values);
    /// <summary>
    /// Encodes a bytes19 array value.
    /// </summary>
    /// <param name="values">The array of 19-byte values to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder Bytes19Array(params ReadOnlyMemory<byte>[] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 19));
    IArrayAbiEncoder IArrayAbiEncoder.Bytes19Array(params ReadOnlyMemory<byte>[] values)
        => Bytes19Array(values);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes19Array(params ReadOnlyMemory<byte>[] values)
        => Bytes19Array(values);
    /// <summary>
    /// Encodes a bytes20 array value.
    /// </summary>
    /// <param name="values">The array of 20-byte values to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder Bytes20Array(params ReadOnlyMemory<byte>[] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 20));
    IArrayAbiEncoder IArrayAbiEncoder.Bytes20Array(params ReadOnlyMemory<byte>[] values)
        => Bytes20Array(values);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes20Array(params ReadOnlyMemory<byte>[] values)
        => Bytes20Array(values);
    /// <summary>
    /// Encodes a bytes21 array value.
    /// </summary>
    /// <param name="values">The array of 21-byte values to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder Bytes21Array(params ReadOnlyMemory<byte>[] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 21));
    IArrayAbiEncoder IArrayAbiEncoder.Bytes21Array(params ReadOnlyMemory<byte>[] values)
        => Bytes21Array(values);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes21Array(params ReadOnlyMemory<byte>[] values)
        => Bytes21Array(values);
    /// <summary>
    /// Encodes a bytes22 array value.
    /// </summary>
    /// <param name="values">The array of 22-byte values to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder Bytes22Array(params ReadOnlyMemory<byte>[] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 22));
    IArrayAbiEncoder IArrayAbiEncoder.Bytes22Array(params ReadOnlyMemory<byte>[] values)
        => Bytes22Array(values);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes22Array(params ReadOnlyMemory<byte>[] values)
        => Bytes22Array(values);
    /// <summary>
    /// Encodes a bytes23 array value.
    /// </summary>
    /// <param name="values">The array of 23-byte values to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder Bytes23Array(params ReadOnlyMemory<byte>[] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 23));
    IArrayAbiEncoder IArrayAbiEncoder.Bytes23Array(params ReadOnlyMemory<byte>[] values)
        => Bytes23Array(values);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes23Array(params ReadOnlyMemory<byte>[] values)
        => Bytes23Array(values);
    /// <summary>
    /// Encodes a bytes24 array value.
    /// </summary>
    /// <param name="values">The array of 24-byte values to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder Bytes24Array(params ReadOnlyMemory<byte>[] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 24));
    IArrayAbiEncoder IArrayAbiEncoder.Bytes24Array(params ReadOnlyMemory<byte>[] values)
        => Bytes24Array(values);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes24Array(params ReadOnlyMemory<byte>[] values)
        => Bytes24Array(values);
    /// <summary>
    /// Encodes a bytes25 array value.
    /// </summary>
    /// <param name="values">The array of 25-byte values to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder Bytes25Array(params ReadOnlyMemory<byte>[] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 25));
    IArrayAbiEncoder IArrayAbiEncoder.Bytes25Array(params ReadOnlyMemory<byte>[] values)
        => Bytes25Array(values);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes25Array(params ReadOnlyMemory<byte>[] values)
        => Bytes25Array(values);
    /// <summary>
    /// Encodes a bytes26 array value.
    /// </summary>
    /// <param name="values">The array of 26-byte values to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder Bytes26Array(params ReadOnlyMemory<byte>[] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 26));
    IArrayAbiEncoder IArrayAbiEncoder.Bytes26Array(params ReadOnlyMemory<byte>[] values)
        => Bytes26Array(values);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes26Array(params ReadOnlyMemory<byte>[] values)
        => Bytes26Array(values);
    /// <summary>
    /// Encodes a bytes27 array value.
    /// </summary>
    /// <param name="values">The array of 27-byte values to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder Bytes27Array(params ReadOnlyMemory<byte>[] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 27));
    IArrayAbiEncoder IArrayAbiEncoder.Bytes27Array(params ReadOnlyMemory<byte>[] values)
        => Bytes27Array(values);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes27Array(params ReadOnlyMemory<byte>[] values)
        => Bytes27Array(values);
    /// <summary>
    /// Encodes a bytes28 array value.
    /// </summary>
    /// <param name="values">The array of 28-byte values to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder Bytes28Array(params ReadOnlyMemory<byte>[] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 28));
    IArrayAbiEncoder IArrayAbiEncoder.Bytes28Array(params ReadOnlyMemory<byte>[] values)
        => Bytes28Array(values);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes28Array(params ReadOnlyMemory<byte>[] values)
        => Bytes28Array(values);
    /// <summary>
    /// Encodes a bytes29 array value.
    /// </summary>
    /// <param name="values">The array of 29-byte values to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder Bytes29Array(params ReadOnlyMemory<byte>[] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 29));
    IArrayAbiEncoder IArrayAbiEncoder.Bytes29Array(params ReadOnlyMemory<byte>[] values)
        => Bytes29Array(values);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes29Array(params ReadOnlyMemory<byte>[] values)
        => Bytes29Array(values);
    /// <summary>
    /// Encodes a bytes30 array value.
    /// </summary>
    /// <param name="values">The array of 30-byte values to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder Bytes30Array(params ReadOnlyMemory<byte>[] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 30));
    IArrayAbiEncoder IArrayAbiEncoder.Bytes30Array(params ReadOnlyMemory<byte>[] values)
        => Bytes30Array(values);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes30Array(params ReadOnlyMemory<byte>[] values)
        => Bytes30Array(values);
    /// <summary>
    /// Encodes a bytes31 array value.
    /// </summary>
    /// <param name="values">The array of 31-byte values to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder Bytes31Array(params ReadOnlyMemory<byte>[] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 31));
    IArrayAbiEncoder IArrayAbiEncoder.Bytes31Array(params ReadOnlyMemory<byte>[] values)
        => Bytes31Array(values);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes31Array(params ReadOnlyMemory<byte>[] values)
        => Bytes31Array(values);
    /// <summary>
    /// Encodes a bytes32 array value.
    /// </summary>
    /// <param name="values">The array of 32-byte values to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public AbiEncoder Bytes32Array(params ReadOnlyMemory<byte>[] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 32));
    IArrayAbiEncoder IArrayAbiEncoder.Bytes32Array(params ReadOnlyMemory<byte>[] values)
        => Bytes32Array(values);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes32Array(params ReadOnlyMemory<byte>[] values)
        => Bytes32Array(values);

}
