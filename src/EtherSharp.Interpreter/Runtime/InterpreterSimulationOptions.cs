using EtherSharp.Interpreter.Runtime.Tracing;
using EtherSharp.Types;

namespace EtherSharp.Interpreter.Runtime;

/// <summary>
/// Configures temporary state, nonce handling, and tracing for interpreter simulation.
/// </summary>
public readonly record struct InterpreterSimulationOptions
{
    /// <summary>Controls top-level sender nonce lookup, validation, and increment.</summary>
    public TopLevelNonceHandling TopLevelNonceHandling { get; init; }

    /// <summary>Optional account overrides applied for the duration of the call.</summary>
    public IReadOnlyDictionary<Address, AccountOverride>? StateOverrides { get; init; }

    /// <summary>Optional execution hooks for the simulation.</summary>
    public IInterpreterExecutionHooks? Hooks { get; init; }
}
