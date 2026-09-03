using EtherSharp.Contract;
using EtherSharp.Interpreter.Precompiles;
using EtherSharp.Types;
using System.Diagnostics.CodeAnalysis;

namespace EtherSharp.Interpreter.Runtime;

/// <summary>
/// Defines consensus behavior selected for an interpreter execution.
/// </summary>
public sealed class InterpreterExecutionSpec
{
    private readonly Dictionary<Address, IPrecompile> _precompiles;

    /// <summary>
    /// Gets the maximum permitted initcode length in bytes.
    /// </summary>
    public int MaxInitCodeLength { get; init; } = EVMByteCode.MAX_INIT_LENGTH;

    /// <summary>
    /// Gets the maximum permitted runtime-code length in bytes.
    /// </summary>
    public int MaxRuntimeCodeLength { get; init; } = EVMByteCode.MAX_RUNTIME_LENGTH;

    /// <summary>
    /// Gets an execution specification with no registered precompiles.
    /// </summary>
    public static InterpreterExecutionSpec Empty { get; } = new();

    /// <summary>
    /// Creates an execution specification with no registered precompiles.
    /// </summary>
    public InterpreterExecutionSpec()
    {
        _precompiles = [];
    }

    /// <summary>
    /// Creates an execution specification with the supplied precompile registrations.
    /// </summary>
    /// <param name="precompiles">Precompiles keyed by their native execution address.</param>
    public InterpreterExecutionSpec(IEnumerable<KeyValuePair<Address, IPrecompile>> precompiles)
    {
        ArgumentNullException.ThrowIfNull(precompiles);

        _precompiles = [];
        foreach(var (address, precompile) in precompiles)
        {
            ArgumentNullException.ThrowIfNull(precompile);
            _precompiles.Add(address, precompile);
        }
    }

    /// <summary>
    /// Attempts to resolve a precompile at the supplied code address.
    /// </summary>
    /// <param name="codeAddress">The message call's code address.</param>
    /// <param name="precompile">The resolved precompile when registered.</param>
    /// <returns><see langword="true"/> when a precompile is registered at the address.</returns>
    public bool TryGetPrecompile(
        in Address codeAddress,
        [NotNullWhen(true)] out IPrecompile? precompile
    ) => _precompiles.TryGetValue(codeAddress, out precompile);
}
