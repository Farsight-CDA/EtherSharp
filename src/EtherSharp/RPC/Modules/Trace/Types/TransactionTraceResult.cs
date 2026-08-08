namespace EtherSharp.RPC.Modules.Trace.Types;

/// <summary>
/// Raw payload returned by ad-hoc <c>trace_*</c> methods.
/// </summary>
/// <param name="Output">Top-level return bytes from transaction execution, or empty bytes when execution returned no data.</param>
/// <param name="Trace">Flat trace entries, or an empty array when transaction tracing was not requested.</param>
/// <param name="VmTrace">Recursive virtual-machine trace when requested.</param>
/// <param name="StateDiff">Altered account state when requested.</param>
public sealed record TransactionTraceResult(
    byte[] Output,
    TransactionTrace[] Trace,
    VmTrace? VmTrace = null,
    TraceStateDiff? StateDiff = null
);
