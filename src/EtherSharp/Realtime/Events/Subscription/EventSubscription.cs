using EtherSharp.Client.Services.Subscriptions;
using EtherSharp.Common;
using EtherSharp.RPC.Modules.Eth;
using EtherSharp.RPC.Transport;
using EtherSharp.Types;
using System.Runtime.CompilerServices;

namespace EtherSharp.Realtime.Events.Subscription;

internal sealed class EventSubscription<TLog>(
    IEthRpcModule ethRpcModule, ISubscriptionsManager subscriptionsManager,
    EtherSharpJsonSerializerContext jsonSerializerContext, EventFilter eventFilter
)
    : SubscriptionBase<Log>(subscriptionsManager, jsonSerializerContext.Log), IEventSubscription<TLog>
    where TLog : ITxLog<TLog>
{
    private static readonly string _displayName = $"{nameof(EventSubscription<>)}<{typeof(TLog)}>".Replace('+', '.');

    private readonly IEthRpcModule _ethRpcModule = ethRpcModule;
    private readonly EventFilter _eventFilter = eventFilter;

    public async IAsyncEnumerable<SubscriptionEvent<TLog>> ListenAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await foreach(var subscriptionEvent in ReadAsync(cancellationToken))
        {
            yield return subscriptionEvent switch
            {
                Log log => TLog.Decode(log),
                SubscriptionInterrupted interrupted => interrupted,
                SubscriptionResumed resumed => resumed,
            };
        }
    }

    protected override Task<string> SubscribeAsync(RpcRequestOptions requestOptions, CancellationToken cancellationToken)
        => _ethRpcModule.SubscribeLogsAsync(_eventFilter, requestOptions, cancellationToken);

    public override string ToString()
        => _displayName;
}
