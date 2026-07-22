namespace EtherSharp.Generator.EIP712;

public sealed class EIP712GenerationModel(
    EIP712TypeModel type,
    string declarationKind,
    byte[] typeHash)
{
    public EIP712TypeModel Type { get; } = type;
    public string DeclarationKind { get; } = declarationKind;
    public byte[] TypeHash { get; } = typeHash;
}
