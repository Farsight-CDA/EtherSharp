using System.Text.Json.Serialization;

namespace EtherSharp.Generator.Abi;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(FunctionAbiMember), "function")]
[JsonDerivedType(typeof(EventAbiMember), "event")]
[JsonDerivedType(typeof(FallbackAbiMember), "fallback")]
public abstract class AbiMember
{
}
