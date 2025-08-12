using EtherSharp.ABI.Encode.Interfaces;
using EtherSharp.ABI.Types;

namespace EtherSharp.ABI;
public partial class AbiEncoder
{
    public AbiEncoder Bytes1Array(params byte[] value)
        => AddElement(new AbiTypes.SizedBytesArray([value], 1));
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes1Array(params byte[] value)
        => Bytes1Array(value);
    public AbiEncoder Bytes2Array(params byte[][] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 2));
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes2Array(params byte[][] values)
        => Bytes2Array(values);
    public AbiEncoder Bytes3Array(params byte[][] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 3));
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes3Array(params byte[][] values)
        => Bytes3Array(values);
    public AbiEncoder Bytes4Array(params byte[][] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 4));
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes4Array(params byte[][] values)
        => Bytes4Array(values);
    public AbiEncoder Bytes5Array(params byte[][] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 5));
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes5Array(params byte[][] values)
        => Bytes5Array(values);
    public AbiEncoder Bytes6Array(params byte[][] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 6));
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes6Array(params byte[][] values)
        => Bytes6Array(values);
    public AbiEncoder Bytes7Array(params byte[][] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 7));
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes7Array(params byte[][] values)
        => Bytes7Array(values);
    public AbiEncoder Bytes8Array(params byte[][] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 8));
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes8Array(params byte[][] values)
        => Bytes8Array(values);
    public AbiEncoder Bytes9Array(params byte[][] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 9));
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes9Array(params byte[][] values)
        => Bytes9Array(values);
    public AbiEncoder Bytes10Array(params byte[][] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 10));
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes10Array(params byte[][] values)
        => Bytes10Array(values);
    public AbiEncoder Bytes11Array(params byte[][] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 11));
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes11Array(params byte[][] values)
        => Bytes11Array(values);
    public AbiEncoder Bytes12Array(params byte[][] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 12));
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes12Array(params byte[][] values)
        => Bytes12Array(values);
    public AbiEncoder Bytes13Array(params byte[][] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 13));
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes13Array(params byte[][] values)
        => Bytes13Array(values);
    public AbiEncoder Bytes14Array(params byte[][] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 14));
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes14Array(params byte[][] values)
        => Bytes14Array(values);
    public AbiEncoder Bytes15Array(params byte[][] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 15));
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes15Array(params byte[][] values)
        => Bytes15Array(values);
    public AbiEncoder Bytes16Array(params byte[][] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 16));
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes16Array(params byte[][] values)
        => Bytes16Array(values);
    public AbiEncoder Bytes17Array(params byte[][] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 17));
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes17Array(params byte[][] values)
        => Bytes17Array(values);
    public AbiEncoder Bytes18Array(params byte[][] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 18));
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes18Array(params byte[][] values)
        => Bytes18Array(values);
    public AbiEncoder Bytes19Array(params byte[][] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 19));
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes19Array(params byte[][] values)
        => Bytes19Array(values);
    public AbiEncoder Bytes20Array(params byte[][] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 20));
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes20Array(params byte[][] values)
        => Bytes20Array(values);
    public AbiEncoder Bytes21Array(params byte[][] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 21));
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes21Array(params byte[][] values)
        => Bytes21Array(values);
    public AbiEncoder Bytes22Array(params byte[][] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 22));
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes22Array(params byte[][] values)
        => Bytes22Array(values);
    public AbiEncoder Bytes23Array(params byte[][] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 23));
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes23Array(params byte[][] values)
        => Bytes23Array(values);
    public AbiEncoder Bytes24Array(params byte[][] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 24));
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes24Array(params byte[][] values)
        => Bytes24Array(values);
    public AbiEncoder Bytes25Array(params byte[][] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 25));
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes25Array(params byte[][] values)
        => Bytes25Array(values);
    public AbiEncoder Bytes26Array(params byte[][] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 26));
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes26Array(params byte[][] values)
        => Bytes26Array(values);
    public AbiEncoder Bytes27Array(params byte[][] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 27));
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes27Array(params byte[][] values)
        => Bytes27Array(values);
    public AbiEncoder Bytes28Array(params byte[][] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 28));
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes28Array(params byte[][] values)
        => Bytes28Array(values);
    public AbiEncoder Bytes29Array(params byte[][] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 29));
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes29Array(params byte[][] values)
        => Bytes29Array(values);
    public AbiEncoder Bytes30Array(params byte[][] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 30));
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes30Array(params byte[][] values)
        => Bytes30Array(values);
    public AbiEncoder Bytes31Array(params byte[][] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 31));
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes31Array(params byte[][] values)
        => Bytes31Array(values);
    public AbiEncoder Bytes32Array(params byte[][] values)
        => AddElement(new AbiTypes.SizedBytesArray(values, 32));
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes32Array(params byte[][] values)
        => Bytes32Array(values);

}