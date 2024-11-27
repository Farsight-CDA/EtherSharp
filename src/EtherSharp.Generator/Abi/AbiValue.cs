using System.Text.Json.Serialization;

namespace EtherSharp.Generator.Abi;
public class AbiValue
{
    [JsonRequired]
    public string Name { get; set; } = null!;
    [JsonRequired]
    public string Type { get; set; } = null!;
}
