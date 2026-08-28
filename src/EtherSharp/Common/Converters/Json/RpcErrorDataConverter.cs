using System.Text.Json;
using System.Text.Json.Serialization;

namespace EtherSharp.Common.Converters.Json;

/// <summary>
/// Converts JSON-RPC error data to a string, preserving non-string values as raw JSON.
/// </summary>
public sealed class RpcErrorDataConverter : JsonConverter<string?>
{
    /// <summary>
    /// Gets the shared converter instance.
    /// </summary>
    public static RpcErrorDataConverter Instance { get; } = new();

    /// <inheritdoc/>
    public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => reader.TokenType switch
        {
            JsonTokenType.Null => null,
            JsonTokenType.String => reader.GetString(),
            _ => JsonElement.ParseValue(ref reader).GetRawText()
        };

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, string? value, JsonSerializerOptions options)
        => writer.WriteStringValue(value);
}
