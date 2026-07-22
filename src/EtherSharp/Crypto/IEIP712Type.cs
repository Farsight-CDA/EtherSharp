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
}
