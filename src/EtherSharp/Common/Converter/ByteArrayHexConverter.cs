using System.Buffers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EtherSharp.Common.Converter;

internal class ByteArrayHexConverter : JsonConverter<byte[]>
{
    public override byte[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
      => Convert.FromHexString((reader.GetString() ?? throw new InvalidOperationException("Null is not a byte[]")).AsSpan()[2..]);

    public override void Write(Utf8JsonWriter writer, byte[] value, JsonSerializerOptions options)
    {
        int charCount = (value.Length * 2) + 2;
        char[]? rented = null;
        var buffer = charCount <= 512
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
