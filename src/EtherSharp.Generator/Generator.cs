using EtherSharp.Generator.Abi;
using EtherSharp.Generator.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Text.Json;

namespace EtherSharp.Generator;

[Generator]
public class Generator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var contractTypesProvider = context.SyntaxProvider
            .CreateSyntaxProvider(
                IsCandidateNode,
                (ctx, _) => (
                    IsPartial: ((InterfaceDeclarationSyntax) ctx.Node).Modifiers.Any(SyntaxKind.PartialKeyword),
                    Symbol: ctx.SemanticModel.GetDeclaredSymbol((InterfaceDeclarationSyntax) ctx.Node)
                )
            )
            .Where(ctx =>
                ctx.Symbol is not null &&
                ctx.Symbol.AllInterfaces.Any(static x => x.Name == "IContract")
            )
            .Select((ctx, _) =>
            {
                var attribute = ctx.Symbol!.GetAttributes()
                    .FirstOrDefault(x => x.AttributeClass?.Name == "AbiFileAttribute");

                return (
                    ctx.Symbol!,
                    ctx.IsPartial,
                    attribute?.ConstructorArguments.Length == 1
                        ? attribute.ConstructorArguments[0].Value?.ToString()
                        : null
                );
            });

        var additionalFilesProvider = context.AdditionalTextsProvider
            .Where(file => file.Path.EndsWith(".json"));

        var combined = contractTypesProvider.Combine(additionalFilesProvider.Collect());

        context.RegisterSourceOutput(combined, GenerateSource);
    }

    private static bool IsCandidateNode(SyntaxNode node, CancellationToken _)
        => node is InterfaceDeclarationSyntax cd &&
            cd.AttributeLists.Count != 0 &&
            cd.BaseList is not null;

    private static void GenerateSource(SourceProductionContext context,
        ((INamedTypeSymbol, bool, string?), ImmutableArray<AdditionalText> additionalFiles) combined)
    {
        var ((contractSymbol, isPartial, schemaFileName), additionalFiles) = combined;

        if (!isPartial)
        {
            ReportDiagnostic(context, GeneratorDiagnostics.InterfaceMustBePartial, contractSymbol, contractSymbol.Name);
            return;
        }

        if(schemaFileName is null)
        {
            ReportDiagnostic(context, GeneratorDiagnostics.SchemaFileNotSpecified, contractSymbol, contractSymbol.Name);
            return;
        }

        var schemaFile = additionalFiles.FirstOrDefault(file => file.Path.EndsWith(schemaFileName));
        if(schemaFile is null)
        {
            ReportDiagnostic(context, GeneratorDiagnostics.SchemaFileNotFound, contractSymbol, schemaFileName);
            return;
        }

        string? schemaText = schemaFile.GetText()?.ToString();
        if(string.IsNullOrEmpty(schemaText) || schemaText is null)
        {
            ReportDiagnostic(context, GeneratorDiagnostics.SchemaFileMalformed, contractSymbol);
            return;
        }

        AbiMember[] abiMembers;

        try
        {
            abiMembers = JsonSerializer.Deserialize<AbiMember[]>(schemaText, ParsingUtils.AbiJsonOptions)
                ?? throw new NotSupportedException("Parsing schema file to ContractAPISchema failed");
        }
        catch(Exception ex)
        {
            ReportDiagnostic(context, GeneratorDiagnostics.SchemaFileMalformed, contractSymbol, ex);
            return;
        }

        try
        {
            var writer = new ContractSourceWriter();
            string contractName = contractSymbol.Name;

            context.AddSource(
                $"{contractName}.generated.cs",
                writer.WriteContractSourceCode(contractSymbol.ContainingNamespace.ToString(), contractName, abiMembers)
            );
        }
        catch(Exception ex)
        {
            ReportDiagnostic(context, GeneratorDiagnostics.GenerationFailed, contractSymbol, ex);
            return;
        }
    }

    private static void ReportDiagnostic(SourceProductionContext context, DiagnosticDescriptor descriptor, ISymbol symbol, params string[] args)
    {
        var diagnostic = Diagnostic.Create(descriptor, symbol.Locations.FirstOrDefault(), args);
        context.ReportDiagnostic(diagnostic);
    }

    private static void ReportDiagnostic(SourceProductionContext context, DiagnosticDescriptor descriptor, ISymbol symbol, Exception e)
    {
        var diagnostic = Diagnostic.Create(descriptor, symbol.Locations.FirstOrDefault(), e.ToString());
        context.ReportDiagnostic(diagnostic);
    }
}