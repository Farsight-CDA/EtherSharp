using Microsoft.CodeAnalysis;

namespace EtherSharp.Generator;
public static class GeneratorDiagnostics
{
    private static class DiagnosticCategory
    {
        public const string Unknown = "Unknown";
        public const string Usage = "Usage";
    }

    public static readonly DiagnosticDescriptor ExecutionFailed = new DiagnosticDescriptor(
        "EVMG0000",
        "Generator Execution Failed",
        "An exception occured while executing CosmWasm Generator. {0}.",
        DiagnosticCategory.Unknown,
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor SchemaFileNotFound = new DiagnosticDescriptor(
        "EVMG0001",
        "Schema file not found",
        "Schema file {0} not found. Ensure the build action is set to \"C# Analyzer additional file\".",
        DiagnosticCategory.Usage,
        DiagnosticSeverity.Warning,
        true
    );

    public static readonly DiagnosticDescriptor SchemaFileMalformed = new DiagnosticDescriptor(
        "EVMG0002",
        "Schema file could not be parsed",
        "Schema file could not be parsed. An exception occured: {0}.",
        DiagnosticCategory.Usage,
        DiagnosticSeverity.Warning,
        true
    );

    public static readonly DiagnosticDescriptor SchemaFileNotSpecified = new DiagnosticDescriptor(
        "EVMG0003",
        "Schema not specified",
        "Schema file not specified. Ensure your contract interface {0} has an attribute of type ContractSchemaFilePathAttribute.",
        DiagnosticCategory.Usage,
        DiagnosticSeverity.Warning,
        true
    );

    public static readonly DiagnosticDescriptor GenerationFailed = new DiagnosticDescriptor(
        "EVMG0004",
        "Exception during Generation",
        "{0}",
        DiagnosticCategory.Unknown,
        DiagnosticSeverity.Warning,
        true
    );

    public static readonly DiagnosticDescriptor InterfaceMustBePartial = new DiagnosticDescriptor(
        "EVMG0005",
        "Contract Interface must be partial",
        "Source generation for the contract interface type {0} requires it to be partial",
        DiagnosticCategory.Usage,
        DiagnosticSeverity.Error,
        true
    );
}
