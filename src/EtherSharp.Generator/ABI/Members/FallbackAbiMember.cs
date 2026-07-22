using System.Text.Json.Serialization;

namespace EtherSharp.Generator.ABI.Members;

public sealed class FallbackAbiMember : AbiMember
{
    [JsonRequired]
    public StateMutability StateMutability { get; set; }
}
