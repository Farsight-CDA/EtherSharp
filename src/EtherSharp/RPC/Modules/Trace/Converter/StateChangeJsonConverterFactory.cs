using EtherSharp.RPC.Modules.Trace.Types;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EtherSharp.RPC.Modules.Trace.Converter;

/// <summary>
/// Converts the tagged state-change values returned by <c>trace_*</c> methods.
/// </summary>
internal sealed class StateChangeJsonConverterFactory : JsonConverterFactory
{
    /// <inheritdoc/>
    public override bool CanConvert(Type typeToConvert)
        => typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition() == typeof(StateChange<>);

    /// <inheritdoc/>
    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        => (JsonConverter) Activator.CreateInstance(
            typeof(StateChangeJsonConverter<>).MakeGenericType(typeToConvert.GetGenericArguments()[0]))!;

    private sealed class StateChangeJsonConverter<T> : JsonConverter<StateChange<T>>
    {
        public override StateChange<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if(reader.TokenType == JsonTokenType.String && reader.GetString() == "=")
            {
                return new StateChange<T>.Same();
            }

            using var document = JsonDocument.ParseValue(ref reader);
            var properties = document.RootElement.EnumerateObject();
            if(!properties.MoveNext())
            {
                throw new JsonException("State change object is empty");
            }

            var property = properties.Current;
            _ = properties.MoveNext()
                ? throw new JsonException("State change object contains multiple variants")
                : false;

            return property.Name switch
            {
                "+" => new StateChange<T>.Added(
                    property.Value.Deserialize<T>(options)
                        ?? throw new JsonException("State change value is null")),
                "-" => new StateChange<T>.Removed(
                    property.Value.Deserialize<T>(options)
                        ?? throw new JsonException("State change value is null")),
                "*" => new StateChange<T>.Changed(
                    property.Value.GetProperty("from").Deserialize<T>(options)
                        ?? throw new JsonException("State change 'from' value is null"),
                    property.Value.GetProperty("to").Deserialize<T>(options)
                        ?? throw new JsonException("State change 'to' value is null")),
                _ => throw new JsonException($"Unknown state change variant '{property.Name}'")
            };
        }

        public override void Write(Utf8JsonWriter writer, StateChange<T> value, JsonSerializerOptions options)
            => throw new NotSupportedException($"Serializing {nameof(StateChange<>)} is not supported.");
    }
}
