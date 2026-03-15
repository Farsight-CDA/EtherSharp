using EtherSharp.Common.Exceptions;
using EtherSharp.Types;

namespace EtherSharp.RPC.Modules.Debug;

internal sealed class DebugRpcModule(IRpcClient rpcClient) : IDebugRpcModule
{
    private readonly IRpcClient _rpcClient = rpcClient;

    public async Task<CallTrace?> TraceTransactionCallsAsync(string transactionHash, CancellationToken cancellationToken = default)
        => await _rpcClient.SendRpcRequestAsync<string, object, CallTrace>(
            "debug_traceTransaction", transactionHash, new { tracer = "callTracer" }, TargetHeight.Latest, cancellationToken) switch
        {
            RpcResult<CallTrace>.Success result => result.Result,
            RpcResult<CallTrace>.Null => null,
            RpcResult<CallTrace>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };

    public Task<CallTrace?> TraceTransactionCallsAsync(in Bytes32 transactionHash, CancellationToken cancellationToken = default)
    {
        var transactionHashValue = transactionHash;
        return TraceTransactionCallsCoreAsync(transactionHashValue, cancellationToken);
    }

    private async Task<CallTrace?> TraceTransactionCallsCoreAsync(Bytes32 transactionHash, CancellationToken cancellationToken = default)
        => await _rpcClient.SendRpcRequestAsync<Bytes32, object, CallTrace>(
            "debug_traceTransaction", transactionHash, new { tracer = "callTracer" }, TargetHeight.Latest, cancellationToken) switch
        {
            RpcResult<CallTrace>.Success result => result.Result,
            RpcResult<CallTrace>.Null => null,
            RpcResult<CallTrace>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };
}
