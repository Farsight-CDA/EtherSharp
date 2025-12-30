using Microsoft.CodeAnalysis;

namespace EtherSharp.Generator.Util;

internal static class TypeIdentificationUtils
{
    public static bool IsAbiFileAttribute(INamedTypeSymbol symbol)
        => symbol.Name == "AbiFileAttribute" && symbol.ContainingNamespace.ToDisplayString() == "EtherSharp.Contract";

    public static bool IsBytecodeFileAttribute(INamedTypeSymbol symbol)
        => symbol.Name == "BytecodeFileAttribute" && symbol.ContainingNamespace.ToDisplayString() == "EtherSharp.Contract";

    public static bool IsIEVMContract(INamedTypeSymbol symbol)
        => symbol.Name == "IEVMContract" && symbol.ContainingNamespace.ToDisplayString() == "EtherSharp.Contract";
}
