using EtherSharp.Numerics;
using EtherSharp.Types;
using System.Buffers.Binary;

namespace EtherSharp.Query.Operations;

internal static class CallQueryEncoding
{
    private const int HEADER_LENGTH = 4 + Address.BYTES_LENGTH + Bytes32.BYTE_LENGTH;

    public static int GetCallDataLength(int inputLength)
        => HEADER_LENGTH + inputLength;

    public static Span<byte> EncodeHeader(
        in Address target,
        UInt256 ethValue,
        int inputLength,
        Span<byte> buffer)
    {
        BinaryPrimitives.WriteUInt32BigEndian(buffer[0..4], (uint) inputLength);

        if(buffer[0] != 0)
        {
            throw new InvalidOperationException("Calldata too large");
        }

        buffer[0] = (byte) QueryOperationId.Call;
        target.CopyTo(buffer[4..24]);
        BinaryPrimitives.WriteUInt256BigEndian(buffer[24..56], ethValue);
        return buffer[HEADER_LENGTH..];
    }

    public static int ParseResultLength(ReadOnlySpan<byte> resultData)
    {
        int dataLength = (int) (BinaryPrimitives.ReadUInt32BigEndian(resultData[0..4]) & 0x00FFFFFF);
        return dataLength + 4;
    }

    public static (bool Success, ReadOnlyMemory<byte> Data) ParseResult(ReadOnlyMemory<byte> result)
        => (result.Span[0] == 0x01, result[4..]);
}
