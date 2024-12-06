using EtherSharp.Generator.Abi;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EtherSharp.Generator.Util;
public static class ParsingUtils
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
