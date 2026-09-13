using EtherSharp.Interpreter.Runtime.Tracing;
using EtherSharp.Types;

namespace EtherSharp.Interpreter.Runtime;

/// <summary>
/// Configures state overrides, nonce handling, and tracing for interpreter execution and simulation.
/// </summary>
public readonly record struct InterpreterExecutionOptions
{
    /// <summary>Controls top-level sender nonce lookup, validation, and increment.</summary>
    public TopLevelNonceHandling TopLevelNonceHandling { get; init; }

    /// <summary>Optional account overrides applied before execution.</summary>
    public IReadOnlyDictionary<Address, AccountOverride>? StateOverrides { get; init; }

    /// <summary>Optional execution hooks for execution or simulation.</summary>
    public IInterpreterExecutionHooks? Hooks { get; init; }
}
