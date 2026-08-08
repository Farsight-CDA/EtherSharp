namespace EtherSharp.RPC.Modules.Trace.Types;

/// <summary>
/// Represents one operation in a virtual-machine execution trace.
/// </summary>
/// <param name="Pc">Program counter before executing the operation.</param>
/// <param name="Cost">Gas cost of the operation.</param>
/// <param name="Ex">Execution effects, or <see langword="null"/> when unavailable.</param>
/// <param name="Sub">Subordinate call or creation trace, when this operation entered one.</param>
public sealed record VmOperation(
    ulong Pc,
    ulong Cost,
    VmExecutedOperation? Ex,
    VmTrace? Sub
);
