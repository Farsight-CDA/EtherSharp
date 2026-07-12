using EtherSharp.Types;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EtherSharp.Common.Converter;

/// <summary>
/// Converts a <see cref="TxType"/> value to or from a JSON number or hex-encoded string.
/// </summary>
public sealed class TransactionTypeHexConverter : JsonConverter<TxType>
{
    /// <summary>
    /// Shared converter instance.
    /// </summary>
    public static TransactionTypeHexConverter Instance { get; } = new();

    /// <inheritdoc/>
    public override TxType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch(reader.TokenType)
        {
            case JsonTokenType.Number:
                return (TxType) reader.GetUInt32();
            case JsonTokenType.String:
                int valueLength = reader.HasValueSequence
                    ? checked((int) reader.ValueSequence.Length)
                    : reader.ValueSpan.Length;

                var value = valueLength <= 12
                    ? stackalloc char[valueLength]
                    : throw new JsonException("Unexpected transaction type length");
                int charsWritten = reader.CopyString(value);

                return Parse(value[..charsWritten]);
            default:
                throw new JsonException($"Cannot parse {nameof(TxType)} from token of type {reader.TokenType}");
        }
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, TxType value, JsonSerializerOptions options)
        => writer.WriteRawValue(value switch
        {
            TxType.Legacy => "\"0x0\""u8,
            TxType.EIP2930AccessList => "\"0x1\""u8,
            TxType.EIP1559DynamicFee => "\"0x2\""u8,
            TxType.EIP4844Blob => "\"0x3\""u8,
            TxType.EIP7702SetCode => "\"0x4\""u8,
            TxType.OPDeposit => "\"0x7e\""u8,
            _ => throw new JsonException($"Cannot write unsupported {nameof(TxType)} value {value}")
        }, skipInputValidation: true);

    private static TxType Parse(ReadOnlySpan<char> value)
        => value.StartsWith("0x", StringComparison.OrdinalIgnoreCase)
            ? (TxType) UInt32.Parse(value[2..], NumberStyles.HexNumber, CultureInfo.InvariantCulture)
            : (TxType) UInt32.Parse(value, NumberStyles.Integer, CultureInfo.InvariantCulture);
}
