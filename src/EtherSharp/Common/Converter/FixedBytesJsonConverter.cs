using EtherSharp.Types;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EtherSharp.Common.Converter;

/// <summary>
/// Converts fixed-width byte values to and from 0x-prefixed hex strings.
/// </summary>
public abstract class FixedBytesJsonConverter<TBytes> : JsonConverter<TBytes>
    where TBytes : struct, IFixedBytes<TBytes>
{
    /// <inheritdoc/>
    public override TBytes Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if(reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Expected string token for {typeof(TBytes).Name}");
        }

        int charCount = reader.HasValueSequence
            ? checked((int) reader.ValueSequence.Length)
            : reader.ValueSpan.Length;
        int unprefixedCharCount = TBytes.BYTE_LENGTH * 2;
        int prefixedCharCount = unprefixedCharCount + 2;

        if(charCount != unprefixedCharCount && charCount != prefixedCharCount)
        {
            throw new JsonException($"Expected a {TBytes.BYTE_LENGTH}-byte hex string for {typeof(TBytes).Name}");
        }

        Span<char> buffer = stackalloc char[prefixedCharCount];
        int written = reader.CopyString(buffer);
        ReadOnlySpan<char> hex = buffer[..written];

        if(hex.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
        {
            hex = hex[2..];
        }

        byte[] bytes;
        try
        {
            bytes = Convert.FromHexString(hex);
        }
        catch(FormatException ex)
        {
            throw new JsonException($"Expected a valid {TBytes.BYTE_LENGTH}-byte hex string for {typeof(TBytes).Name}", ex);
        }

        return bytes.Length == TBytes.BYTE_LENGTH
            ? TBytes.FromBytes(bytes)
            : throw new JsonException($"Expected a valid {TBytes.BYTE_LENGTH}-byte hex string for {typeof(TBytes).Name}");
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, TBytes value, JsonSerializerOptions options)
    {
        Span<char> buffer = stackalloc char[(TBytes.BYTE_LENGTH * 2) + 2];
        buffer[0] = '0';
        buffer[1] = 'x';

        if(!Convert.TryToHexString(value.Bytes, buffer[2..], out int charsWritten))
        {
            throw new InvalidOperationException("Failed to write hex string.");
        }

        writer.WriteStringValue(buffer[..(charsWritten + 2)]);
    }
}
