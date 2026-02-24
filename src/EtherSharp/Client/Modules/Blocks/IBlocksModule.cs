using EtherSharp.Realtime.Blocks.Subscription;
using EtherSharp.Types;

namespace EtherSharp.Client.Modules.Blocks;

/// <summary>
/// Provides block retrieval and block-head subscription operations.
/// </summary>
public interface IBlocksModule
{
    /// <summary>
    /// Gets the current canonical chain height.
    /// </summary>
    /// <param name="cancellationToken">Token used to cancel the RPC request.</param>
    /// <returns>The latest block number.</returns>
    public Task<ulong> GetPeakHeightAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a block by number.
    /// </summary>
    /// <param name="targetHeight">Block number selector (explicit height or symbolic target such as latest).</param>
    /// <param name="cancellationToken">Token used to cancel the RPC request.</param>
    /// <returns>Block data with transaction hashes.</returns>
    public Task<BlockDataTransactionAsString> GetBlockAtHeightAsync(TargetBlockNumber targetHeight, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates and installs a live subscription for new block headers.
    /// </summary>
    /// <param name="cancellationToken">Token used to cancel subscription setup.</param>
    /// <returns>An active blocks subscription.</returns>
    public Task<IBlocksSubscription> SubscribeNewHeadsAsync(CancellationToken cancellationToken = default);
}
