namespace EtherSharp.Realtime.Blocks.Subscription;

/// <summary>
/// Represents a realtime stream of newly produced block headers.
/// </summary>
public interface IBlocksSubscription : IAsyncDisposable
{
    /// <summary>
    /// Listens for newly received block headers.
    /// </summary>
    /// <param name="cancellationToken">Token used to stop listening.</param>
    /// <returns>An async stream of block headers.</returns>
    public IAsyncEnumerable<BlockHeader> ListenAsync(CancellationToken cancellationToken = default);
}
