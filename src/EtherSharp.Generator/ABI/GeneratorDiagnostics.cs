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

    public static readonly DiagnosticDescriptor MultipleBytecodeAttributeFound = new DiagnosticDescriptor(
        "ABI0030",
        "Too many bytecode attributes specified",
        "Ensure your contract interface {0} only has one attribute of type BytecodeAttribute",
        DiagnosticCategory.USAGE,
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor BytecodeVariantInvalid = new DiagnosticDescriptor(
        "ABI0031",
        "Bytecode variant is invalid",
        "BytecodeAttribute must specify exactly one non-empty initCode or runtimeCode value",
        DiagnosticCategory.USAGE,
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor BytecodeTooLong = new DiagnosticDescriptor(
        "ABI0032",
        "Bytecode is too long",
        "The supplied {0} is {1} bytes, exceeding the maximum of {2} bytes",
        DiagnosticCategory.USAGE,
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor BytecodeMalformed = new DiagnosticDescriptor(
        "ABI0033",
        "Bytecode could not be parsed",
        "Bytecode could not be parsed. An exception occured: {0}.",
        DiagnosticCategory.USAGE,
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor RuntimeBytecodeWithConstructorParameters = new DiagnosticDescriptor(
        "ABI0034",
        "Runtime bytecode cannot use constructor parameters",
        "Runtime bytecode cannot be used when the ABI constructor has parameters; supply initCode instead",
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
