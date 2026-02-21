namespace EtherSharp.RPC.Modules.Trace.Types;

/// <summary>
/// Output/result portion of a transaction trace entry.
/// </summary>
/// <param name="GasUsed">Gas consumed by this execution step.</param>
/// <param name="Output">Return bytes produced by this execution step.</param>
public record TraceResult(
    ulong GasUsed,
    byte[] Output
);
