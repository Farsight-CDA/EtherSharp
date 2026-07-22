using Microsoft.CodeAnalysis;

namespace EtherSharp.Generator.ABI;

internal static class GeneratorDiagnostics
{
    private static class DiagnosticCategory
    {
        public const string UNKNOWN = "Unknown";
        public const string USAGE = "Usage";
    }

    public static readonly DiagnosticDescriptor ExecutionFailed = new DiagnosticDescriptor(
        "ABI0000",
        "Generator Execution Failed",
        "An exception occured while executing Generator. {0}.",
        DiagnosticCategory.UNKNOWN,
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor InterfaceMustBePartial = new DiagnosticDescriptor(
        "ABI0010",
        "Contract Interface must be partial",
        "Source generation for the contract interface type {0} requires it to be partial",
        DiagnosticCategory.USAGE,
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor AbiFileAttributeNotFound = new DiagnosticDescriptor(
        "ABI0020",
        "ABI File not specified",
        "Ensure your contract interface {0} has an attribute of type AbiFileAttribute",
        DiagnosticCategory.USAGE,
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor MultipleAbiFileAttributeFound = new DiagnosticDescriptor(
        "ABI0021",
        "Too many ABI files specified",
        "Ensure your contract interface {0} only has one attribute of type AbiFileAttribute",
        DiagnosticCategory.USAGE,
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor AbiFileNotFound = new DiagnosticDescriptor(
        "ABI0022",
        "ABI file not found",
        "ABI file {0} not found. Ensure the build action is set to \"C# Analyzer additional file\".",
        DiagnosticCategory.USAGE,
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor MultipleAbiFilesWithNameFound = new DiagnosticDescriptor(
        "ABI0023",
        "ABI file name must be unique",
        "Multiple contract ABI files with the name {0} have been found. Ensure they all have unique names.",
        DiagnosticCategory.USAGE,
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor AbiFileMalformed = new DiagnosticDescriptor(
        "ABI0024",
        "ABI file could not be parsed",
        "ABI file could not be parsed. An exception occured: {0}.",
        DiagnosticCategory.USAGE,
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor MultipleBytecodeFileAttributeFound = new DiagnosticDescriptor(
        "ABI0030",
        "Too many bytecode files specified",
        "Ensure your contract interface {0} only has one attribute of type BytecodeFileAttribute",
        DiagnosticCategory.USAGE,
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor BytecodeFileNotFound = new DiagnosticDescriptor(
        "ABI0031",
        "Bytecode file not found",
        "Bytecode file {0} not found. Ensure the build action is set to \"C# Analyzer additional file\".",
        DiagnosticCategory.USAGE,
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor MultipleBytecodeFilesWithNameFound = new DiagnosticDescriptor(
        "ABI0032",
        "Bytecode file name must be unique",
        "Multiple contract bytecode files with the name {0} have been found. Ensure they all have unique names.",
        DiagnosticCategory.USAGE,
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor BytecodeFileMalformed = new DiagnosticDescriptor(
        "ABI0033",
        "Bytecode file could not be parsed",
        "Bytecode file could not be parsed. An exception occured: {0}.",
        DiagnosticCategory.USAGE,
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor AnonymousEventNotSupported = new DiagnosticDescriptor(
        "ABI0040",
        "Anonymous event is not supported",
        "Anonymous event {0} is not supported and will be skipped",
        DiagnosticCategory.USAGE,
        DiagnosticSeverity.Warning,
        true
    );
}
