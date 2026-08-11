using EtherSharp.Generator.ABI.Util;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;

namespace EtherSharp.Generator.ABI;

internal readonly struct ContractInfo(
    string @namespace,
    string name,
    string metadataName,
    bool isPartial,
    int abiFileAttributeCount,
    string? abiFileName,
    int bytecodeAttributeCount,
    string? initCode,
    string? runtimeCode,
    Location? location) : IEquatable<ContractInfo>
{
    public string Namespace { get; } = @namespace;
    public string Name { get; } = name;
    public string MetadataName { get; } = metadataName;
    public bool IsPartial { get; } = isPartial;
    public int AbiFileAttributeCount { get; } = abiFileAttributeCount;
    public string? AbiFileName { get; } = abiFileName;
    public int BytecodeAttributeCount { get; } = bytecodeAttributeCount;
    public string? InitCode { get; } = initCode;
    public string? RuntimeCode { get; } = runtimeCode;
    public Location? Location { get; } = location;

    public static ContractInfo? Create(GeneratorAttributeSyntaxContext context, CancellationToken cancellationToken)
    {
        if(context.TargetSymbol is not INamedTypeSymbol symbol
            || !symbol.AllInterfaces.Any(TypeIdentificationUtils.IsIEVMContract))
        {
            return null;
        }

        bool isPartial = symbol.DeclaringSyntaxReferences
            .Select(reference => reference.GetSyntax(cancellationToken))
            .OfType<InterfaceDeclarationSyntax>()
            .Any(declaration => declaration.Modifiers.Any(SyntaxKind.PartialKeyword));

        var rawAttributes = symbol.GetAttributes().Where(attribute => attribute.AttributeClass is not null).ToArray();
        var abiFileAttributes = rawAttributes
            .Where(attribute => TypeIdentificationUtils.IsAbiFileAttribute(attribute.AttributeClass!))
            .ToArray();
        var bytecodeAttributes = rawAttributes
            .Where(attribute => TypeIdentificationUtils.IsBytecodeAttribute(attribute.AttributeClass!))
            .ToArray();
        var (initCode, runtimeCode) = GetBytecode(bytecodeAttributes);

        return new ContractInfo(
            symbol.ContainingNamespace.IsGlobalNamespace
                ? String.Empty
                : symbol.ContainingNamespace.ToDisplayString(),
            symbol.Name,
            symbol.MetadataName,
            isPartial,
            abiFileAttributes.Length,
            GetFileName(abiFileAttributes),
            bytecodeAttributes.Length,
            initCode,
            runtimeCode,
            symbol.Locations.FirstOrDefault()
        );
    }

    public bool Equals(ContractInfo other)
        => Namespace == other.Namespace
            && Name == other.Name
            && MetadataName == other.MetadataName
            && IsPartial == other.IsPartial
            && AbiFileAttributeCount == other.AbiFileAttributeCount
            && AbiFileName == other.AbiFileName
            && BytecodeAttributeCount == other.BytecodeAttributeCount
            && InitCode == other.InitCode
            && RuntimeCode == other.RuntimeCode
            && LocationsEqual(Location, other.Location);

    public override bool Equals(object? obj)
        => obj is ContractInfo other && Equals(other);

    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(Namespace, StringComparer.Ordinal);
        hashCode.Add(Name, StringComparer.Ordinal);
        hashCode.Add(MetadataName, StringComparer.Ordinal);
        hashCode.Add(IsPartial);
        hashCode.Add(AbiFileAttributeCount);
        hashCode.Add(AbiFileName, StringComparer.Ordinal);
        hashCode.Add(BytecodeAttributeCount);
        hashCode.Add(InitCode, StringComparer.Ordinal);
        hashCode.Add(RuntimeCode, StringComparer.Ordinal);
        hashCode.Add(Location?.SourceTree);
        hashCode.Add(Location?.SourceSpan);
        return hashCode.ToHashCode();
    }

    private static bool LocationsEqual(Location? left, Location? right)
        => left is null || right is null
            ? left is null && right is null
            : ReferenceEquals(left.SourceTree, right.SourceTree)
            && left.SourceSpan.Equals(right.SourceSpan);

    private static string? GetFileName(AttributeData[] attributes)
    {
        if(attributes.Length != 1 || attributes[0].ConstructorArguments.Length == 0)
        {
            return null;
        }

        var argument = attributes[0].ConstructorArguments[0];
        return argument.Kind == TypedConstantKind.Primitive
            ? argument.Value?.ToString()
            : null;
    }

    private static (string? InitCode, string? RuntimeCode) GetBytecode(AttributeData[] attributes)
    {
        if(attributes.Length != 1)
        {
            return (null, null);
        }

        var arguments = attributes[0].ConstructorArguments;
        return (
            GetStringArgument(arguments, 0),
            GetStringArgument(arguments, 1)
        );
    }

    private static string? GetStringArgument(ImmutableArray<TypedConstant> arguments, int index)
        => arguments.Length > index
            && arguments[index] is { Kind: TypedConstantKind.Primitive, Value: string value }
                ? value
                : null;
}
