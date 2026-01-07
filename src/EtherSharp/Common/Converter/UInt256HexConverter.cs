using EtherSharp.Numerics;
using System.Buffers.Binary;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EtherSharp.Common.Converter;

public class UInt256HexConverter : JsonConverter<UInt256>
{
    public override UInt256 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
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
            throw new JsonException("Unexpected number length");
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

        return !UInt256.TryParseFromHex(sourceBuffer[startIndex..(startIndex + charsWritten)], out var result)
            ? throw new JsonException("Failed parsing UInt256")
            : result;
    }

    public override void Write(Utf8JsonWriter writer, UInt256 value, JsonSerializerOptions options)
    {
        Span<char> hexBuffer = stackalloc char[66];
        hexBuffer[0] = '0';
        hexBuffer[1] = 'x';

        Span<byte> byteBuffer = stackalloc byte[32];

        BinaryPrimitives.WriteUInt256BigEndian(byteBuffer, value);

        byteBuffer = byteBuffer.Trim((byte) 0);

        if(byteBuffer.Length == 0)
        {
            writer.WriteStringValue("0x0");
            return;
        }

        if(!Convert.TryToHexString(byteBuffer, hexBuffer[2..], out int charsWritten))
        {
            throw new InvalidOperationException("Failed to convert to hex");
        }

        writer.WriteStringValue(hexBuffer[0..(2 + charsWritten)]);
    }
}