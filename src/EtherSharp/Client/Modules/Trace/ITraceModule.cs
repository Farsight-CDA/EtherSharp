using EtherSharp.RPC.Modules.Trace.Types;
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
    /// <param name="cancellationToken">Token used to cancel the RPC request.</param>
    /// <returns>The root call trace with nested child calls.</returns>
    public Task<CallTrace> TraceTransactionCallsAsync(Hash32 transactionHash, CancellationToken cancellationToken = default);

    /// <summary>
    /// Replays a transaction and returns the raw trace payload for the requested trace types.
    /// </summary>
    /// <param name="transactionHash">Transaction hash to replay.</param>
    /// <param name="traceTypes">Trace categories requested by the RPC endpoint (for example <c>trace</c> or <c>stateDiff</c>).</param>
    /// <param name="cancellationToken">Token used to cancel the RPC request.</param>
    /// <returns>Raw transaction trace result as returned by the node.</returns>
    public Task<TransactionTraceResult> ReplayTransactionAsync(Hash32 transactionHash, string[] traceTypes, CancellationToken cancellationToken = default);
}
