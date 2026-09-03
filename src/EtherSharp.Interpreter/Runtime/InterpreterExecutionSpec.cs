using EtherSharp.Contract;
using EtherSharp.Interpreter.Precompiles;
using EtherSharp.Types;
using System.Collections.Frozen;
using System.Collections.Immutable;

namespace EtherSharp.Interpreter.Runtime;

/// <summary>
/// Defines consensus behavior selected for an interpreter execution.
/// </summary>
public sealed record InterpreterExecutionSpec
{
    /// <summary>
    /// Gets the maximum permitted initcode length in bytes.
    /// </summary>
    public int MaxInitCodeLength { get; init; } = EVMByteCode.MAX_INIT_LENGTH;

    /// <summary>
    /// Gets the maximum permitted runtime-code length in bytes.
    /// </summary>
    public int MaxRuntimeCodeLength { get; init; } = EVMByteCode.MAX_RUNTIME_LENGTH;

    /// <summary>
    /// Gets the precompiles enabled by this execution specification.
    /// </summary>
    public ImmutableArray<IPrecompile> Precompiles { get; init; } = [];

    /// <summary>
    /// Gets an execution specification with no registered precompiles.
    /// </summary>
    public static InterpreterExecutionSpec Empty { get; } = new();

    internal InterpreterExecutionSpec Validate()
    {
        ArgumentOutOfRangeException.ThrowIfNegative(MaxInitCodeLength);
        ArgumentOutOfRangeException.ThrowIfNegative(MaxRuntimeCodeLength);

        if(Precompiles.IsDefault)
        {
            throw new InvalidOperationException("The precompile collection is uninitialized.");
        }

        HashSet<Address> precompileAddresses = [];
        foreach(var precompile in Precompiles)
        {
            ArgumentNullException.ThrowIfNull(precompile);
            if(!precompileAddresses.Add(precompile.Address))
            {
                throw new ArgumentException(
                    $"Multiple precompiles are registered at address {precompile.Address}.",
                    nameof(Precompiles)
                );
            }
        }

        return this;
    }

    internal FrozenDictionary<Address, IPrecompile> CreatePrecompileLookup()
        => Precompiles.IsEmpty
            ? FrozenDictionary<Address, IPrecompile>.Empty
            : Precompiles.ToFrozenDictionary(precompile => precompile.Address);
}
