using EtherSharp.ABI.Types;
using EtherSharp.ABI.Decode.Interfaces;

namespace EtherSharp.ABI;
public partial class AbiDecoder
{
    public byte Bytes1()
    {
        var result = AbiTypes.Byte.Decode(CurrentSlot);
        ConsumeBytes();
        return result;
    }
    byte IFixedTupleDecoder.Bytes1()
        => Bytes1();
    byte IDynamicTupleDecoder.Bytes1()
        => Bytes1();

    public ReadOnlySpan<byte> Bytes2()
    {
        var result = AbiTypes.SizedBytes.Decode(CurrentSlot, 2);
        ConsumeBytes();
        return result;
    }
    ReadOnlySpan<byte> IFixedTupleDecoder.Bytes2()
        => Bytes2();
    ReadOnlySpan<byte> IDynamicTupleDecoder.Bytes2()
        => Bytes2();
    public ReadOnlySpan<byte> Bytes3()
    {
        var result = AbiTypes.SizedBytes.Decode(CurrentSlot, 3);
        ConsumeBytes();
        return result;
    }
    ReadOnlySpan<byte> IFixedTupleDecoder.Bytes3()
        => Bytes3();
    ReadOnlySpan<byte> IDynamicTupleDecoder.Bytes3()
        => Bytes3();
    public ReadOnlySpan<byte> Bytes4()
    {
        var result = AbiTypes.SizedBytes.Decode(CurrentSlot, 4);
        ConsumeBytes();
        return result;
    }
    ReadOnlySpan<byte> IFixedTupleDecoder.Bytes4()
        => Bytes4();
    ReadOnlySpan<byte> IDynamicTupleDecoder.Bytes4()
        => Bytes4();
    public ReadOnlySpan<byte> Bytes5()
    {
        var result = AbiTypes.SizedBytes.Decode(CurrentSlot, 5);
        ConsumeBytes();
        return result;
    }
    ReadOnlySpan<byte> IFixedTupleDecoder.Bytes5()
        => Bytes5();
    ReadOnlySpan<byte> IDynamicTupleDecoder.Bytes5()
        => Bytes5();
    public ReadOnlySpan<byte> Bytes6()
    {
        var result = AbiTypes.SizedBytes.Decode(CurrentSlot, 6);
        ConsumeBytes();
        return result;
    }
    ReadOnlySpan<byte> IFixedTupleDecoder.Bytes6()
        => Bytes6();
    ReadOnlySpan<byte> IDynamicTupleDecoder.Bytes6()
        => Bytes6();
    public ReadOnlySpan<byte> Bytes7()
    {
        var result = AbiTypes.SizedBytes.Decode(CurrentSlot, 7);
        ConsumeBytes();
        return result;
    }
    ReadOnlySpan<byte> IFixedTupleDecoder.Bytes7()
        => Bytes7();
    ReadOnlySpan<byte> IDynamicTupleDecoder.Bytes7()
        => Bytes7();
    public ReadOnlySpan<byte> Bytes8()
    {
        var result = AbiTypes.SizedBytes.Decode(CurrentSlot, 8);
        ConsumeBytes();
        return result;
    }
    ReadOnlySpan<byte> IFixedTupleDecoder.Bytes8()
        => Bytes8();
    ReadOnlySpan<byte> IDynamicTupleDecoder.Bytes8()
        => Bytes8();
    public ReadOnlySpan<byte> Bytes9()
    {
        var result = AbiTypes.SizedBytes.Decode(CurrentSlot, 9);
        ConsumeBytes();
        return result;
    }
    ReadOnlySpan<byte> IFixedTupleDecoder.Bytes9()
        => Bytes9();
    ReadOnlySpan<byte> IDynamicTupleDecoder.Bytes9()
        => Bytes9();
    public ReadOnlySpan<byte> Bytes10()
    {
        var result = AbiTypes.SizedBytes.Decode(CurrentSlot, 10);
        ConsumeBytes();
        return result;
    }
    ReadOnlySpan<byte> IFixedTupleDecoder.Bytes10()
        => Bytes10();
    ReadOnlySpan<byte> IDynamicTupleDecoder.Bytes10()
        => Bytes10();
    public ReadOnlySpan<byte> Bytes11()
    {
        var result = AbiTypes.SizedBytes.Decode(CurrentSlot, 11);
        ConsumeBytes();
        return result;
    }
    ReadOnlySpan<byte> IFixedTupleDecoder.Bytes11()
        => Bytes11();
    ReadOnlySpan<byte> IDynamicTupleDecoder.Bytes11()
        => Bytes11();
    public ReadOnlySpan<byte> Bytes12()
    {
        var result = AbiTypes.SizedBytes.Decode(CurrentSlot, 12);
        ConsumeBytes();
        return result;
    }
    ReadOnlySpan<byte> IFixedTupleDecoder.Bytes12()
        => Bytes12();
    ReadOnlySpan<byte> IDynamicTupleDecoder.Bytes12()
        => Bytes12();
    public ReadOnlySpan<byte> Bytes13()
    {
        var result = AbiTypes.SizedBytes.Decode(CurrentSlot, 13);
        ConsumeBytes();
        return result;
    }
    ReadOnlySpan<byte> IFixedTupleDecoder.Bytes13()
        => Bytes13();
    ReadOnlySpan<byte> IDynamicTupleDecoder.Bytes13()
        => Bytes13();
    public ReadOnlySpan<byte> Bytes14()
    {
        var result = AbiTypes.SizedBytes.Decode(CurrentSlot, 14);
        ConsumeBytes();
        return result;
    }
    ReadOnlySpan<byte> IFixedTupleDecoder.Bytes14()
        => Bytes14();
    ReadOnlySpan<byte> IDynamicTupleDecoder.Bytes14()
        => Bytes14();
    public ReadOnlySpan<byte> Bytes15()
    {
        var result = AbiTypes.SizedBytes.Decode(CurrentSlot, 15);
        ConsumeBytes();
        return result;
    }
    ReadOnlySpan<byte> IFixedTupleDecoder.Bytes15()
        => Bytes15();
    ReadOnlySpan<byte> IDynamicTupleDecoder.Bytes15()
        => Bytes15();
    public ReadOnlySpan<byte> Bytes16()
    {
        var result = AbiTypes.SizedBytes.Decode(CurrentSlot, 16);
        ConsumeBytes();
        return result;
    }
    ReadOnlySpan<byte> IFixedTupleDecoder.Bytes16()
        => Bytes16();
    ReadOnlySpan<byte> IDynamicTupleDecoder.Bytes16()
        => Bytes16();
    public ReadOnlySpan<byte> Bytes17()
    {
        var result = AbiTypes.SizedBytes.Decode(CurrentSlot, 17);
        ConsumeBytes();
        return result;
    }
    ReadOnlySpan<byte> IFixedTupleDecoder.Bytes17()
        => Bytes17();
    ReadOnlySpan<byte> IDynamicTupleDecoder.Bytes17()
        => Bytes17();
    public ReadOnlySpan<byte> Bytes18()
    {
        var result = AbiTypes.SizedBytes.Decode(CurrentSlot, 18);
        ConsumeBytes();
        return result;
    }
    ReadOnlySpan<byte> IFixedTupleDecoder.Bytes18()
        => Bytes18();
    ReadOnlySpan<byte> IDynamicTupleDecoder.Bytes18()
        => Bytes18();
    public ReadOnlySpan<byte> Bytes19()
    {
        var result = AbiTypes.SizedBytes.Decode(CurrentSlot, 19);
        ConsumeBytes();
        return result;
    }
    ReadOnlySpan<byte> IFixedTupleDecoder.Bytes19()
        => Bytes19();
    ReadOnlySpan<byte> IDynamicTupleDecoder.Bytes19()
        => Bytes19();
    public ReadOnlySpan<byte> Bytes20()
    {
        var result = AbiTypes.SizedBytes.Decode(CurrentSlot, 20);
        ConsumeBytes();
        return result;
    }
    ReadOnlySpan<byte> IFixedTupleDecoder.Bytes20()
        => Bytes20();
    ReadOnlySpan<byte> IDynamicTupleDecoder.Bytes20()
        => Bytes20();
    public ReadOnlySpan<byte> Bytes21()
    {
        var result = AbiTypes.SizedBytes.Decode(CurrentSlot, 21);
        ConsumeBytes();
        return result;
    }
    ReadOnlySpan<byte> IFixedTupleDecoder.Bytes21()
        => Bytes21();
    ReadOnlySpan<byte> IDynamicTupleDecoder.Bytes21()
        => Bytes21();
    public ReadOnlySpan<byte> Bytes22()
    {
        var result = AbiTypes.SizedBytes.Decode(CurrentSlot, 22);
        ConsumeBytes();
        return result;
    }
    ReadOnlySpan<byte> IFixedTupleDecoder.Bytes22()
        => Bytes22();
    ReadOnlySpan<byte> IDynamicTupleDecoder.Bytes22()
        => Bytes22();
    public ReadOnlySpan<byte> Bytes23()
    {
        var result = AbiTypes.SizedBytes.Decode(CurrentSlot, 23);
        ConsumeBytes();
        return result;
    }
    ReadOnlySpan<byte> IFixedTupleDecoder.Bytes23()
        => Bytes23();
    ReadOnlySpan<byte> IDynamicTupleDecoder.Bytes23()
        => Bytes23();
    public ReadOnlySpan<byte> Bytes24()
    {
        var result = AbiTypes.SizedBytes.Decode(CurrentSlot, 24);
        ConsumeBytes();
        return result;
    }
    ReadOnlySpan<byte> IFixedTupleDecoder.Bytes24()
        => Bytes24();
    ReadOnlySpan<byte> IDynamicTupleDecoder.Bytes24()
        => Bytes24();
    public ReadOnlySpan<byte> Bytes25()
    {
        var result = AbiTypes.SizedBytes.Decode(CurrentSlot, 25);
        ConsumeBytes();
        return result;
    }
    ReadOnlySpan<byte> IFixedTupleDecoder.Bytes25()
        => Bytes25();
    ReadOnlySpan<byte> IDynamicTupleDecoder.Bytes25()
        => Bytes25();
    public ReadOnlySpan<byte> Bytes26()
    {
        var result = AbiTypes.SizedBytes.Decode(CurrentSlot, 26);
        ConsumeBytes();
        return result;
    }
    ReadOnlySpan<byte> IFixedTupleDecoder.Bytes26()
        => Bytes26();
    ReadOnlySpan<byte> IDynamicTupleDecoder.Bytes26()
        => Bytes26();
    public ReadOnlySpan<byte> Bytes27()
    {
        var result = AbiTypes.SizedBytes.Decode(CurrentSlot, 27);
        ConsumeBytes();
        return result;
    }
    ReadOnlySpan<byte> IFixedTupleDecoder.Bytes27()
        => Bytes27();
    ReadOnlySpan<byte> IDynamicTupleDecoder.Bytes27()
        => Bytes27();
    public ReadOnlySpan<byte> Bytes28()
    {
        var result = AbiTypes.SizedBytes.Decode(CurrentSlot, 28);
        ConsumeBytes();
        return result;
    }
    ReadOnlySpan<byte> IFixedTupleDecoder.Bytes28()
        => Bytes28();
    ReadOnlySpan<byte> IDynamicTupleDecoder.Bytes28()
        => Bytes28();
    public ReadOnlySpan<byte> Bytes29()
    {
        var result = AbiTypes.SizedBytes.Decode(CurrentSlot, 29);
        ConsumeBytes();
        return result;
    }
    ReadOnlySpan<byte> IFixedTupleDecoder.Bytes29()
        => Bytes29();
    ReadOnlySpan<byte> IDynamicTupleDecoder.Bytes29()
        => Bytes29();
    public ReadOnlySpan<byte> Bytes30()
    {
        var result = AbiTypes.SizedBytes.Decode(CurrentSlot, 30);
        ConsumeBytes();
        return result;
    }
    ReadOnlySpan<byte> IFixedTupleDecoder.Bytes30()
        => Bytes30();
    ReadOnlySpan<byte> IDynamicTupleDecoder.Bytes30()
        => Bytes30();
    public ReadOnlySpan<byte> Bytes31()
    {
        var result = AbiTypes.SizedBytes.Decode(CurrentSlot, 31);
        ConsumeBytes();
        return result;
    }
    ReadOnlySpan<byte> IFixedTupleDecoder.Bytes31()
        => Bytes31();
    ReadOnlySpan<byte> IDynamicTupleDecoder.Bytes31()
        => Bytes31();
    public ReadOnlySpan<byte> Bytes32()
    {
        var result = AbiTypes.SizedBytes.Decode(CurrentSlot, 32);
        ConsumeBytes();
        return result;
    }
    ReadOnlySpan<byte> IFixedTupleDecoder.Bytes32()
        => Bytes32();
    ReadOnlySpan<byte> IDynamicTupleDecoder.Bytes32()
        => Bytes32();
}
