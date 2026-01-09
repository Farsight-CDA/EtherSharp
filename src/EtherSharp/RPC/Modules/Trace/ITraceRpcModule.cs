using EtherSharp.RPC.Modules.Trace.Types;

namespace EtherSharp.RPC.Modules.Trace;

public interface ITraceRpcModule
{
    public Task<TransactionTraceResult> ReplayTransactionAsync(string txHash, string[] traceTypes, CancellationToken cancellationToken = default);
}
