using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EtherSharp.Common.Converter;

internal class DateTimeOffsetHexConverter : JsonConverter<DateTimeOffset>
{
    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string hexTimestamp = reader.GetString() ?? throw new InvalidOperationException("Null is not a DateTimeOffset");
        long utcTimestamp = Int64.Parse(hexTimestamp.AsSpan()[2..], NumberStyles.HexNumber, CultureInfo.InvariantCulture);
        return DateTimeOffset.FromUnixTimeSeconds(utcTimestamp);
    }

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