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
    /// <summary>The dynamic gas pricing parameters.</summary>
    public required GasParameters GasParameters { get; init; }

    /// <summary>The fixed gas charge indexed by opcode byte, with exactly 256 entries.</summary>
    /// <remarks>Dynamic charges are additional to these costs. Entries do not control opcode availability.</remarks>
    public required ImmutableArray<ulong> FixedOpcodeGasCosts { get; init; }

    /// <summary>The initcode size limit in bytes.</summary>
    public int MaxInitCodeLength { get; init; } = EVMByteCode.MAX_INIT_LENGTH;

    /// <summary>The runtime-code size limit in bytes.</summary>
    public int MaxRuntimeCodeLength { get; init; } = EVMByteCode.MAX_RUNTIME_LENGTH;

    /// <summary>The enabled precompiles.</summary>
    public ImmutableArray<IPrecompile> Precompiles { get; init; } = [];

    /// <summary>The latest supplied preset, currently <see cref="Osaka"/>.</summary>
    /// <remarks>This does not select rules based on the fork's block height or chain.</remarks>
    public static InterpreterExecutionSpec Latest
        => Osaka;

    internal FrozenDictionary<Address, IPrecompile> ValidateAndCreatePrecompileLookup()
    {
        ArgumentOutOfRangeException.ThrowIfNegative(MaxInitCodeLength);
        ArgumentOutOfRangeException.ThrowIfNegative(MaxRuntimeCodeLength);
        ArgumentNullException.ThrowIfNull(GasParameters);
        GasParameters.Validate();

        if(FixedOpcodeGasCosts.IsDefault || FixedOpcodeGasCosts.Length != 256)
        {
            throw new ArgumentException("The fixed opcode gas table must contain exactly 256 entries.", nameof(FixedOpcodeGasCosts));
        }

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
