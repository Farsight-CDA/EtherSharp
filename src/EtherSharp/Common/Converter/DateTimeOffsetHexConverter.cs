using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EtherSharp.Common.Converter;
internal class DateTimeOffsetHexConverter : JsonConverter<DateTimeOffset>
{
    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string hexTimestamp = reader.GetString() ?? throw new InvalidOperationException("Null is not a DateTimeOffset");
        long utcTimestamp = long.Parse(hexTimestamp.AsSpan()[2..], NumberStyles.HexNumber, CultureInfo.InvariantCulture);
        return DateTimeOffset.FromUnixTimeSeconds(utcTimestamp);
    }

    public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
        => writer.WriteStringValue($"0x{value:X}");
}