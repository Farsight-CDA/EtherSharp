using EtherSharp.Numerics;
using EtherSharp.Types;

namespace EtherSharp.Crypto;

/// <summary>
/// Defines the standard domain fields used to separate EIP-712 signatures.
/// </summary>
/// <param name="Name">Human-readable name of the signing domain.</param>
/// <param name="Version">Current major version of the signing domain.</param>
/// <param name="ChainId">Chain where the verifying contract is deployed.</param>
/// <param name="VerifyingContract">Contract that will verify the signature.</param>
/// <param name="Salt">Application-specific domain salt.</param>
public readonly record struct EIP712Domain(
    string? Name = null,
    string? Version = null,
    UInt256? ChainId = null,
    Address? VerifyingContract = null,
    Bytes32? Salt = null
);
