using EtherSharp.Generator.ABI.Members;
using System.Text.Json.Serialization;

namespace EtherSharp.Generator.ABI;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(FunctionAbiMember), "function")]
[JsonDerivedType(typeof(EventAbiMember), "event")]
[JsonDerivedType(typeof(FallbackAbiMember), "fallback")]
[JsonDerivedType(typeof(ReceiveAbiMember), "receive")]
[JsonDerivedType(typeof(ConstructorAbiMember), "constructor")]
[JsonDerivedType(typeof(ErrorAbiMember), "error")]
public abstract class AbiMember
{
}
