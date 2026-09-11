using EtherSharp.Types;
using System.Security.Cryptography;

namespace EtherSharp.Interpreter.Runtime.Precompiles;

/// <summary>The EVM SHA-256 precompile.</summary>
public sealed class Sha256Precompile : IPrecompile
{
    /// <summary>The shared instance.</summary>
    public static Sha256Precompile Instance { get; } = new();

    private Sha256Precompile()
    {
    }

    /// <inheritdoc/>
    public Address Address { get; } = Address.FromString("0x0000000000000000000000000000000000000002");

    /// <inheritdoc/>
    public ValueTask<ExecutionResult> ExecuteAsync(IInterpreterHost host, PrecompileCall call)
        => ValueTask.FromResult(call.Gas.TryCharge(GetGasCost(call.Input.Length))
            ? ExecutionResult.Success(SHA256.HashData(call.Input.Span))
            : ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.OutOfGas)
        );

    // Osaka gas prices: 60 base gas plus 12 per input word.
    private static ulong GetGasCost(int inputLength)
        => 60 + (12 * (((ulong) inputLength + 31) / 32));
}
