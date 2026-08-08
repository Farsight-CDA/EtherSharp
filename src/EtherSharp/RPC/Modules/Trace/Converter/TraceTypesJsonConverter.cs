using EtherSharp.RPC.Modules.Trace.Types;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EtherSharp.RPC.Modules.Trace.Converter;

internal sealed class TraceTypesJsonConverter : JsonConverter<TraceTypes>
{
    public override TraceTypes Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => throw new NotSupportedException($"Deserializing {nameof(TraceTypes)} is not supported.");

    public override void Write(Utf8JsonWriter writer, TraceTypes value, JsonSerializerOptions options)
    {
        Validate(value);
        writer.WriteStartArray();

        if((value & TraceTypes.Trace) != 0)
        {
            writer.WriteStringValue("trace");
        }
        if((value & TraceTypes.VmTrace) != 0)
        {
            writer.WriteStringValue("vmTrace");
        }
        if((value & TraceTypes.StateDiff) != 0)
        {
            writer.WriteStringValue("stateDiff");
        }

        writer.WriteEndArray();
    }

    private static void Validate(TraceTypes value)
    {
        if(value == 0 || (value & ~TraceTypes.All) != 0)
        {
            throw new JsonException($"Invalid {nameof(TraceTypes)} value '{value}'.");
        }
    }
}
