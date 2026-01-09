namespace EtherSharp.RPC.Modules.Trace.Types;

public record TransactionTrace(
    TraceAction Action,
    TraceResult? Result,
    int Subtraces,
    int[] TraceAddress,
    string Type,
    string? Error
);
