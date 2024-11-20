using System.Text.Json.Serialization;

namespace EtherSharp.Generator.Abi;
public class FunctionAbiMember : AbiMember
{
    [JsonRequired]
    public StateMutability StateMutability { get; private set; }

    [JsonRequired]
    public AbiValue[] Inputs { get; private set; } = null!;

    [JsonRequired]
    public AbiValue[] Outputs { get; private set; } = null!;
}
