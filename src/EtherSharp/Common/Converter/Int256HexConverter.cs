using EtherSharp.Numerics;
using System.Buffers.Binary;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EtherSharp.Common.Converter;

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
        => WriteCore(writer, value, isPropertyName: false);

    /// <inheritdoc/>
    public override void WriteAsPropertyName(Utf8JsonWriter writer, Int256 value, JsonSerializerOptions options)
        => WriteCore(writer, value, isPropertyName: true);

    private static Int256 ReadCore(scoped ref Utf8JsonReader reader, JsonTokenType tokenType)
    {
        if(reader.TokenType != tokenType)
        {
            throw new JsonException($"Cannot parse {nameof(Int256)} from token of type {reader.TokenType}");
        }

        int valueLength = reader.HasValueSequence
            ? (int) reader.ValueSequence.Length
            : reader.ValueSpan.Length;

        if(valueLength > 68)
        {
            throw new InvalidOperationException("Unexpected number length");
        }

        Span<char> sourceBuffer = stackalloc char[valueLength + 1];
        int charsWritten = reader.CopyString(sourceBuffer[1..]);

        if(charsWritten > 66)
        {
            throw new JsonException("Unexpected number length");
        }

        int startIndex = 1;

        if(sourceBuffer[1..].StartsWith("0x"))
        {
            startIndex += 2;
            charsWritten -= 2;
        }

        if(charsWritten % 2 != 0)
        {
            startIndex--;
            charsWritten++;
            sourceBuffer[startIndex] = '0';
        }

        return !Int256.TryParseFromHex(sourceBuffer[startIndex..(startIndex + charsWritten)], out var result)
            ? throw new JsonException($"Failed parsing {nameof(Int256)}")
            : result;
    }

    private static void WriteCore(Utf8JsonWriter writer, Int256 value, bool isPropertyName)
    {
        Span<char> hexBuffer = stackalloc char[66];
        int charsWritten = FormatHex(value, hexBuffer);

        if(isPropertyName)
        {
            writer.WritePropertyName(hexBuffer[..charsWritten]);
            return;
        }

        writer.WriteStringValue(hexBuffer[..charsWritten]);
    }

    private static int FormatHex(Int256 value, scoped Span<char> hexBuffer)
    {
        if(value == 0)
        {
            "0x0".AsSpan().CopyTo(hexBuffer);
            return 3;
        }

        Span<byte> byteBuffer = stackalloc byte[32];

        BinaryPrimitives.WriteInt256BigEndian(byteBuffer, value);

        byteBuffer = byteBuffer.TrimStart((byte) 0);

        int dataIndex = byteBuffer[0] < 16 ? 1 : 2;
        int charCount = (byteBuffer.Length * 2) + dataIndex;
        Span<char> destination = hexBuffer[..charCount];

        if(!Convert.TryToHexString(byteBuffer, destination[dataIndex..], out _))
        {
            throw new InvalidOperationException("Failed to convert to hex");
        }

        destination[0] = '0';
        destination[1] = 'x';

        return charCount;
    }
}
