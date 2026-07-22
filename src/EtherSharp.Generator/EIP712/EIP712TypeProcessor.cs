using Epoche;
using EtherSharp.Generator.Util;
using Microsoft.CodeAnalysis;
using System.Text;

namespace EtherSharp.Generator.EIP712;

public sealed class EIP712TypeProcessor(
    SourceProductionContext context,
    IEnumerable<INamedTypeSymbol> validatedTypes)
{
    private readonly SourceProductionContext _context = context;
    private readonly HashSet<INamedTypeSymbol> _validatedTypes = new(validatedTypes, SymbolEqualityComparer.Default);
    private readonly Dictionary<INamedTypeSymbol, EIP712TypeModel> _models = new(SymbolEqualityComparer.Default);
    private readonly HashSet<INamedTypeSymbol> _visiting = new(SymbolEqualityComparer.Default);

    public EIP712GenerationModel? Process(
        INamedTypeSymbol symbol,
        string declarationKind)
    {
        if(!TryCreateModel(symbol, out var model))
        {
            return null;
        }
        var processedModel = model!;
        if(!TryCreateCanonicalType(processedModel, out string? canonicalType))
        {
            return null;
        }

        byte[] typeHash = Keccak256.ComputeHash(canonicalType!);
        return new EIP712GenerationModel(processedModel, declarationKind, typeHash);
    }

    private bool TryCreateModel(
        INamedTypeSymbol symbol,
        out EIP712TypeModel? model)
    {
        if(_models.TryGetValue(symbol, out model))
        {
            return true;
        }
        if(!_visiting.Add(symbol))
        {
            _context.ReportDiagnostic(Diagnostic.Create(
                EIP712GeneratorDiagnostics.UnsupportedDeclaration,
                symbol.Locations.FirstOrDefault(),
                symbol.Name,
                "recursive type declarations are not supported"));

            model = null;
            return false;
        }

        try
        {
            var properties = symbol.GetMembers()
                .OfType<IPropertySymbol>()
                .Where(static property => !property.IsStatic
                    && !property.IsIndexer
                    && property.GetMethod is { DeclaredAccessibility: Accessibility.Public }
                    && property.Locations.Any(static location => location.IsInSource))
                .OrderBy(static property => property.Locations.FirstOrDefault()?.SourceTree?.FilePath, StringComparer.Ordinal)
                .ThenBy(static property => property.Locations.FirstOrDefault()?.SourceSpan.Start ?? Int32.MaxValue)
                .ToArray();

            if(properties.Select(static property => property.Locations.First().SourceTree).Distinct().Skip(1).Any())
            {
                _context.ReportDiagnostic(Diagnostic.Create(
                    EIP712GeneratorDiagnostics.UnsupportedDeclaration,
                    symbol.Locations.FirstOrDefault(),
                    symbol.Name,
                    "encoded members must be declared in the same source file"));
                model = null;
                return false;
            }

            var members = new List<EIP712MemberModel>(properties.Length);
            var eipMemberNames = new HashSet<string>(StringComparer.Ordinal);
            foreach(var property in properties)
            {
                _context.CancellationToken.ThrowIfCancellationRequested();
                if(!TryCreateMember(property, out var member))
                {
                    model = null;
                    return false;
                }
                var processedMember = member!;
                if(!NameUtils.IsValidIdentifier(processedMember.EIPName) || !eipMemberNames.Add(processedMember.EIPName))
                {
                    _context.ReportDiagnostic(Diagnostic.Create(
                        EIP712GeneratorDiagnostics.UnsupportedDeclaration,
                        property.Locations.FirstOrDefault(),
                        symbol.Name,
                        $"member name {processedMember.EIPName} is invalid or duplicated after lower-camel conversion"));
                    model = null;
                    return false;
                }

                members.Add(processedMember);
            }

            model = new EIP712TypeModel(symbol, members);
            _models.Add(symbol, model);
            return true;
        }
        finally
        {
            _visiting.Remove(symbol);
        }
    }

    private bool TryCreateMember(
        IPropertySymbol property,
        out EIP712MemberModel? member)
    {
        var type = property.Type;
        string propertyName = NameUtils.EscapeIdentifier(property.Name);
        string eipName = NameUtils.Uncapitalize(property.Name);

        if(type.SpecialType == SpecialType.System_Boolean)
        {
            member = new EIP712MemberModel(propertyName, eipName, "bool", EIP712EncodingKind.Bool);
            return true;
        }
        if(type.SpecialType == SpecialType.System_String
            && type.NullableAnnotation != NullableAnnotation.Annotated)
        {
            member = new EIP712MemberModel(propertyName, eipName, "string", EIP712EncodingKind.String);
            return true;
        }
        if(type is IArrayTypeSymbol { ElementType.SpecialType: SpecialType.System_Byte, Rank: 1 }
            && type.NullableAnnotation != NullableAnnotation.Annotated)
        {
            member = new EIP712MemberModel(propertyName, eipName, "bytes", EIP712EncodingKind.ByteArray);
            return true;
        }
        if(type is INamedTypeSymbol namedType
            && TryCreateNamedMember(propertyName, eipName, property, namedType, out member))
        {
            return true;
        }

        ReportUnsupportedMember(property, type.ToDisplayString());
        member = null;
        return false;
    }

    private bool TryCreateNamedMember(
        string propertyName,
        string eipName,
        IPropertySymbol property,
        INamedTypeSymbol type,
        out EIP712MemberModel? member)
    {
        string typeNamespace = type.ContainingNamespace.ToDisplayString();
        if(typeNamespace == "System"
            && type.Name == "ReadOnlyMemory"
            && type.TypeArguments.Length == 1
            && type.TypeArguments[0].SpecialType == SpecialType.System_Byte)
        {
            member = new EIP712MemberModel(propertyName, eipName, "bytes", EIP712EncodingKind.ReadOnlyMemory);
            return true;
        }
        if(typeNamespace == "EtherSharp.Types" && type.Name == "Address")
        {
            member = new EIP712MemberModel(propertyName, eipName, "address", EIP712EncodingKind.Address);
            return true;
        }
        if(typeNamespace == "EtherSharp.Numerics" && type.Name == "UInt256")
        {
            member = new EIP712MemberModel(propertyName, eipName, "uint256", EIP712EncodingKind.UInt256);
            return true;
        }
        if(typeNamespace == "EtherSharp.Numerics" && type.Name == "Int256")
        {
            member = new EIP712MemberModel(propertyName, eipName, "int256", EIP712EncodingKind.Int256);
            return true;
        }
        if(typeNamespace == "EtherSharp.Types" && SolidityTypeUtils.TryGetFixedBytesLength(type.Name, "Bytes", out int byteLength))
        {
            member = new EIP712MemberModel(
                propertyName,
                eipName,
                $"bytes{byteLength}",
                EIP712EncodingKind.FixedBytes,
                byteLength);
            return true;
        }
        if(property.NullableAnnotation != NullableAnnotation.Annotated && _validatedTypes.Contains(type))
        {
            if(!TryCreateModel(type, out var dependency))
            {
                member = null;
                return false;
            }
            var processedDependency = dependency!;

            member = new EIP712MemberModel(
                propertyName,
                eipName,
                processedDependency.Name,
                EIP712EncodingKind.Struct,
                dependency: processedDependency);
            return true;
        }

        member = null;
        return false;
    }

    private bool TryCreateCanonicalType(
        EIP712TypeModel root,
        out string? canonicalType)
    {
        var dependencies = new Dictionary<string, EIP712TypeModel>(StringComparer.Ordinal);
        if(!TryCollectDependencies(root, root, dependencies))
        {
            canonicalType = null;
            return false;
        }

        var definition = new StringBuilder(WriteTypeDefinition(root));
        foreach(var dependency in dependencies.Values.OrderBy(static type => type.Name, StringComparer.Ordinal))
        {
            definition.Append(WriteTypeDefinition(dependency));
        }

        canonicalType = definition.ToString();
        return true;
    }

    private bool TryCollectDependencies(
        EIP712TypeModel root,
        EIP712TypeModel current,
        Dictionary<string, EIP712TypeModel> dependencies)
    {
        foreach(var member in current.Members)
        {
            var dependency = member.Dependency;
            if(dependency is null || ReferenceEquals(dependency, root))
            {
                continue;
            }
            if(dependency.Name == root.Name)
            {
                _context.ReportDiagnostic(Diagnostic.Create(
                    EIP712GeneratorDiagnostics.UnsupportedDeclaration,
                    dependency.Symbol.Locations.FirstOrDefault(),
                    root.Symbol.Name,
                    $"multiple EIP-712 types are named {root.Name}"));
                return false;
            }
            if(dependencies.TryGetValue(dependency.Name, out var existing))
            {
                if(!SymbolEqualityComparer.Default.Equals(existing.Symbol, dependency.Symbol))
                {
                    _context.ReportDiagnostic(Diagnostic.Create(
                        EIP712GeneratorDiagnostics.UnsupportedDeclaration,
                        dependency.Symbol.Locations.FirstOrDefault(),
                        root.Symbol.Name,
                        $"multiple EIP-712 types are named {dependency.Name}"));
                    return false;
                }

                continue;
            }

            dependencies.Add(dependency.Name, dependency);
            if(!TryCollectDependencies(root, dependency, dependencies))
            {
                return false;
            }
        }

        return true;
    }

    private static string WriteTypeDefinition(EIP712TypeModel type)
    {
        var definition = new StringBuilder(type.Name);
        definition.Append('(');
        for(int i = 0; i < type.Members.Count; i++)
        {
            if(i > 0)
            {
                definition.Append(',');
            }

            var member = type.Members[i];
            definition.Append(member.EIPType);
            definition.Append(' ');
            definition.Append(member.EIPName);
        }

        definition.Append(')');
        return definition.ToString();
    }

    private void ReportUnsupportedMember(IPropertySymbol property, string typeName)
        => _context.ReportDiagnostic(Diagnostic.Create(
            EIP712GeneratorDiagnostics.UnsupportedMemberType,
            property.Locations.FirstOrDefault(),
            property.Name,
            typeName));

}
