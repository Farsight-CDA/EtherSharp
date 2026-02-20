using EtherSharp.ABI.Types;
using EtherSharp.ABI.Decode.Interfaces;

namespace EtherSharp.ABI;
public partial class AbiDecoder
{
    /// <summary>
    /// Reads a bytes1 array from the input.
    /// </summary>
    /// <returns>The decoded byte array.</returns>
    public byte[] Bytes1Array()
    {
        ReadOnlyMemory<byte>[] result = AbiTypes.SizedBytesArray.Decode(_bytes, BytesRead, 1);
        ConsumeBytes();
        return [.. result[0].Span];
    }
    /// <summary>
    /// Reads a bytes2 array from the input.
    /// </summary>
    /// <returns>The decoded array of 2-byte values.</returns>
    public ReadOnlyMemory<byte>[] Bytes2Array()
    {
        ReadOnlyMemory<byte>[] result = AbiTypes.SizedBytesArray.Decode(_bytes, BytesRead, 2);
        ConsumeBytes();
        return result;
    }
    /// <summary>
    /// Reads a bytes3 array from the input.
    /// </summary>
    /// <returns>The decoded array of 3-byte values.</returns>
    public ReadOnlyMemory<byte>[] Bytes3Array()
    {
        ReadOnlyMemory<byte>[] result = AbiTypes.SizedBytesArray.Decode(_bytes, BytesRead, 3);
        ConsumeBytes();
        return result;
    }
    /// <summary>
    /// Reads a bytes4 array from the input.
    /// </summary>
    /// <returns>The decoded array of 4-byte values.</returns>
    public ReadOnlyMemory<byte>[] Bytes4Array()
    {
        ReadOnlyMemory<byte>[] result = AbiTypes.SizedBytesArray.Decode(_bytes, BytesRead, 4);
        ConsumeBytes();
        return result;
    }
    /// <summary>
    /// Reads a bytes5 array from the input.
    /// </summary>
    /// <returns>The decoded array of 5-byte values.</returns>
    public ReadOnlyMemory<byte>[] Bytes5Array()
    {
        ReadOnlyMemory<byte>[] result = AbiTypes.SizedBytesArray.Decode(_bytes, BytesRead, 5);
        ConsumeBytes();
        return result;
    }
    /// <summary>
    /// Reads a bytes6 array from the input.
    /// </summary>
    /// <returns>The decoded array of 6-byte values.</returns>
    public ReadOnlyMemory<byte>[] Bytes6Array()
    {
        ReadOnlyMemory<byte>[] result = AbiTypes.SizedBytesArray.Decode(_bytes, BytesRead, 6);
        ConsumeBytes();
        return result;
    }
    /// <summary>
    /// Reads a bytes7 array from the input.
    /// </summary>
    /// <returns>The decoded array of 7-byte values.</returns>
    public ReadOnlyMemory<byte>[] Bytes7Array()
    {
        ReadOnlyMemory<byte>[] result = AbiTypes.SizedBytesArray.Decode(_bytes, BytesRead, 7);
        ConsumeBytes();
        return result;
    }
    /// <summary>
    /// Reads a bytes8 array from the input.
    /// </summary>
    /// <returns>The decoded array of 8-byte values.</returns>
    public ReadOnlyMemory<byte>[] Bytes8Array()
    {
        ReadOnlyMemory<byte>[] result = AbiTypes.SizedBytesArray.Decode(_bytes, BytesRead, 8);
        ConsumeBytes();
        return result;
    }
    /// <summary>
    /// Reads a bytes9 array from the input.
    /// </summary>
    /// <returns>The decoded array of 9-byte values.</returns>
    public ReadOnlyMemory<byte>[] Bytes9Array()
    {
        ReadOnlyMemory<byte>[] result = AbiTypes.SizedBytesArray.Decode(_bytes, BytesRead, 9);
        ConsumeBytes();
        return result;
    }
    /// <summary>
    /// Reads a bytes10 array from the input.
    /// </summary>
    /// <returns>The decoded array of 10-byte values.</returns>
    public ReadOnlyMemory<byte>[] Bytes10Array()
    {
        ReadOnlyMemory<byte>[] result = AbiTypes.SizedBytesArray.Decode(_bytes, BytesRead, 10);
        ConsumeBytes();
        return result;
    }
    /// <summary>
    /// Reads a bytes11 array from the input.
    /// </summary>
    /// <returns>The decoded array of 11-byte values.</returns>
    public ReadOnlyMemory<byte>[] Bytes11Array()
    {
        ReadOnlyMemory<byte>[] result = AbiTypes.SizedBytesArray.Decode(_bytes, BytesRead, 11);
        ConsumeBytes();
        return result;
    }
    /// <summary>
    /// Reads a bytes12 array from the input.
    /// </summary>
    /// <returns>The decoded array of 12-byte values.</returns>
    public ReadOnlyMemory<byte>[] Bytes12Array()
    {
        ReadOnlyMemory<byte>[] result = AbiTypes.SizedBytesArray.Decode(_bytes, BytesRead, 12);
        ConsumeBytes();
        return result;
    }
    /// <summary>
    /// Reads a bytes13 array from the input.
    /// </summary>
    /// <returns>The decoded array of 13-byte values.</returns>
    public ReadOnlyMemory<byte>[] Bytes13Array()
    {
        ReadOnlyMemory<byte>[] result = AbiTypes.SizedBytesArray.Decode(_bytes, BytesRead, 13);
        ConsumeBytes();
        return result;
    }
    /// <summary>
    /// Reads a bytes14 array from the input.
    /// </summary>
    /// <returns>The decoded array of 14-byte values.</returns>
    public ReadOnlyMemory<byte>[] Bytes14Array()
    {
        ReadOnlyMemory<byte>[] result = AbiTypes.SizedBytesArray.Decode(_bytes, BytesRead, 14);
        ConsumeBytes();
        return result;
    }
    /// <summary>
    /// Reads a bytes15 array from the input.
    /// </summary>
    /// <returns>The decoded array of 15-byte values.</returns>
    public ReadOnlyMemory<byte>[] Bytes15Array()
    {
        ReadOnlyMemory<byte>[] result = AbiTypes.SizedBytesArray.Decode(_bytes, BytesRead, 15);
        ConsumeBytes();
        return result;
    }
    /// <summary>
    /// Reads a bytes16 array from the input.
    /// </summary>
    /// <returns>The decoded array of 16-byte values.</returns>
    public ReadOnlyMemory<byte>[] Bytes16Array()
    {
        ReadOnlyMemory<byte>[] result = AbiTypes.SizedBytesArray.Decode(_bytes, BytesRead, 16);
        ConsumeBytes();
        return result;
    }
    /// <summary>
    /// Reads a bytes17 array from the input.
    /// </summary>
    /// <returns>The decoded array of 17-byte values.</returns>
    public ReadOnlyMemory<byte>[] Bytes17Array()
    {
        ReadOnlyMemory<byte>[] result = AbiTypes.SizedBytesArray.Decode(_bytes, BytesRead, 17);
        ConsumeBytes();
        return result;
    }
    /// <summary>
    /// Reads a bytes18 array from the input.
    /// </summary>
    /// <returns>The decoded array of 18-byte values.</returns>
    public ReadOnlyMemory<byte>[] Bytes18Array()
    {
        ReadOnlyMemory<byte>[] result = AbiTypes.SizedBytesArray.Decode(_bytes, BytesRead, 18);
        ConsumeBytes();
        return result;
    }
    /// <summary>
    /// Reads a bytes19 array from the input.
    /// </summary>
    /// <returns>The decoded array of 19-byte values.</returns>
    public ReadOnlyMemory<byte>[] Bytes19Array()
    {
        ReadOnlyMemory<byte>[] result = AbiTypes.SizedBytesArray.Decode(_bytes, BytesRead, 19);
        ConsumeBytes();
        return result;
    }
    /// <summary>
    /// Reads a bytes20 array from the input.
    /// </summary>
    /// <returns>The decoded array of 20-byte values.</returns>
    public ReadOnlyMemory<byte>[] Bytes20Array()
    {
        ReadOnlyMemory<byte>[] result = AbiTypes.SizedBytesArray.Decode(_bytes, BytesRead, 20);
        ConsumeBytes();
        return result;
    }
    /// <summary>
    /// Reads a bytes21 array from the input.
    /// </summary>
    /// <returns>The decoded array of 21-byte values.</returns>
    public ReadOnlyMemory<byte>[] Bytes21Array()
    {
        ReadOnlyMemory<byte>[] result = AbiTypes.SizedBytesArray.Decode(_bytes, BytesRead, 21);
        ConsumeBytes();
        return result;
    }
    /// <summary>
    /// Reads a bytes22 array from the input.
    /// </summary>
    /// <returns>The decoded array of 22-byte values.</returns>
    public ReadOnlyMemory<byte>[] Bytes22Array()
    {
        ReadOnlyMemory<byte>[] result = AbiTypes.SizedBytesArray.Decode(_bytes, BytesRead, 22);
        ConsumeBytes();
        return result;
    }
    /// <summary>
    /// Reads a bytes23 array from the input.
    /// </summary>
    /// <returns>The decoded array of 23-byte values.</returns>
    public ReadOnlyMemory<byte>[] Bytes23Array()
    {
        ReadOnlyMemory<byte>[] result = AbiTypes.SizedBytesArray.Decode(_bytes, BytesRead, 23);
        ConsumeBytes();
        return result;
    }
    /// <summary>
    /// Reads a bytes24 array from the input.
    /// </summary>
    /// <returns>The decoded array of 24-byte values.</returns>
    public ReadOnlyMemory<byte>[] Bytes24Array()
    {
        ReadOnlyMemory<byte>[] result = AbiTypes.SizedBytesArray.Decode(_bytes, BytesRead, 24);
        ConsumeBytes();
        return result;
    }
    /// <summary>
    /// Reads a bytes25 array from the input.
    /// </summary>
    /// <returns>The decoded array of 25-byte values.</returns>
    public ReadOnlyMemory<byte>[] Bytes25Array()
    {
        ReadOnlyMemory<byte>[] result = AbiTypes.SizedBytesArray.Decode(_bytes, BytesRead, 25);
        ConsumeBytes();
        return result;
    }
    /// <summary>
    /// Reads a bytes26 array from the input.
    /// </summary>
    /// <returns>The decoded array of 26-byte values.</returns>
    public ReadOnlyMemory<byte>[] Bytes26Array()
    {
        ReadOnlyMemory<byte>[] result = AbiTypes.SizedBytesArray.Decode(_bytes, BytesRead, 26);
        ConsumeBytes();
        return result;
    }
    /// <summary>
    /// Reads a bytes27 array from the input.
    /// </summary>
    /// <returns>The decoded array of 27-byte values.</returns>
    public ReadOnlyMemory<byte>[] Bytes27Array()
    {
        ReadOnlyMemory<byte>[] result = AbiTypes.SizedBytesArray.Decode(_bytes, BytesRead, 27);
        ConsumeBytes();
        return result;
    }
    /// <summary>
    /// Reads a bytes28 array from the input.
    /// </summary>
    /// <returns>The decoded array of 28-byte values.</returns>
    public ReadOnlyMemory<byte>[] Bytes28Array()
    {
        ReadOnlyMemory<byte>[] result = AbiTypes.SizedBytesArray.Decode(_bytes, BytesRead, 28);
        ConsumeBytes();
        return result;
    }
    /// <summary>
    /// Reads a bytes29 array from the input.
    /// </summary>
    /// <returns>The decoded array of 29-byte values.</returns>
    public ReadOnlyMemory<byte>[] Bytes29Array()
    {
        ReadOnlyMemory<byte>[] result = AbiTypes.SizedBytesArray.Decode(_bytes, BytesRead, 29);
        ConsumeBytes();
        return result;
    }
    /// <summary>
    /// Reads a bytes30 array from the input.
    /// </summary>
    /// <returns>The decoded array of 30-byte values.</returns>
    public ReadOnlyMemory<byte>[] Bytes30Array()
    {
        ReadOnlyMemory<byte>[] result = AbiTypes.SizedBytesArray.Decode(_bytes, BytesRead, 30);
        ConsumeBytes();
        return result;
    }
    /// <summary>
    /// Reads a bytes31 array from the input.
    /// </summary>
    /// <returns>The decoded array of 31-byte values.</returns>
    public ReadOnlyMemory<byte>[] Bytes31Array()
    {
        ReadOnlyMemory<byte>[] result = AbiTypes.SizedBytesArray.Decode(_bytes, BytesRead, 31);
        ConsumeBytes();
        return result;
    }
    /// <summary>
    /// Reads a bytes32 array from the input.
    /// </summary>
    /// <returns>The decoded array of 32-byte values.</returns>
    public ReadOnlyMemory<byte>[] Bytes32Array()
    {
        ReadOnlyMemory<byte>[] result = AbiTypes.SizedBytesArray.Decode(_bytes, BytesRead, 32);
        ConsumeBytes();
        return result;
    }
}
