using EtherSharp.Realtime.Mempool.Subscription;
using EtherSharp.RPC.Transport;

namespace EtherSharp.Client.Modules.Mempool;

/// <summary>
/// Provides access to the connected node's pending transaction pool.
/// </summary>
public interface IMempoolModule
{
    /// <summary>
    /// Creates and installs a live subscription for the hashes of new pending transactions.
    /// </summary>
    /// <param name="requestOptions">Options controlling subscription RPC requests.</param>
    /// <param name="cancellationToken">Token used to cancel subscription setup.</param>
    /// <returns>An active pending transaction hashes subscription.</returns>
    public Task<IPendingTransactionHashesSubscription> SubscribePendingTransactionHashesAsync(
        RpcRequestOptions requestOptions = default, CancellationToken cancellationToken = default);
}
