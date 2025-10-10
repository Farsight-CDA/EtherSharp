using EtherSharp.Types;

namespace EtherSharp.Realtime.Blocks.Subscription;

public interface IBlocksSubscription : IAsyncDisposable
{
    public IAsyncEnumerable<BlockHeader> ListenAsync(CancellationToken cancellationToken = default);
}
