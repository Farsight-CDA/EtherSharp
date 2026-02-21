using EtherSharp.Types;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EtherSharp.Common.Converter;

/// <summary>
/// Converts a <see cref="TargetBlockNumber"/> to a JSON block selector string.
/// </summary>
public class TargetBlockNumberConverter : JsonConverter<TargetBlockNumber>
{
    /// <inheritdoc/>
    public override TargetBlockNumber Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => throw new NotSupportedException();

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, TargetBlockNumber value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.ToString());
}
