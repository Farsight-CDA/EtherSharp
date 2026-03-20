using EtherSharp.Types;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EtherSharp.Common.Converter;

/// <summary>
/// Converts an <see cref="Address"/> to or from JSON.
/// </summary>
public sealed class AddressConverter : JsonConverter<Address>
{
    /// <summary>
    /// Shared converter instance.
    /// </summary>
    public static AddressConverter Instance { get; } = new();

    /// <inheritdoc/>
    public override Address Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        Span<byte> buffer = stackalloc byte[Address.BYTES_LENGTH];
        HexJsonConverter.ReadBytes(ref reader, buffer, nameof(Address));
        return Address.FromBytes(buffer);
    }

    /// <inheritdoc/>
    public override Address ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        Span<byte> buffer = stackalloc byte[Address.BYTES_LENGTH];
        HexJsonConverter.ReadPropertyNameBytes(ref reader, buffer, nameof(Address));
        return Address.FromBytes(buffer);
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, Address value, JsonSerializerOptions options)
        => HexJsonConverter.WriteBytes(writer, value.DangerousGetReadOnlySpan());

    /// <inheritdoc/>
    public override void WriteAsPropertyName(Utf8JsonWriter writer, Address value, JsonSerializerOptions options)
        => HexJsonConverter.WritePropertyNameBytes(writer, value.DangerousGetReadOnlySpan());
}
