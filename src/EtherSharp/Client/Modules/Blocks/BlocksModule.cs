using EtherSharp.Client.Services.Subscriptions;
using EtherSharp.Realtime.Blocks.Subscription;
using EtherSharp.RPC.Modules.Eth;
using EtherSharp.Types;
using System.Text.Json;

namespace EtherSharp.Client.Modules.Blocks;

internal class BlocksModule(IEthRpcModule ethRpcModule, ISubscriptionsManager subscriptionsManager,
    JsonSerializerOptions jsonSerializerOptions) : IBlocksModule
{
    private readonly IEthRpcModule _ethRpcModule = ethRpcModule;
    private readonly ISubscriptionsManager _subscriptionsManager = subscriptionsManager;
    private readonly JsonSerializerOptions _jsonSerializerOptions = jsonSerializerOptions;

    public Task<Block> GetBlockAtHeightAsync(TargetHeight targetHeight, CancellationToken cancellationToken = default)
        => _ethRpcModule.GetBlockByNumberAsync(targetHeight, cancellationToken);
    public Task<ulong> GetPeakHeightAsync(CancellationToken cancellationToken = default)
        => _ethRpcModule.BlockNumberAsync(cancellationToken);
    public async Task<IBlocksSubscription> SubscribeNewHeadsAsync(CancellationToken cancellationToken)
    {
        var subscription = new BlocksSubscription(_ethRpcModule, _subscriptionsManager, _jsonSerializerOptions);
        await _subscriptionsManager.InstallSubscriptionAsync(subscription, cancellationToken);
        return subscription;
    }
}
