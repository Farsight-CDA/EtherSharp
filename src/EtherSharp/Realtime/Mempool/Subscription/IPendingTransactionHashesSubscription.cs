using EtherSharp.Types;

namespace EtherSharp.Realtime.Mempool.Subscription;

/// <summary>
/// Represents a realtime stream of hashes of transactions entering the connected node's mempool.
/// The subscription is reinstalled automatically after the connection is lost or the node terminates it.
/// </summary>
public interface IPendingTransactionHashesSubscription : IAsyncDisposable
{
    /// <summary>
    /// Listens for the hashes of newly received pending transactions.
    /// </summary>
    /// <param name="cancellationToken">Token used to stop listening.</param>
    /// <returns>
    /// An async stream of transaction hashes. A <see cref="SubscriptionInterrupted"/> is emitted when the subscription is interrupted
    /// and a <see cref="SubscriptionResumed"/> once it is live again, transactions received in between may have been missed.
    /// Use <see cref="SubscriptionEventExtensions.IgnoreInterruptions{T}"/> if missed transactions are acceptable.
    /// </returns>
    public IAsyncEnumerable<SubscriptionEvent<Bytes32>> ListenAsync(CancellationToken cancellationToken = default);
}
