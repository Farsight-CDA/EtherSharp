namespace EtherSharp.RPC.Modules.Trace.Types;

/// <summary>
/// Represents a contiguous memory change.
/// </summary>
/// <param name="Off">Byte offset at which the change begins.</param>
/// <param name="Data">Changed memory bytes.</param>
public sealed record VmMemoryDiff(
    ulong Off,
    byte[] Data
);
