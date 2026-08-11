using Microsoft.CodeAnalysis;

namespace EtherSharp.Generator.ABI.Util;

internal static class TypeIdentificationUtils
{
    public static bool IsAbiFileAttribute(INamedTypeSymbol symbol)
        => symbol.Name == "AbiFileAttribute" && symbol.ContainingNamespace.ToDisplayString() == "EtherSharp.Contract";

    public static bool IsBytecodeAttribute(INamedTypeSymbol symbol)
        => symbol.Name == "BytecodeAttribute" && symbol.ContainingNamespace.ToDisplayString() == "EtherSharp.Contract";

    public static bool IsIEVMContract(INamedTypeSymbol symbol)
        => symbol.Name == "IEVMContract" && symbol.ContainingNamespace.ToDisplayString() == "EtherSharp.Contract";
}
