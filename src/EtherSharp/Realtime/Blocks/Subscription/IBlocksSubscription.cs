namespace EtherSharp.Realtime.Blocks.Subscription;

/// <summary>
/// Represents a realtime stream of newly produced block headers.
/// The subscription is reinstalled automatically after the connection is lost.
/// </summary>
public interface IBlocksSubscription : IAsyncDisposable
{
    /// <summary>
    /// Listens for newly received block headers.
    /// </summary>
    /// <param name="cancellationToken">Token used to stop listening.</param>
    /// <returns>
    /// An async stream of block headers. A <see cref="SubscriptionInterrupted"/> is emitted when the subscription is interrupted
    /// and a <see cref="SubscriptionResumed"/> once it is live again, headers produced in between may have been missed.
    /// Use <see cref="SubscriptionEventExtensions.IgnoreInterruptions{T}"/> if missed headers are acceptable.
    /// </returns>
    public IAsyncEnumerable<SubscriptionEvent<BlockHeader>> ListenAsync(CancellationToken cancellationToken = default);
}
