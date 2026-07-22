using EtherSharp.Generator.ABI.Members;
using EtherSharp.Generator.ABI.SourceWriters;
using EtherSharp.Generator.ABI.SourceWriters.Components;
using EtherSharp.Generator.ABI.Util;
using EtherSharp.Generator.Util;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Text.Json;

namespace EtherSharp.Generator.ABI;

/// <summary>
/// Contract Interface Source Generator.
/// </summary>
[Generator]
public sealed class Generator : IIncrementalGenerator
{
    private const string ABI_FILE_ATTRIBUTE_METADATA_NAME = "EtherSharp.Contract.AbiFileAttribute";

    /// <inheritdoc/>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var contractTypesProvider = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                ABI_FILE_ATTRIBUTE_METADATA_NAME,
                static (node, _) => node is InterfaceDeclarationSyntax,
                static (ctx, cancellationToken) => ContractInfo.Create(ctx, cancellationToken)
            )
            .Where(static contract => contract.HasValue)
            .Select(static (contract, _) => contract!.Value);

        var additionalFilesByNameProvider = context.AdditionalTextsProvider
            .Select(static (file, cancellationToken) => (
                FileName: Path.GetFileName(file.Path),
                Content: file.GetText(cancellationToken)?.ToString()
            ))
            .Collect()
            .Select(static (files, _) =>
            {
                var builder = ImmutableDictionary.CreateBuilder<string, ImmutableArray<string?>>(StringComparer.OrdinalIgnoreCase);

                foreach(var group in files.GroupBy(file => file.FileName, StringComparer.OrdinalIgnoreCase))
                {
                    builder[group.Key] = [.. group.Select(file => file.Content)];
                }

                return builder.ToImmutable();
            });

        var combined = contractTypesProvider
            .Combine(additionalFilesByNameProvider)
            .Select(static (combined, _) => ContractGenerationInput.Create(combined.Left, combined.Right))
            .WithTrackingName("ContractGenerationInput");
        context.RegisterSourceOutput(combined, GenerateSource);
    }

    private static void GenerateSource(SourceProductionContext context, ContractGenerationInput input)
    {
        var contract = input.Contract;

        try
        {
            if(!TryGetContractDetails(context, input,
                out var abiMembers, out byte[]? bytecode))
            {
                return;
            }

            foreach(var anonymousEvent in abiMembers.OfType<EventAbiMember>().Where(x => x.IsAnonymous))
            {
                ReportDiagnostic(context, GeneratorDiagnostics.AnonymousEventNotSupported, contract.Location, anonymousEvent.Name);
            }
            abiMembers.RemoveAll(static member => member is EventAbiMember { IsAnonymous: true });

            bool hasConstructor = abiMembers.Any(x => x is ConstructorAbiMember);

            if(!hasConstructor && bytecode is not null)
            {
                abiMembers.Add(ConstructorAbiMember.Empty);
            }

            var writer = CreateSourceWriter(contract.Namespace, contract.Name);

            context.AddSource(
                NameUtils.ToValidFileName($"{contract.Namespace}.{contract.MetadataName}.generated.cs"),
                writer.WriteContractSourceCode(contract.Namespace, contract.Name, abiMembers, bytecode)
            );
        }
        catch(Exception ex)
        {
            ReportDiagnostic(context, GeneratorDiagnostics.ExecutionFailed, contract.Location, ex);
            return;
        }
    }

    private static bool TryGetContractDetails(
        SourceProductionContext context, ContractGenerationInput input,
        out List<AbiMember> abiMembers, out byte[]? byteCode)
    {
        abiMembers = null!;
        byteCode = null!;
        var contract = input.Contract;

        if(!contract.IsPartial)
        {
            ReportDiagnostic(context, GeneratorDiagnostics.InterfaceMustBePartial, contract.Location, contract.Name);
            return false;
        }

        if(contract.AbiFileAttributeCount == 0)
        {
            ReportDiagnostic(context, GeneratorDiagnostics.AbiFileAttributeNotFound, contract.Location, contract.Name);
            return false;
        }
        if(contract.AbiFileAttributeCount > 1)
        {
            ReportDiagnostic(context, GeneratorDiagnostics.MultipleAbiFileAttributeFound, contract.Location, contract.Name);
            return false;
        }

        string? abiFileName = contract.AbiFileName;

        if(abiFileName is null || String.IsNullOrEmpty(abiFileName))
        {
            string fileDisplayName = abiFileName is null
                ? "null"
                : $"\"{abiFileName}\"";
            ReportDiagnostic(context, GeneratorDiagnostics.AbiFileNotFound, contract.Location, fileDisplayName);
            return false;
        }

        if(input.AbiFile.Count == 0)
        {
            ReportDiagnostic(context, GeneratorDiagnostics.AbiFileNotFound, contract.Location, abiFileName);
            return false;
        }
        if(input.AbiFile.Count > 1)
        {
            ReportDiagnostic(context, GeneratorDiagnostics.MultipleAbiFilesWithNameFound, contract.Location, abiFileName);
            return false;
        }

        string? schemaText = input.AbiFile.Content;
        if(String.IsNullOrEmpty(schemaText) || schemaText is null)
        {
            ReportDiagnostic(context, GeneratorDiagnostics.AbiFileMalformed, contract.Location);
            return false;
        }

        try
        {
            abiMembers = JsonSerializer.Deserialize<List<AbiMember>>(schemaText, ParsingUtils.AbiJsonOptions)
                ?? throw new NotSupportedException("Parsing schema file to ContractAPISchema failed");
        }
        catch(Exception ex)
        {
            ReportDiagnostic(context, GeneratorDiagnostics.AbiFileMalformed, contract.Location, ex);
            return false;
        }

        if(contract.BytecodeFileAttributeCount > 1)
        {
            ReportDiagnostic(context, GeneratorDiagnostics.MultipleBytecodeFileAttributeFound, contract.Location, contract.Name);
            return false;
        }

        if(contract.BytecodeFileAttributeCount == 1)
        {
            string? bytecodeFileName = contract.BytecodeFileName;

            if(bytecodeFileName is null || String.IsNullOrEmpty(bytecodeFileName))
            {
                string fileDisplayName = bytecodeFileName is null
                    ? "null"
                    : $"\"{bytecodeFileName}\"";
                ReportDiagnostic(context, GeneratorDiagnostics.BytecodeFileNotFound, contract.Location, fileDisplayName);
                return false;
            }

            if(input.BytecodeFile.Count == 0)
            {
                ReportDiagnostic(context, GeneratorDiagnostics.BytecodeFileNotFound, contract.Location, bytecodeFileName);
                return false;
            }
            if(input.BytecodeFile.Count > 1)
            {
                ReportDiagnostic(context, GeneratorDiagnostics.MultipleBytecodeFilesWithNameFound, contract.Location, bytecodeFileName);
                return false;
            }

            string bytecodeText = input.BytecodeFile.Content?.Trim() ?? String.Empty;
            if(bytecodeText.Length == 0)
            {
                ReportDiagnostic(context, GeneratorDiagnostics.BytecodeFileNotFound, contract.Location);
                return false;
            }

            try
            {
                byteCode = HexUtils.FromHex(bytecodeText);
            }
            catch(Exception ex)
            {
                ReportDiagnostic(context, GeneratorDiagnostics.BytecodeFileMalformed, contract.Location, ex);
                return false;
            }
        }

        return true;
    }

    private static void ReportDiagnostic(SourceProductionContext context, DiagnosticDescriptor descriptor, Location? location, params string[] args)
    {
        var diagnostic = Diagnostic.Create(descriptor, location, args);
        context.ReportDiagnostic(diagnostic);
    }

    private static void ReportDiagnostic(SourceProductionContext context, DiagnosticDescriptor descriptor, Location? location, Exception e)
    {
        var diagnostic = Diagnostic.Create(descriptor, location, e.ToString());
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
