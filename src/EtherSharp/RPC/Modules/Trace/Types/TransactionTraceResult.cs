namespace EtherSharp.RPC.Modules.Trace.Types;

/// <summary>
/// Raw payload returned by <c>trace_replayTransaction</c>.
/// </summary>
/// <param name="Output">Top-level return bytes from transaction execution.</param>
/// <param name="Trace">Flat list of trace entries describing internal execution steps.</param>
public record TransactionTraceResult(
    byte[] Output,
    TransactionTrace[] Trace
);
