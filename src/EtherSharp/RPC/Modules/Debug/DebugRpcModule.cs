using EtherSharp.Client;
using EtherSharp.Common.Exceptions;
using EtherSharp.Numerics;
using EtherSharp.RPC.Transport;
using EtherSharp.Types;
using System.Globalization;

namespace EtherSharp.RPC.Modules.Debug;

internal sealed class DebugRpcModule(RpcClient rpcClient) : IDebugRpcModule
{
    private readonly RpcClient _rpcClient = rpcClient;
    private readonly record struct CallTracerOptions(string Tracer);
    public async Task<CallTrace?> TraceTransactionCallsAsync(
        string transactionHash, RpcRequestOptions requestOptions, CancellationToken cancellationToken
    ) => await _rpcClient.SendRpcRequestAsync<string, CallTracerOptions, CallTrace>(
        "debug_traceTransaction", transactionHash, new CallTracerOptions("callTracer"), TargetHeight.Latest, requestOptions, cancellationToken) switch
    {
        RpcResult<CallTrace>.Success result => result.Result,
        RpcResult<CallTrace>.Null => null,
        RpcResult<CallTrace>.Error error => throw RPCException.FromRPCError(error),
        _ => throw new NotImplementedException(),
    };

    public Task<CallTrace?> TraceTransactionCallsAsync(
        in Bytes32 transactionHash, RpcRequestOptions requestOptions, CancellationToken cancellationToken
    )
    {
        var transactionHashValue = transactionHash;
        return TraceTransactionCallsCoreAsync(transactionHashValue, requestOptions, cancellationToken);
    }

    private async Task<CallTrace?> TraceTransactionCallsCoreAsync(
        Bytes32 transactionHash, RpcRequestOptions requestOptions, CancellationToken cancellationToken
    ) => await _rpcClient.SendRpcRequestAsync<Bytes32, CallTracerOptions, CallTrace>(
        "debug_traceTransaction", transactionHash, new CallTracerOptions("callTracer"), TargetHeight.Latest, requestOptions, cancellationToken) switch
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
        Address? to, UInt256? gasPrice, UInt256 value, ReadOnlyMemory<byte> data,
        in TraceCallOptions options, bool onlyTopCall, RpcRequestOptions requestOptions,
        CancellationToken cancellationToken
    )
    {
        var request = new TraceCallRequest(options.From, to, options.GasLimit, gasPrice, value, data);
        var targetHeight = options.TargetHeight;
        var config = new TraceCallConfig<CallTracerConfig>(
            "callTracer",
            new CallTracerConfig(onlyTopCall ? true : null),
            FormatTimeout(options.TimeoutMilliseconds),
            options.StateOverrides,
            options.BlockOverrides,
            options.TransactionIndex
        );

        return SendAsync();

        async Task<CallTrace> SendAsync()
            => await _rpcClient.SendRpcRequestAsync<TraceCallRequest, TargetHeight, TraceCallConfig<CallTracerConfig>, CallTrace>(
                "debug_traceCall", request, targetHeight, config, targetHeight, requestOptions, cancellationToken) switch
            {
                RpcResult<CallTrace>.Success result => result.Result,
                RpcResult<CallTrace>.Null => throw new RPCException(-1, "debug_traceCall returned null", null),
                RpcResult<CallTrace>.Error error => throw RPCException.FromRPCError(error),
                _ => throw new NotImplementedException(),
            };
    }

    private sealed record PrestateTracerConfig(bool? DisableCode, bool? DisableStorage);
    public Task<PrestateTrace> TraceCallPrestateAsync(
        Address? to, UInt256? gasPrice, UInt256 value, ReadOnlyMemory<byte> data,
        in TraceCallOptions options, bool disableCode, bool disableStorage, RpcRequestOptions requestOptions,
        CancellationToken cancellationToken
    )
    {
        var request = new TraceCallRequest(options.From, to, options.GasLimit, gasPrice, value, data);
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
                "debug_traceCall", request, targetHeight, config, targetHeight, requestOptions, cancellationToken) switch
            {
                RpcResult<PrestateTrace>.Success result => result.Result,
                RpcResult<PrestateTrace>.Null => throw new RPCException(-1, "debug_traceCall returned null", null),
                RpcResult<PrestateTrace>.Error error => throw RPCException.FromRPCError(error),
                _ => throw new NotImplementedException(),
            };
    }

    private sealed record PrestateDiffTracerConfig(bool DiffMode, bool? DisableCode, bool? DisableStorage);
    public Task<PrestateDiffTrace> TraceCallPrestateDiffAsync(
        Address? to, UInt256? gasPrice, UInt256 value, ReadOnlyMemory<byte> data,
        in TraceCallOptions options, bool disableCode, bool disableStorage, RpcRequestOptions requestOptions,
        CancellationToken cancellationToken
    )
    {
        var request = new TraceCallRequest(options.From, to, options.GasLimit, gasPrice, value, data);
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
                "debug_traceCall", request, targetHeight, config, targetHeight, requestOptions, cancellationToken) switch
            {
                RpcResult<PrestateDiffTrace>.Success result => result.Result,
                RpcResult<PrestateDiffTrace>.Null => throw new RPCException(-1, "debug_traceCall returned null", null),
                RpcResult<PrestateDiffTrace>.Error error => throw RPCException.FromRPCError(error),
                _ => throw new NotImplementedException(),
            };
    }

    public Task<TResult> TraceCallJavaScriptAsync<TTracerConfig, TResult>(
        Address? to, UInt256? gasPrice, UInt256 value, ReadOnlyMemory<byte> data,
        in TraceCallOptions options, JavaScriptTracer tracer, TTracerConfig tracerConfig,
        RpcRequestOptions requestOptions, CancellationToken cancellationToken
    )
    {
        var request = new TraceCallRequest(options.From, to, options.GasLimit, gasPrice, value, data);
        var targetHeight = options.TargetHeight;
        var config = new TraceCallConfig<TTracerConfig>(
            tracer.Source,
            tracerConfig,
            FormatTimeout(options.TimeoutMilliseconds),
            options.StateOverrides,
            options.BlockOverrides,
            options.TransactionIndex
        );

        return SendAsync();

        async Task<TResult> SendAsync()
            => await _rpcClient.SendRpcRequestAsync<TraceCallRequest, TargetHeight, TraceCallConfig<TTracerConfig>, TResult>(
                "debug_traceCall", request, targetHeight, config, targetHeight, requestOptions, cancellationToken) switch
            {
                RpcResult<TResult>.Success result => result.Result,
                RpcResult<TResult>.Null => throw new RPCException(-1, "debug_traceCall returned null", null),
                RpcResult<TResult>.Error error => throw RPCException.FromRPCError(error),
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
