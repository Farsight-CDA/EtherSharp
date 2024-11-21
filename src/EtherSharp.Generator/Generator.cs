﻿using EtherSharp.Generator.Abi;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection;
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
                (ctx, _) => ctx.SemanticModel.GetDeclaredSymbol((InterfaceDeclarationSyntax) ctx.Node)
            )
            .Where(contractType =>
                contractType is not null &&
                contractType.AllInterfaces.Any(static x => x.Name == "IContract")
            )
            .Select((contractType, _) =>
            {
                var attribute = contractType!.GetAttributes()
                    .FirstOrDefault(x => x.AttributeClass?.Name == "AbiFileAttribute");

                return (
                    contractType!,
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
            cd.BaseList is not null &&
            cd.Modifiers.Any(SyntaxKind.PartialKeyword);

    private static void GenerateSource(SourceProductionContext context,
        ((INamedTypeSymbol, string?), ImmutableArray<AdditionalText> additionalFiles) combined)
    {
        var ((contractSymbol, schemaFileName), additionalFiles) = combined;

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

        if(!Debugger.IsAttached)
        {
            Debugger.Launch();
        }

        AbiMember[] abiMembers;
        try
        {
            abiMembers = JsonSerializer.Deserialize<AbiMember[]>(schemaText)
                ?? throw new NotSupportedException("Parsing schema file to ContractAPISchema failed");
        }
        catch(Exception ex)
        {
            ReportDiagnostic(context, GeneratorDiagnostics.SchemaFileMalformed, contractSymbol, ex);
            return;
        }

        throw new InvalidOperationException($"Found {abiMembers} Members");
    }

    private static void ReportDiagnostic(SourceProductionContext context, DiagnosticDescriptor descriptor, ISymbol symbol, params string[] args)
    {
        var diagnostic = Diagnostic.Create(descriptor, symbol.Locations.FirstOrDefault(), args);
        context.ReportDiagnostic(diagnostic);
    }

    private static void ReportDiagnostic(SourceProductionContext context, DiagnosticDescriptor descriptor, ISymbol symbol, Exception e)
    {
        var diagnostic = Diagnostic.Create(descriptor, symbol.Locations.FirstOrDefault(), e.Message);
        context.ReportDiagnostic(diagnostic);
    }
}