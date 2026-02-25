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
    private const string ABI_FILE_ATTRIBUTE_METADATA_NAME = "EtherSharp.Contract.AbiFileAttribute";

    /// <inheritdoc/>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var contractTypesProvider = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                ABI_FILE_ATTRIBUTE_METADATA_NAME,
                static (node, _) => node is InterfaceDeclarationSyntax,
                static (ctx, _) => ctx.TargetSymbol is INamedTypeSymbol symbol && symbol.AllInterfaces.Any(TypeIdentificationUtils.IsIEVMContract)
                    ? symbol
                    : null
            )
            .Where(symbol => symbol is not null)
            .Select((symbol, _) => symbol!);

        var additionalFilesByNameProvider = context.AdditionalTextsProvider
            .Select((file, _) => (FileName: Path.GetFileName(file.Path), File: file))
            .Collect()
            .Select((files, _) =>
            {
                var builder = ImmutableDictionary.CreateBuilder<string, ImmutableArray<AdditionalText>>(StringComparer.OrdinalIgnoreCase);

                foreach(var group in files.GroupBy(file => file.FileName, StringComparer.OrdinalIgnoreCase))
                {
                    builder[group.Key] = [.. group.Select(file => file.File)];
                }

                return builder.ToImmutable();
            });

        var combined = contractTypesProvider.Combine(additionalFilesByNameProvider);
        context.RegisterSourceOutput(combined, GenerateSource);
    }

    private static void GenerateSource(SourceProductionContext context,
        (INamedTypeSymbol contractSymbol, ImmutableDictionary<string, ImmutableArray<AdditionalText>> additionalFilesByName) combined)
    {
        var (contractSymbol, additionalFilesByName) = combined;

        try
        {
            if(!TryGetContractDetails(context, contractSymbol, additionalFilesByName,
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
        SourceProductionContext context, INamedTypeSymbol contractSymbol, ImmutableDictionary<string, ImmutableArray<AdditionalText>> additionalFilesByName,
        out AbiMember[] abiMembers, out byte[]? byteCode)
    {
        abiMembers = null!;
        byteCode = null!;

        bool isPartial = contractSymbol.DeclaringSyntaxReferences
            .Select(reference => reference.GetSyntax())
            .OfType<InterfaceDeclarationSyntax>()
            .Any(declaration => declaration.Modifiers.Any(SyntaxKind.PartialKeyword));

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

        additionalFilesByName.TryGetValue(abiFileName, out var abiFiles);

        if(abiFiles.IsDefaultOrEmpty)
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

            additionalFilesByName.TryGetValue(bytecodeFileName, out var bytecodeFiles);

            if(bytecodeFiles.IsDefaultOrEmpty)
            {
                ReportDiagnostic(context, GeneratorDiagnostics.BytecodeFileNotFound, contractSymbol, bytecodeFileName);
                return false;
            }
            if(bytecodeFiles.Length > 1)
            {
                ReportDiagnostic(context, GeneratorDiagnostics.MultipleBytecodeFilesWithNameFound, contractSymbol, bytecodeFileName);
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
