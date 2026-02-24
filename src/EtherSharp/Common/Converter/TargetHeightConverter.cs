using EtherSharp.Types;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EtherSharp.Common.Converter;

/// <summary>
/// Converts a <see cref="TargetHeight"/> to a JSON block selector string.
/// </summary>
public class TargetHeightConverter : JsonConverter<TargetHeight>
{
    /// <inheritdoc/>
    public override TargetHeight Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => throw new NotSupportedException();

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, TargetHeight value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.ToString());
}
