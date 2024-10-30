using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EtherSharp.Converter;
internal class DateTimeOffsetHexConverter : JsonConverter<DateTimeOffset>
{
    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => new DateTimeOffset(long.Parse((reader.GetString() ?? throw new InvalidOperationException("Null is not a DateTimeOffset")).AsSpan()[2..], NumberStyles.HexNumber), TimeSpan.Zero);

    public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
        => writer.WriteStringValue($"0x{value:X}");
}