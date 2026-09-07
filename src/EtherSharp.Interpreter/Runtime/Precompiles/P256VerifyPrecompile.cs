using EtherSharp.Types;
using System.Security.Cryptography;

namespace EtherSharp.Interpreter.Runtime.Precompiles;

/// <summary>
/// Implements P-256 signature verification as specified by EIP-7951.
/// </summary>
public sealed class P256VerifyPrecompile : IPrecompile
{
    private static readonly byte[] _order = Convert.FromHexString("ffffffff00000000ffffffffffffffffbce6faada7179e84f3b9cac2fc632551");
    private static readonly byte[] _prime = Convert.FromHexString("ffffffff00000001000000000000000000000000ffffffffffffffffffffffff");

    /// <summary>The shared instance.</summary>
    public static P256VerifyPrecompile Instance { get; } = new();

    private P256VerifyPrecompile()
    {
    }

    /// <inheritdoc/>
    public Address Address { get; } = Address.FromString("0x0000000000000000000000000000000000000100");

    /// <inheritdoc/>
    public ValueTask<ExecutionResult> ExecuteAsync(IInterpreterHost host, PrecompileCall call)
    {
        // Input is exactly hash || r || s || qx || qy, with unsigned big-endian words.
        var input = call.Input.Span;
        if(input.Length != 160)
        {
            return ValueTask.FromResult(ExecutionResult.Success());
        }

        var r = input[32..64];
        var s = input[64..96];
        var x = input[96..128];
        var y = input[128..160];
        // High-s signatures are valid. Coordinates must not be reduced modulo p.
        if(!r.ContainsAnyExcept((byte) 0) || r.SequenceCompareTo(_order) >= 0
            || !s.ContainsAnyExcept((byte) 0) || s.SequenceCompareTo(_order) >= 0
            || x.SequenceCompareTo(_prime) >= 0 || y.SequenceCompareTo(_prime) >= 0
            || !input[96..].ContainsAnyExcept((byte) 0))
        {
            return ValueTask.FromResult(ExecutionResult.Success());
        }

        try
        {
            // The platform backend validates curve membership during public-key import.
            using var key = ECDsa.Create(new ECParameters
            {
                Curve = ECCurve.NamedCurves.nistP256,
                Q = new ECPoint { X = x.ToArray(), Y = y.ToArray() }
            });
            // Verify the supplied hash directly; ECDSA handles R at infinity and R.x mod n.
            if(!key.VerifyHash(input[..32], input[32..96], DSASignatureFormat.IeeeP1363FixedFieldConcatenation))
            {
                return ValueTask.FromResult(ExecutionResult.Success());
            }
        }
        catch(CryptographicException)
        {
            // Invalid keys/signatures are successful EVM calls with empty return data.
            return ValueTask.FromResult(ExecutionResult.Success());
        }

        byte[] output = new byte[32];
        output[31] = 1;
        return ValueTask.FromResult(ExecutionResult.Success(output));
    }
}
