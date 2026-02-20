using EtherSharp.ABI.Types;
using EtherSharp.ABI.Decode.Interfaces;

namespace EtherSharp.ABI;
public partial class AbiDecoder
{
    /// <summary>
    /// Reads a bytes1 value from the input.
    /// </summary>
    /// <returns>The decoded byte value.</returns>
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

    /// <summary>
    /// Reads a bytes2 value from the input.
    /// </summary>
    /// <returns>The decoded 2-byte value.</returns>
    public ReadOnlyMemory<byte> Bytes2()
    {
        var result = AbiTypes.SizedBytes.Decode(_bytes.Slice(BytesRead, 32), 2);
        ConsumeBytes();
        return result;
    }
    ReadOnlyMemory<byte> IFixedTupleDecoder.Bytes2()
        => Bytes2();
    ReadOnlyMemory<byte> IDynamicTupleDecoder.Bytes2()
        => Bytes2();

    /// <summary>
    /// Reads a bytes3 value from the input.
    /// </summary>
    /// <returns>The decoded 3-byte value.</returns>
    public ReadOnlyMemory<byte> Bytes3()
    {
        var result = AbiTypes.SizedBytes.Decode(_bytes.Slice(BytesRead, 32), 3);
        ConsumeBytes();
        return result;
    }
    ReadOnlyMemory<byte> IFixedTupleDecoder.Bytes3()
        => Bytes3();
    ReadOnlyMemory<byte> IDynamicTupleDecoder.Bytes3()
        => Bytes3();

    /// <summary>
    /// Reads a bytes4 value from the input.
    /// </summary>
    /// <returns>The decoded 4-byte value.</returns>
    public ReadOnlyMemory<byte> Bytes4()
    {
        var result = AbiTypes.SizedBytes.Decode(_bytes.Slice(BytesRead, 32), 4);
        ConsumeBytes();
        return result;
    }
    ReadOnlyMemory<byte> IFixedTupleDecoder.Bytes4()
        => Bytes4();
    ReadOnlyMemory<byte> IDynamicTupleDecoder.Bytes4()
        => Bytes4();

    /// <summary>
    /// Reads a bytes5 value from the input.
    /// </summary>
    /// <returns>The decoded 5-byte value.</returns>
    public ReadOnlyMemory<byte> Bytes5()
    {
        var result = AbiTypes.SizedBytes.Decode(_bytes.Slice(BytesRead, 32), 5);
        ConsumeBytes();
        return result;
    }
    ReadOnlyMemory<byte> IFixedTupleDecoder.Bytes5()
        => Bytes5();
    ReadOnlyMemory<byte> IDynamicTupleDecoder.Bytes5()
        => Bytes5();

    /// <summary>
    /// Reads a bytes6 value from the input.
    /// </summary>
    /// <returns>The decoded 6-byte value.</returns>
    public ReadOnlyMemory<byte> Bytes6()
    {
        var result = AbiTypes.SizedBytes.Decode(_bytes.Slice(BytesRead, 32), 6);
        ConsumeBytes();
        return result;
    }
    ReadOnlyMemory<byte> IFixedTupleDecoder.Bytes6()
        => Bytes6();
    ReadOnlyMemory<byte> IDynamicTupleDecoder.Bytes6()
        => Bytes6();

    /// <summary>
    /// Reads a bytes7 value from the input.
    /// </summary>
    /// <returns>The decoded 7-byte value.</returns>
    public ReadOnlyMemory<byte> Bytes7()
    {
        var result = AbiTypes.SizedBytes.Decode(_bytes.Slice(BytesRead, 32), 7);
        ConsumeBytes();
        return result;
    }
    ReadOnlyMemory<byte> IFixedTupleDecoder.Bytes7()
        => Bytes7();
    ReadOnlyMemory<byte> IDynamicTupleDecoder.Bytes7()
        => Bytes7();

    /// <summary>
    /// Reads a bytes8 value from the input.
    /// </summary>
    /// <returns>The decoded 8-byte value.</returns>
    public ReadOnlyMemory<byte> Bytes8()
    {
        var result = AbiTypes.SizedBytes.Decode(_bytes.Slice(BytesRead, 32), 8);
        ConsumeBytes();
        return result;
    }
    ReadOnlyMemory<byte> IFixedTupleDecoder.Bytes8()
        => Bytes8();
    ReadOnlyMemory<byte> IDynamicTupleDecoder.Bytes8()
        => Bytes8();

    /// <summary>
    /// Reads a bytes9 value from the input.
    /// </summary>
    /// <returns>The decoded 9-byte value.</returns>
    public ReadOnlyMemory<byte> Bytes9()
    {
        var result = AbiTypes.SizedBytes.Decode(_bytes.Slice(BytesRead, 32), 9);
        ConsumeBytes();
        return result;
    }
    ReadOnlyMemory<byte> IFixedTupleDecoder.Bytes9()
        => Bytes9();
    ReadOnlyMemory<byte> IDynamicTupleDecoder.Bytes9()
        => Bytes9();

    /// <summary>
    /// Reads a bytes10 value from the input.
    /// </summary>
    /// <returns>The decoded 10-byte value.</returns>
    public ReadOnlyMemory<byte> Bytes10()
    {
        var result = AbiTypes.SizedBytes.Decode(_bytes.Slice(BytesRead, 32), 10);
        ConsumeBytes();
        return result;
    }
    ReadOnlyMemory<byte> IFixedTupleDecoder.Bytes10()
        => Bytes10();
    ReadOnlyMemory<byte> IDynamicTupleDecoder.Bytes10()
        => Bytes10();

    /// <summary>
    /// Reads a bytes11 value from the input.
    /// </summary>
    /// <returns>The decoded 11-byte value.</returns>
    public ReadOnlyMemory<byte> Bytes11()
    {
        var result = AbiTypes.SizedBytes.Decode(_bytes.Slice(BytesRead, 32), 11);
        ConsumeBytes();
        return result;
    }
    ReadOnlyMemory<byte> IFixedTupleDecoder.Bytes11()
        => Bytes11();
    ReadOnlyMemory<byte> IDynamicTupleDecoder.Bytes11()
        => Bytes11();

    /// <summary>
    /// Reads a bytes12 value from the input.
    /// </summary>
    /// <returns>The decoded 12-byte value.</returns>
    public ReadOnlyMemory<byte> Bytes12()
    {
        var result = AbiTypes.SizedBytes.Decode(_bytes.Slice(BytesRead, 32), 12);
        ConsumeBytes();
        return result;
    }
    ReadOnlyMemory<byte> IFixedTupleDecoder.Bytes12()
        => Bytes12();
    ReadOnlyMemory<byte> IDynamicTupleDecoder.Bytes12()
        => Bytes12();

    /// <summary>
    /// Reads a bytes13 value from the input.
    /// </summary>
    /// <returns>The decoded 13-byte value.</returns>
    public ReadOnlyMemory<byte> Bytes13()
    {
        var result = AbiTypes.SizedBytes.Decode(_bytes.Slice(BytesRead, 32), 13);
        ConsumeBytes();
        return result;
    }
    ReadOnlyMemory<byte> IFixedTupleDecoder.Bytes13()
        => Bytes13();
    ReadOnlyMemory<byte> IDynamicTupleDecoder.Bytes13()
        => Bytes13();

    /// <summary>
    /// Reads a bytes14 value from the input.
    /// </summary>
    /// <returns>The decoded 14-byte value.</returns>
    public ReadOnlyMemory<byte> Bytes14()
    {
        var result = AbiTypes.SizedBytes.Decode(_bytes.Slice(BytesRead, 32), 14);
        ConsumeBytes();
        return result;
    }
    ReadOnlyMemory<byte> IFixedTupleDecoder.Bytes14()
        => Bytes14();
    ReadOnlyMemory<byte> IDynamicTupleDecoder.Bytes14()
        => Bytes14();

    /// <summary>
    /// Reads a bytes15 value from the input.
    /// </summary>
    /// <returns>The decoded 15-byte value.</returns>
    public ReadOnlyMemory<byte> Bytes15()
    {
        var result = AbiTypes.SizedBytes.Decode(_bytes.Slice(BytesRead, 32), 15);
        ConsumeBytes();
        return result;
    }
    ReadOnlyMemory<byte> IFixedTupleDecoder.Bytes15()
        => Bytes15();
    ReadOnlyMemory<byte> IDynamicTupleDecoder.Bytes15()
        => Bytes15();

    /// <summary>
    /// Reads a bytes16 value from the input.
    /// </summary>
    /// <returns>The decoded 16-byte value.</returns>
    public ReadOnlyMemory<byte> Bytes16()
    {
        var result = AbiTypes.SizedBytes.Decode(_bytes.Slice(BytesRead, 32), 16);
        ConsumeBytes();
        return result;
    }
    ReadOnlyMemory<byte> IFixedTupleDecoder.Bytes16()
        => Bytes16();
    ReadOnlyMemory<byte> IDynamicTupleDecoder.Bytes16()
        => Bytes16();

    /// <summary>
    /// Reads a bytes17 value from the input.
    /// </summary>
    /// <returns>The decoded 17-byte value.</returns>
    public ReadOnlyMemory<byte> Bytes17()
    {
        var result = AbiTypes.SizedBytes.Decode(_bytes.Slice(BytesRead, 32), 17);
        ConsumeBytes();
        return result;
    }
    ReadOnlyMemory<byte> IFixedTupleDecoder.Bytes17()
        => Bytes17();
    ReadOnlyMemory<byte> IDynamicTupleDecoder.Bytes17()
        => Bytes17();

    /// <summary>
    /// Reads a bytes18 value from the input.
    /// </summary>
    /// <returns>The decoded 18-byte value.</returns>
    public ReadOnlyMemory<byte> Bytes18()
    {
        var result = AbiTypes.SizedBytes.Decode(_bytes.Slice(BytesRead, 32), 18);
        ConsumeBytes();
        return result;
    }
    ReadOnlyMemory<byte> IFixedTupleDecoder.Bytes18()
        => Bytes18();
    ReadOnlyMemory<byte> IDynamicTupleDecoder.Bytes18()
        => Bytes18();

    /// <summary>
    /// Reads a bytes19 value from the input.
    /// </summary>
    /// <returns>The decoded 19-byte value.</returns>
    public ReadOnlyMemory<byte> Bytes19()
    {
        var result = AbiTypes.SizedBytes.Decode(_bytes.Slice(BytesRead, 32), 19);
        ConsumeBytes();
        return result;
    }
    ReadOnlyMemory<byte> IFixedTupleDecoder.Bytes19()
        => Bytes19();
    ReadOnlyMemory<byte> IDynamicTupleDecoder.Bytes19()
        => Bytes19();

    /// <summary>
    /// Reads a bytes20 value from the input.
    /// </summary>
    /// <returns>The decoded 20-byte value.</returns>
    public ReadOnlyMemory<byte> Bytes20()
    {
        var result = AbiTypes.SizedBytes.Decode(_bytes.Slice(BytesRead, 32), 20);
        ConsumeBytes();
        return result;
    }
    ReadOnlyMemory<byte> IFixedTupleDecoder.Bytes20()
        => Bytes20();
    ReadOnlyMemory<byte> IDynamicTupleDecoder.Bytes20()
        => Bytes20();

    /// <summary>
    /// Reads a bytes21 value from the input.
    /// </summary>
    /// <returns>The decoded 21-byte value.</returns>
    public ReadOnlyMemory<byte> Bytes21()
    {
        var result = AbiTypes.SizedBytes.Decode(_bytes.Slice(BytesRead, 32), 21);
        ConsumeBytes();
        return result;
    }
    ReadOnlyMemory<byte> IFixedTupleDecoder.Bytes21()
        => Bytes21();
    ReadOnlyMemory<byte> IDynamicTupleDecoder.Bytes21()
        => Bytes21();

    /// <summary>
    /// Reads a bytes22 value from the input.
    /// </summary>
    /// <returns>The decoded 22-byte value.</returns>
    public ReadOnlyMemory<byte> Bytes22()
    {
        var result = AbiTypes.SizedBytes.Decode(_bytes.Slice(BytesRead, 32), 22);
        ConsumeBytes();
        return result;
    }
    ReadOnlyMemory<byte> IFixedTupleDecoder.Bytes22()
        => Bytes22();
    ReadOnlyMemory<byte> IDynamicTupleDecoder.Bytes22()
        => Bytes22();

    /// <summary>
    /// Reads a bytes23 value from the input.
    /// </summary>
    /// <returns>The decoded 23-byte value.</returns>
    public ReadOnlyMemory<byte> Bytes23()
    {
        var result = AbiTypes.SizedBytes.Decode(_bytes.Slice(BytesRead, 32), 23);
        ConsumeBytes();
        return result;
    }
    ReadOnlyMemory<byte> IFixedTupleDecoder.Bytes23()
        => Bytes23();
    ReadOnlyMemory<byte> IDynamicTupleDecoder.Bytes23()
        => Bytes23();

    /// <summary>
    /// Reads a bytes24 value from the input.
    /// </summary>
    /// <returns>The decoded 24-byte value.</returns>
    public ReadOnlyMemory<byte> Bytes24()
    {
        var result = AbiTypes.SizedBytes.Decode(_bytes.Slice(BytesRead, 32), 24);
        ConsumeBytes();
        return result;
    }
    ReadOnlyMemory<byte> IFixedTupleDecoder.Bytes24()
        => Bytes24();
    ReadOnlyMemory<byte> IDynamicTupleDecoder.Bytes24()
        => Bytes24();

    /// <summary>
    /// Reads a bytes25 value from the input.
    /// </summary>
    /// <returns>The decoded 25-byte value.</returns>
    public ReadOnlyMemory<byte> Bytes25()
    {
        var result = AbiTypes.SizedBytes.Decode(_bytes.Slice(BytesRead, 32), 25);
        ConsumeBytes();
        return result;
    }
    ReadOnlyMemory<byte> IFixedTupleDecoder.Bytes25()
        => Bytes25();
    ReadOnlyMemory<byte> IDynamicTupleDecoder.Bytes25()
        => Bytes25();

    /// <summary>
    /// Reads a bytes26 value from the input.
    /// </summary>
    /// <returns>The decoded 26-byte value.</returns>
    public ReadOnlyMemory<byte> Bytes26()
    {
        var result = AbiTypes.SizedBytes.Decode(_bytes.Slice(BytesRead, 32), 26);
        ConsumeBytes();
        return result;
    }
    ReadOnlyMemory<byte> IFixedTupleDecoder.Bytes26()
        => Bytes26();
    ReadOnlyMemory<byte> IDynamicTupleDecoder.Bytes26()
        => Bytes26();

    /// <summary>
    /// Reads a bytes27 value from the input.
    /// </summary>
    /// <returns>The decoded 27-byte value.</returns>
    public ReadOnlyMemory<byte> Bytes27()
    {
        var result = AbiTypes.SizedBytes.Decode(_bytes.Slice(BytesRead, 32), 27);
        ConsumeBytes();
        return result;
    }
    ReadOnlyMemory<byte> IFixedTupleDecoder.Bytes27()
        => Bytes27();
    ReadOnlyMemory<byte> IDynamicTupleDecoder.Bytes27()
        => Bytes27();

    /// <summary>
    /// Reads a bytes28 value from the input.
    /// </summary>
    /// <returns>The decoded 28-byte value.</returns>
    public ReadOnlyMemory<byte> Bytes28()
    {
        var result = AbiTypes.SizedBytes.Decode(_bytes.Slice(BytesRead, 32), 28);
        ConsumeBytes();
        return result;
    }
    ReadOnlyMemory<byte> IFixedTupleDecoder.Bytes28()
        => Bytes28();
    ReadOnlyMemory<byte> IDynamicTupleDecoder.Bytes28()
        => Bytes28();

    /// <summary>
    /// Reads a bytes29 value from the input.
    /// </summary>
    /// <returns>The decoded 29-byte value.</returns>
    public ReadOnlyMemory<byte> Bytes29()
    {
        var result = AbiTypes.SizedBytes.Decode(_bytes.Slice(BytesRead, 32), 29);
        ConsumeBytes();
        return result;
    }
    ReadOnlyMemory<byte> IFixedTupleDecoder.Bytes29()
        => Bytes29();
    ReadOnlyMemory<byte> IDynamicTupleDecoder.Bytes29()
        => Bytes29();

    /// <summary>
    /// Reads a bytes30 value from the input.
    /// </summary>
    /// <returns>The decoded 30-byte value.</returns>
    public ReadOnlyMemory<byte> Bytes30()
    {
        var result = AbiTypes.SizedBytes.Decode(_bytes.Slice(BytesRead, 32), 30);
        ConsumeBytes();
        return result;
    }
    ReadOnlyMemory<byte> IFixedTupleDecoder.Bytes30()
        => Bytes30();
    ReadOnlyMemory<byte> IDynamicTupleDecoder.Bytes30()
        => Bytes30();

    /// <summary>
    /// Reads a bytes31 value from the input.
    /// </summary>
    /// <returns>The decoded 31-byte value.</returns>
    public ReadOnlyMemory<byte> Bytes31()
    {
        var result = AbiTypes.SizedBytes.Decode(_bytes.Slice(BytesRead, 32), 31);
        ConsumeBytes();
        return result;
    }
    ReadOnlyMemory<byte> IFixedTupleDecoder.Bytes31()
        => Bytes31();
    ReadOnlyMemory<byte> IDynamicTupleDecoder.Bytes31()
        => Bytes31();

    /// <summary>
    /// Reads a bytes32 value from the input.
    /// </summary>
    /// <returns>The decoded 32-byte value.</returns>
    public ReadOnlyMemory<byte> Bytes32()
    {
        var result = AbiTypes.SizedBytes.Decode(_bytes.Slice(BytesRead, 32), 32);
        ConsumeBytes();
        return result;
    }
    ReadOnlyMemory<byte> IFixedTupleDecoder.Bytes32()
        => Bytes32();
    ReadOnlyMemory<byte> IDynamicTupleDecoder.Bytes32()
        => Bytes32();

}
