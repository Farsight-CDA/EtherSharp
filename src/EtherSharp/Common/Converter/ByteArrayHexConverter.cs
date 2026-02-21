using System.Buffers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EtherSharp.Common.Converter;

/// <summary>
/// Converts a hex-encoded <see cref="Byte"/> array to or from JSON.
/// </summary>
internal class ByteArrayHexConverter : JsonConverter<byte[]>
{
    /// <inheritdoc/>
    public override byte[]? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        if(reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        int length = reader.HasValueSequence
            ? (int) reader.ValueSequence.Length
            : reader.ValueSpan.Length;

        char[]? rented = null;
        var buffer = length <= 2048
            ? stackalloc char[length]
            : (rented = ArrayPool<char>.Shared.Rent(length));

        try
        {
            int written = reader.CopyString(buffer);
            ReadOnlySpan<char> hex = buffer[..written];

            if(hex.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                hex = hex[2..];
            }

            return Convert.FromHexString(hex);
        }
        finally
        {
            if(rented is not null)
            {
                ArrayPool<char>.Shared.Return(rented);
            }
        }
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, byte[] value, JsonSerializerOptions options)
    {
        int charCount = (value.Length * 2) + 2;
        char[]? rented = null;
        var buffer = charCount <= 4096
            ? stackalloc char[charCount]
            : (rented = ArrayPool<char>.Shared.Rent(charCount));

        try
        {
            buffer[0] = '0';
            buffer[1] = 'x';

            if(Convert.TryToHexString(value, buffer[2..], out int written))
            {
                writer.WriteStringValue(buffer[..(2 + written)]);
            }
            else
            {
                throw new InvalidOperationException("Failed to format hex.");
            }
        }
        finally
        {
            if(rented != null)
            {
                ArrayPool<char>.Shared.Return(rented);
            }
        }
    }
}
