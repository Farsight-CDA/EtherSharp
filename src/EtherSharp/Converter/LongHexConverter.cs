using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EtherSharp.Converter;
internal class LongHexConverter : JsonConverter<long>
{
    public override long Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)

        => long.Parse((reader.GetString() ?? throw new InvalidOperationException("Null is not a long")).AsSpan()[2..], NumberStyles.HexNumber);

    public override void Write(Utf8JsonWriter writer, long value, JsonSerializerOptions options)
        => writer.WriteStringValue($"0x{value:X}");
}
