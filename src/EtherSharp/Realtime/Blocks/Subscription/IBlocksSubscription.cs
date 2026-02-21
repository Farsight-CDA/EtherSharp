using EtherSharp.Types;

namespace EtherSharp.Realtime.Blocks.Subscription;

/// <summary>
/// Represents a realtime stream of newly produced block headers.
/// </summary>
public interface IBlocksSubscription : IAsyncDisposable
{
    public IAsyncEnumerable<BlockHeader> ListenAsync(CancellationToken cancellationToken = default);
}
