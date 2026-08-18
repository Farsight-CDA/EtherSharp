using EtherSharp.RPC.Modules.Trace.Types;
using EtherSharp.RPC.Transport;
using EtherSharp.Types;

namespace EtherSharp.Client.Modules.Trace;

/// <summary>
/// Provides tracing endpoints backed by the node trace RPC module.
/// </summary>
public interface ITraceModule
{
    /// <summary>
    /// Replays a transaction and materializes a hierarchical call tree.
    /// </summary>
    /// <param name="transactionHash">Transaction hash to trace.</param>
    /// <param name="requestOptions">Options controlling the RPC request.</param>
    /// <param name="cancellationToken">Token used to cancel the RPC request.</param>
    /// <returns>
    /// The root call trace with nested child calls, or <see langword="null"/> when the transaction cannot be traced.
    /// </returns>
    public Task<CallTrace?> TraceTransactionCallsAsync(
        Bytes32 transactionHash, RpcRequestOptions requestOptions = default,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Replays a transaction and returns the raw trace payload for the requested trace types.
    /// </summary>
    /// <param name="transactionHash">Transaction hash to replay.</param>
    /// <param name="traceTypes">Trace payloads requested from the RPC endpoint.</param>
    /// <param name="requestOptions">Options controlling the RPC request.</param>
    /// <param name="cancellationToken">Token used to cancel the RPC request.</param>
    /// <returns>
    /// Raw transaction trace result as returned by the node, or <see langword="null"/> when the transaction cannot be replayed.
    /// </returns>
    public Task<TransactionTraceResult?> ReplayTransactionAsync(
        Bytes32 transactionHash, TraceTypes traceTypes,
        RpcRequestOptions requestOptions = default,
        CancellationToken cancellationToken = default
    );

}
