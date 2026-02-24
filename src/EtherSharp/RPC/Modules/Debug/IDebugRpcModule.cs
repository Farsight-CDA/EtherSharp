using EtherSharp.Types;

namespace EtherSharp.RPC.Modules.Debug;

/// <summary>
/// Low-level wrapper for <c>debug_*</c> JSON-RPC methods.
/// </summary>
public interface IDebugRpcModule
{
    /// <summary>
    /// Traces calls executed by a mined transaction.
    /// </summary>
    /// <returns>
    /// The traced call tree for the transaction, or <see langword="null"/> when the node returns a null result (for example, transaction not found).
    /// </returns>
    public Task<CallTrace?> TraceTransactionCallsAsync(Hash32 transactionHash, CancellationToken cancellationToken = default);
}
