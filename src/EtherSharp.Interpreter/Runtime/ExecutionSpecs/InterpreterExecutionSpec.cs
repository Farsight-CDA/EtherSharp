using EtherSharp.Contract;
using EtherSharp.Interpreter.Runtime.Precompiles;
using EtherSharp.Types;
using System.Collections.Frozen;
using System.Collections.Immutable;

namespace EtherSharp.Interpreter.Runtime.ExecutionSpecs;

/// <summary>
/// Defines consensus behavior selected for an interpreter execution.
/// </summary>
public sealed partial record InterpreterExecutionSpec
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

    /// <summary>
    /// Gets the latest execution preset supplied by this library, currently <see cref="Osaka"/>.
    /// </summary>
    /// <remarks>This does not select rules based on the fork's block height or chain.</remarks>
    public static InterpreterExecutionSpec Latest
        => Osaka;

    internal FrozenDictionary<Address, IPrecompile> ValidateAndCreatePrecompileLookup()
    {
        ArgumentOutOfRangeException.ThrowIfNegative(MaxInitCodeLength);
        ArgumentOutOfRangeException.ThrowIfNegative(MaxRuntimeCodeLength);

        if(Precompiles.IsDefault)
        {
            throw new InvalidOperationException("The precompile collection is uninitialized.");
        }

        if(Precompiles.IsEmpty)
        {
            return FrozenDictionary<Address, IPrecompile>.Empty;
        }

        var precompiles = new Dictionary<Address, IPrecompile>(Precompiles.Length);
        foreach(var precompile in Precompiles)
        {
            ArgumentNullException.ThrowIfNull(precompile);
            if(!precompiles.TryAdd(precompile.Address, precompile))
            {
                throw new ArgumentException(
                    $"Multiple precompiles are registered at address {precompile.Address}.",
                    nameof(Precompiles)
                );
            }
        }

        return precompiles.ToFrozenDictionary();
    }
}
