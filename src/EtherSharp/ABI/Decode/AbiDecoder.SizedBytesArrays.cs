using EtherSharp.ABI.Types;
using EtherSharp.ABI.Decode.Interfaces;

namespace EtherSharp.ABI;
public partial class AbiDecoder
{
    /// <summary>
    /// Reads a bytes1 from the input.
    /// </summary>
    /// <returns></returns>
    public byte[] Bytes1Array()
    {
        byte[][] result = AbiTypes.SizedBytesArray.Decode(_bytes, BytesRead, 1);
        ConsumeBytes();
        return result[0];
    }
    /// <summary>
    /// Reads a bytes2 array from the input.
    /// </summary>
    /// <returns></returns>
    public byte[][] Bytes2Array()
    {
        byte[][] result = AbiTypes.SizedBytesArray.Decode(_bytes, BytesRead, 2);
        ConsumeBytes();
        return result;
    }
    /// <summary>
    /// Reads a bytes3 array from the input.
    /// </summary>
    /// <returns></returns>
    public byte[][] Bytes3Array()
    {
        byte[][] result = AbiTypes.SizedBytesArray.Decode(_bytes, BytesRead, 3);
        ConsumeBytes();
        return result;
    }
    /// <summary>
    /// Reads a bytes4 array from the input.
    /// </summary>
    /// <returns></returns>
    public byte[][] Bytes4Array()
    {
        byte[][] result = AbiTypes.SizedBytesArray.Decode(_bytes, BytesRead, 4);
        ConsumeBytes();
        return result;
    }
    /// <summary>
    /// Reads a bytes5 array from the input.
    /// </summary>
    /// <returns></returns>
    public byte[][] Bytes5Array()
    {
        byte[][] result = AbiTypes.SizedBytesArray.Decode(_bytes, BytesRead, 5);
        ConsumeBytes();
        return result;
    }
    /// <summary>
    /// Reads a bytes6 array from the input.
    /// </summary>
    /// <returns></returns>
    public byte[][] Bytes6Array()
    {
        byte[][] result = AbiTypes.SizedBytesArray.Decode(_bytes, BytesRead, 6);
        ConsumeBytes();
        return result;
    }
    /// <summary>
    /// Reads a bytes7 array from the input.
    /// </summary>
    /// <returns></returns>
    public byte[][] Bytes7Array()
    {
        byte[][] result = AbiTypes.SizedBytesArray.Decode(_bytes, BytesRead, 7);
        ConsumeBytes();
        return result;
    }
    /// <summary>
    /// Reads a bytes8 array from the input.
    /// </summary>
    /// <returns></returns>
    public byte[][] Bytes8Array()
    {
        byte[][] result = AbiTypes.SizedBytesArray.Decode(_bytes, BytesRead, 8);
        ConsumeBytes();
        return result;
    }
    /// <summary>
    /// Reads a bytes9 array from the input.
    /// </summary>
    /// <returns></returns>
    public byte[][] Bytes9Array()
    {
        byte[][] result = AbiTypes.SizedBytesArray.Decode(_bytes, BytesRead, 9);
        ConsumeBytes();
        return result;
    }
    /// <summary>
    /// Reads a bytes10 array from the input.
    /// </summary>
    /// <returns></returns>
    public byte[][] Bytes10Array()
    {
        byte[][] result = AbiTypes.SizedBytesArray.Decode(_bytes, BytesRead, 10);
        ConsumeBytes();
        return result;
    }
    /// <summary>
    /// Reads a bytes11 array from the input.
    /// </summary>
    /// <returns></returns>
    public byte[][] Bytes11Array()
    {
        byte[][] result = AbiTypes.SizedBytesArray.Decode(_bytes, BytesRead, 11);
        ConsumeBytes();
        return result;
    }
    /// <summary>
    /// Reads a bytes12 array from the input.
    /// </summary>
    /// <returns></returns>
    public byte[][] Bytes12Array()
    {
        byte[][] result = AbiTypes.SizedBytesArray.Decode(_bytes, BytesRead, 12);
        ConsumeBytes();
        return result;
    }
    /// <summary>
    /// Reads a bytes13 array from the input.
    /// </summary>
    /// <returns></returns>
    public byte[][] Bytes13Array()
    {
        byte[][] result = AbiTypes.SizedBytesArray.Decode(_bytes, BytesRead, 13);
        ConsumeBytes();
        return result;
    }
    /// <summary>
    /// Reads a bytes14 array from the input.
    /// </summary>
    /// <returns></returns>
    public byte[][] Bytes14Array()
    {
        byte[][] result = AbiTypes.SizedBytesArray.Decode(_bytes, BytesRead, 14);
        ConsumeBytes();
        return result;
    }
    /// <summary>
    /// Reads a bytes15 array from the input.
    /// </summary>
    /// <returns></returns>
    public byte[][] Bytes15Array()
    {
        byte[][] result = AbiTypes.SizedBytesArray.Decode(_bytes, BytesRead, 15);
        ConsumeBytes();
        return result;
    }
    /// <summary>
    /// Reads a bytes16 array from the input.
    /// </summary>
    /// <returns></returns>
    public byte[][] Bytes16Array()
    {
        byte[][] result = AbiTypes.SizedBytesArray.Decode(_bytes, BytesRead, 16);
        ConsumeBytes();
        return result;
    }
    /// <summary>
    /// Reads a bytes17 array from the input.
    /// </summary>
    /// <returns></returns>
    public byte[][] Bytes17Array()
    {
        byte[][] result = AbiTypes.SizedBytesArray.Decode(_bytes, BytesRead, 17);
        ConsumeBytes();
        return result;
    }
    /// <summary>
    /// Reads a bytes18 array from the input.
    /// </summary>
    /// <returns></returns>
    public byte[][] Bytes18Array()
    {
        byte[][] result = AbiTypes.SizedBytesArray.Decode(_bytes, BytesRead, 18);
        ConsumeBytes();
        return result;
    }
    /// <summary>
    /// Reads a bytes19 array from the input.
    /// </summary>
    /// <returns></returns>
    public byte[][] Bytes19Array()
    {
        byte[][] result = AbiTypes.SizedBytesArray.Decode(_bytes, BytesRead, 19);
        ConsumeBytes();
        return result;
    }
    /// <summary>
    /// Reads a bytes20 array from the input.
    /// </summary>
    /// <returns></returns>
    public byte[][] Bytes20Array()
    {
        byte[][] result = AbiTypes.SizedBytesArray.Decode(_bytes, BytesRead, 20);
        ConsumeBytes();
        return result;
    }
    /// <summary>
    /// Reads a bytes21 array from the input.
    /// </summary>
    /// <returns></returns>
    public byte[][] Bytes21Array()
    {
        byte[][] result = AbiTypes.SizedBytesArray.Decode(_bytes, BytesRead, 21);
        ConsumeBytes();
        return result;
    }
    /// <summary>
    /// Reads a bytes22 array from the input.
    /// </summary>
    /// <returns></returns>
    public byte[][] Bytes22Array()
    {
        byte[][] result = AbiTypes.SizedBytesArray.Decode(_bytes, BytesRead, 22);
        ConsumeBytes();
        return result;
    }
    /// <summary>
    /// Reads a bytes23 array from the input.
    /// </summary>
    /// <returns></returns>
    public byte[][] Bytes23Array()
    {
        byte[][] result = AbiTypes.SizedBytesArray.Decode(_bytes, BytesRead, 23);
        ConsumeBytes();
        return result;
    }
    /// <summary>
    /// Reads a bytes24 array from the input.
    /// </summary>
    /// <returns></returns>
    public byte[][] Bytes24Array()
    {
        byte[][] result = AbiTypes.SizedBytesArray.Decode(_bytes, BytesRead, 24);
        ConsumeBytes();
        return result;
    }
    /// <summary>
    /// Reads a bytes25 array from the input.
    /// </summary>
    /// <returns></returns>
    public byte[][] Bytes25Array()
    {
        byte[][] result = AbiTypes.SizedBytesArray.Decode(_bytes, BytesRead, 25);
        ConsumeBytes();
        return result;
    }
    /// <summary>
    /// Reads a bytes26 array from the input.
    /// </summary>
    /// <returns></returns>
    public byte[][] Bytes26Array()
    {
        byte[][] result = AbiTypes.SizedBytesArray.Decode(_bytes, BytesRead, 26);
        ConsumeBytes();
        return result;
    }
    /// <summary>
    /// Reads a bytes27 array from the input.
    /// </summary>
    /// <returns></returns>
    public byte[][] Bytes27Array()
    {
        byte[][] result = AbiTypes.SizedBytesArray.Decode(_bytes, BytesRead, 27);
        ConsumeBytes();
        return result;
    }
    /// <summary>
    /// Reads a bytes28 array from the input.
    /// </summary>
    /// <returns></returns>
    public byte[][] Bytes28Array()
    {
        byte[][] result = AbiTypes.SizedBytesArray.Decode(_bytes, BytesRead, 28);
        ConsumeBytes();
        return result;
    }
    /// <summary>
    /// Reads a bytes29 array from the input.
    /// </summary>
    /// <returns></returns>
    public byte[][] Bytes29Array()
    {
        byte[][] result = AbiTypes.SizedBytesArray.Decode(_bytes, BytesRead, 29);
        ConsumeBytes();
        return result;
    }
    /// <summary>
    /// Reads a bytes30 array from the input.
    /// </summary>
    /// <returns></returns>
    public byte[][] Bytes30Array()
    {
        byte[][] result = AbiTypes.SizedBytesArray.Decode(_bytes, BytesRead, 30);
        ConsumeBytes();
        return result;
    }
    /// <summary>
    /// Reads a bytes31 array from the input.
    /// </summary>
    /// <returns></returns>
    public byte[][] Bytes31Array()
    {
        byte[][] result = AbiTypes.SizedBytesArray.Decode(_bytes, BytesRead, 31);
        ConsumeBytes();
        return result;
    }
    /// <summary>
    /// Reads a bytes32 array from the input.
    /// </summary>
    /// <returns></returns>
    public byte[][] Bytes32Array()
    {
        byte[][] result = AbiTypes.SizedBytesArray.Decode(_bytes, BytesRead, 32);
        ConsumeBytes();
        return result;
    }
}