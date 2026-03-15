using System.Text.Json.Serialization;

namespace EtherSharp.Generator.Abi.Members;

public sealed class ReceiveAbiMember : AbiMember
{
    [JsonRequired]
    public StateMutability StateMutability { get; set; }
}
