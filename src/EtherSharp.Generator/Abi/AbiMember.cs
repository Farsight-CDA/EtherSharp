using System.Text.Json.Serialization;

namespace EtherSharp.Generator.Abi;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(FunctionAbiMember), "function")]
public abstract class AbiMember
{
    [JsonRequired]
    public string Name { get; private set; } = null!;
}
