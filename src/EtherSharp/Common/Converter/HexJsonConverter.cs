using System.Buffers;
using System.Text.Json;

namespace EtherSharp.Common.Converter;

internal static class HexJsonConverter
{
    public static void ReadBytes(scoped ref Utf8JsonReader reader, scoped Span<byte> buffer, string typeName)
    {
        if(reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Expected string token for {typeName}");
        }

        int unprefixedCharCount = buffer.Length * 2;
        int prefixedCharCount = unprefixedCharCount + 2;
        int charCount = reader.HasValueSequence
            ? checked((int) reader.ValueSequence.Length)
            : reader.ValueSpan.Length;
        if(charCount != unprefixedCharCount && charCount != prefixedCharCount)
        {
            throw new JsonException($"Expected a {buffer.Length}-byte hex string for {typeName}");
        }

        Span<char> chars = stackalloc char[prefixedCharCount];
        int written = reader.CopyString(chars);
        ReadOnlySpan<char> hex = chars[..written];

        if(hex.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
        {
            hex = hex[2..];
        }

        var status = Convert.FromHexString(hex, buffer, out int charsConsumed, out int bytesWritten);
        if(status != OperationStatus.Done
            || charsConsumed != unprefixedCharCount
            || bytesWritten != buffer.Length)
        {
            throw new JsonException($"Expected a valid {buffer.Length}-byte hex string for {typeName}");
        }
    }

    public static void WriteBytes(Utf8JsonWriter writer, ReadOnlySpan<byte> bytes)
    {
        Span<char> buffer = stackalloc char[(bytes.Length * 2) + 2];
        buffer[0] = '0';
        buffer[1] = 'x';

        if(!Convert.TryToHexString(bytes, buffer[2..], out int charsWritten))
        {
            throw new InvalidOperationException("Failed to write hex string.");
        }

        writer.WriteStringValue(buffer[..(charsWritten + 2)]);
    }
}
