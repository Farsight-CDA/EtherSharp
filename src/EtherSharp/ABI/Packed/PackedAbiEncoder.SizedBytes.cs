using EtherSharp.ABI.Types;
using EtherSharp.ABI.Encode.Interfaces;

namespace EtherSharp.ABI.Packed;
public partial class PackedAbiEncoder
{
    public PackedAbiEncoder Bytes1(byte value)
        => AddElement(new AbiTypes.Byte(value));    
    public PackedAbiEncoder Bytes2(params byte[] value)
        => AddElement(new AbiTypes.SizedBytes(value, 2));
    public PackedAbiEncoder Bytes3(params byte[] value)
        => AddElement(new AbiTypes.SizedBytes(value, 3));
    public PackedAbiEncoder Bytes4(params byte[] value)
        => AddElement(new AbiTypes.SizedBytes(value, 4));
    public PackedAbiEncoder Bytes5(params byte[] value)
        => AddElement(new AbiTypes.SizedBytes(value, 5));
    public PackedAbiEncoder Bytes6(params byte[] value)
        => AddElement(new AbiTypes.SizedBytes(value, 6));
    public PackedAbiEncoder Bytes7(params byte[] value)
        => AddElement(new AbiTypes.SizedBytes(value, 7));
    public PackedAbiEncoder Bytes8(params byte[] value)
        => AddElement(new AbiTypes.SizedBytes(value, 8));
    public PackedAbiEncoder Bytes9(params byte[] value)
        => AddElement(new AbiTypes.SizedBytes(value, 9));
    public PackedAbiEncoder Bytes10(params byte[] value)
        => AddElement(new AbiTypes.SizedBytes(value, 10));
    public PackedAbiEncoder Bytes11(params byte[] value)
        => AddElement(new AbiTypes.SizedBytes(value, 11));
    public PackedAbiEncoder Bytes12(params byte[] value)
        => AddElement(new AbiTypes.SizedBytes(value, 12));
    public PackedAbiEncoder Bytes13(params byte[] value)
        => AddElement(new AbiTypes.SizedBytes(value, 13));
    public PackedAbiEncoder Bytes14(params byte[] value)
        => AddElement(new AbiTypes.SizedBytes(value, 14));
    public PackedAbiEncoder Bytes15(params byte[] value)
        => AddElement(new AbiTypes.SizedBytes(value, 15));
    public PackedAbiEncoder Bytes16(params byte[] value)
        => AddElement(new AbiTypes.SizedBytes(value, 16));
    public PackedAbiEncoder Bytes17(params byte[] value)
        => AddElement(new AbiTypes.SizedBytes(value, 17));
    public PackedAbiEncoder Bytes18(params byte[] value)
        => AddElement(new AbiTypes.SizedBytes(value, 18));
    public PackedAbiEncoder Bytes19(params byte[] value)
        => AddElement(new AbiTypes.SizedBytes(value, 19));
    public PackedAbiEncoder Bytes20(params byte[] value)
        => AddElement(new AbiTypes.SizedBytes(value, 20));
    public PackedAbiEncoder Bytes21(params byte[] value)
        => AddElement(new AbiTypes.SizedBytes(value, 21));
    public PackedAbiEncoder Bytes22(params byte[] value)
        => AddElement(new AbiTypes.SizedBytes(value, 22));
    public PackedAbiEncoder Bytes23(params byte[] value)
        => AddElement(new AbiTypes.SizedBytes(value, 23));
    public PackedAbiEncoder Bytes24(params byte[] value)
        => AddElement(new AbiTypes.SizedBytes(value, 24));
    public PackedAbiEncoder Bytes25(params byte[] value)
        => AddElement(new AbiTypes.SizedBytes(value, 25));
    public PackedAbiEncoder Bytes26(params byte[] value)
        => AddElement(new AbiTypes.SizedBytes(value, 26));
    public PackedAbiEncoder Bytes27(params byte[] value)
        => AddElement(new AbiTypes.SizedBytes(value, 27));
    public PackedAbiEncoder Bytes28(params byte[] value)
        => AddElement(new AbiTypes.SizedBytes(value, 28));
    public PackedAbiEncoder Bytes29(params byte[] value)
        => AddElement(new AbiTypes.SizedBytes(value, 29));
    public PackedAbiEncoder Bytes30(params byte[] value)
        => AddElement(new AbiTypes.SizedBytes(value, 30));
    public PackedAbiEncoder Bytes31(params byte[] value)
        => AddElement(new AbiTypes.SizedBytes(value, 31));
    public PackedAbiEncoder Bytes32(params byte[] value)
        => AddElement(new AbiTypes.SizedBytes(value, 32));

}