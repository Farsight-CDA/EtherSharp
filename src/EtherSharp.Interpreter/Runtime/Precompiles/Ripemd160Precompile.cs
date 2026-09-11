using EtherSharp.Interpreter.Crypto;
using EtherSharp.Types;

namespace EtherSharp.Interpreter.Runtime.Precompiles;

/// <summary>The EVM RIPEMD-160 precompile.</summary>
public sealed class Ripemd160Precompile : IPrecompile
{
    /// <summary>The shared instance.</summary>
    public static Ripemd160Precompile Instance { get; } = new();

    private Ripemd160Precompile()
    {
    }

    /// <inheritdoc/>
    public Address Address { get; } = Address.FromString("0x0000000000000000000000000000000000000003");

    /// <inheritdoc/>
    public ValueTask<ExecutionResult> ExecuteAsync(IInterpreterHost host, PrecompileCall call)
    {
        if(!call.Gas.TryCharge(GetGasCost(call.Input.Length)))
        {
            return ValueTask.FromResult(ExecutionResult.ExceptionalHalt(ExceptionalHaltReason.OutOfGas));
        }

        // EVM results are left-padded to 32 bytes; the digest retains its byte order.
        byte[] output = new byte[32];
        Ripemd160.HashData(call.Input.Span, output.AsSpan(12));
        return ValueTask.FromResult(ExecutionResult.Success(output));
    }

    // Osaka gas prices: 600 base gas plus 120 per input word.
    private static ulong GetGasCost(int inputLength)
        => 600 + (120 * (((ulong) inputLength + 31) / 32));
}
