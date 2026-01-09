namespace EtherSharp.RPC.Modules.Trace.Types;

public record TraceResult(
    ulong GasUsed,
    byte[] Output
);
