using EtherSharp.ABI.Types;
using EtherSharp.ABI.Encode.Interfaces;

namespace EtherSharp.ABI;
public partial class AbiEncoder
{
    public AbiEncoder Bytes1(byte value)
        => AddElement(new AbiTypes.Byte(value));    
    IFixedTupleEncoder IFixedTupleEncoder.Bytes1(byte value)
        => Bytes1(value);    
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes1(byte value)
        => Bytes1(value);
    public AbiEncoder Bytes2(params byte[] value)
        => AddElement(new AbiTypes.SizedBytes(value, 2));
    IFixedTupleEncoder IFixedTupleEncoder.Bytes2(params byte[] value)
        => Bytes2(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes2(params byte[] value)
        => Bytes2(value);
    public AbiEncoder Bytes3(params byte[] value)
        => AddElement(new AbiTypes.SizedBytes(value, 3));
    IFixedTupleEncoder IFixedTupleEncoder.Bytes3(params byte[] value)
        => Bytes3(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes3(params byte[] value)
        => Bytes3(value);
    public AbiEncoder Bytes4(params byte[] value)
        => AddElement(new AbiTypes.SizedBytes(value, 4));
    IFixedTupleEncoder IFixedTupleEncoder.Bytes4(params byte[] value)
        => Bytes4(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes4(params byte[] value)
        => Bytes4(value);
    public AbiEncoder Bytes5(params byte[] value)
        => AddElement(new AbiTypes.SizedBytes(value, 5));
    IFixedTupleEncoder IFixedTupleEncoder.Bytes5(params byte[] value)
        => Bytes5(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes5(params byte[] value)
        => Bytes5(value);
    public AbiEncoder Bytes6(params byte[] value)
        => AddElement(new AbiTypes.SizedBytes(value, 6));
    IFixedTupleEncoder IFixedTupleEncoder.Bytes6(params byte[] value)
        => Bytes6(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes6(params byte[] value)
        => Bytes6(value);
    public AbiEncoder Bytes7(params byte[] value)
        => AddElement(new AbiTypes.SizedBytes(value, 7));
    IFixedTupleEncoder IFixedTupleEncoder.Bytes7(params byte[] value)
        => Bytes7(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes7(params byte[] value)
        => Bytes7(value);
    public AbiEncoder Bytes8(params byte[] value)
        => AddElement(new AbiTypes.SizedBytes(value, 8));
    IFixedTupleEncoder IFixedTupleEncoder.Bytes8(params byte[] value)
        => Bytes8(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes8(params byte[] value)
        => Bytes8(value);
    public AbiEncoder Bytes9(params byte[] value)
        => AddElement(new AbiTypes.SizedBytes(value, 9));
    IFixedTupleEncoder IFixedTupleEncoder.Bytes9(params byte[] value)
        => Bytes9(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes9(params byte[] value)
        => Bytes9(value);
    public AbiEncoder Bytes10(params byte[] value)
        => AddElement(new AbiTypes.SizedBytes(value, 10));
    IFixedTupleEncoder IFixedTupleEncoder.Bytes10(params byte[] value)
        => Bytes10(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes10(params byte[] value)
        => Bytes10(value);
    public AbiEncoder Bytes11(params byte[] value)
        => AddElement(new AbiTypes.SizedBytes(value, 11));
    IFixedTupleEncoder IFixedTupleEncoder.Bytes11(params byte[] value)
        => Bytes11(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes11(params byte[] value)
        => Bytes11(value);
    public AbiEncoder Bytes12(params byte[] value)
        => AddElement(new AbiTypes.SizedBytes(value, 12));
    IFixedTupleEncoder IFixedTupleEncoder.Bytes12(params byte[] value)
        => Bytes12(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes12(params byte[] value)
        => Bytes12(value);
    public AbiEncoder Bytes13(params byte[] value)
        => AddElement(new AbiTypes.SizedBytes(value, 13));
    IFixedTupleEncoder IFixedTupleEncoder.Bytes13(params byte[] value)
        => Bytes13(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes13(params byte[] value)
        => Bytes13(value);
    public AbiEncoder Bytes14(params byte[] value)
        => AddElement(new AbiTypes.SizedBytes(value, 14));
    IFixedTupleEncoder IFixedTupleEncoder.Bytes14(params byte[] value)
        => Bytes14(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes14(params byte[] value)
        => Bytes14(value);
    public AbiEncoder Bytes15(params byte[] value)
        => AddElement(new AbiTypes.SizedBytes(value, 15));
    IFixedTupleEncoder IFixedTupleEncoder.Bytes15(params byte[] value)
        => Bytes15(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes15(params byte[] value)
        => Bytes15(value);
    public AbiEncoder Bytes16(params byte[] value)
        => AddElement(new AbiTypes.SizedBytes(value, 16));
    IFixedTupleEncoder IFixedTupleEncoder.Bytes16(params byte[] value)
        => Bytes16(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes16(params byte[] value)
        => Bytes16(value);
    public AbiEncoder Bytes17(params byte[] value)
        => AddElement(new AbiTypes.SizedBytes(value, 17));
    IFixedTupleEncoder IFixedTupleEncoder.Bytes17(params byte[] value)
        => Bytes17(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes17(params byte[] value)
        => Bytes17(value);
    public AbiEncoder Bytes18(params byte[] value)
        => AddElement(new AbiTypes.SizedBytes(value, 18));
    IFixedTupleEncoder IFixedTupleEncoder.Bytes18(params byte[] value)
        => Bytes18(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes18(params byte[] value)
        => Bytes18(value);
    public AbiEncoder Bytes19(params byte[] value)
        => AddElement(new AbiTypes.SizedBytes(value, 19));
    IFixedTupleEncoder IFixedTupleEncoder.Bytes19(params byte[] value)
        => Bytes19(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes19(params byte[] value)
        => Bytes19(value);
    public AbiEncoder Bytes20(params byte[] value)
        => AddElement(new AbiTypes.SizedBytes(value, 20));
    IFixedTupleEncoder IFixedTupleEncoder.Bytes20(params byte[] value)
        => Bytes20(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes20(params byte[] value)
        => Bytes20(value);
    public AbiEncoder Bytes21(params byte[] value)
        => AddElement(new AbiTypes.SizedBytes(value, 21));
    IFixedTupleEncoder IFixedTupleEncoder.Bytes21(params byte[] value)
        => Bytes21(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes21(params byte[] value)
        => Bytes21(value);
    public AbiEncoder Bytes22(params byte[] value)
        => AddElement(new AbiTypes.SizedBytes(value, 22));
    IFixedTupleEncoder IFixedTupleEncoder.Bytes22(params byte[] value)
        => Bytes22(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes22(params byte[] value)
        => Bytes22(value);
    public AbiEncoder Bytes23(params byte[] value)
        => AddElement(new AbiTypes.SizedBytes(value, 23));
    IFixedTupleEncoder IFixedTupleEncoder.Bytes23(params byte[] value)
        => Bytes23(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes23(params byte[] value)
        => Bytes23(value);
    public AbiEncoder Bytes24(params byte[] value)
        => AddElement(new AbiTypes.SizedBytes(value, 24));
    IFixedTupleEncoder IFixedTupleEncoder.Bytes24(params byte[] value)
        => Bytes24(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes24(params byte[] value)
        => Bytes24(value);
    public AbiEncoder Bytes25(params byte[] value)
        => AddElement(new AbiTypes.SizedBytes(value, 25));
    IFixedTupleEncoder IFixedTupleEncoder.Bytes25(params byte[] value)
        => Bytes25(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes25(params byte[] value)
        => Bytes25(value);
    public AbiEncoder Bytes26(params byte[] value)
        => AddElement(new AbiTypes.SizedBytes(value, 26));
    IFixedTupleEncoder IFixedTupleEncoder.Bytes26(params byte[] value)
        => Bytes26(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes26(params byte[] value)
        => Bytes26(value);
    public AbiEncoder Bytes27(params byte[] value)
        => AddElement(new AbiTypes.SizedBytes(value, 27));
    IFixedTupleEncoder IFixedTupleEncoder.Bytes27(params byte[] value)
        => Bytes27(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes27(params byte[] value)
        => Bytes27(value);
    public AbiEncoder Bytes28(params byte[] value)
        => AddElement(new AbiTypes.SizedBytes(value, 28));
    IFixedTupleEncoder IFixedTupleEncoder.Bytes28(params byte[] value)
        => Bytes28(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes28(params byte[] value)
        => Bytes28(value);
    public AbiEncoder Bytes29(params byte[] value)
        => AddElement(new AbiTypes.SizedBytes(value, 29));
    IFixedTupleEncoder IFixedTupleEncoder.Bytes29(params byte[] value)
        => Bytes29(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes29(params byte[] value)
        => Bytes29(value);
    public AbiEncoder Bytes30(params byte[] value)
        => AddElement(new AbiTypes.SizedBytes(value, 30));
    IFixedTupleEncoder IFixedTupleEncoder.Bytes30(params byte[] value)
        => Bytes30(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes30(params byte[] value)
        => Bytes30(value);
    public AbiEncoder Bytes31(params byte[] value)
        => AddElement(new AbiTypes.SizedBytes(value, 31));
    IFixedTupleEncoder IFixedTupleEncoder.Bytes31(params byte[] value)
        => Bytes31(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes31(params byte[] value)
        => Bytes31(value);
    public AbiEncoder Bytes32(params byte[] value)
        => AddElement(new AbiTypes.SizedBytes(value, 32));
    IFixedTupleEncoder IFixedTupleEncoder.Bytes32(params byte[] value)
        => Bytes32(value);
    IDynamicTupleEncoder IDynamicTupleEncoder.Bytes32(params byte[] value)
        => Bytes32(value);

}