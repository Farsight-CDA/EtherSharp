using EtherSharp.Generator.Abi;
using EtherSharp.Generator.Abi.Members;
using EtherSharp.Generator.SourceWriters;
using EtherSharp.Generator.SourceWriters.Components;
using EtherSharp.Generator.Util;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Text.Json;

namespace EtherSharp.Generator;

/// <summary>
/// Contract Interface Source Generator.
/// </summary>
[Generator]
public class Generator : IIncrementalGenerator
{
    /// <inheritdoc/>
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

        var combined = contractTypesProvider.Combine(context.AdditionalTextsProvider.Collect());
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
            if(!TryGetContractDetails(context, contractSymbol, contractNode, additionalFiles,
                out var abiMembers, out byte[]? bytecode))
            {
                return;
            }

            string @namespace = contractSymbol.ContainingNamespace.ToString();
            string contractName = contractSymbol.Name;

            bool hasConstructor = abiMembers.Any(x => x is ConstructorAbiMember);

            if(!hasConstructor && bytecode is not null)
            {
                ReportDiagnostic(context, GeneratorDiagnostics.MissingConstructorAbiMember, contractSymbol);
            }

            var writer = CreateSourceWriter(@namespace, contractName);

            context.AddSource(
                $"{contractName}.generated.cs",
                writer.WriteContractSourceCode(@namespace, contractName, abiMembers, bytecode)
            );
        }
        catch(Exception ex)
        {
            ReportDiagnostic(context, GeneratorDiagnostics.ExecutionFailed, contractSymbol, ex);
            return;
        }
    }

    private static bool TryGetContractDetails(
        SourceProductionContext context, INamedTypeSymbol contractSymbol, InterfaceDeclarationSyntax contractInterface, ImmutableArray<AdditionalText> additionalFiles,
        out AbiMember[] abiMembers, out byte[]? byteCode)
    {
        abiMembers = null!;
        byteCode = null!;

        bool isPartial = contractInterface.Modifiers.Any(SyntaxKind.PartialKeyword);

        if(!isPartial)
        {
            ReportDiagnostic(context, GeneratorDiagnostics.InterfaceMustBePartial, contractSymbol, contractSymbol.Name);
            return false;
        }

        var rawAttributes = contractSymbol.GetAttributes().Where(x => x.AttributeClass is not null).ToArray();

        var abiFileAttributes = rawAttributes
            .Where(x => TypeIdentificationUtils.IsAbiFileAttribute(x.AttributeClass!))
            .ToArray();
        var bytecodeFileAttributes = rawAttributes
            .Where(x => TypeIdentificationUtils.IsBytecodeFileAttribute(x.AttributeClass!))
            .ToArray();

        if(abiFileAttributes.Length == 0)
        {
            ReportDiagnostic(context, GeneratorDiagnostics.AbiFileAttributeNotFound, contractSymbol, contractSymbol.Name);
            return false;
        }
        if(abiFileAttributes.Length > 1)
        {
            ReportDiagnostic(context, GeneratorDiagnostics.MultipleAbiFileAttributeFound, contractSymbol, contractSymbol.Name);
            return false;
        }

        string? abiFileName = abiFileAttributes.Single().ConstructorArguments[0].Value?.ToString();

        if(abiFileName is null || String.IsNullOrEmpty(abiFileName))
        {
            string fileDisplayName = abiFileName is null
                ? "null"
                : $"\"{abiFileName}\"";
            ReportDiagnostic(context, GeneratorDiagnostics.AbiFileNotFound, contractSymbol, fileDisplayName);
            return false;
        }

        var abiFiles = additionalFiles
            .Where(file => Path.GetFileName(file.Path).Equals(abiFileName, StringComparison.OrdinalIgnoreCase))
            .ToArray();

        if(abiFiles.Length == 0)
        {
            ReportDiagnostic(context, GeneratorDiagnostics.AbiFileNotFound, contractSymbol, abiFileName);
            return false;
        }
        if(abiFiles.Length > 1)
        {
            ReportDiagnostic(context, GeneratorDiagnostics.MultipleAbiFilesWithNameFound, contractSymbol, abiFileName);
            return false;
        }

        string? schemaText = abiFiles.Single().GetText()?.ToString();
        if(String.IsNullOrEmpty(schemaText) || schemaText is null)
        {
            ReportDiagnostic(context, GeneratorDiagnostics.AbiFileMalformed, contractSymbol);
            return false;
        }

        try
        {
            abiMembers = JsonSerializer.Deserialize<AbiMember[]>(schemaText, ParsingUtils.AbiJsonOptions)
                ?? throw new NotSupportedException("Parsing schema file to ContractAPISchema failed");
        }
        catch(Exception ex)
        {
            ReportDiagnostic(context, GeneratorDiagnostics.AbiFileMalformed, contractSymbol, ex);
            return false;
        }

        if(bytecodeFileAttributes.Length > 1)
        {
            ReportDiagnostic(context, GeneratorDiagnostics.MultipleBytecodeFileAttributeFound, contractSymbol, contractSymbol.Name);
            return false;
        }

        if(bytecodeFileAttributes.Length == 1)
        {
            string? bytecodeFileName = bytecodeFileAttributes.Single().ConstructorArguments[0].Value?.ToString();

            if(bytecodeFileName is null || String.IsNullOrEmpty(bytecodeFileName))
            {
                string fileDisplayName = bytecodeFileName is null
                    ? "null"
                    : $"\"{bytecodeFileName}\"";
                ReportDiagnostic(context, GeneratorDiagnostics.BytecodeFileNotFound, contractSymbol, fileDisplayName);
                return false;
            }

            var bytecodeFiles = additionalFiles
                .Where(file => Path.GetFileName(file.Path).Equals(bytecodeFileName, StringComparison.OrdinalIgnoreCase))
                .ToArray();

            if(bytecodeFiles.Length == 0)
            {
                ReportDiagnostic(context, GeneratorDiagnostics.BytecodeFileNotFound, contractSymbol, bytecodeFileName);
                return false;
            }
            if(bytecodeFiles.Length > 1)
            {
                ReportDiagnostic(context, GeneratorDiagnostics.MultipleBytecodeFileAttributeFound, contractSymbol, bytecodeFileName);
                return false;
            }

            string? bytecodeText = bytecodeFiles.Single().GetText()?.ToString();
            if(String.IsNullOrEmpty(bytecodeText) || bytecodeText is null)
            {
                ReportDiagnostic(context, GeneratorDiagnostics.BytecodeFileNotFound, contractSymbol);
                return false;
            }

            try
            {
                byteCode = HexUtils.FromHex(bytecodeText);
            }
            catch(Exception ex)
            {
                ReportDiagnostic(context, GeneratorDiagnostics.BytecodeFileMalformed, contractSymbol, ex);
                return false;
            }
        }

        return true;
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

    private static ContractSourceWriter CreateSourceWriter(string @namespace, string contractInterfaceName)
    {
        var typesSectionWriter = new ContractTypesSectionWriter(@namespace, contractInterfaceName);
        var parameterTypeWriter = new AbiParameterTypeWriter(typesSectionWriter);
        var paramEncodingWriter = new ParamEncodingWriter(parameterTypeWriter);
        var memberTypeWriter = new MemberTypeWriter(paramEncodingWriter);

        return new ContractSourceWriter(
            new ContractErrorSectionWriter(new ErrorTypeWriter()),
            new ContractEventSectionWriter(new EventTypeWriter(paramEncodingWriter, memberTypeWriter)),
            new ContractFunctionSectionWriter(paramEncodingWriter, memberTypeWriter),
            typesSectionWriter
        );
    }
}
