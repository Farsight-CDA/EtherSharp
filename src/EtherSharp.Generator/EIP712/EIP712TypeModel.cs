using Microsoft.CodeAnalysis;

namespace EtherSharp.Generator.EIP712;

public sealed class EIP712TypeModel(
    INamedTypeSymbol symbol,
    IReadOnlyList<EIP712MemberModel> members)
{
    public INamedTypeSymbol Symbol { get; } = symbol;
    public string Name => Symbol.Name;
    public IReadOnlyList<EIP712MemberModel> Members { get; } = members;
}
