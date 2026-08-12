using EtherSharp.Client;
using EtherSharp.Numerics;
using EtherSharp.RPC.Modules.Debug;
using EtherSharp.Types;

namespace EtherSharp.Client.Modules.Debug;

internal sealed class DebugModule(IDebugRpcModule debugRpcModule) : IDebugModule
{
    private readonly IDebugRpcModule _debugRpcModule = debugRpcModule;

    public Task<CallTrace?> TraceTransactionCallsAsync(in Bytes32 transactionHash, CancellationToken cancellationToken = default)
        => _debugRpcModule.TraceTransactionCallsAsync(in transactionHash, cancellationToken);

    public Task<CallTrace?> TraceTransactionCallsAsync(string transactionHash, CancellationToken cancellationToken = default)
        => _debugRpcModule.TraceTransactionCallsAsync(transactionHash, cancellationToken);

    public Task<CallTrace> TraceCallCallsAsync(
        Address? to, ulong? gas, UInt256? gasPrice, UInt256 value, ReadOnlyMemory<byte> data,
        in TraceCallOptions options, bool onlyTopCall = false,
        CancellationToken cancellationToken = default)
        => _debugRpcModule.TraceCallCallsAsync(
            to, gas, gasPrice, value, data, in options, onlyTopCall, cancellationToken);

    public Task<PrestateTrace> TraceCallPrestateAsync(
        Address? to, ulong? gas, UInt256? gasPrice, UInt256 value, ReadOnlyMemory<byte> data,
        in TraceCallOptions options, bool disableCode = false, bool disableStorage = false,
        CancellationToken cancellationToken = default)
        => _debugRpcModule.TraceCallPrestateAsync(
            to, gas, gasPrice, value, data, in options, disableCode, disableStorage, cancellationToken);

    public Task<PrestateDiffTrace> TraceCallPrestateDiffAsync(
        Address? to, ulong? gas, UInt256? gasPrice, UInt256 value, ReadOnlyMemory<byte> data,
        in TraceCallOptions options, bool disableCode = false, bool disableStorage = false,
        CancellationToken cancellationToken = default)
        => _debugRpcModule.TraceCallPrestateDiffAsync(
            to, gas, gasPrice, value, data, in options, disableCode, disableStorage, cancellationToken);

    public Task<TResult> TraceCallJavaScriptAsync<TTracerConfig, TResult>(
        Address? to, ulong? gas, UInt256? gasPrice, UInt256 value, ReadOnlyMemory<byte> data,
        in TraceCallOptions options, JavaScriptTracer tracer, TTracerConfig tracerConfig,
        CancellationToken cancellationToken = default)
        => _debugRpcModule.TraceCallJavaScriptAsync<TTracerConfig, TResult>(
            to, gas, gasPrice, value, data, in options, tracer, tracerConfig, cancellationToken);
}
