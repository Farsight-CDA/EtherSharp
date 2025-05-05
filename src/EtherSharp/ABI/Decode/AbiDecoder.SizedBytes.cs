using EtherSharp.ABI.Types;
using EtherSharp.ABI.Decode.Interfaces;

namespace EtherSharp.ABI;
public partial class AbiDecoder
{
    public AbiDecoder Bytes1(out byte value)
    {
        value = AbiTypes.Byte.Decode(CurrentSlot);
        return this;
    }
    byte IFixedTupleDecoder.Bytes1()
    {
        _ = Bytes1(out byte value);
        return value;
    }
    byte IDynamicTupleDecoder.Bytes1()
    {
        _ = Bytes1(out byte value);
        return value;
    }
    public AbiDecoder Bytes2(out ReadOnlySpan<byte> value)
    {
        value = AbiTypes.SizedBytes.Decode(CurrentSlot, 2);
        return this;
    }
    ReadOnlySpan<byte> IFixedTupleDecoder.Bytes2()
    {
        _ = Bytes2(out var value);
        return value;
    }
    ReadOnlySpan<byte> IDynamicTupleDecoder.Bytes2()
    {
        _ = Bytes2(out var value);
        return value;
    }
    public AbiDecoder Bytes3(out ReadOnlySpan<byte> value)
    {
        value = AbiTypes.SizedBytes.Decode(CurrentSlot, 3);
        return this;
    }
    ReadOnlySpan<byte> IFixedTupleDecoder.Bytes3()
    {
        _ = Bytes3(out var value);
        return value;
    }
    ReadOnlySpan<byte> IDynamicTupleDecoder.Bytes3()
    {
        _ = Bytes3(out var value);
        return value;
    }
    public AbiDecoder Bytes4(out ReadOnlySpan<byte> value)
    {
        value = AbiTypes.SizedBytes.Decode(CurrentSlot, 4);
        return this;
    }
    ReadOnlySpan<byte> IFixedTupleDecoder.Bytes4()
    {
        _ = Bytes4(out var value);
        return value;
    }
    ReadOnlySpan<byte> IDynamicTupleDecoder.Bytes4()
    {
        _ = Bytes4(out var value);
        return value;
    }
    public AbiDecoder Bytes5(out ReadOnlySpan<byte> value)
    {
        value = AbiTypes.SizedBytes.Decode(CurrentSlot, 5);
        return this;
    }
    ReadOnlySpan<byte> IFixedTupleDecoder.Bytes5()
    {
        _ = Bytes5(out var value);
        return value;
    }
    ReadOnlySpan<byte> IDynamicTupleDecoder.Bytes5()
    {
        _ = Bytes5(out var value);
        return value;
    }
    public AbiDecoder Bytes6(out ReadOnlySpan<byte> value)
    {
        value = AbiTypes.SizedBytes.Decode(CurrentSlot, 6);
        return this;
    }
    ReadOnlySpan<byte> IFixedTupleDecoder.Bytes6()
    {
        _ = Bytes6(out var value);
        return value;
    }
    ReadOnlySpan<byte> IDynamicTupleDecoder.Bytes6()
    {
        _ = Bytes6(out var value);
        return value;
    }
    public AbiDecoder Bytes7(out ReadOnlySpan<byte> value)
    {
        value = AbiTypes.SizedBytes.Decode(CurrentSlot, 7);
        return this;
    }
    ReadOnlySpan<byte> IFixedTupleDecoder.Bytes7()
    {
        _ = Bytes7(out var value);
        return value;
    }
    ReadOnlySpan<byte> IDynamicTupleDecoder.Bytes7()
    {
        _ = Bytes7(out var value);
        return value;
    }
    public AbiDecoder Bytes8(out ReadOnlySpan<byte> value)
    {
        value = AbiTypes.SizedBytes.Decode(CurrentSlot, 8);
        return this;
    }
    ReadOnlySpan<byte> IFixedTupleDecoder.Bytes8()
    {
        _ = Bytes8(out var value);
        return value;
    }
    ReadOnlySpan<byte> IDynamicTupleDecoder.Bytes8()
    {
        _ = Bytes8(out var value);
        return value;
    }
    public AbiDecoder Bytes9(out ReadOnlySpan<byte> value)
    {
        value = AbiTypes.SizedBytes.Decode(CurrentSlot, 9);
        return this;
    }
    ReadOnlySpan<byte> IFixedTupleDecoder.Bytes9()
    {
        _ = Bytes9(out var value);
        return value;
    }
    ReadOnlySpan<byte> IDynamicTupleDecoder.Bytes9()
    {
        _ = Bytes9(out var value);
        return value;
    }
    public AbiDecoder Bytes10(out ReadOnlySpan<byte> value)
    {
        value = AbiTypes.SizedBytes.Decode(CurrentSlot, 10);
        return this;
    }
    ReadOnlySpan<byte> IFixedTupleDecoder.Bytes10()
    {
        _ = Bytes10(out var value);
        return value;
    }
    ReadOnlySpan<byte> IDynamicTupleDecoder.Bytes10()
    {
        _ = Bytes10(out var value);
        return value;
    }
    public AbiDecoder Bytes11(out ReadOnlySpan<byte> value)
    {
        value = AbiTypes.SizedBytes.Decode(CurrentSlot, 11);
        return this;
    }
    ReadOnlySpan<byte> IFixedTupleDecoder.Bytes11()
    {
        _ = Bytes11(out var value);
        return value;
    }
    ReadOnlySpan<byte> IDynamicTupleDecoder.Bytes11()
    {
        _ = Bytes11(out var value);
        return value;
    }
    public AbiDecoder Bytes12(out ReadOnlySpan<byte> value)
    {
        value = AbiTypes.SizedBytes.Decode(CurrentSlot, 12);
        return this;
    }
    ReadOnlySpan<byte> IFixedTupleDecoder.Bytes12()
    {
        _ = Bytes12(out var value);
        return value;
    }
    ReadOnlySpan<byte> IDynamicTupleDecoder.Bytes12()
    {
        _ = Bytes12(out var value);
        return value;
    }
    public AbiDecoder Bytes13(out ReadOnlySpan<byte> value)
    {
        value = AbiTypes.SizedBytes.Decode(CurrentSlot, 13);
        return this;
    }
    ReadOnlySpan<byte> IFixedTupleDecoder.Bytes13()
    {
        _ = Bytes13(out var value);
        return value;
    }
    ReadOnlySpan<byte> IDynamicTupleDecoder.Bytes13()
    {
        _ = Bytes13(out var value);
        return value;
    }
    public AbiDecoder Bytes14(out ReadOnlySpan<byte> value)
    {
        value = AbiTypes.SizedBytes.Decode(CurrentSlot, 14);
        return this;
    }
    ReadOnlySpan<byte> IFixedTupleDecoder.Bytes14()
    {
        _ = Bytes14(out var value);
        return value;
    }
    ReadOnlySpan<byte> IDynamicTupleDecoder.Bytes14()
    {
        _ = Bytes14(out var value);
        return value;
    }
    public AbiDecoder Bytes15(out ReadOnlySpan<byte> value)
    {
        value = AbiTypes.SizedBytes.Decode(CurrentSlot, 15);
        return this;
    }
    ReadOnlySpan<byte> IFixedTupleDecoder.Bytes15()
    {
        _ = Bytes15(out var value);
        return value;
    }
    ReadOnlySpan<byte> IDynamicTupleDecoder.Bytes15()
    {
        _ = Bytes15(out var value);
        return value;
    }
    public AbiDecoder Bytes16(out ReadOnlySpan<byte> value)
    {
        value = AbiTypes.SizedBytes.Decode(CurrentSlot, 16);
        return this;
    }
    ReadOnlySpan<byte> IFixedTupleDecoder.Bytes16()
    {
        _ = Bytes16(out var value);
        return value;
    }
    ReadOnlySpan<byte> IDynamicTupleDecoder.Bytes16()
    {
        _ = Bytes16(out var value);
        return value;
    }
    public AbiDecoder Bytes17(out ReadOnlySpan<byte> value)
    {
        value = AbiTypes.SizedBytes.Decode(CurrentSlot, 17);
        return this;
    }
    ReadOnlySpan<byte> IFixedTupleDecoder.Bytes17()
    {
        _ = Bytes17(out var value);
        return value;
    }
    ReadOnlySpan<byte> IDynamicTupleDecoder.Bytes17()
    {
        _ = Bytes17(out var value);
        return value;
    }
    public AbiDecoder Bytes18(out ReadOnlySpan<byte> value)
    {
        value = AbiTypes.SizedBytes.Decode(CurrentSlot, 18);
        return this;
    }
    ReadOnlySpan<byte> IFixedTupleDecoder.Bytes18()
    {
        _ = Bytes18(out var value);
        return value;
    }
    ReadOnlySpan<byte> IDynamicTupleDecoder.Bytes18()
    {
        _ = Bytes18(out var value);
        return value;
    }
    public AbiDecoder Bytes19(out ReadOnlySpan<byte> value)
    {
        value = AbiTypes.SizedBytes.Decode(CurrentSlot, 19);
        return this;
    }
    ReadOnlySpan<byte> IFixedTupleDecoder.Bytes19()
    {
        _ = Bytes19(out var value);
        return value;
    }
    ReadOnlySpan<byte> IDynamicTupleDecoder.Bytes19()
    {
        _ = Bytes19(out var value);
        return value;
    }
    public AbiDecoder Bytes20(out ReadOnlySpan<byte> value)
    {
        value = AbiTypes.SizedBytes.Decode(CurrentSlot, 20);
        return this;
    }
    ReadOnlySpan<byte> IFixedTupleDecoder.Bytes20()
    {
        _ = Bytes20(out var value);
        return value;
    }
    ReadOnlySpan<byte> IDynamicTupleDecoder.Bytes20()
    {
        _ = Bytes20(out var value);
        return value;
    }
    public AbiDecoder Bytes21(out ReadOnlySpan<byte> value)
    {
        value = AbiTypes.SizedBytes.Decode(CurrentSlot, 21);
        return this;
    }
    ReadOnlySpan<byte> IFixedTupleDecoder.Bytes21()
    {
        _ = Bytes21(out var value);
        return value;
    }
    ReadOnlySpan<byte> IDynamicTupleDecoder.Bytes21()
    {
        _ = Bytes21(out var value);
        return value;
    }
    public AbiDecoder Bytes22(out ReadOnlySpan<byte> value)
    {
        value = AbiTypes.SizedBytes.Decode(CurrentSlot, 22);
        return this;
    }
    ReadOnlySpan<byte> IFixedTupleDecoder.Bytes22()
    {
        _ = Bytes22(out var value);
        return value;
    }
    ReadOnlySpan<byte> IDynamicTupleDecoder.Bytes22()
    {
        _ = Bytes22(out var value);
        return value;
    }
    public AbiDecoder Bytes23(out ReadOnlySpan<byte> value)
    {
        value = AbiTypes.SizedBytes.Decode(CurrentSlot, 23);
        return this;
    }
    ReadOnlySpan<byte> IFixedTupleDecoder.Bytes23()
    {
        _ = Bytes23(out var value);
        return value;
    }
    ReadOnlySpan<byte> IDynamicTupleDecoder.Bytes23()
    {
        _ = Bytes23(out var value);
        return value;
    }
    public AbiDecoder Bytes24(out ReadOnlySpan<byte> value)
    {
        value = AbiTypes.SizedBytes.Decode(CurrentSlot, 24);
        return this;
    }
    ReadOnlySpan<byte> IFixedTupleDecoder.Bytes24()
    {
        _ = Bytes24(out var value);
        return value;
    }
    ReadOnlySpan<byte> IDynamicTupleDecoder.Bytes24()
    {
        _ = Bytes24(out var value);
        return value;
    }
    public AbiDecoder Bytes25(out ReadOnlySpan<byte> value)
    {
        value = AbiTypes.SizedBytes.Decode(CurrentSlot, 25);
        return this;
    }
    ReadOnlySpan<byte> IFixedTupleDecoder.Bytes25()
    {
        _ = Bytes25(out var value);
        return value;
    }
    ReadOnlySpan<byte> IDynamicTupleDecoder.Bytes25()
    {
        _ = Bytes25(out var value);
        return value;
    }
    public AbiDecoder Bytes26(out ReadOnlySpan<byte> value)
    {
        value = AbiTypes.SizedBytes.Decode(CurrentSlot, 26);
        return this;
    }
    ReadOnlySpan<byte> IFixedTupleDecoder.Bytes26()
    {
        _ = Bytes26(out var value);
        return value;
    }
    ReadOnlySpan<byte> IDynamicTupleDecoder.Bytes26()
    {
        _ = Bytes26(out var value);
        return value;
    }
    public AbiDecoder Bytes27(out ReadOnlySpan<byte> value)
    {
        value = AbiTypes.SizedBytes.Decode(CurrentSlot, 27);
        return this;
    }
    ReadOnlySpan<byte> IFixedTupleDecoder.Bytes27()
    {
        _ = Bytes27(out var value);
        return value;
    }
    ReadOnlySpan<byte> IDynamicTupleDecoder.Bytes27()
    {
        _ = Bytes27(out var value);
        return value;
    }
    public AbiDecoder Bytes28(out ReadOnlySpan<byte> value)
    {
        value = AbiTypes.SizedBytes.Decode(CurrentSlot, 28);
        return this;
    }
    ReadOnlySpan<byte> IFixedTupleDecoder.Bytes28()
    {
        _ = Bytes28(out var value);
        return value;
    }
    ReadOnlySpan<byte> IDynamicTupleDecoder.Bytes28()
    {
        _ = Bytes28(out var value);
        return value;
    }
    public AbiDecoder Bytes29(out ReadOnlySpan<byte> value)
    {
        value = AbiTypes.SizedBytes.Decode(CurrentSlot, 29);
        return this;
    }
    ReadOnlySpan<byte> IFixedTupleDecoder.Bytes29()
    {
        _ = Bytes29(out var value);
        return value;
    }
    ReadOnlySpan<byte> IDynamicTupleDecoder.Bytes29()
    {
        _ = Bytes29(out var value);
        return value;
    }
    public AbiDecoder Bytes30(out ReadOnlySpan<byte> value)
    {
        value = AbiTypes.SizedBytes.Decode(CurrentSlot, 30);
        return this;
    }
    ReadOnlySpan<byte> IFixedTupleDecoder.Bytes30()
    {
        _ = Bytes30(out var value);
        return value;
    }
    ReadOnlySpan<byte> IDynamicTupleDecoder.Bytes30()
    {
        _ = Bytes30(out var value);
        return value;
    }
    public AbiDecoder Bytes31(out ReadOnlySpan<byte> value)
    {
        value = AbiTypes.SizedBytes.Decode(CurrentSlot, 31);
        return this;
    }
    ReadOnlySpan<byte> IFixedTupleDecoder.Bytes31()
    {
        _ = Bytes31(out var value);
        return value;
    }
    ReadOnlySpan<byte> IDynamicTupleDecoder.Bytes31()
    {
        _ = Bytes31(out var value);
        return value;
    }
    public AbiDecoder Bytes32(out ReadOnlySpan<byte> value)
    {
        value = AbiTypes.SizedBytes.Decode(CurrentSlot, 32);
        return this;
    }
    ReadOnlySpan<byte> IFixedTupleDecoder.Bytes32()
    {
        _ = Bytes32(out var value);
        return value;
    }
    ReadOnlySpan<byte> IDynamicTupleDecoder.Bytes32()
    {
        _ = Bytes32(out var value);
        return value;
    }
}