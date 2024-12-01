using System.Text.Json.Serialization;

namespace EtherSharp.Generator.Abi.Parameters;
public class AbiInputParameter
{
    [JsonRequired]
    public string Name { get; set; } = null!;
    [JsonRequired]
    public string Type { get; set; } = null!;

    public string? InternalType { get; set; }
    public AbiInputParameter[]? Components { get; set; }
}
