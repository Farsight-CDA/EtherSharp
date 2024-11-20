using System.Text.Json.Serialization;

namespace EtherSharp.Generator.Abi;
public class AbiValue
{
    [JsonRequired]
    public string Name { get; private set; } = null!;
    [JsonRequired]
    public string Type { get; private set; } = null!;
}
