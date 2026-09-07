using EtherSharp.Crypto;
using EtherSharp.Types;
using Secp256k1Net;

namespace EtherSharp.Interpreter.Runtime.Precompiles;

/// <summary>
/// Implements the EVM ECRECOVER precompile.
/// </summary>
public sealed class EcRecoverPrecompile : IPrecompile
{
    // Lock initialization and retain the read-only recovery context for the process lifetime.
    private static readonly Lazy<Secp256k1> _secp256k1 = new(() => new(), LazyThreadSafetyMode.ExecutionAndPublication);

    /// <summary>
    /// Gets the shared ECRECOVER precompile instance.
    /// </summary>
    public static EcRecoverPrecompile Instance { get; } = new();

    private EcRecoverPrecompile()
    {
    }

    /// <inheritdoc/>
    public Address Address { get; } = Address.FromString("0x0000000000000000000000000000000000000001");

    /// <inheritdoc/>
    public ValueTask<ExecutionResult> ExecuteAsync(IInterpreterHost host, PrecompileCall call)
    {
        // Input is hash || v || r || s, right-padded with zeros and truncated to 128 bytes.
        Span<byte> input = stackalloc byte[128];
        input.Clear();
        call.Input.Span[..Math.Min(call.Input.Length, input.Length)].CopyTo(input);

        if(input[32..63].ContainsAnyExcept((byte) 0) || input[63] is not (27 or 28))
        {
            return ValueTask.FromResult(ExecutionResult.Success());
        }

        var secp256k1 = _secp256k1.Value;
        Span<byte> signature = stackalloc byte[65];
        Span<byte> publicKey = stackalloc byte[64];
        // Recovery accepts high-s signatures, unlike normal ECDSA verification.
        if(!secp256k1.EcdsaRecoverableSignatureParseCompact(signature, input[64..], input[63] - 27)
            || !secp256k1.EcdsaRecover(publicKey, signature, input[..32]))
        {
            return ValueTask.FromResult(ExecutionResult.Success());
        }

        Span<byte> serialized = stackalloc byte[65];
        nuint serializedLength = (nuint) serialized.Length;
        _ = secp256k1.EcPubkeySerialize(serialized, ref serializedLength, publicKey, Secp256k1EcFlags.Uncompressed);

        byte[] output = new byte[32];
        _ = Keccak256.TryHashData(serialized[1..], output);
        output.AsSpan(0, 12).Clear();
        return ValueTask.FromResult(ExecutionResult.Success(output));
    }
}
