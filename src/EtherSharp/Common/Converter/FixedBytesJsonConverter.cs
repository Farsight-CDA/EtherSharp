using EtherSharp.Types;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EtherSharp.Common.Converter;

/// <summary>
/// Converts fixed-width byte values to and from 0x-prefixed hex strings.
/// </summary>
public abstract class FixedBytesJsonConverter<TBytes> : JsonConverter<TBytes>
    where TBytes : struct, IFixedBytes<TBytes>
{
    /// <inheritdoc/>
    public override TBytes Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        Span<byte> bytes = stackalloc byte[TBytes.BYTE_LENGTH];
        HexJsonConverter.ReadBytes(ref reader, bytes, typeof(TBytes).Name);
        return TBytes.FromBytes(bytes);
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, TBytes value, JsonSerializerOptions options)
        => WriteUnsafe(writer, value);

    /// <summary>
    /// Writes the value using its internal byte span without copying.
    /// </summary>
    protected abstract void WriteUnsafe(Utf8JsonWriter writer, TBytes value);
}
