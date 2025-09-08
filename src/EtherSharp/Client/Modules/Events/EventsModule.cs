using EtherSharp.Client.Services.RPC;
using EtherSharp.Client.Services.Subscriptions;
using EtherSharp.Common.Comparer;
using EtherSharp.Common.Exceptions;
using EtherSharp.Contract;
using EtherSharp.Realtime.Events;
using EtherSharp.Realtime.Events.Filter;
using EtherSharp.Realtime.Events.Subscription;
using EtherSharp.Types;

namespace EtherSharp.Client.Modules.Events;
internal class EventsModule<TLog>(IRpcClient rpcClient, SubscriptionsManager subscriptionsManager) : IEventsModule<TLog>
    where TLog : ITxLog<TLog>
{
    protected readonly IRpcClient _rpcClient = rpcClient;
    protected readonly SubscriptionsManager _subscriptionsManager = subscriptionsManager;

    protected Dictionary<int, string[]?> _topics = [];
    protected Address[]? _contractAddresses;

    private void AssertNoTopics(int index)
    {
        if(!_topics.ContainsKey(index))
        {
            return;
        }

        throw new InvalidOperationException("Topics filter already configured");
    }
    private void AssertNoContractAddresses()
    {
        if(_contractAddresses is null)
        {
            return;
        }

        throw new InvalidOperationException("Contract address filter already configured");
    }

    public IEventsModule<TLog> HasContract(IEVMContract contract)
    {
        AssertNoContractAddresses();
        _contractAddresses = [contract.Address];
        return this;
    }
    public IEventsModule<TLog> HasContractAddress(Address contractAddress)
    {
        AssertNoContractAddresses();
        _contractAddresses = [contractAddress];
        return this;
    }

    public IEventsModule<TLog> HasContracts(params ReadOnlySpan<IEVMContract> contracts)
    {
        AssertNoContractAddresses();

        _contractAddresses = new Address[contracts.Length];

        for(int i = 0; i < _contractAddresses.Length; i++)
        {
            _contractAddresses[i] = contracts[i].Address;
        }

        return this;
    }
    public IEventsModule<TLog> HasContractAddresses(params ReadOnlySpan<Address> contractAddresses)
    {
        AssertNoContractAddresses();
        _contractAddresses = contractAddresses.ToArray();
        return this;
    }

    public IEventsModule<TLog> HasContracts(params IEnumerable<IEVMContract> contracts)
    {
        AssertNoContractAddresses();
        _contractAddresses = [.. contracts.Select(x => x.Address)];
        return this;
    }
    public IEventsModule<TLog> HasContractAddresses(params IEnumerable<Address> contractAddresses)
    {
        AssertNoContractAddresses();
        _contractAddresses = [.. contractAddresses];
        return this;
    }

    public IEventsModule<TLog> HasTopic(string topic, int index = 0)
    {
        AssertNoTopics(index);
        _topics[index] = [topic];
        return this;
    }
    public IEventsModule<TLog> HasTopics(int index = 0, params ReadOnlySpan<string> topics)
    {
        AssertNoTopics(index);
        _topics[index] = topics.ToArray();
        return this;
    }
    public IEventsModule<TLog> HasTopics(int index = 0, params IEnumerable<string> topics)
    {
        AssertNoTopics(index);
        _topics[index] = [.. topics];
        return this;
    }

    public async Task<TLog[]> GetAllAsync(TargetBlockNumber fromBlock = default, TargetBlockNumber toBlock = default, string? blockHash = null,
        CancellationToken cancellationToken = default)
    {
        if(fromBlock == default)
        {
            fromBlock = TargetBlockNumber.Earliest;
        }

        var rawResults = await _rpcClient.EthGetLogsAsync(fromBlock, toBlock, _contractAddresses, CreateTopicsArray(), blockHash, cancellationToken);

        Array.Sort(rawResults, EventComparer.Instance);

        if(typeof(TLog) == typeof(Log))
        {
            return rawResults as TLog[]
                ?? throw new ImpossibleException();
        }
        //
        return [.. rawResults.Select(TLog.Decode)];
    }

    public async Task<IEventFilter<TLog>> CreateFilterAsync(TargetBlockNumber fromBlock = default, TargetBlockNumber toBlock = default,
        CancellationToken cancellationToken = default)
    {
        var filter = new EventFilter<TLog>(_rpcClient, fromBlock, toBlock, _contractAddresses, CreateTopicsArray());
        await filter.InitializeAsync(cancellationToken);
        return filter;
    }

    public async Task<IEventSubscription<TLog>> CreateSubscriptionAsync(CancellationToken cancellationToken = default)
    {
        var subscription = new EventSubscription<TLog>(_rpcClient, _subscriptionsManager, _contractAddresses, CreateTopicsArray());
        await _subscriptionsManager.InstallSubscriptionAsync(subscription, cancellationToken);
        return subscription;
    }

    private string[]?[]? CreateTopicsArray()
    {
        if(_topics.Count == 0)
        {
            return null;
        }

        string[]?[]? topics = new string[]?[_topics.Max(x => x.Key) + 1];

        foreach(var (index, topicsArr) in _topics)
        {
            topics[index] = topicsArr;
        }

        return topics;
    }
}
