using EtherSharp.Client.Services.Subscriptions;
using EtherSharp.Common.Json;
using EtherSharp.Realtime.Blocks.Subscription;
using EtherSharp.RPC.Modules.Eth;
using EtherSharp.RPC.Transport;
using EtherSharp.Types;

namespace EtherSharp.Client.Modules.Blocks;

internal sealed class BlocksModule(IEthRpcModule ethRpcModule, ISubscriptionsManager subscriptionsManager,
    EtherSharpJsonSerializerContext jsonSerializerContext) : IBlocksModule
{
    private readonly IEthRpcModule _ethRpcModule = ethRpcModule;
    private readonly ISubscriptionsManager _subscriptionsManager = subscriptionsManager;
    private readonly EtherSharpJsonSerializerContext _jsonSerializerContext = jsonSerializerContext;

    public Task<Block> GetBlockAtHeightAsync(TargetHeight targetHeight,
        RpcRequestOptions requestOptions = default, CancellationToken cancellationToken = default)
        => _ethRpcModule.GetBlockByNumberAsync(targetHeight, requestOptions, cancellationToken);
    public Task<ulong> GetPeakHeightAsync(RpcRequestOptions requestOptions = default,
        CancellationToken cancellationToken = default)
        => _ethRpcModule.BlockNumberAsync(requestOptions, cancellationToken);
    public async Task<IBlocksSubscription> SubscribeNewHeadsAsync(
        RpcRequestOptions requestOptions = default, CancellationToken cancellationToken = default)
    {
        var subscription = new BlocksSubscription(_ethRpcModule, _subscriptionsManager, _jsonSerializerContext);
        await _subscriptionsManager.InstallSubscriptionAsync(subscription, requestOptions, cancellationToken);
        return subscription;
    }
}
