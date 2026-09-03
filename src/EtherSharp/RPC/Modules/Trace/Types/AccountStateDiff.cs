using EtherSharp.Numerics;
using EtherSharp.Types;

namespace EtherSharp.RPC.Modules.Trace.Types;

/// <summary>
/// Represents field-level changes to one account.
/// </summary>
/// <param name="Balance">Account balance change.</param>
/// <param name="Nonce">Account nonce change.</param>
/// <param name="Code">Account bytecode change.</param>
/// <param name="Storage">Storage changes keyed by slot.</param>
public sealed record AccountStateDiff(
    StateChange<UInt256> Balance,
    StateChange<ulong> Nonce,
    StateChange<byte[]> Code,
    IReadOnlyDictionary<Bytes32, StateChange<Bytes32>> Storage
);
