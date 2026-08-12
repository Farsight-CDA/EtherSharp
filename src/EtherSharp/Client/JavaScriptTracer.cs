namespace EtherSharp.Client;

/// <summary>
/// Represents the source code for a custom JavaScript EVM tracer.
/// </summary>
/// <param name="Source">JavaScript expression that evaluates to the tracer object.</param>
public readonly record struct JavaScriptTracer(string Source);
