using Microsoft.CodeAnalysis;

namespace EtherSharp.Generator.EIP712;

public static class EIP712GeneratorDiagnostics
{
    private const string CATEGORY = "Usage";

    public static readonly DiagnosticDescriptor ExecutionFailed = new(
        "EIP0000",
        "EIP-712 generation failed",
        "An exception occurred while generating EIP-712 type {0}: {1}",
        "Unknown",
        DiagnosticSeverity.Error,
        true);

    public static readonly DiagnosticDescriptor TypeMustBePartial = new(
        "EIP0010",
        "EIP-712 type must be partial",
        "Source generation for EIP-712 type {0} requires it to be partial",
        CATEGORY,
        DiagnosticSeverity.Error,
        true);

    public static readonly DiagnosticDescriptor InterfaceRequired = new(
        "EIP0011",
        "EIP-712 interface is required",
        "EIP-712 type {0} must implement IEIP712Type",
        CATEGORY,
        DiagnosticSeverity.Error,
        true);

    public static readonly DiagnosticDescriptor UnsupportedDeclaration = new(
        "EIP0012",
        "EIP-712 declaration is not supported",
        "EIP-712 type {0} is not supported: {1}",
        CATEGORY,
        DiagnosticSeverity.Error,
        true);

    public static readonly DiagnosticDescriptor UnsupportedMemberType = new(
        "EIP0020",
        "EIP-712 member type is not supported",
        "Member {0} has unsupported EIP-712 type {1}",
        CATEGORY,
        DiagnosticSeverity.Error,
        true);
}
