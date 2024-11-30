using Microsoft.CodeAnalysis;

namespace EtherSharp.Generator.Util;
public static class TypeIdentificationUtils
{
    public static bool IsAbiFileAttribute(INamedTypeSymbol symbol)
        => symbol.Name == "AbiFileAttribute" && symbol.ContainingNamespace.ToDisplayString() == "EtherSharp.Contract";

    public static bool IsIEVMContract(INamedTypeSymbol symbol)
        => symbol.Name == "IEVMContract" && symbol.ContainingNamespace.ToDisplayString() == "EtherSharp.Contract";
}
