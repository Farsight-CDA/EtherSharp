using EtherSharp.Client.Services.Subscriptions;
using EtherSharp.Common;
using EtherSharp.RPC.Modules.Eth;
using EtherSharp.RPC.Transport;
using EtherSharp.Types;

namespace EtherSharp.Realtime.Mempool.Subscription;

internal sealed class PendingTransactionHashesSubscription(
    IEthRpcModule ethRpcModule, ISubscriptionsManager subscriptionsManager,
    EtherSharpJsonSerializerContext jsonSerializerContext)
    : SubscriptionBase<Bytes32>(subscriptionsManager, jsonSerializerContext.Bytes32), IPendingTransactionHashesSubscription
{
    private readonly IEthRpcModule _ethRpcModule = ethRpcModule;

    public IAsyncEnumerable<SubscriptionEvent<Bytes32>> ListenAsync(CancellationToken cancellationToken = default)
        => ReadAsync(cancellationToken);

    protected override Task<string> SubscribeAsync(RpcRequestOptions requestOptions, CancellationToken cancellationToken)
        => _ethRpcModule.SubscribeNewPendingTransactionHashesAsync(requestOptions, cancellationToken);

    public override string ToString()
        => nameof(PendingTransactionHashesSubscription);
}
