using EtherSharp.Interpreter.Runtime.Tracing;
using EtherSharp.Types;

namespace EtherSharp.Interpreter.Runtime;

/// <summary>
/// Configures temporary state and tracing for interpreter simulation.
/// </summary>
public readonly record struct InterpreterSimulationOptions
{
    /// <summary>Optional account overrides applied for the duration of the call.</summary>
    public IReadOnlyDictionary<Address, AccountOverride>? StateOverrides { get; init; }

    /// <summary>Optional execution hooks for the simulation.</summary>
    public IInterpreterExecutionHooks? Hooks { get; init; }
}
