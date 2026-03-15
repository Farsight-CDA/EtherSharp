using EtherSharp.Types;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EtherSharp.Common.Converter;

/// <summary>
/// Converts a <see cref="TxType"/> value to or from a JSON number or hex-encoded string.
/// </summary>
public class TransactionTypeHexConverter : JsonConverter<TxType>
{
    /// <summary>
    /// Shared converter instance.
    /// </summary>
    public static TransactionTypeHexConverter Instance { get; } = new();

    /// <inheritdoc/>
    public override TxType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => reader.TokenType switch
        {
            JsonTokenType.Number => (TxType) reader.GetUInt32(),
            JsonTokenType.String => Parse(reader.GetString() ?? throw new InvalidOperationException("Cannot parse null as transaction type")),
            _ => throw new JsonException($"Cannot parse {nameof(TxType)} from token of type {reader.TokenType}")
        };

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, TxType value, JsonSerializerOptions options)
    {
        uint numericValue = (uint) value;
        writer.WriteStringValue($"0x{numericValue:x}");
    }

    private static TxType Parse(string value)
        => value.StartsWith("0x", StringComparison.OrdinalIgnoreCase)
            ? (TxType) UInt32.Parse(value.AsSpan(2), NumberStyles.HexNumber, CultureInfo.InvariantCulture)
            : (TxType) UInt32.Parse(value, NumberStyles.Integer, CultureInfo.InvariantCulture);
}
