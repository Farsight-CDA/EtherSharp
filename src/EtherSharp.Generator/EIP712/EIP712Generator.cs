using EtherSharp.Generator.Util;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;

namespace EtherSharp.Generator.EIP712;

/// <summary>
/// Generates EIP-712 struct hashing implementations.
/// </summary>
[Generator]
public sealed class EIP712Generator : IIncrementalGenerator
{
    private const string ATTRIBUTE_METADATA_NAME = "EtherSharp.Crypto.EIP712TypeAttribute";

    /// <inheritdoc/>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var typeProvider = context.SyntaxProvider.ForAttributeWithMetadataName(
                ATTRIBUTE_METADATA_NAME,
                static (node, _) => node is TypeDeclarationSyntax,
                static (attributeContext, _) => attributeContext.TargetSymbol as INamedTypeSymbol)
            .Where(static symbol => symbol is not null)
            .Select(static (symbol, _) => symbol!);

        context.RegisterSourceOutput(typeProvider.Collect(), GenerateSources);
    }

    private static void GenerateSources(
        SourceProductionContext context,
        ImmutableArray<INamedTypeSymbol> symbols)
    {
        var validatedTypes = new Dictionary<INamedTypeSymbol, string>(SymbolEqualityComparer.Default);
        foreach(var symbol in symbols)
        {
            if(TryValidateType(context, symbol, out string? declarationKind))
            {
                validatedTypes.Add(symbol, declarationKind!);
            }
        }

        var processor = new EIP712TypeProcessor(context, validatedTypes.Keys);
        foreach(var type in validatedTypes)
        {
            GenerateSource(context, processor, type.Key, type.Value);
        }
    }

    private static void GenerateSource(
        SourceProductionContext context,
        EIP712TypeProcessor processor,
        INamedTypeSymbol symbol,
        string declarationKind)
    {
        try
        {
            var model = processor.Process(symbol, declarationKind);
            if(model is null)
            {
                return;
            }

            string namespaceName = symbol.ContainingNamespace.IsGlobalNamespace
                ? String.Empty
                : symbol.ContainingNamespace.ToDisplayString();
            string hintName = NameUtils.ToValidFileName(
                $"{namespaceName}.{symbol.MetadataName}.EIP712.generated.cs");
            context.AddSource(hintName, EIP712SourceWriter.Write(model));
        }
        catch(OperationCanceledException)
        {
            throw;
        }
        catch(Exception ex)
        {
            context.ReportDiagnostic(Diagnostic.Create(
                EIP712GeneratorDiagnostics.ExecutionFailed,
                symbol.Locations.FirstOrDefault(),
                symbol.Name,
                ex.ToString()));
        }
    }

    private static bool TryValidateType(
        SourceProductionContext context,
        INamedTypeSymbol symbol,
        out string? declarationKind)
    {
        declarationKind = null;
        if(!symbol.Interfaces.Any(static type =>
            type.Name == "IEIP712Type"
            && type.ContainingNamespace.ToDisplayString() == "EtherSharp.Crypto"))
        {
            context.ReportDiagnostic(Diagnostic.Create(
                EIP712GeneratorDiagnostics.InterfaceRequired,
                symbol.Locations.FirstOrDefault(),
                symbol.Name));
            return false;
        }

        bool isPartial = symbol.DeclaringSyntaxReferences
            .Select(reference => reference.GetSyntax(context.CancellationToken))
            .OfType<TypeDeclarationSyntax>()
            .Any(type => type.Modifiers.Any(SyntaxKind.PartialKeyword));
        if(!isPartial)
        {
            context.ReportDiagnostic(Diagnostic.Create(
                EIP712GeneratorDiagnostics.TypeMustBePartial,
                symbol.Locations.FirstOrDefault(),
                symbol.Name));
            return false;
        }

        declarationKind = GetDeclarationKind(context, symbol);
        if(declarationKind is null)
        {
            string reason = symbol.ContainingType is not null
                ? "nested types are not supported"
                : "generic, ref-like, and derived types are not supported";
            context.ReportDiagnostic(Diagnostic.Create(
                EIP712GeneratorDiagnostics.UnsupportedDeclaration,
                symbol.Locations.FirstOrDefault(),
                symbol.Name,
                reason));
            return false;
        }

        if(symbol.GetMembers("HashStruct").Any()
            || symbol.GetMembers("GetSigningHash").Any()
            || symbol.GetMembers("__etherSharpEIP712TypeHash").Any())
        {
            context.ReportDiagnostic(Diagnostic.Create(
                EIP712GeneratorDiagnostics.UnsupportedDeclaration,
                symbol.Locations.FirstOrDefault(),
                symbol.Name,
                "HashStruct, GetSigningHash, and __etherSharpEIP712TypeHash are reserved for generated code"));
            return false;
        }

        if(!NameUtils.IsValidIdentifier(symbol.Name))
        {
            context.ReportDiagnostic(Diagnostic.Create(
                EIP712GeneratorDiagnostics.UnsupportedDeclaration,
                symbol.Locations.FirstOrDefault(),
                symbol.Name,
                $"type name {symbol.Name} is not a valid EIP-712 identifier"));
            return false;
        }

        return true;
    }

    private static string? GetDeclarationKind(
        SourceProductionContext context,
        INamedTypeSymbol symbol)
    {
        if(symbol.ContainingType is not null
            || symbol.TypeParameters.Length > 0
            || symbol.IsRefLikeType
            || (!symbol.IsValueType && symbol.BaseType is not { SpecialType: SpecialType.System_Object }))
        {
            return null;
        }

        var declaration = symbol.DeclaringSyntaxReferences
            .Select(reference => reference.GetSyntax(context.CancellationToken))
            .OfType<TypeDeclarationSyntax>()
            .FirstOrDefault();
        string readonlyModifier = symbol.IsValueType && symbol.IsReadOnly ? "readonly " : String.Empty;
        return declaration switch
        {
            RecordDeclarationSyntax record when record.ClassOrStructKeyword.IsKind(SyntaxKind.StructKeyword)
                => $"{readonlyModifier}partial record struct",
            RecordDeclarationSyntax => "partial record",
            StructDeclarationSyntax => $"{readonlyModifier}partial struct",
            ClassDeclarationSyntax => "partial class",
            _ => null
        };
    }
}
