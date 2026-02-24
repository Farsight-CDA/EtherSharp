using EtherSharp.Common.Exceptions;
using EtherSharp.RPC.Modules.Trace.Types;
using EtherSharp.Types;

namespace EtherSharp.RPC.Modules.Trace;

internal class TraceRpcModule(IRpcClient rpcClient) : ITraceRpcModule
{
    private readonly IRpcClient _rpcClient = rpcClient;

    public async Task<TransactionTraceResult> ReplayTransactionAsync(Hash32 txHash, string[] traceTypes, CancellationToken cancellationToken = default)
        => await _rpcClient.SendRpcRequestAsync<Hash32, string[], TransactionTraceResult>(
            "trace_replayTransaction", txHash, traceTypes, TargetHeight.Latest, cancellationToken) switch
        {
            RpcResult<TransactionTraceResult>.Success result => result.Result,
            RpcResult<TransactionTraceResult>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };
}
