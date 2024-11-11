using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EtherSharp.Common.Converter;
internal class UIntHexConverter : JsonConverter<uint>
{
    public override uint Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => uint.Parse((reader.GetString() ?? throw new InvalidOperationException("Null is not a uint")).AsSpan()[2..], NumberStyles.HexNumber);

    public override void Write(Utf8JsonWriter writer, uint value, JsonSerializerOptions options)
        => writer.WriteStringValue($"0x{value:X}");
}