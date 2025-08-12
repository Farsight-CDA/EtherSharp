using EtherSharp.Generator.Abi;
using EtherSharp.Generator.SourceWriters;
using EtherSharp.Generator.SourceWriters.Components;
using EtherSharp.Generator.Util;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
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
                (ctx, cancellationToken) => (
                    Node: (InterfaceDeclarationSyntax) ctx.Node,
                    Symbol: ctx.SemanticModel.GetDeclaredSymbol((InterfaceDeclarationSyntax) ctx.Node, cancellationToken: cancellationToken)
                )
            )
            .Where(ctx =>
                ctx.Symbol is not null &&
                ctx.Symbol.AllInterfaces.Any(TypeIdentificationUtils.IsIEVMContract)
            )
            .Select((ctx, _) => (
                ctx.Symbol!,
                ctx.Node
            ));

        var additionalFilesProvider = context.AdditionalTextsProvider
            .Where(file => file.Path.EndsWith(".json", StringComparison.OrdinalIgnoreCase));

        var combined = contractTypesProvider.Combine(additionalFilesProvider.Collect());

        context.RegisterSourceOutput(combined, GenerateSource);
    }

    private static bool IsCandidateNode(SyntaxNode node, CancellationToken _)
        => node is InterfaceDeclarationSyntax cd &&
            cd.BaseList is not null &&
            cd.BaseList.Types.Any(baseType =>
                baseType.Type is IdentifierNameSyntax identifier &&
                identifier.Identifier.Text == "IEVMContract"
            );

    private static void GenerateSource(SourceProductionContext context,
        ((INamedTypeSymbol, InterfaceDeclarationSyntax), ImmutableArray<AdditionalText> additionalFiles) combined)
    {
        var ((contractSymbol, contractNode), additionalFiles) = combined;

        try
        {
            bool isPartial = contractNode.Modifiers.Any(SyntaxKind.PartialKeyword);

            if(!isPartial)
            {
                ReportDiagnostic(context, GeneratorDiagnostics.InterfaceMustBePartial, contractSymbol, contractSymbol.Name);
                return;
            }

            var attributes = contractSymbol.GetAttributes()
                .Where(x => x.AttributeClass is not null && TypeIdentificationUtils.IsAbiFileAttribute(x.AttributeClass))
                .ToArray();

            if(attributes.Length == 0)
            {
                ReportDiagnostic(context, GeneratorDiagnostics.AbiFileAttributeNotFound, contractSymbol, contractSymbol.Name);
                return;
            }
            if(attributes.Length > 1)
            {
                ReportDiagnostic(context, GeneratorDiagnostics.MultipleAbiFileAttributeFound, contractSymbol, contractSymbol.Name);
                return;
            }

            string? schemaFileName = attributes.Single().ConstructorArguments[0].Value?.ToString();

            if(schemaFileName is null || string.IsNullOrEmpty(schemaFileName))
            {
                string fileDisplayName = schemaFileName is null
                    ? "null"
                    : $"\"{schemaFileName ?? "null"}\"";
                ReportDiagnostic(context, GeneratorDiagnostics.SchemaFileNotFound, contractSymbol, fileDisplayName);
                return;
            }

            var schemaFiles = additionalFiles
                .Where(file => Path.GetFileName(file.Path).Equals(schemaFileName, StringComparison.OrdinalIgnoreCase))
                .ToArray();

            if(schemaFiles.Length == 0)
            {
                ReportDiagnostic(context, GeneratorDiagnostics.SchemaFileNotFound, contractSymbol, schemaFileName);
                return;
            }
            if(schemaFiles.Length > 1)
            {
                ReportDiagnostic(context, GeneratorDiagnostics.MultipleSchemaFilesWithNameFound, contractSymbol, schemaFileName);
                return;
            }

            string? schemaText = schemaFiles.Single().GetText()?.ToString();
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

            var writer = CreateSourceWriter(contractSymbol.ContainingNamespace.ToString());
            string contractName = contractSymbol.Name;

            context.AddSource(
                $"{contractName}.generated.cs",
                writer.WriteContractSourceCode(contractSymbol.ContainingNamespace.ToString(), contractName, abiMembers)
            );
        }
        catch(Exception ex)
        {
            ReportDiagnostic(context, GeneratorDiagnostics.ExecutionFailed, contractSymbol, ex);
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

    private static ContractSourceWriter CreateSourceWriter(string @namespace)
    {
        var abiTypeWriter = new AbiTypeWriter(@namespace);
        var parameterTypeWriter = new AbiParameterTypeWriter(abiTypeWriter);

        return new ContractSourceWriter(
            abiTypeWriter,
            new ContractErrorSectionWriter(new ErrorTypeWriter()),
            new ContractEventSectionWriter(new EventTypeWriter()),
            new ContractFunctionSectionWriter(new ParamEncodingWriter(parameterTypeWriter))
        );
    }
}