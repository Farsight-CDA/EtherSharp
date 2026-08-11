using EtherSharp.Generator.ABI.Members;
using EtherSharp.Generator.ABI.SourceWriters;
using EtherSharp.Generator.ABI.SourceWriters.Components;
using EtherSharp.Generator.ABI.Util;
using EtherSharp.Generator.Util;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Buffers.Binary;
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
    private const int RUNTIME_OFFSET = 12;

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
                out var abiMembers, out byte[]? initCode, out byte[]? runtimeCode))
            {
                return;
            }

            foreach(var anonymousEvent in abiMembers.OfType<EventAbiMember>().Where(x => x.IsAnonymous))
            {
                ReportDiagnostic(context, GeneratorDiagnostics.AnonymousEventNotSupported, contract.Location, anonymousEvent.Name);
            }
            abiMembers.RemoveAll(static member => member is EventAbiMember { IsAnonymous: true });

            var constructorMember = abiMembers.OfType<ConstructorAbiMember>().SingleOrDefault();
            if(runtimeCode is not null && constructorMember is { Inputs.Length: > 0 })
            {
                ReportDiagnostic(context, GeneratorDiagnostics.RuntimeBytecodeWithConstructorParameters, contract.Location);
                return;
            }

            if(constructorMember is null && initCode is not null)
            {
                abiMembers.Add(ConstructorAbiMember.Empty);
            }

            var writer = CreateSourceWriter(contract.Namespace, contract.Name);

            context.AddSource(
                NameUtils.ToValidFileName($"{contract.Namespace}.{contract.MetadataName}.generated.cs"),
                writer.WriteContractSourceCode(contract.Namespace, contract.Name, abiMembers, initCode, runtimeCode)
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
        out List<AbiMember> abiMembers, out byte[]? initCode, out byte[]? runtimeCode)
    {
        abiMembers = null!;
        initCode = null;
        runtimeCode = null;
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

        if(contract.BytecodeAttributeCount > 1)
        {
            ReportDiagnostic(context, GeneratorDiagnostics.MultipleBytecodeAttributeFound, contract.Location, contract.Name);
            return false;
        }

        if(contract.BytecodeAttributeCount == 1)
        {
            bool hasInitCode = !String.IsNullOrWhiteSpace(contract.InitCode);
            bool hasRuntimeCode = !String.IsNullOrWhiteSpace(contract.RuntimeCode);
            if(hasInitCode == hasRuntimeCode)
            {
                ReportDiagnostic(context, GeneratorDiagnostics.BytecodeVariantInvalid, contract.Location);
                return false;
            }

            try
            {
                if(hasRuntimeCode)
                {
                    runtimeCode = HexUtils.FromHex(contract.RuntimeCode!.Trim());
                    if(runtimeCode.Length == 0)
                    {
                        ReportDiagnostic(context, GeneratorDiagnostics.BytecodeVariantInvalid, contract.Location);
                        return false;
                    }
                    if(runtimeCode.Length > UInt16.MaxValue)
                    {
                        ReportDiagnostic(context, GeneratorDiagnostics.BytecodeTooLong, contract.Location,
                            "runtime code", runtimeCode.Length.ToString(), UInt16.MaxValue.ToString());
                        return false;
                    }
                    initCode = CreateInitCode(runtimeCode);
                }
                else
                {
                    initCode = HexUtils.FromHex(contract.InitCode!.Trim());
                    if(initCode.Length == 0)
                    {
                        ReportDiagnostic(context, GeneratorDiagnostics.BytecodeVariantInvalid, contract.Location);
                        return false;
                    }
                }
            }
            catch(Exception ex)
            {
                ReportDiagnostic(context, GeneratorDiagnostics.BytecodeMalformed, contract.Location, ex);
                return false;
            }
        }

        return true;
    }

    private static byte[] CreateInitCode(byte[] runtimeCode)
    {
        byte[] initCode = new byte[RUNTIME_OFFSET + runtimeCode.Length];
        var header = initCode.AsSpan(0, RUNTIME_OFFSET);
        header[0] = 0x61;
        BinaryPrimitives.WriteUInt16BigEndian(header.Slice(1), (ushort) runtimeCode.Length);
        header[3] = 0x60;
        header[4] = RUNTIME_OFFSET;
        header[5] = 0x3D;
        header[6] = 0x39;
        header[7] = 0x61;
        BinaryPrimitives.WriteUInt16BigEndian(header.Slice(8), (ushort) runtimeCode.Length);
        header[10] = 0x3D;
        header[11] = 0xF3;
        runtimeCode.CopyTo(initCode, RUNTIME_OFFSET);
        return initCode;
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
