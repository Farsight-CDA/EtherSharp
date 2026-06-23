using System.Buffers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EtherSharp.Common.Converter;

/// <summary>
/// Converts a hex-encoded <see cref="Byte"/> array to or from JSON.
/// </summary>
public sealed class ByteArrayHexConverter : JsonConverter<byte[]>
{
    private const int STACK_BUFFER_SIZE = 2048;

    /// <summary>
    /// Shared converter instance.
    /// </summary>
    public static ByteArrayHexConverter Instance { get; } = new();

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

        if(!reader.ValueIsEscaped && !reader.HasValueSequence)
        {
            return reader.ValueSpan.StartsWith("0x"u8) || reader.ValueSpan.StartsWith("0X"u8)
                ? Convert.FromHexString(reader.ValueSpan[2..])
                : Convert.FromHexString(reader.ValueSpan);
        }

        int length = reader.HasValueSequence
            ? (int) reader.ValueSequence.Length
            : reader.ValueSpan.Length;

        char[]? rented = null;
        var buffer = length <= STACK_BUFFER_SIZE
            ? stackalloc char[length]
            : (rented = ArrayPool<char>.Shared.Rent(length));

        try
        {
            int written = reader.CopyString(buffer);
            ReadOnlySpan<char> hex = buffer[..written];

            return hex.StartsWith("0x", StringComparison.OrdinalIgnoreCase)
                ? Convert.FromHexString(hex[2..])
                : Convert.FromHexString(hex);
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
        if(value.Length < STACK_BUFFER_SIZE)
        {
            int charCount = (value.Length * 2) + 2;
            Span<char> charBuffer = stackalloc char[charCount];

            charBuffer[0] = '0';
            charBuffer[1] = 'x';

            if(Convert.TryToHexString(value, charBuffer[2..], out int written))
            {
                writer.WriteStringValue(charBuffer[..(2 + written)]);
            }
            else
            {
                throw new InvalidOperationException("Failed to format hex.");
            }

            return;
        }

        Span<byte> buffer = stackalloc byte[STACK_BUFFER_SIZE];
        int offset = 0;
        bool first = true;

        do
        {
            int prefixLength = first ? 2 : 0;

            if(first)
            {
                buffer[0] = (byte) '0';
                buffer[1] = (byte) 'x';
                first = false;
            }

            int byteCount = Math.Min(value.Length - offset, (STACK_BUFFER_SIZE - prefixLength) / 2);

            if(!Convert.TryToHexString(value.AsSpan(offset, byteCount), buffer[prefixLength..], out int written))
            {
                throw new InvalidOperationException("Failed to format hex.");
            }

            offset += byteCount;
            writer.WriteStringValueSegment(buffer[..(prefixLength + written)], offset == value.Length);
        }
        while(offset < value.Length);
    }
}
