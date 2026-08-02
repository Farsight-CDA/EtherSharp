using EtherSharp.Types;

namespace EtherSharp.Wallet;

/// <summary>
/// Represents a recoverable Ethereum ECDSA signature.
/// </summary>
/// <param name="R">The signature's 32-byte <c>r</c> component.</param>
/// <param name="S">The signature's 32-byte <c>s</c> component.</param>
/// <param name="RecoveryId">The recovery identifier, represented as 0/1 or 27/28.</param>
public readonly record struct RecoverableEtherSignature(Bytes32 R, Bytes32 S, byte RecoveryId)
{
    /// <summary>
    /// Writes the signature as <c>r</c> followed by <c>s</c> and the recovery identifier.
    /// </summary>
    /// <param name="destination">The destination, which must be at least 65 bytes long.</param>
    public void CopyTo(Span<byte> destination)
    {
        R.CopyTo(destination[..32]);
        S.CopyTo(destination[32..64]);
        destination[64] = RecoveryId;
    }
}
