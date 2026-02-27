using EtherSharp.RPC.Modules.Trace.Types;
using EtherSharp.Types;

namespace EtherSharp.RPC.Modules.Trace;

/// <summary>
/// Low-level wrapper for <c>trace_*</c> JSON-RPC methods.
/// </summary>
public interface ITraceRpcModule
{
    /// <summary>
    /// Replays a transaction and returns requested trace payloads.
    /// </summary>
    /// <returns>
    /// The raw trace payload returned by the node, or <see langword="null"/> when the node returns a null result (for example, transaction not found).
    /// </returns>
    public Task<TransactionTraceResult?> ReplayTransactionAsync(Bytes32 txHash, string[] traceTypes, CancellationToken cancellationToken = default);
}
