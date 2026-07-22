using EtherSharp.Types;

namespace EtherSharp.Crypto;

/// <summary>
/// Defines an EIP-712 struct whose hash implementation is supplied by source generation.
/// </summary>
public interface IEIP712Type
{
    /// <summary>
    /// Calculates the EIP-712 struct hash for this value.
    /// </summary>
    /// <returns>The 32-byte struct hash.</returns>
    public Bytes32 HashStruct();

    /// <summary>
    /// Calculates the EIP-712 signing hash for this value and the supplied domain.
    /// </summary>
    /// <param name="domain">Signature domain.</param>
    /// <returns>The digest ready to pass to an Ethereum signer.</returns>
    public Bytes32 GetSigningHash(in EIP712Domain domain);
}
