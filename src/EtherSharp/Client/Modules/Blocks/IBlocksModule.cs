using EtherSharp.Realtime.Blocks.Subscription;
using EtherSharp.Types;

namespace EtherSharp.Client.Modules.Blocks;

public interface IBlocksModule
{
    public Task<ulong> GetPeakHeightAsync(CancellationToken cancellationToken = default);
    public Task<BlockDataTrasactionAsString> GetBlockAtHeightAsync(TargetBlockNumber targetBlockNumber, CancellationToken cancellationToken = default);

    public Task<IBlocksSubscription> SubscribeNewHeadsAsync(CancellationToken cancellationToken = default);
}
