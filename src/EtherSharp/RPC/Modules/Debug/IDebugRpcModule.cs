using EtherSharp.Types;

namespace EtherSharp.RPC.Modules.Debug;

public interface IDebugRpcModule
{
    public Task<CallTrace> TraceTransactionCallsAsync(string transactionHash, CancellationToken cancellationToken = default);
}
