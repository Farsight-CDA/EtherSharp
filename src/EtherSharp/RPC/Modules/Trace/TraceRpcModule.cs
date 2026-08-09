using EtherSharp.Common.Exceptions;
using EtherSharp.Numerics;
using EtherSharp.RPC.Modules.Trace.Types;
using EtherSharp.Types;

namespace EtherSharp.RPC.Modules.Trace;

internal sealed class TraceRpcModule(RpcClient rpcClient) : ITraceRpcModule
{
    private readonly RpcClient _rpcClient = rpcClient;

    private sealed record TraceCallRequest(
        Address? From,
        Address? To,
        ulong? Gas,
        UInt256? GasPrice,
        UInt256 Value,
        ReadOnlyMemory<byte> Data
    );

    public async Task<TransactionTraceResult> CallAsync(
        Address? from, Address? to, ulong? gas, UInt256? gasPrice, UInt256 value, ReadOnlyMemory<byte> data,
        TraceTypes traceTypes, TargetHeight targetHeight = default,
        CancellationToken cancellationToken = default)
    {
        var transaction = new TraceCallRequest(from, to, gas, gasPrice, value, data);
        return await _rpcClient.SendRpcRequestAsync<TraceCallRequest, TraceTypes, TargetHeight, TransactionTraceResult>(
            "trace_call", transaction, traceTypes, targetHeight, targetHeight, cancellationToken) switch
        {
            RpcResult<TransactionTraceResult>.Success result => result.Result,
            RpcResult<TransactionTraceResult>.Null => throw new RPCException(-1, "trace_call returned null", null),
            RpcResult<TransactionTraceResult>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };
    }

    public async Task<TransactionTraceResult?> ReplayTransactionAsync(
        Bytes32 txHash, TraceTypes traceTypes, CancellationToken cancellationToken = default)
        => await _rpcClient.SendRpcRequestAsync<Bytes32, TraceTypes, TransactionTraceResult>(
            "trace_replayTransaction", txHash, traceTypes, TargetHeight.Latest, cancellationToken) switch
        {
            RpcResult<TransactionTraceResult>.Success result => result.Result,
            RpcResult<TransactionTraceResult>.Null => null,
            RpcResult<TransactionTraceResult>.Error error => throw RPCException.FromRPCError(error),
            _ => throw new NotImplementedException(),
        };
}
