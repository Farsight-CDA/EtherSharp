using System.Text.Json.Serialization;

namespace EtherSharp.Generator.ABI.Members;

public sealed class ReceiveAbiMember : AbiMember
{
    [JsonRequired]
    public StateMutability StateMutability { get; set; }
}
