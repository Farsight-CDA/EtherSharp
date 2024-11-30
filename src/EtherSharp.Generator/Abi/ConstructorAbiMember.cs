using System.Text.Json.Serialization;

namespace EtherSharp.Generator.Abi;
public class ConstructorAbiMember : AbiMember
{
    [JsonRequired]
    public StateMutability StateMutability { get; set; }

    [JsonRequired]
    public AbiValue[] Inputs { get; set; } = null!;
}
