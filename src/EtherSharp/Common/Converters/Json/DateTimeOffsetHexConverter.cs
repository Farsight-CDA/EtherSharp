using System.Text.Json;
using System.Text.Json.Serialization;

namespace EtherSharp.Common.Converters.Json;

/// <summary>
/// Converts a <see cref="DateTimeOffset"/> value to or from a hex-encoded Unix timestamp JSON string.
/// </summary>
public sealed class DateTimeOffsetHexConverter : JsonConverter<DateTimeOffset>
{
    /// <summary>
    /// Gets the shared converter instance.
    /// </summary>
    public static DateTimeOffsetHexConverter Instance { get; } = new();

    /// <inheritdoc/>
    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if(reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException("Expected string token.");
        }

        long utcTimestamp = JsonHexNumberReader.Read<long>(ref reader, maxDigits: 16);
        return DateTimeOffset.FromUnixTimeSeconds(utcTimestamp);
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
    {
        Span<char> buffer = stackalloc char[18];
        buffer[0] = '0';
        buffer[1] = 'x';

        if(value.ToUnixTimeSeconds().TryFormat(buffer[2..], out int charsWritten, "X"))
        {
            writer.WriteStringValue(buffer[..(2 + charsWritten)]);
        }
        else
        {
            throw new FormatException("The value could not be formatted as hex.");
        }
    }
}
