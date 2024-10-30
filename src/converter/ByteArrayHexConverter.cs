using System.Text.Json;
using System.Text.Json.Serialization;

namespace EVM.net.converter;
internal class ByteArrayHexConverter : JsonConverter<byte[]>
{
    public override byte[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
      => Convert.FromHexString((reader.GetString() ?? throw new InvalidOperationException("Null is not a byte[]")).AsSpan()[2..]);

    public override void Write(Utf8JsonWriter writer, byte[] value, JsonSerializerOptions options)
        => writer.WriteStringValue($"0x{Convert.ToHexString(value)}");
}
