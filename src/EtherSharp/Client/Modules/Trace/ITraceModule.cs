using EtherSharp.RPC.Modules.Trace.Types;
using EtherSharp.Types;

namespace EtherSharp.Client.Modules.Trace;

public interface ITraceModule
{
    public Task<CallTrace> TraceTransactionCallsAsync(string transactionHash, CancellationToken cancellationToken = default);
    public Task<TransactionTraceResult> ReplayTransactionAsync(string transactionHash, IEnumerable<string> traceTypes, CancellationToken cancellationToken = default);
}
