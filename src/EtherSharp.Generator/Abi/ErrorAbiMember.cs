using System.Text.Json.Serialization;

namespace EtherSharp.Generator.Abi;
public class ErrorAbiMember : AbiMember
{
    [JsonRequired]
    public string Name { get; set; } = null!;
}
