namespace EtherSharp.RPC.Modules.Trace.Types;

/// <summary>
/// Represents the recursive virtual-machine execution trace of a call or creation.
/// </summary>
/// <param name="Code">EVM bytecode executed by this frame.</param>
/// <param name="Ops">Operations executed by this frame.</param>
public sealed record VmTrace(
    byte[] Code,
    VmOperation[] Ops
);
