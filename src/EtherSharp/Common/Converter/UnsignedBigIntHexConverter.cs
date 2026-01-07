using System.Buffers;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EtherSharp.Common.Converter;

internal class UnsignedBigIntHexConverter : JsonConverter<BigInteger>
{
    public override BigInteger Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if(reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException("Expected string token.");
        }
        int valueLength = reader.HasValueSequence
            ? (int) reader.ValueSequence.Length
            : reader.ValueSpan.Length;

        if(valueLength > 80)
        {
            throw new InvalidOperationException("Unexpected number length");
        }

        Span<char> sourceBuffer = stackalloc char[valueLength];
        int charsWritten = reader.CopyString(sourceBuffer);

        int hexChars = charsWritten - 2;
        if(hexChars == 0 || hexChars > 64)
        {
            throw new InvalidOperationException("Unexpected number length");
        }

        bool isUneven = hexChars % 2 != 0;
        Span<char> rawHex = stackalloc char[isUneven ? hexChars + 1 : hexChars];

        if(isUneven)
        {
            rawHex[0] = '0';
            sourceBuffer[2..charsWritten].CopyTo(rawHex[1..]);
        }
        else
        {
            sourceBuffer[2..charsWritten].CopyTo(rawHex);
        }

        Span<byte> byteBuffer = stackalloc byte[rawHex.Length / 2];

        var status = Convert.FromHexString(rawHex, byteBuffer, out _, out _);

        return status != OperationStatus.Done
            ? throw new InvalidOperationException("Failed to parse resulting hex")
            : new BigInteger(byteBuffer, isUnsigned: true, isBigEndian: true);
    }

    public override void Write(Utf8JsonWriter writer, BigInteger value, JsonSerializerOptions options)
    {
        Span<char> buffer = stackalloc char[66];
        buffer[0] = '0';
        buffer[1] = 'x';

        if(value.TryFormat(buffer[2..], out int charsWritten, "X"))
        {
            writer.WriteStringValue(buffer[..(2 + charsWritten)]);
        }
        else
        {
            throw new FormatException("The value could not be formatted as hex.");
        }
    }
}