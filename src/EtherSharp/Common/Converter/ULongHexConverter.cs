using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EtherSharp.Common.Converter;
internal class ULongHexConverter : JsonConverter<ulong>
{
    public override ulong Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)

        => ulong.Parse((reader.GetString() ?? throw new InvalidOperationException("Null is not a ulong")).AsSpan()[2..], NumberStyles.HexNumber);

    public override void Write(Utf8JsonWriter writer, ulong value, JsonSerializerOptions options)
        => writer.WriteStringValue($"0x{value:X}");
}
