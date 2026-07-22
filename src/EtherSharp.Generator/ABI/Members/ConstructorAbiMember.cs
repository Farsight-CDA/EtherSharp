using EtherSharp.Generator.ABI.Parameters;
using System.Text.Json.Serialization;

namespace EtherSharp.Generator.ABI.Members;

public sealed class ConstructorAbiMember : AbiMember
{
    public static ConstructorAbiMember Empty { get; } = new ConstructorAbiMember()
    {
        Inputs = [],
        StateMutability = StateMutability.NonPayable
    };

    [JsonRequired]
    public StateMutability StateMutability { get; set; }

    [JsonRequired]
    public AbiParameter[] Inputs { get; set; } = null!;
}
