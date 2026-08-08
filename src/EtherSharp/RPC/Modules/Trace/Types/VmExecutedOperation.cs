using EtherSharp.Numerics;

namespace EtherSharp.RPC.Modules.Trace.Types;

/// <summary>
/// Represents effects produced by an executed virtual-machine operation.
/// </summary>
/// <param name="Used">Total gas used by the operation.</param>
/// <param name="Push">Values pushed onto the stack.</param>
/// <param name="Mem">Memory change produced by the operation.</param>
/// <param name="Store">Storage change produced by the operation.</param>
public sealed record VmExecutedOperation(
    ulong Used,
    UInt256[] Push,
    VmMemoryDiff? Mem,
    VmStorageDiff? Store
);
