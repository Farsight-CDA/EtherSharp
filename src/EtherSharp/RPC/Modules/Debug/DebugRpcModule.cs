using EtherSharp.Common.Exceptions;
using EtherSharp.Types;

namespace EtherSharp.RPC.Modules.Debug;

internal class DebugRpcModule(IRpcClient rpcClient) : IDebugRpcModule
{
    private readonly IRpcClient _rpcClient = rpcClient;

    public async Task<CallTrace> TraceTransactionCallsAsync(string transactionHash, CancellationToken cancellationToken = default)
        => await _rpcClient.SendRpcRequestAsync<string, object, CallTrace>(
            "debug_traceTransaction", transactionHash, new { tracer = "callTracer" }, TargetHeight.Latest, cancellationToken) switch
        {
            RpcResult<CallTrace>.Success result => result.Result,
            RpcResult<CallTrace>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };
}
