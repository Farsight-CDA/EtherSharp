using EtherSharp.Client.Services.Subscriptions;
using EtherSharp.Common;
using EtherSharp.RPC.Modules.Eth;
using EtherSharp.RPC.Transport;

namespace EtherSharp.Realtime.Blocks.Subscription;

internal sealed class BlocksSubscription(
    IEthRpcModule ethRpcModule, ISubscriptionsManager subscriptionsManager,
    EtherSharpJsonSerializerContext jsonSerializerContext)
    : SubscriptionBase<BlockHeader>(subscriptionsManager, jsonSerializerContext.BlockHeader), IBlocksSubscription
{
    private readonly IEthRpcModule _ethRpcModule = ethRpcModule;

    public IAsyncEnumerable<SubscriptionEvent<BlockHeader>> ListenAsync(CancellationToken cancellationToken = default)
        => ReadAsync(cancellationToken);

    protected override Task<string> SubscribeAsync(RpcRequestOptions requestOptions, CancellationToken cancellationToken)
        => _ethRpcModule.SubscribeNewHeadsAsync(requestOptions, cancellationToken);

    public override string ToString()
        => nameof(BlocksSubscription);
}
