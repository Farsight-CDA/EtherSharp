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
    public Task<TransactionTraceResult> ReplayTransactionAsync(Hash32 txHash, string[] traceTypes, CancellationToken cancellationToken = default);
}
