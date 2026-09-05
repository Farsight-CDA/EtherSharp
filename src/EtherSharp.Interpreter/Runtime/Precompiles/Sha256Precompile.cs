using EtherSharp.Types;
using System.Security.Cryptography;

namespace EtherSharp.Interpreter.Runtime.Precompiles;

/// <summary>
/// Implements the EVM SHA-256 precompile.
/// </summary>
public sealed class Sha256Precompile : IPrecompile
{
    /// <summary>
    /// Gets the shared SHA-256 precompile instance.
    /// </summary>
    public static Sha256Precompile Instance { get; } = new();

    private Sha256Precompile()
    {
    }

    /// <inheritdoc/>
    public Address Address { get; } = Address.FromString("0x0000000000000000000000000000000000000002");

    /// <inheritdoc/>
    public ValueTask<TxCallResult> ExecuteAsync(IInterpreterHost host, PrecompileCall call)
        => ValueTask.FromResult(new TxCallResult(true, SHA256.HashData(call.Input.Span)));
}
