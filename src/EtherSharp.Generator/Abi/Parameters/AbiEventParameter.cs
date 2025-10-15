using System.Text.Json.Serialization;

namespace EtherSharp.Generator.Abi.Parameters;

public class AbiEventParameter : AbiParameter
{
    [JsonRequired]
    [JsonPropertyName("indexed")]
    public bool IsIndexed { get; set; }

    public AbiEventParameter(string name, string type, bool isIndexed, string? internalType, AbiParameter[]? components)
        : base(name, type, internalType, components)
    {
        Name = name;
        Type = type;
        IsIndexed = isIndexed;
        InternalType = internalType;
        Components = components;
    }
}
