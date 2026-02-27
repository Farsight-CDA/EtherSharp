using EtherSharp.Types;

namespace EtherSharp.Client.Modules.Debug;

/// <summary>
/// Provides debugging endpoints backed by the node debug RPC module.
/// </summary>
public interface IDebugModule
{
    /// <summary>
    /// Traces call execution for a mined transaction.
    /// </summary>
    /// <param name="transactionHash">Transaction hash to trace.</param>
    /// <param name="cancellationToken">Token used to cancel the RPC request.</param>
    /// <returns>
    /// A call trace rooted at the transaction entry call, or <see langword="null"/> when the transaction cannot be traced.
    /// </returns>
    public Task<CallTrace?> TraceTransactionCallsAsync(Bytes32 transactionHash, CancellationToken cancellationToken = default);
}
