using EtherSharp.Client;
using EtherSharp.Common.Exceptions;
using EtherSharp.Numerics;
using EtherSharp.Types;
using System.Globalization;

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

    private sealed record TraceCallRequest(
        Address? From,
        Address? To,
        ulong? Gas,
        UInt256? GasPrice,
        UInt256 Value,
        ReadOnlyMemory<byte> Data);

    private sealed record TraceCallConfig<TTracerConfig>(
        string Tracer,
        TTracerConfig TracerConfig,
        string? Timeout,
        IReadOnlyDictionary<Address, AccountOverride>? StateOverrides,
        BlockOverride? BlockOverrides,
        uint? TxIndex
    );

    private sealed record CallTracerConfig(bool? OnlyTopCall);
    public Task<CallTrace> TraceCallCallsAsync(
        Address? to, ulong? gas, UInt256? gasPrice, UInt256 value, ReadOnlyMemory<byte> data,
        in TraceCallOptions options, bool onlyTopCall = false,
        CancellationToken cancellationToken = default)
    {
        var request = new TraceCallRequest(options.From, to, gas, gasPrice, value, data);
        var targetHeight = options.TargetHeight;
        var config = new TraceCallConfig<CallTracerConfig>(
            "callTracer",
            new CallTracerConfig(onlyTopCall ? true : null),
            FormatTimeout(options.TimeoutMilliseconds),
            options.StateOverrides,
            options.BlockOverrides,
            options.TransactionIndex);

        return SendAsync();

        async Task<CallTrace> SendAsync()
            => await _rpcClient.SendRpcRequestAsync<TraceCallRequest, TargetHeight, TraceCallConfig<CallTracerConfig>, CallTrace>(
                "debug_traceCall", request, targetHeight, config, targetHeight, cancellationToken) switch
            {
                RpcResult<CallTrace>.Success result => result.Result,
                RpcResult<CallTrace>.Null => throw new RPCException(-1, "debug_traceCall returned null", null),
                RpcResult<CallTrace>.Error error => throw RPCException.FromRPCError(error),
                _ => throw new NotImplementedException(),
            };
    }

    private sealed record PrestateTracerConfig(bool? DisableCode, bool? DisableStorage);
    public Task<PrestateTrace> TraceCallPrestateAsync(
        Address? to, ulong? gas, UInt256? gasPrice, UInt256 value, ReadOnlyMemory<byte> data,
        in TraceCallOptions options, bool disableCode = false, bool disableStorage = false,
        CancellationToken cancellationToken = default)
    {
        var request = new TraceCallRequest(options.From, to, gas, gasPrice, value, data);
        var targetHeight = options.TargetHeight;
        var config = new TraceCallConfig<PrestateTracerConfig>(
            "prestateTracer",
            new PrestateTracerConfig(disableCode ? true : null, disableStorage ? true : null),
            FormatTimeout(options.TimeoutMilliseconds),
            options.StateOverrides,
            options.BlockOverrides,
            options.TransactionIndex);

        return SendAsync();

        async Task<PrestateTrace> SendAsync()
            => await _rpcClient.SendRpcRequestAsync<TraceCallRequest, TargetHeight, TraceCallConfig<PrestateTracerConfig>, PrestateTrace>(
                "debug_traceCall", request, targetHeight, config, targetHeight, cancellationToken) switch
            {
                RpcResult<PrestateTrace>.Success result => result.Result,
                RpcResult<PrestateTrace>.Null => throw new RPCException(-1, "debug_traceCall returned null", null),
                RpcResult<PrestateTrace>.Error error => throw RPCException.FromRPCError(error),
                _ => throw new NotImplementedException(),
            };
    }

    private sealed record PrestateDiffTracerConfig(bool DiffMode, bool? DisableCode, bool? DisableStorage);
    public Task<PrestateDiffTrace> TraceCallPrestateDiffAsync(
        Address? to, ulong? gas, UInt256? gasPrice, UInt256 value, ReadOnlyMemory<byte> data,
        in TraceCallOptions options, bool disableCode = false, bool disableStorage = false,
        CancellationToken cancellationToken = default)
    {
        var request = new TraceCallRequest(options.From, to, gas, gasPrice, value, data);
        var targetHeight = options.TargetHeight;
        var config = new TraceCallConfig<PrestateDiffTracerConfig>(
            "prestateTracer",
            new PrestateDiffTracerConfig(true, disableCode ? true : null, disableStorage ? true : null),
            FormatTimeout(options.TimeoutMilliseconds),
            options.StateOverrides,
            options.BlockOverrides,
            options.TransactionIndex);

        return SendAsync();

        async Task<PrestateDiffTrace> SendAsync()
            => await _rpcClient.SendRpcRequestAsync<TraceCallRequest, TargetHeight, TraceCallConfig<PrestateDiffTracerConfig>, PrestateDiffTrace>(
                "debug_traceCall", request, targetHeight, config, targetHeight, cancellationToken) switch
            {
                RpcResult<PrestateDiffTrace>.Success result => result.Result,
                RpcResult<PrestateDiffTrace>.Null => throw new RPCException(-1, "debug_traceCall returned null", null),
                RpcResult<PrestateDiffTrace>.Error error => throw RPCException.FromRPCError(error),
                _ => throw new NotImplementedException(),
            };
    }

    private static string? FormatTimeout(uint? timeoutMilliseconds)
    {
        if(timeoutMilliseconds is null)
        {
            return null;
        }

        ArgumentOutOfRangeException.ThrowIfZero(timeoutMilliseconds.Value, nameof(timeoutMilliseconds));
        return timeoutMilliseconds.Value.ToString(CultureInfo.InvariantCulture) + "ms";
    }
}
