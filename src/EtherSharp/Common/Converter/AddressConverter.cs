using EtherSharp.Types;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EtherSharp.Common.Converter;

/// <summary>
/// Converts an <see cref="Address"/> to or from JSON.
/// </summary>
public class AddressConverter : JsonConverter<Address>
{
    /// <inheritdoc/>
    public override Address? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => Address.FromString(reader.GetString() ?? throw new InvalidOperationException("Cannot read null as an address"));
    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, Address value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.String);
}
