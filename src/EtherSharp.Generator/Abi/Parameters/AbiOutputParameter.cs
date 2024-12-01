using System.Text.Json.Serialization;

namespace EtherSharp.Generator.Abi.Parameters;
public class AbiOutputParameter
{
    [JsonRequired]
    public string Name { get; set; } = null!;
    [JsonRequired]
    public string Type { get; set; } = null!;
}
