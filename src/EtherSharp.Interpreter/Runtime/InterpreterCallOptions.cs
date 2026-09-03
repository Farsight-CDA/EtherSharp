using EtherSharp.Types;

namespace EtherSharp.Interpreter.Runtime;

/// <summary>
/// Configures the execution state for an interpreter call simulation.
/// </summary>
public readonly record struct InterpreterCallOptions
{
    /// <summary>
    /// Gets the optional account state overrides applied for the duration of the call.
    /// </summary>
    public IReadOnlyDictionary<Address, AccountOverride>? StateOverrides { get; init; }
}
