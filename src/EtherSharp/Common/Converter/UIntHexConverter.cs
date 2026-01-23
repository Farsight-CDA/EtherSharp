using System.Buffers.Binary;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EtherSharp.Common.Converter;

internal class UIntHexConverter : JsonConverter<uint>
{
    public override uint Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch(reader.TokenType)
        {
            case JsonTokenType.Number:
                return reader.GetUInt32();
            case JsonTokenType.String:
                int valueLength = reader.HasValueSequence
                    ? (int) reader.ValueSequence.Length
                    : reader.ValueSpan.Length;

                if(valueLength > 12)
                {
                    throw new InvalidOperationException("Unexpected number length");
                }

                Span<char> sourceBuffer = stackalloc char[valueLength];
                int charsWritten = reader.CopyString(sourceBuffer);

                return charsWritten > 10
                    ? throw new InvalidOperationException("Unexpected number length")
                    : UInt32.Parse(sourceBuffer[2..charsWritten], NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            default:
                throw new JsonException($"Cannot parse {nameof(UInt32)} from token of type {reader.TokenType}");
        }
    }

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