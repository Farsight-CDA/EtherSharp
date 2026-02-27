using EtherSharp.Types;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EtherSharp.Common.Converter;

/// <summary>
/// Converts <see cref="Bytes32"/> values to and from 0x-prefixed hex strings.
/// </summary>
public class Bytes32Converter : JsonConverter<Bytes32>
{
    /// <inheritdoc/>
    public override Bytes32 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => reader.TokenType == JsonTokenType.String
            ? Bytes32.Parse(reader.GetString() ?? throw new InvalidOperationException("Cannot parse null bytes32"))
            : throw new JsonException($"Expected string token for {nameof(Bytes32)}");

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, Bytes32 value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.ToString());
}
