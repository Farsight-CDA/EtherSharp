using System.Buffers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EtherSharp.Common.Converters.Json;

/// <summary>
/// Converts a hex-encoded <see cref="Byte"/> array to or from JSON.
/// Odd-length hex strings are read as if prefixed with a zero nibble.
/// </summary>
public sealed class HexStringByteArrayConverter : JsonConverter<byte[]>
{
    private const int STACK_BUFFER_SIZE = 2048;

    /// <summary>
    /// Shared converter instance.
    /// </summary>
    public static HexStringByteArrayConverter Instance { get; } = new();

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

        if(reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException("Expected a hex-encoded string.");
        }

        if(!reader.ValueIsEscaped && !reader.HasValueSequence)
        {
            var hex = reader.ValueSpan.StartsWith("0x"u8) || reader.ValueSpan.StartsWith("0X"u8)
                ? reader.ValueSpan[2..]
                : reader.ValueSpan;
            if(hex.Length % 2 == 0)
            {
                byte[] bytes = hex.IsEmpty
                    ? []
                    : GC.AllocateUninitializedArray<byte>(hex.Length / 2);
                return HexUtils.TryDecodeHex(hex, bytes)
                    ? bytes
                    : throw new JsonException("Expected a valid hex-encoded string.");
            }
        }

        int length = reader.HasValueSequence
            ? (int) reader.ValueSequence.Length
            : reader.ValueSpan.Length;
        int bufferLength = length + 1;

        char[]? rented = null;
        var buffer = bufferLength <= STACK_BUFFER_SIZE
            ? stackalloc char[bufferLength]
            : (rented = ArrayPool<char>.Shared.Rent(bufferLength));

        try
        {
            int written = reader.CopyString(buffer[1..]);
            buffer = buffer[..(written + 1)];
            int start = buffer[1..].StartsWith("0x", StringComparison.OrdinalIgnoreCase) ? 3 : 1;

            if((buffer.Length - start) % 2 != 0)
            {
                buffer[--start] = '0';
            }

            var hex = buffer[start..];
            byte[] bytes = hex.IsEmpty
                ? []
                : GC.AllocateUninitializedArray<byte>(hex.Length / 2);
            return HexUtils.TryDecodeHex(hex, bytes)
                ? bytes
                : throw new JsonException("Expected a valid hex-encoded string.");
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
        => WriteHexStringValue(writer, value);

    internal static void WriteHexStringValue(Utf8JsonWriter writer, ReadOnlySpan<byte> value)
    {
        if(value.Length < STACK_BUFFER_SIZE)
        {
            int byteCount = (value.Length * 2) + 2;
            Span<byte> byteBuffer = stackalloc byte[byteCount];

            byteBuffer[0] = (byte) '0';
            byteBuffer[1] = (byte) 'x';

            if(Convert.TryToHexString(value, byteBuffer[2..], out int written))
            {
                writer.WriteStringValue(byteBuffer[..(2 + written)]);
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

            if(!Convert.TryToHexString(value.Slice(offset, byteCount), buffer[prefixLength..], out int written))
            {
                throw new InvalidOperationException("Failed to format hex.");
            }

            offset += byteCount;
            writer.WriteStringValueSegment(buffer[..(prefixLength + written)], offset == value.Length);
        }
        while(offset < value.Length);
    }
}
