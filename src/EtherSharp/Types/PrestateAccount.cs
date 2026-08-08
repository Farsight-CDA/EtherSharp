using EtherSharp.Numerics;

namespace EtherSharp.Types;

/// <summary>
/// Represents account state captured by the prestate tracer.
/// </summary>
/// <param name="Balance">Account balance when included in the trace.</param>
/// <param name="Nonce">Account nonce when included in the trace.</param>
/// <param name="Code">Account bytecode when included in the trace.</param>
/// <param name="Storage">Captured storage slots when included in the trace.</param>
public sealed record PrestateAccount(
    UInt256? Balance,
    ulong? Nonce,
    byte[]? Code,
    IReadOnlyDictionary<Bytes32, Bytes32>? Storage
);
