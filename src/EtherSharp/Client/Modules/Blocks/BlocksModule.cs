using EtherSharp.Client.Services.Subscriptions;
using EtherSharp.Realtime.Blocks.Subscription;
using EtherSharp.RPC.Modules.Eth;
using EtherSharp.Types;

namespace EtherSharp.Client.Modules.Blocks;

internal class BlocksModule(IEthRpcModule ethRpcModule, ISubscriptionsManager subscriptionsManager) : IBlocksModule
{
    private readonly IEthRpcModule _ethRpcModule = ethRpcModule;
    private readonly ISubscriptionsManager _subscriptionsManager = subscriptionsManager;

    public Task<BlockDataTrasactionAsString> GetBlockAtHeightAsync(TargetBlockNumber targetHeight, CancellationToken cancellationToken = default)
        => _ethRpcModule.GetBlockByNumberAsync(targetHeight, cancellationToken);
    public Task<ulong> GetPeakHeightAsync(CancellationToken cancellationToken = default)
        => _ethRpcModule.BlockNumberAsync(cancellationToken);
    public async Task<IBlocksSubscription> SubscribeNewHeadsAsync(CancellationToken cancellationToken)
    {
        var subscription = new BlocksSubscription(_ethRpcModule, _subscriptionsManager);
        await _subscriptionsManager.InstallSubscriptionAsync(subscription, cancellationToken);
        return subscription;
    }
}
