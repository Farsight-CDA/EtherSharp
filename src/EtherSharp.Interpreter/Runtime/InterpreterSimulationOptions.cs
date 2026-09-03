using EtherSharp.Types;

namespace EtherSharp.Interpreter.Runtime;

/// <summary>
/// Configures state applied temporarily during interpreter simulation.
/// </summary>
public readonly record struct InterpreterSimulationOptions
{
    /// <summary>
    /// Gets the optional account state overrides applied for the duration of the call.
    /// </summary>
    public IReadOnlyDictionary<Address, AccountOverride>? StateOverrides { get; init; }
}
