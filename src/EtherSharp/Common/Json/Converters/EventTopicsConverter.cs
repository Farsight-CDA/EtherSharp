using EtherSharp.Types;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EtherSharp.Common.Json.Converters;

/// <summary>
/// Converts <see cref="EventTopics"/> to and from the EVM JSON-RPC topic-filter representation.
/// </summary>
public sealed class EventTopicsConverter : JsonConverter<EventTopics>
{
    /// <inheritdoc/>
    public override EventTopics Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if(reader.TokenType != JsonTokenType.StartArray)
        {
            throw new JsonException("Expected an array of EVM topic conditions.");
        }

        var topics = new Bytes32[]?[EventTopics.MAX_TOPIC_COUNT];
        int index = 0;
        while(reader.Read() && reader.TokenType != JsonTokenType.EndArray)
        {
            if(index == EventTopics.MAX_TOPIC_COUNT)
            {
                throw new JsonException("EVM log filters support at most four topic slots.");
            }

            topics[index++] = ReadSlot(ref reader, options);
        }

        if(reader.TokenType != JsonTokenType.EndArray)
        {
            throw new JsonException("Incomplete EVM topic conditions.");
        }

        try
        {
            return new EventTopics(topics[0], topics[1], topics[2], topics[3]);
        }
        catch(ArgumentException exception)
        {
            throw new JsonException("An EVM topic slot cannot be an empty array.", exception);
        }
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, EventTopics value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();

        int length = EventTopics.MAX_TOPIC_COUNT;
        while(length > 0 && value[length - 1].IsEmpty)
        {
            length--;
        }

        for(int i = 0; i < length; i++)
        {
            var slot = value[i];
            if(slot.IsEmpty)
            {
                writer.WriteNullValue();
                continue;
            }

            if(slot.Length == 1)
            {
                Bytes32Converter.Instance.Write(writer, slot[0], options);
                continue;
            }

            writer.WriteStartArray();
            foreach(var topic in slot)
            {
                Bytes32Converter.Instance.Write(writer, topic, options);
            }
            writer.WriteEndArray();
        }

        writer.WriteEndArray();
    }

    private static Bytes32[]? ReadSlot(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        if(reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }
        if(reader.TokenType == JsonTokenType.String)
        {
            return [Bytes32Converter.Instance.Read(ref reader, typeof(Bytes32), options)];
        }
        if(reader.TokenType != JsonTokenType.StartArray)
        {
            throw new JsonException("Expected a topic value, an array of topic values, or null.");
        }

        var values = new List<Bytes32>();
        while(reader.Read() && reader.TokenType != JsonTokenType.EndArray)
        {
            if(reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException("Expected a 32-byte topic value.");
            }

            values.Add(Bytes32Converter.Instance.Read(ref reader, typeof(Bytes32), options));
        }

        return reader.TokenType != JsonTokenType.EndArray
            ? throw new JsonException("Incomplete EVM topic alternatives array.")
            : [.. values];
    }
}
