using System.Text.Json;
using System.Text.Json.Serialization;

namespace EtherSharp.Common.Converter;

/// <summary>
/// Converts hex-encoded <see cref="ReadOnlyMemory{T}"/> byte values to or from JSON.
/// </summary>
public sealed class HexStringReadOnlyMemoryByteConverter : JsonConverter<ReadOnlyMemory<byte>>
{
    /// <summary>
    /// Shared converter instance.
    /// </summary>
    public static HexStringReadOnlyMemoryByteConverter Instance { get; } = new();

    /// <inheritdoc/>
    public override ReadOnlyMemory<byte> Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        byte[]? bytes = HexStringByteArrayConverter.Instance.Read(ref reader, typeof(byte[]), options);
        return bytes is null ? default : bytes;
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, ReadOnlyMemory<byte> value, JsonSerializerOptions options)
        => HexStringByteArrayConverter.WriteHexStringValue(writer, value.Span);
}
