using System.Text.Json.Serialization;

namespace EtherSharp.Generator.Abi.Members;
public class ErrorAbiMember : AbiMember
{
    [JsonRequired]
    public string Name { get; set; } = null!;
}
