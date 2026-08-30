using EtherSharp.Client;
using EtherSharp.Numerics;
using EtherSharp.RPC.Modules.Debug;
using EtherSharp.RPC.Transport;
using EtherSharp.Types;

namespace EtherSharp.Client.Modules.Debug;

internal sealed class DebugModule(IDebugRpcModule debugRpcModule) : IDebugModule
{
    private readonly IDebugRpcModule _debugRpcModule = debugRpcModule;

    public Task<CallTrace?> TraceTransactionCallsAsync(
        in Bytes32 transactionHash, RpcRequestOptions requestOptions, CancellationToken cancellationToken
    ) => _debugRpcModule.TraceTransactionCallsAsync(
        in transactionHash, requestOptions, cancellationToken
    );

    public Task<CallTrace?> TraceTransactionCallsAsync(
        string transactionHash, RpcRequestOptions requestOptions, CancellationToken cancellationToken
    ) => _debugRpcModule.TraceTransactionCallsAsync(
        transactionHash, requestOptions, cancellationToken
    );

    public Task<CallTrace> TraceCallCallsAsync(
        Address? to, UInt256? gasPrice, UInt256 value, ReadOnlyMemory<byte> data,
        in TraceCallOptions options, bool onlyTopCall, RpcRequestOptions requestOptions,
        CancellationToken cancellationToken
    ) => _debugRpcModule.TraceCallCallsAsync(
        to, gasPrice, value, data, in options, onlyTopCall, requestOptions, cancellationToken
    );

    public Task<PrestateTrace> TraceCallPrestateAsync(
        Address? to, UInt256? gasPrice, UInt256 value, ReadOnlyMemory<byte> data,
        in TraceCallOptions options, bool disableCode, bool disableStorage, RpcRequestOptions requestOptions,
        CancellationToken cancellationToken
    ) => _debugRpcModule.TraceCallPrestateAsync(
        to, gasPrice, value, data, in options, disableCode, disableStorage, requestOptions, cancellationToken
    );

    public Task<PrestateDiffTrace> TraceCallPrestateDiffAsync(
        Address? to, UInt256? gasPrice, UInt256 value, ReadOnlyMemory<byte> data,
        in TraceCallOptions options, bool disableCode, bool disableStorage, RpcRequestOptions requestOptions,
        CancellationToken cancellationToken
    ) => _debugRpcModule.TraceCallPrestateDiffAsync(
        to, gasPrice, value, data, in options, disableCode, disableStorage, requestOptions, cancellationToken
    );

    public Task<TResult> TraceCallJavaScriptAsync<TTracerConfig, TResult>(
        Address? to, UInt256? gasPrice, UInt256 value, ReadOnlyMemory<byte> data,
        in TraceCallOptions options, JavaScriptTracer tracer, TTracerConfig tracerConfig,
        RpcRequestOptions requestOptions, CancellationToken cancellationToken
    ) => _debugRpcModule.TraceCallJavaScriptAsync<TTracerConfig, TResult>(
        to, gasPrice, value, data, in options, tracer, tracerConfig, requestOptions, cancellationToken
    );
}
