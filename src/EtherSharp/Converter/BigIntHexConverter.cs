using System.Globalization;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EtherSharp.Converter;

internal class BigIntHexConverter : JsonConverter<BigInteger>
{
    public override BigInteger Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => BigInteger.Parse((reader.GetString() ?? throw new InvalidOperationException("Null is not a BigInteger")).AsSpan()[2..], NumberStyles.HexNumber);

    public override void Write(Utf8JsonWriter writer, BigInteger value, JsonSerializerOptions options)
        => writer.WriteStringValue($"0x{value:X}");
}
