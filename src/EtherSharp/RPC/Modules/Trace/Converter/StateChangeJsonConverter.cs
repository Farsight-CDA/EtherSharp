using EtherSharp.RPC.Modules.Trace.Types;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EtherSharp.RPC.Modules.Trace.Converter;

/// <summary>
/// Converts tagged state-change values returned by <c>trace_*</c> methods.
/// </summary>
internal sealed class StateChangeJsonConverter<T>(JsonConverter<T> valueConverter) : JsonConverter<StateChange<T>>
{
    private readonly JsonConverter<T> _valueConverter = valueConverter;

    public override StateChange<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if(reader.TokenType == JsonTokenType.String && reader.ValueTextEquals("="u8))
        {
            return new StateChange<T>.Same();
        }

        if(reader.TokenType != JsonTokenType.StartObject || !reader.Read())
        {
            throw new JsonException("State change must be '=' or an object");
        }
        if(reader.TokenType == JsonTokenType.EndObject)
        {
            throw new JsonException("State change object is empty");
        }
        if(reader.TokenType != JsonTokenType.PropertyName)
        {
            throw new JsonException("State change variant must be a property");
        }

        StateChange<T> stateChange;
        if(reader.ValueTextEquals("+"u8))
        {
            if(!reader.Read())
            {
                throw new JsonException("State change variant has no value");
            }
            stateChange = new StateChange<T>.Added(
                _valueConverter.Read(ref reader, typeof(T), options)
                    ?? throw new JsonException("State change value is null")
            );
        }
        else if(reader.ValueTextEquals("-"u8))
        {
            if(!reader.Read())
            {
                throw new JsonException("State change variant has no value");
            }
            stateChange = new StateChange<T>.Removed(
                _valueConverter.Read(ref reader, typeof(T), options)
                    ?? throw new JsonException("State change value is null")
            );
        }
        else if(reader.ValueTextEquals("*"u8))
        {
            if(!reader.Read())
            {
                throw new JsonException("State change variant has no value");
            }
            stateChange = ReadChanged(ref reader, _valueConverter, options);
        }
        else
        {
            throw new JsonException($"Unknown state change variant '{reader.GetString()}'");
        }

        return !reader.Read() || reader.TokenType != JsonTokenType.EndObject
            ? throw new JsonException("State change object contains multiple variants")
            : stateChange;
    }

    public override void Write(Utf8JsonWriter writer, StateChange<T> value, JsonSerializerOptions options)
        => throw new NotSupportedException($"Serializing {nameof(StateChange<>)} is not supported.");

    private static StateChange<T>.Changed ReadChanged(
        ref Utf8JsonReader reader,
        JsonConverter<T> valueConverter,
        JsonSerializerOptions options
    )
    {
        if(reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException("Changed state value must be an object");
        }

        T? from = default;
        T? to = default;
        bool hasFrom = false;
        bool hasTo = false;

        while(reader.Read() && reader.TokenType != JsonTokenType.EndObject)
        {
            if(reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException("Changed state fields must be properties");
            }

            if(reader.ValueTextEquals("from"u8))
            {
                if(hasFrom)
                {
                    throw new JsonException("Changed state value contains multiple 'from' fields");
                }
                if(!reader.Read())
                {
                    throw new JsonException("Changed state field has no value");
                }
                from = valueConverter.Read(ref reader, typeof(T), options)
                    ?? throw new JsonException("State change 'from' value is null");
                hasFrom = true;
            }
            else if(reader.ValueTextEquals("to"u8))
            {
                if(hasTo)
                {
                    throw new JsonException("Changed state value contains multiple 'to' fields");
                }
                if(!reader.Read())
                {
                    throw new JsonException("Changed state field has no value");
                }
                to = valueConverter.Read(ref reader, typeof(T), options)
                    ?? throw new JsonException("State change 'to' value is null");
                hasTo = true;
            }
            else
            {
                if(!reader.Read())
                {
                    throw new JsonException("Changed state field has no value");
                }
                reader.Skip();
            }
        }

        return reader.TokenType != JsonTokenType.EndObject || !hasFrom || !hasTo
            ? throw new JsonException("Changed state value must contain 'from' and 'to'")
            : new StateChange<T>.Changed(from!, to!);
    }
}
