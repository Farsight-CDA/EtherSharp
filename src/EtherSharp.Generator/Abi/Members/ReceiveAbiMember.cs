using System.Text.Json.Serialization;

namespace EtherSharp.Generator.Abi.Members;

public class ReceiveAbiMember : AbiMember
{
    [JsonRequired]
    public StateMutability StateMutability { get; set; }
}
