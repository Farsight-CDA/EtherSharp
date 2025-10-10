using EtherSharp.Realtime.Blocks.Subscription;
using EtherSharp.Types;

namespace EtherSharp.Client.Modules.Blocks;

/// <summary>
/// Module used to interact with blocks.
/// </summary>
public interface IBlocksModule
{
    /// <summary>
    /// Fetches the current peak block number.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<ulong> GetPeakHeightAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Fetches block information at the given height.
    /// </summary>
    /// <param name="targetBlockNumber"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<BlockDataTrasactionAsString> GetBlockAtHeightAsync(TargetBlockNumber targetBlockNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a subscription for listening to new block heads.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<IBlocksSubscription> SubscribeNewHeadsAsync(CancellationToken cancellationToken = default);
}
