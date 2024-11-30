using EtherSharp.ABI.Fixed;

namespace EtherSharp.ABI;
public partial class AbiEncoder
{
    public AbiEncoder Bytes8(byte value)
        => AddElement(new FixedType<object>.Byte(value));
    public AbiEncoder Bytes2(params byte[] value)
        => AddElement(new FixedType<object>.Bytes(value, 2));
    public AbiEncoder Bytes3(params byte[] value)
        => AddElement(new FixedType<object>.Bytes(value, 3));
    public AbiEncoder Bytes4(params byte[] value)
        => AddElement(new FixedType<object>.Bytes(value, 4));
    public AbiEncoder Bytes5(params byte[] value)
        => AddElement(new FixedType<object>.Bytes(value, 5));
    public AbiEncoder Bytes6(params byte[] value)
        => AddElement(new FixedType<object>.Bytes(value, 6));
    public AbiEncoder Bytes7(params byte[] value)
        => AddElement(new FixedType<object>.Bytes(value, 7));
    public AbiEncoder Bytes8(params byte[] value)
        => AddElement(new FixedType<object>.Bytes(value, 8));
    public AbiEncoder Bytes9(params byte[] value)
        => AddElement(new FixedType<object>.Bytes(value, 9));
    public AbiEncoder Bytes10(params byte[] value)
        => AddElement(new FixedType<object>.Bytes(value, 10));
    public AbiEncoder Bytes11(params byte[] value)
        => AddElement(new FixedType<object>.Bytes(value, 11));
    public AbiEncoder Bytes12(params byte[] value)
        => AddElement(new FixedType<object>.Bytes(value, 12));
    public AbiEncoder Bytes13(params byte[] value)
        => AddElement(new FixedType<object>.Bytes(value, 13));
    public AbiEncoder Bytes14(params byte[] value)
        => AddElement(new FixedType<object>.Bytes(value, 14));
    public AbiEncoder Bytes15(params byte[] value)
        => AddElement(new FixedType<object>.Bytes(value, 15));
    public AbiEncoder Bytes16(params byte[] value)
        => AddElement(new FixedType<object>.Bytes(value, 16));
    public AbiEncoder Bytes17(params byte[] value)
        => AddElement(new FixedType<object>.Bytes(value, 17));
    public AbiEncoder Bytes18(params byte[] value)
        => AddElement(new FixedType<object>.Bytes(value, 18));
    public AbiEncoder Bytes19(params byte[] value)
        => AddElement(new FixedType<object>.Bytes(value, 19));
    public AbiEncoder Bytes20(params byte[] value)
        => AddElement(new FixedType<object>.Bytes(value, 20));
    public AbiEncoder Bytes21(params byte[] value)
        => AddElement(new FixedType<object>.Bytes(value, 21));
    public AbiEncoder Bytes22(params byte[] value)
        => AddElement(new FixedType<object>.Bytes(value, 22));
    public AbiEncoder Bytes23(params byte[] value)
        => AddElement(new FixedType<object>.Bytes(value, 23));
    public AbiEncoder Bytes24(params byte[] value)
        => AddElement(new FixedType<object>.Bytes(value, 24));
    public AbiEncoder Bytes25(params byte[] value)
        => AddElement(new FixedType<object>.Bytes(value, 25));
    public AbiEncoder Bytes26(params byte[] value)
        => AddElement(new FixedType<object>.Bytes(value, 26));
    public AbiEncoder Bytes27(params byte[] value)
        => AddElement(new FixedType<object>.Bytes(value, 27));
    public AbiEncoder Bytes28(params byte[] value)
        => AddElement(new FixedType<object>.Bytes(value, 28));
    public AbiEncoder Bytes29(params byte[] value)
        => AddElement(new FixedType<object>.Bytes(value, 29));
    public AbiEncoder Bytes30(params byte[] value)
        => AddElement(new FixedType<object>.Bytes(value, 30));
    public AbiEncoder Bytes31(params byte[] value)
        => AddElement(new FixedType<object>.Bytes(value, 31));
    public AbiEncoder Bytes32(params byte[] value)
        => AddElement(new FixedType<object>.Bytes(value, 32));

}