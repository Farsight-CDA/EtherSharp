namespace EtherSharp.RPC.Modules.Trace.Types;

public record TransactionTraceResult(
    byte[] Output,
    TransactionTrace[] Trace
);
