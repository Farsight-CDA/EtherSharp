using System.Text.Json.Serialization;

namespace EtherSharp.Generator.Abi.Parameters;
public class AbiEventParameter
{
    [JsonRequired]
    public string Name { get; set; } = null!;
    [JsonRequired]
    public string Type { get; set; } = null!;
    [JsonRequired]
    [JsonPropertyName("indexed")]
    public bool IsIndexed { get; set; }
}
