using EtherSharp.RPC.Modules.Debug;
using EtherSharp.Types;

namespace EtherSharp.Client.Modules.Debug;

internal class DebugModule(IDebugRpcModule debugRpcModule) : IDebugModule
{
    private readonly IDebugRpcModule _debugRpcModule = debugRpcModule;

    public Task<CallTrace> TraceTransactionCallsAsync(string transactionHash, CancellationToken cancellationToken = default)
        => _debugRpcModule.TraceTransactionCallsAsync(transactionHash, cancellationToken);
}
