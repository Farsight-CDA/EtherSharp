using EtherSharp.Numerics;
using EtherSharp.Types;
using System.Buffers.Binary;

namespace EtherSharp.Query.Operations;

internal static class CallQueryEncoding
{
    private const int HEADER_LENGTH = 3 + Address.BYTES_LENGTH + Bytes32.BYTE_LENGTH;

    public static int GetCallDataLength(int inputLength, bool includeOpcode = true)
        => HEADER_LENGTH + inputLength + (includeOpcode ? 1 : 0);

    public static Span<byte> EncodeHeader(
        in Address target,
        UInt256 ethValue,
        int inputLength,
        Span<byte> buffer,
        bool includeOpcode = true
    )
    {
        if((uint) inputLength > 0xFFFFFF)
        {
            throw new InvalidOperationException("Calldata too large");
        }

        if(includeOpcode)
        {
            buffer[0] = (byte) QueryOperationId.Call;
            buffer = buffer[1..];
        }

        BinaryPrimitives.WriteUInt32BigEndian(buffer[..4], (uint) inputLength << 8);
        target.CopyTo(buffer[3..23]);
        BinaryPrimitives.WriteUInt256BigEndian(buffer[23..HEADER_LENGTH], ethValue);
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
