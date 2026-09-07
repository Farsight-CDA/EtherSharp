using EtherSharp.Interpreter.Crypto;
using EtherSharp.Types;

namespace EtherSharp.Interpreter.Runtime.Precompiles;

/// <summary>
/// Implements the EVM RIPEMD-160 precompile.
/// </summary>
public sealed class Ripemd160Precompile : IPrecompile
{
    /// <summary>
    /// Gets the shared RIPEMD-160 precompile instance.
    /// </summary>
    public static Ripemd160Precompile Instance { get; } = new();

    private Ripemd160Precompile()
    {
    }

    /// <inheritdoc/>
    public Address Address { get; } = Address.FromString("0x0000000000000000000000000000000000000003");

    /// <inheritdoc/>
    public ValueTask<ExecutionResult> ExecuteAsync(IInterpreterHost host, PrecompileCall call)
    {
        // EVM results are left-padded to 32 bytes; the digest retains its byte order.
        byte[] output = new byte[32];
        Ripemd160.HashData(call.Input.Span, output.AsSpan(12));
        return ValueTask.FromResult(ExecutionResult.Success(output));
    }
}
