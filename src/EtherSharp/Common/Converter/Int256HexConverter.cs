using EtherSharp.Numerics;
using System.Buffers.Binary;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EtherSharp.Common.Converter;

public class Int256HexConverter : JsonConverter<Int256>
{
    public override Int256 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if(reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException("Expected string token.");
        }
        int valueLength = reader.HasValueSequence
            ? (int) reader.ValueSequence.Length
            : reader.ValueSpan.Length;

        if(valueLength > 68)
        {
            throw new InvalidOperationException("Unexpected number length");
        }

        Span<char> sourceBuffer = stackalloc char[valueLength + 1];
        int charsWritten = reader.CopyString(sourceBuffer[1..]);

        if(charsWritten > 66)
        {
            throw new JsonException("Unexpected number length");
        }

        int startIndex = 1;

        if(sourceBuffer[1..].StartsWith("0x"))
        {
            startIndex += 2;
            charsWritten -= 2;
        }

        if(charsWritten % 2 != 0)
        {
            startIndex--;
            charsWritten++;
            sourceBuffer[startIndex] = '0';
        }

        return !Int256.TryParseFromHex(sourceBuffer[startIndex..(startIndex + charsWritten)], out var result)
            ? throw new JsonException("Failed parsing UInt256")
            : result;
    }

    public override void Write(Utf8JsonWriter writer, Int256 value, JsonSerializerOptions options)
    {
        if(value == 0)
        {
            writer.WriteStringValue("0x0");
            return;
        }

        Span<byte> byteBuffer = stackalloc byte[32];

        BinaryPrimitives.WriteInt256BigEndian(byteBuffer, value);

        byteBuffer = byteBuffer.TrimStart((byte) 0);

        int dataIndex = byteBuffer[0] < 16 ? 1 : 2;
        Span<char> hexBuffer = stackalloc char[(byteBuffer.Length * 2) + dataIndex];

        if(!Convert.TryToHexString(byteBuffer, hexBuffer[dataIndex..], out _))
        {
            throw new InvalidOperationException("Failed to convert to hex");
        }

        hexBuffer[0] = '0';
        hexBuffer[1] = 'x';

        writer.WriteStringValue(hexBuffer);
    }
}