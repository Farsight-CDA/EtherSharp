using EtherSharp.Types;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EtherSharp.Common.Converter;

public class TargetBlockNumberConverter : JsonConverter<TargetBlockNumber>
{
    public override TargetBlockNumber Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => throw new NotSupportedException();
    public override void Write(Utf8JsonWriter writer, TargetBlockNumber value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.ToString());
}
