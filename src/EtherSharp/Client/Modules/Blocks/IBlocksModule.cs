using EtherSharp.Realtime.Blocks.Subscription;
using EtherSharp.RPC.Transport;
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
    /// <param name="requestOptions">Options controlling the RPC request.</param>
    /// <param name="cancellationToken">Token used to cancel the RPC request.</param>
    /// <returns>The latest block number.</returns>
    public Task<ulong> GetPeakHeightAsync(RpcRequestOptions requestOptions = default,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a block by number.
    /// </summary>
    /// <param name="targetHeight">Block number selector (explicit height or symbolic target such as latest).</param>
    /// <param name="requestOptions">Options controlling the RPC request.</param>
    /// <param name="cancellationToken">Token used to cancel the RPC request.</param>
    /// <returns>Block data with transaction hashes.</returns>
    public Task<Block> GetBlockAtHeightAsync(TargetHeight targetHeight,
        RpcRequestOptions requestOptions = default, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates and installs a live subscription for new block headers.
    /// </summary>
    /// <param name="requestOptions">Options controlling subscription RPC requests.</param>
    /// <param name="cancellationToken">Token used to cancel subscription setup.</param>
    /// <returns>An active blocks subscription.</returns>
    public Task<IBlocksSubscription> SubscribeNewHeadsAsync(
        RpcRequestOptions requestOptions = default, CancellationToken cancellationToken = default);
}
