﻿using Microsoft.CodeAnalysis;

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
        "An exception occured while executing Generator. {0}.",
        DiagnosticCategory.Unknown,
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor InterfaceMustBePartial = new DiagnosticDescriptor(
        "EVMG0010",
        "Contract Interface must be partial",
        "Source generation for the contract interface type {0} requires it to be partial",
        DiagnosticCategory.Usage,
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor AbiFileAttributeNotFound = new DiagnosticDescriptor(
        "EVMG0020",
        "Schema not specified",
        "Ensure your contract interface {0} has an attribute of type AbiFileAttribute",
        DiagnosticCategory.Usage,
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor MultipleAbiFileAttributeFound = new DiagnosticDescriptor(
        "EVMG0021",
        "Too many schema files specified",
        "Ensure your contract interface {0} only has one attribute of type AbiFileAttribute",
        DiagnosticCategory.Usage,
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor SchemaFileNotFound = new DiagnosticDescriptor(
        "EVMG0020",
        "Schema file not found",
        "Schema file {0} not found. Ensure the build action is set to \"C# Analyzer additional file\".",
        DiagnosticCategory.Usage,
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor MultipleSchemaFilesWithNameFound = new DiagnosticDescriptor(
        "EVMG0022",
        "Schema file name must be unique",
        "Multiple contract schema files with the name {0} have been found. Ensure they all have unique names.",
        DiagnosticCategory.Usage,
        DiagnosticSeverity.Error,
        true
    );

    public static readonly DiagnosticDescriptor SchemaFileMalformed = new DiagnosticDescriptor(
        "EVMG0030",
        "Schema file could not be parsed",
        "Schema file could not be parsed. An exception occured: {0}.",
        DiagnosticCategory.Usage,
        DiagnosticSeverity.Error,
        true
    );
}
