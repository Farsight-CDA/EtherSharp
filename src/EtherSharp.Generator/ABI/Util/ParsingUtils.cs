using EtherSharp.Generator.ABI;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EtherSharp.Generator.ABI.Util;

internal static class ParsingUtils
{
    public static readonly JsonSerializerOptions AbiJsonOptions = new JsonSerializerOptions()
    {
        AllowOutOfOrderMetadataProperties = true,
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters =
        {
            new JsonStringEnumConverter<StateMutability>()
        }
    };
}
