using System.Text.Json.Serialization;

namespace EtherSharp.Generator.Abi.Members;

public sealed class FallbackAbiMember : AbiMember
{
    [JsonRequired]
    public StateMutability StateMutability { get; set; }
}
