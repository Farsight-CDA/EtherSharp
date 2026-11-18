using System.Buffers.Binary;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EtherSharp.Common.Converters.Json;

/// <summary>
/// Converts a <see cref="UInt32"/> to or from a hex-encoded JSON string.
/// </summary>
public sealed class UIntHexConverter : JsonConverter<uint>
{
    /// <summary>
    /// Gets the shared converter instance.
    /// </summary>
    public static UIntHexConverter Instance { get; } = new();

    /// <inheritdoc/>
    public override uint Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => reader.TokenType switch
        {
            JsonTokenType.Number => reader.GetUInt32(),
            JsonTokenType.String => JsonHexNumberReader.Read<uint>(ref reader, maxDigits: 8),
            _ => throw new JsonException($"Cannot parse {nameof(UInt32)} from token of type {reader.TokenType}")
        };

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, uint value, JsonSerializerOptions options)
    {
        if(value == 0)
        {
            writer.WriteStringValue("0x0");
            return;
        }

        Span<byte> byteBuffer = stackalloc byte[sizeof(uint)];

        BinaryPrimitives.WriteUInt32BigEndian(byteBuffer, value);

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
