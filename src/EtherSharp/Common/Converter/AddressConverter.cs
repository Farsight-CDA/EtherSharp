using EtherSharp.Types;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EtherSharp.Common.Converter;
public class AddressConverter : JsonConverter<Address>
{
    public override Address? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => Address.FromString(reader.GetString() ?? throw new InvalidOperationException("Cannot read null as an address"));
    public override void Write(Utf8JsonWriter writer, Address value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.String);
}
