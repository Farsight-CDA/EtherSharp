using Microsoft.CodeAnalysis;

namespace EtherSharp.Generator;

internal static class GeneratorDiagnostics
{
    private static class DiagnosticCategory
    {
        public const string UNKNOWN = "Unknown";
        public const string USAGE = "Usage";
    }

    public static readonly DiagnosticDescriptor ExecutionFailed = new DiagnosticDescriptor(
        "EVMG0000",
        "Generator Execution Failed",
        "An exception occured while executing Generator. {0}.",
        DiagnosticCategory.UNKNOWN,
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor InterfaceMustBePartial = new DiagnosticDescriptor(
        "EVMG0010",
        "Contract Interface must be partial",
        "Source generation for the contract interface type {0} requires it to be partial",
        DiagnosticCategory.USAGE,
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor AbiFileAttributeNotFound = new DiagnosticDescriptor(
        "EVMG0020",
        "ABI File not specified",
        "Ensure your contract interface {0} has an attribute of type AbiFileAttribute",
        DiagnosticCategory.USAGE,
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor MultipleAbiFileAttributeFound = new DiagnosticDescriptor(
        "EVMG0021",
        "Too many ABI files specified",
        "Ensure your contract interface {0} only has one attribute of type AbiFileAttribute",
        DiagnosticCategory.USAGE,
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor AbiFileNotFound = new DiagnosticDescriptor(
        "EVMG0022",
        "ABI file not found",
        "ABI file {0} not found. Ensure the build action is set to \"C# Analyzer additional file\".",
        DiagnosticCategory.USAGE,
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor MultipleAbiFilesWithNameFound = new DiagnosticDescriptor(
        "EVMG0023",
        "ABI file name must be unique",
        "Multiple contract ABI files with the name {0} have been found. Ensure they all have unique names.",
        DiagnosticCategory.USAGE,
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor AbiFileMalformed = new DiagnosticDescriptor(
        "EVMG0024",
        "ABI file could not be parsed",
        "ABI file could not be parsed. An exception occured: {0}.",
        DiagnosticCategory.USAGE,
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor MultipleBytecodeFileAttributeFound = new DiagnosticDescriptor(
        "EVMG0030",
        "Too many bytecode files specified",
        "Ensure your contract interface {0} only has one attribute of type BytecodeFileAttribute",
        DiagnosticCategory.USAGE,
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor BytecodeFileNotFound = new DiagnosticDescriptor(
        "EVMG0031",
        "Bytecode file not found",
        "Bytecode file {0} not found. Ensure the build action is set to \"C# Analyzer additional file\".",
        DiagnosticCategory.USAGE,
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor MultipleBytecodeFilesWithNameFound = new DiagnosticDescriptor(
        "EVMG0032",
        "Bytecode file name must be unique",
        "Multiple contract bytecode files with the name {0} have been found. Ensure they all have unique names.",
        DiagnosticCategory.USAGE,
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor BytecodeFileMalformed = new DiagnosticDescriptor(
        "EVMG0033",
        "Bytecode file could not be parsed",
        "Bytecode file could not be parsed. An exception occured: {0}.",
        DiagnosticCategory.USAGE,
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor MissingConstructorAbiMember = new DiagnosticDescriptor(
        "EVMG0100",
        "Bytecode file provided but missing constructor in ABI",
        "Bytecode file provided but missing constructor in ABI",
        DiagnosticCategory.USAGE,
        DiagnosticSeverity.Warning,
        true
    );
}
