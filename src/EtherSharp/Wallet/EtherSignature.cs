using EtherSharp.Types;

namespace EtherSharp.Wallet;

/// <summary>
/// Represents an Ethereum ECDSA signature.
/// </summary>
/// <param name="R">The signature's 32-byte <c>r</c> component.</param>
/// <param name="S">The signature's 32-byte <c>s</c> component.</param>
public readonly record struct EtherSignature(Bytes32 R, Bytes32 S)
{
    /// <summary>
    /// Writes the signature as <c>r</c> followed by <c>s</c>.
    /// </summary>
    /// <param name="destination">The destination, which must be at least 64 bytes long.</param>
    public void CopyTo(Span<byte> destination)
    {
        R.CopyTo(destination[..32]);
        S.CopyTo(destination[32..64]);
    }
}
