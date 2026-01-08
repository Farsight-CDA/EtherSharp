using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EtherSharp.Common.Converter;

internal class LongHexConverter : JsonConverter<long>
{
    public override long Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch(reader.TokenType)
        {
            case JsonTokenType.Number:
                return reader.GetInt64();
            case JsonTokenType.String:
                int valueLength = reader.HasValueSequence
                    ? (int) reader.ValueSequence.Length
                    : reader.ValueSpan.Length;

                if(valueLength > 20)
                {
                    throw new InvalidOperationException("Unexpected number length");
                }

                Span<char> sourceBuffer = stackalloc char[valueLength];
                int charsWritten = reader.CopyString(sourceBuffer);

                return charsWritten > 18
                    ? throw new InvalidOperationException("Unexpected number length")
                    : Int64.Parse(sourceBuffer[2..charsWritten], NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            default:
                throw new JsonException($"Cannot parse {nameof(Int64)} from token of type {reader.TokenType}");
        }
    }

    public override void Write(Utf8JsonWriter writer, long value, JsonSerializerOptions options)
    {
        Span<char> buffer = stackalloc char[18];
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
