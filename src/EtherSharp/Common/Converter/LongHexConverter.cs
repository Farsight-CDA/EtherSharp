using System.Buffers.Binary;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EtherSharp.Common.Converter;

internal class LongHexConverter : JsonConverter<long>
{
    public override long Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch(reader.TokenType)
        {
            case JsonTokenType.Number:
                return reader.GetInt64();
            case JsonTokenType.String:
                int valueLength = reader.HasValueSequence
                    ? (int) reader.ValueSequence.Length
                    : reader.ValueSpan.Length;

                if(valueLength > 20)
                {
                    throw new InvalidOperationException("Unexpected number length");
                }

                Span<char> sourceBuffer = stackalloc char[valueLength];
                int charsWritten = reader.CopyString(sourceBuffer);

                return charsWritten > 18
                    ? throw new InvalidOperationException("Unexpected number length")
                    : Int64.Parse(sourceBuffer[2..charsWritten], NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            default:
                throw new JsonException($"Cannot parse {nameof(Int64)} from token of type {reader.TokenType}");
        }
    }

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
