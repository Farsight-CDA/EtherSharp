using EtherSharp.Client.Services.Subscriptions;
using EtherSharp.Common;
using EtherSharp.Common.Comparer;
using EtherSharp.Common.Exceptions;
using EtherSharp.Contract;
using EtherSharp.Realtime.Events;
using EtherSharp.Realtime.Events.Polling;
using EtherSharp.Realtime.Events.Subscription;
using EtherSharp.RPC.Modules.Eth;
using EtherSharp.RPC.Transport;
using EtherSharp.Types;

namespace EtherSharp.Client.Modules.Events;

internal sealed class EventsModule<TLog>(IRPCTransport rpcTransport, IEthRpcModule ethRpcModule, ISubscriptionsManager subscriptionsManager,
    EtherSharpJsonSerializerContext jsonSerializerContext, EventFilter? eventFilter = null) : IEventsModule<TLog>
    where TLog : ITxLog<TLog>
{
    private readonly IRPCTransport _rpcTransport = rpcTransport;
    private readonly IEthRpcModule _ethRpcModule = ethRpcModule;
    private readonly ISubscriptionsManager _subscriptionsManager = subscriptionsManager;
    private readonly EtherSharpJsonSerializerContext _jsonSerializerContext = jsonSerializerContext;

    private readonly EventFilterBuilder _eventFilterBuilder = new();
    private readonly EventFilter? _eventFilter = eventFilter;

    public IEventsModule<TLog> WithContracts(params ReadOnlySpan<IEVMContract> contracts)
    {
        _eventFilterBuilder.WithContracts(contracts);
        return this;
    }
    public IEventsModule<TLog> WithContractAddresses(params ReadOnlySpan<Address> contractAddresses)
    {
        _eventFilterBuilder.WithContractAddresses(contractAddresses);
        return this;
    }

    public IEventsModule<TLog> WithContracts(params IEnumerable<IEVMContract> contracts)
    {
        _eventFilterBuilder.WithContracts(contracts);
        return this;
    }
    public IEventsModule<TLog> WithContractAddresses(params IEnumerable<Address> contractAddresses)
    {
        _eventFilterBuilder.WithContractAddresses(contractAddresses);
        return this;
    }

    public IEventsModule<TLog> WithTopic0(params ReadOnlySpan<Bytes32> topics)
    {
        _eventFilterBuilder.WithTopic0(topics);
        return this;
    }
    public IEventsModule<TLog> WithTopic0(params IEnumerable<Bytes32> topics)
    {
        _eventFilterBuilder.WithTopic0(topics);
        return this;
    }
    public IEventsModule<TLog> WithTopic1(params ReadOnlySpan<Bytes32> topics)
    {
        _eventFilterBuilder.WithTopic1(topics);
        return this;
    }
    public IEventsModule<TLog> WithTopic1(params IEnumerable<Bytes32> topics)
    {
        _eventFilterBuilder.WithTopic1(topics);
        return this;
    }
    public IEventsModule<TLog> WithTopic2(params ReadOnlySpan<Bytes32> topics)
    {
        _eventFilterBuilder.WithTopic2(topics);
        return this;
    }
    public IEventsModule<TLog> WithTopic2(params IEnumerable<Bytes32> topics)
    {
        _eventFilterBuilder.WithTopic2(topics);
        return this;
    }
    public IEventsModule<TLog> WithTopic3(params ReadOnlySpan<Bytes32> topics)
    {
        _eventFilterBuilder.WithTopic3(topics);
        return this;
    }
    public IEventsModule<TLog> WithTopic3(params IEnumerable<Bytes32> topics)
    {
        _eventFilterBuilder.WithTopic3(topics);
        return this;
    }

    public EventFilter BuildEventFilter()
        => _eventFilter ?? _eventFilterBuilder.Build();

    public async Task<TLog[]> GetAllAsync(TargetHeight fromBlock = default, TargetHeight toBlock = default, Bytes32? blockHash = null,
        RpcRequestOptions requestOptions = default, CancellationToken cancellationToken = default)
    {
        if(fromBlock == default)
        {
            fromBlock = TargetHeight.Earliest;
        }

        var rawResults = await _ethRpcModule.GetLogsAsync(
            fromBlock,
            toBlock,
            BuildEventFilter(),
            blockHash,
            requestOptions,
            cancellationToken
        );

        Array.Sort(rawResults, EventComparer.Instance);

        if(typeof(TLog) == typeof(Log))
        {
            return rawResults as TLog[]
                ?? throw new ImpossibleException();
        }
        if(rawResults.Length == 0)
        {
            return [];
        }

        var results = new TLog[rawResults.Length];

        for(int i = 0; i < rawResults.Length; i++)
        {
            results[i] = TLog.Decode(rawResults[i]);
        }

        return results;
    }

    public async Task<IPollingEventFilter<TLog>> CreatePollingFilterAsync(TargetHeight fromBlock = default, TargetHeight toBlock = default,
        RpcRequestOptions requestOptions = default, CancellationToken cancellationToken = default)
    {
        var filter = new PollingEventFilter<TLog>(
            _rpcTransport,
            _ethRpcModule,
            fromBlock,
            toBlock,
            BuildEventFilter(),
            requestOptions
        );
        await filter.InitializeAsync(cancellationToken);
        return filter;
    }

    public async Task<IEventSubscription<TLog>> CreateSubscriptionAsync(
        RpcRequestOptions requestOptions = default, CancellationToken cancellationToken = default)
    {
        var subscription = new EventSubscription<TLog>(
            _ethRpcModule,
            _subscriptionsManager,
            _jsonSerializerContext,
            BuildEventFilter()
        );
        await _subscriptionsManager.InstallSubscriptionAsync(subscription, requestOptions, cancellationToken);
        return subscription;
    }
}
