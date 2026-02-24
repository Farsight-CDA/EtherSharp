using EtherSharp.Types;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EtherSharp.Common.Converter;

/// <summary>
/// Converts <see cref="Hash32"/> values to and from 0x-prefixed hex strings.
/// </summary>
public class Hash32Converter : JsonConverter<Hash32>
{
    /// <inheritdoc/>
    public override Hash32 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => reader.TokenType == JsonTokenType.String
            ? Hash32.Parse(reader.GetString() ?? throw new InvalidOperationException("Cannot parse null hash"))
            : throw new JsonException($"Expected string token for {nameof(Hash32)}");

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, Hash32 value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.ToString());
}
