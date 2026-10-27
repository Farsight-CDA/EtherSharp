using EtherSharp.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EtherSharp.Common.Converters.Json;

/// <summary>
/// Converts an <see cref="Int256"/> to or from a hex-encoded JSON string.
/// </summary>
public sealed class Int256HexConverter : JsonConverter<Int256>
{
    /// <summary>
    /// Shared converter instance.
    /// </summary>
    public static Int256HexConverter Instance { get; } = new();

    /// <inheritdoc/>
    public override Int256 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => ReadCore(ref reader, JsonTokenType.String);

    /// <inheritdoc/>
    public override Int256 ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => ReadCore(ref reader, JsonTokenType.PropertyName);

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, Int256 value, JsonSerializerOptions options)
        => UInt256HexConverter.WriteCore(writer, unchecked((UInt256) value), isPropertyName: false);

    /// <inheritdoc/>
    public override void WriteAsPropertyName(Utf8JsonWriter writer, Int256 value, JsonSerializerOptions options)
        => UInt256HexConverter.WriteCore(writer, unchecked((UInt256) value), isPropertyName: true);

    private static Int256 ReadCore(scoped ref Utf8JsonReader reader, JsonTokenType tokenType)
    {
        if(reader.TokenType != tokenType)
        {
            throw new JsonException($"Cannot parse {nameof(Int256)} from token of type {reader.TokenType}");
        }

        long valueLength = reader.HasValueSequence
            ? reader.ValueSequence.Length
            : reader.ValueSpan.Length;

        // Each of the 66 decoded characters can occupy six bytes as a JSON Unicode escape.
        if(valueLength > 66 * 6)
        {
            throw new JsonException("Unexpected number length");
        }

        Span<char> sourceBuffer = stackalloc char[(int) valueLength];
        int charsWritten = reader.CopyString(sourceBuffer);

        if(charsWritten > 66)
        {
            throw new JsonException("Unexpected number length");
        }
        //
        return !Int256.TryParseFromHex(sourceBuffer[..charsWritten], out var result)
            ? throw new JsonException($"'{reader.GetString()}' is not a valid hexadecimal {nameof(Int256)} value.")
            : result;
    }
}
