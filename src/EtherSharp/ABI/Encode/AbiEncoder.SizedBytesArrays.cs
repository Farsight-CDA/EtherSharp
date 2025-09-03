using EtherSharp.ABI.Encode.Interfaces;
using EtherSharp.ABI.Types;

namespace EtherSharp.ABI;
public partial class AbiEncoder
{
    public AbiEncoder Bytes1Array(params byte[] value)
        => AddElement(new AbiTypes.SizedBytesArray([value], 1));
    IArrayAbiEncoder IArrayAbiEncoder.Bytes1Array(params byte[] value)
        => Bytes1Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes1Array(params byte[] value)
        => Bytes1Array(value);
    public AbiEncoder Bytes2Array(params byte[][] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 2));
    IArrayAbiEncoder IArrayAbiEncoder.Bytes2Array(params byte[][] value)
        => Bytes2Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes2Array(params byte[][] values)
        => Bytes2Array(values);
    public AbiEncoder Bytes3Array(params byte[][] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 3));
    IArrayAbiEncoder IArrayAbiEncoder.Bytes3Array(params byte[][] value)
        => Bytes3Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes3Array(params byte[][] values)
        => Bytes3Array(values);
    public AbiEncoder Bytes4Array(params byte[][] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 4));
    IArrayAbiEncoder IArrayAbiEncoder.Bytes4Array(params byte[][] value)
        => Bytes4Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes4Array(params byte[][] values)
        => Bytes4Array(values);
    public AbiEncoder Bytes5Array(params byte[][] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 5));
    IArrayAbiEncoder IArrayAbiEncoder.Bytes5Array(params byte[][] value)
        => Bytes5Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes5Array(params byte[][] values)
        => Bytes5Array(values);
    public AbiEncoder Bytes6Array(params byte[][] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 6));
    IArrayAbiEncoder IArrayAbiEncoder.Bytes6Array(params byte[][] value)
        => Bytes6Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes6Array(params byte[][] values)
        => Bytes6Array(values);
    public AbiEncoder Bytes7Array(params byte[][] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 7));
    IArrayAbiEncoder IArrayAbiEncoder.Bytes7Array(params byte[][] value)
        => Bytes7Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes7Array(params byte[][] values)
        => Bytes7Array(values);
    public AbiEncoder Bytes8Array(params byte[][] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 8));
    IArrayAbiEncoder IArrayAbiEncoder.Bytes8Array(params byte[][] value)
        => Bytes8Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes8Array(params byte[][] values)
        => Bytes8Array(values);
    public AbiEncoder Bytes9Array(params byte[][] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 9));
    IArrayAbiEncoder IArrayAbiEncoder.Bytes9Array(params byte[][] value)
        => Bytes9Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes9Array(params byte[][] values)
        => Bytes9Array(values);
    public AbiEncoder Bytes10Array(params byte[][] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 10));
    IArrayAbiEncoder IArrayAbiEncoder.Bytes10Array(params byte[][] value)
        => Bytes10Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes10Array(params byte[][] values)
        => Bytes10Array(values);
    public AbiEncoder Bytes11Array(params byte[][] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 11));
    IArrayAbiEncoder IArrayAbiEncoder.Bytes11Array(params byte[][] value)
        => Bytes11Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes11Array(params byte[][] values)
        => Bytes11Array(values);
    public AbiEncoder Bytes12Array(params byte[][] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 12));
    IArrayAbiEncoder IArrayAbiEncoder.Bytes12Array(params byte[][] value)
        => Bytes12Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes12Array(params byte[][] values)
        => Bytes12Array(values);
    public AbiEncoder Bytes13Array(params byte[][] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 13));
    IArrayAbiEncoder IArrayAbiEncoder.Bytes13Array(params byte[][] value)
        => Bytes13Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes13Array(params byte[][] values)
        => Bytes13Array(values);
    public AbiEncoder Bytes14Array(params byte[][] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 14));
    IArrayAbiEncoder IArrayAbiEncoder.Bytes14Array(params byte[][] value)
        => Bytes14Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes14Array(params byte[][] values)
        => Bytes14Array(values);
    public AbiEncoder Bytes15Array(params byte[][] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 15));
    IArrayAbiEncoder IArrayAbiEncoder.Bytes15Array(params byte[][] value)
        => Bytes15Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes15Array(params byte[][] values)
        => Bytes15Array(values);
    public AbiEncoder Bytes16Array(params byte[][] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 16));
    IArrayAbiEncoder IArrayAbiEncoder.Bytes16Array(params byte[][] value)
        => Bytes16Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes16Array(params byte[][] values)
        => Bytes16Array(values);
    public AbiEncoder Bytes17Array(params byte[][] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 17));
    IArrayAbiEncoder IArrayAbiEncoder.Bytes17Array(params byte[][] value)
        => Bytes17Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes17Array(params byte[][] values)
        => Bytes17Array(values);
    public AbiEncoder Bytes18Array(params byte[][] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 18));
    IArrayAbiEncoder IArrayAbiEncoder.Bytes18Array(params byte[][] value)
        => Bytes18Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes18Array(params byte[][] values)
        => Bytes18Array(values);
    public AbiEncoder Bytes19Array(params byte[][] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 19));
    IArrayAbiEncoder IArrayAbiEncoder.Bytes19Array(params byte[][] value)
        => Bytes19Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes19Array(params byte[][] values)
        => Bytes19Array(values);
    public AbiEncoder Bytes20Array(params byte[][] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 20));
    IArrayAbiEncoder IArrayAbiEncoder.Bytes20Array(params byte[][] value)
        => Bytes20Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes20Array(params byte[][] values)
        => Bytes20Array(values);
    public AbiEncoder Bytes21Array(params byte[][] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 21));
    IArrayAbiEncoder IArrayAbiEncoder.Bytes21Array(params byte[][] value)
        => Bytes21Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes21Array(params byte[][] values)
        => Bytes21Array(values);
    public AbiEncoder Bytes22Array(params byte[][] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 22));
    IArrayAbiEncoder IArrayAbiEncoder.Bytes22Array(params byte[][] value)
        => Bytes22Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes22Array(params byte[][] values)
        => Bytes22Array(values);
    public AbiEncoder Bytes23Array(params byte[][] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 23));
    IArrayAbiEncoder IArrayAbiEncoder.Bytes23Array(params byte[][] value)
        => Bytes23Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes23Array(params byte[][] values)
        => Bytes23Array(values);
    public AbiEncoder Bytes24Array(params byte[][] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 24));
    IArrayAbiEncoder IArrayAbiEncoder.Bytes24Array(params byte[][] value)
        => Bytes24Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes24Array(params byte[][] values)
        => Bytes24Array(values);
    public AbiEncoder Bytes25Array(params byte[][] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 25));
    IArrayAbiEncoder IArrayAbiEncoder.Bytes25Array(params byte[][] value)
        => Bytes25Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes25Array(params byte[][] values)
        => Bytes25Array(values);
    public AbiEncoder Bytes26Array(params byte[][] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 26));
    IArrayAbiEncoder IArrayAbiEncoder.Bytes26Array(params byte[][] value)
        => Bytes26Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes26Array(params byte[][] values)
        => Bytes26Array(values);
    public AbiEncoder Bytes27Array(params byte[][] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 27));
    IArrayAbiEncoder IArrayAbiEncoder.Bytes27Array(params byte[][] value)
        => Bytes27Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes27Array(params byte[][] values)
        => Bytes27Array(values);
    public AbiEncoder Bytes28Array(params byte[][] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 28));
    IArrayAbiEncoder IArrayAbiEncoder.Bytes28Array(params byte[][] value)
        => Bytes28Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes28Array(params byte[][] values)
        => Bytes28Array(values);
    public AbiEncoder Bytes29Array(params byte[][] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 29));
    IArrayAbiEncoder IArrayAbiEncoder.Bytes29Array(params byte[][] value)
        => Bytes29Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes29Array(params byte[][] values)
        => Bytes29Array(values);
    public AbiEncoder Bytes30Array(params byte[][] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 30));
    IArrayAbiEncoder IArrayAbiEncoder.Bytes30Array(params byte[][] value)
        => Bytes30Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes30Array(params byte[][] values)
        => Bytes30Array(values);
    public AbiEncoder Bytes31Array(params byte[][] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 31));
    IArrayAbiEncoder IArrayAbiEncoder.Bytes31Array(params byte[][] value)
        => Bytes31Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes31Array(params byte[][] values)
        => Bytes31Array(values);
    public AbiEncoder Bytes32Array(params byte[][] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 32));
    IArrayAbiEncoder IArrayAbiEncoder.Bytes32Array(params byte[][] value)
        => Bytes32Array(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes32Array(params byte[][] values)
        => Bytes32Array(values);

}