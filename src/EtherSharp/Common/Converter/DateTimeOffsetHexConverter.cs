using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EtherSharp.Common.Converter;

/// <summary>
/// Converts a <see cref="DateTimeOffset"/> value to or from a hex-encoded Unix timestamp JSON string.
/// </summary>
internal class DateTimeOffsetHexConverter : JsonConverter<DateTimeOffset>
{
    /// <inheritdoc/>
    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if(reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException("Expected string token.");
        }
        int valueLength = reader.HasValueSequence
            ? (int) reader.ValueSequence.Length
            : reader.ValueSpan.Length;

        if(valueLength > 20)
        {
            throw new InvalidOperationException("Unexpected number length");
        }

        Span<char> sourceBuffer = stackalloc char[valueLength];
        int charsWritten = reader.CopyString(sourceBuffer);

        long utcTimestamp = charsWritten > 18
            ? throw new InvalidOperationException("Unexpected number length")
            : Int64.Parse(sourceBuffer[2..charsWritten], NumberStyles.HexNumber, CultureInfo.InvariantCulture);

        return DateTimeOffset.FromUnixTimeSeconds(utcTimestamp);
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
    {
        Span<char> buffer = stackalloc char[10];
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
