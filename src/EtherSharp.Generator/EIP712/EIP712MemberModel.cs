namespace EtherSharp.Generator.EIP712;

public sealed class EIP712MemberModel(
    string propertyName,
    string eipName,
    string eipType,
    EIP712EncodingKind encodingKind,
    int fixedBytesLength = 0,
    EIP712TypeModel? dependency = null)
{
    public string PropertyName { get; } = propertyName;
    public string EIPName { get; } = eipName;
    public string EIPType { get; } = eipType;
    public EIP712EncodingKind EncodingKind { get; } = encodingKind;
    public int FixedBytesLength { get; } = fixedBytesLength;
    public EIP712TypeModel? Dependency { get; } = dependency;
}
