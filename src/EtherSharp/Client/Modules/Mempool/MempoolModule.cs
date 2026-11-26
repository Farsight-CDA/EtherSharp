using EtherSharp.Client.Services.Subscriptions;
using EtherSharp.Common;
using EtherSharp.Realtime.Mempool.Subscription;
using EtherSharp.RPC.Modules.Eth;
using EtherSharp.RPC.Transport;

namespace EtherSharp.Client.Modules.Mempool;

internal sealed class MempoolModule(
    IEthRpcModule ethRpcModule, ISubscriptionsManager subscriptionsManager,
    EtherSharpJsonSerializerContext jsonSerializerContext) : IMempoolModule
{
    private readonly IEthRpcModule _ethRpcModule = ethRpcModule;
    private readonly ISubscriptionsManager _subscriptionsManager = subscriptionsManager;
    private readonly EtherSharpJsonSerializerContext _jsonSerializerContext = jsonSerializerContext;

    public async Task<IPendingTransactionHashesSubscription> SubscribePendingTransactionHashesAsync(
        RpcRequestOptions requestOptions = default, CancellationToken cancellationToken = default)
    {
        var subscription = new PendingTransactionHashesSubscription(_ethRpcModule, _subscriptionsManager, _jsonSerializerContext);
        await _subscriptionsManager.InstallSubscriptionAsync(subscription, requestOptions, cancellationToken);
        return subscription;
    }
}
