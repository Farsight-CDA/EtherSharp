using EtherSharp.ABI.Types;
using EtherSharp.ABI.Decode.Interfaces;

namespace EtherSharp.ABI;
public partial class AbiDecoder
{
    /// <summary>
    /// Reads a bytes1 from the input.
    /// </summary>
    /// <returns></returns>
    public byte Bytes1()
    {
        byte result = AbiTypes.Byte.Decode(CurrentSlot);
        ConsumeBytes();
        return result;
    }
    byte IFixedTupleDecoder.Bytes1()
        => Bytes1();
    byte IDynamicTupleDecoder.Bytes1()
        => Bytes1();

    /// <summary>
    /// Reads a bytes2 from the input.
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Reads a bytes3 from the input.
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Reads a bytes4 from the input.
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Reads a bytes5 from the input.
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Reads a bytes6 from the input.
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Reads a bytes7 from the input.
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Reads a bytes8 from the input.
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Reads a bytes9 from the input.
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Reads a bytes10 from the input.
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Reads a bytes11 from the input.
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Reads a bytes12 from the input.
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Reads a bytes13 from the input.
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Reads a bytes14 from the input.
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Reads a bytes15 from the input.
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Reads a bytes16 from the input.
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Reads a bytes17 from the input.
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Reads a bytes18 from the input.
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Reads a bytes19 from the input.
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Reads a bytes20 from the input.
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Reads a bytes21 from the input.
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Reads a bytes22 from the input.
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Reads a bytes23 from the input.
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Reads a bytes24 from the input.
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Reads a bytes25 from the input.
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Reads a bytes26 from the input.
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Reads a bytes27 from the input.
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Reads a bytes28 from the input.
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Reads a bytes29 from the input.
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Reads a bytes30 from the input.
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Reads a bytes31 from the input.
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Reads a bytes32 from the input.
    /// </summary>
    /// <returns></returns>
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
