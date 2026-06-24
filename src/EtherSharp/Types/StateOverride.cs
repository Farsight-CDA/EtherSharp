using EtherSharp.Numerics;

namespace EtherSharp.Types;

/// <summary>
/// Describes a single account entry in an <c>eth_call</c> state override set.
/// </summary>
public sealed record StateOverride(
    UInt256? Balance = null,
    ulong? Nonce = null,
    string? Code = null,
    IReadOnlyDictionary<Bytes32, Bytes32>? State = null,
    IReadOnlyDictionary<Bytes32, Bytes32>? StateDiff = null,
    Address? MovePrecompileToAddress = null
);
