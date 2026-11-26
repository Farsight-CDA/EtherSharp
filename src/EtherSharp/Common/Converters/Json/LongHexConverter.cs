using System.Buffers.Binary;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EtherSharp.Common.Converters.Json;

/// <summary>
/// Converts a <see cref="Int64"/> to or from a hex-encoded JSON string.
/// </summary>
public sealed class LongHexConverter : JsonConverter<long>
{
    /// <summary>
    /// Gets the shared converter instance.
    /// </summary>
    public static LongHexConverter Instance { get; } = new();

    /// <inheritdoc/>
    public override long Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => reader.TokenType switch
        {
            JsonTokenType.Number => reader.GetInt64(),
            JsonTokenType.String => JsonHexNumberReader.Read<long>(ref reader, maxDigits: 16),
            _ => throw new JsonException($"Cannot parse {nameof(Int64)} from token of type {reader.TokenType}")
        };

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, long value, JsonSerializerOptions options)
    {
        if(value == 0)
        {
            writer.WriteStringValue("0x0");
            return;
        }

        Span<byte> byteBuffer = stackalloc byte[sizeof(long)];

        BinaryPrimitives.WriteInt64BigEndian(byteBuffer, value);

        byteBuffer = byteBuffer.TrimStart((byte) 0);

        int dataIndex = byteBuffer[0] < 16 ? 1 : 2;
        Span<char> hexBuffer = stackalloc char[(byteBuffer.Length * 2) + dataIndex];

        if(!Convert.TryToHexString(byteBuffer, hexBuffer[dataIndex..], out _))
        {
            throw new InvalidOperationException("Failed to convert to hex");
        }

        hexBuffer[0] = '0';
        hexBuffer[1] = 'x';

        writer.WriteStringValue(hexBuffer);
    }
}
