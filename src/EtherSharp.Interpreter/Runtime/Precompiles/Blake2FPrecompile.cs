using EtherSharp.Interpreter.Crypto;
using EtherSharp.Types;

namespace EtherSharp.Interpreter.Runtime.Precompiles;

/// <summary>
/// Implements the EIP-152 BLAKE2F precompile.
/// </summary>
public sealed class Blake2FPrecompile : IPrecompile
{
    /// <summary>
    /// Gets the shared BLAKE2F precompile instance.
    /// </summary>
    public static Blake2FPrecompile Instance { get; } = new();

    private Blake2FPrecompile()
    {
    }

    /// <inheritdoc/>
    public Address Address { get; } = Address.FromString("0x0000000000000000000000000000000000000009");

    /// <inheritdoc/>
    public ValueTask<TxCallResult> ExecuteAsync(IInterpreterHost host, PrecompileCall call)
    {
        if(call.Input.Span.Length != Blake2F.INPUT_LENGTH || call.Input.Span[212] > 1)
        {
            return ValueTask.FromResult(new TxCallResult(false, ReadOnlyMemory<byte>.Empty));
        }

        byte[] output = new byte[Blake2F.OUTPUT_LENGTH];
        Blake2F.Compress(call.Input.Span, output);
        return ValueTask.FromResult(new TxCallResult(true, output));
    }
}
