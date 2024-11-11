using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EtherSharp.Common.Converter;
internal class IntHexConverter : JsonConverter<int>
{
    public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch(reader.TokenType)
        {
            case JsonTokenType.Number:
                return reader.GetInt32();

            case JsonTokenType.String:

                string s = reader.GetString() ?? throw new InvalidOperationException("Null is not an int");
                if(!s.StartsWith("0x"))
                {
                    throw new InvalidOperationException("Hex String Dos not star");
                }
                return int.Parse(s.AsSpan()[2..], NumberStyles.HexNumber);
            default:
                throw new NotImplementedException();
        }
    }

    public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
        => writer.WriteStringValue($"0x{value:X}");
}