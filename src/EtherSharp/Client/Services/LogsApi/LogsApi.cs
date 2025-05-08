using EtherSharp.Client.Services.RPC;
using EtherSharp.Common.Exceptions;
using EtherSharp.Contract;
using EtherSharp.Realtime.Events;
using EtherSharp.Realtime.Events.Filter;
using EtherSharp.Realtime.Events.Subscription;
using EtherSharp.Types;

namespace EtherSharp.Client.Services.LogsApi;
internal class LogsApi<TEvent>(IRpcClient rpcClient) : ILogsApi<TEvent>
    where TEvent : ITxEvent<TEvent>
{
    protected readonly IRpcClient _rpcClient = rpcClient;
    protected Dictionary<int, string[]?> _topics = [];
    protected string[]? _contractAddresses;

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

    public ILogsApi<TEvent> HasContract(IEVMContract contract)
    {
        AssertNoContractAddresses();
        _contractAddresses = [contract.Address.String];
        return this;
    }
    public ILogsApi<TEvent> HasContractAddress(string contractAddress)
    {
        AssertNoContractAddresses();
        _contractAddresses = [contractAddress];
        return this;
    }
    public ILogsApi<TEvent> HasContractAddress(Address contractAddress)
    {
        AssertNoContractAddresses();
        _contractAddresses = [contractAddress.String];
        return this;
    }

    public ILogsApi<TEvent> HasContractAddresses(params ReadOnlySpan<string> contractAddresses)
    {
        AssertNoContractAddresses();
        _contractAddresses = contractAddresses.ToArray();
        return this;
    }
    public ILogsApi<TEvent> HasContracts(params ReadOnlySpan<IEVMContract> contracts)
    {
        AssertNoContractAddresses();

        _contractAddresses = new string[contracts.Length];

        for(int i = 0; i < _contractAddresses.Length; i++)
        {
            _contractAddresses[i] = contracts[i].Address.String;
        }

        return this;
    }
    public ILogsApi<TEvent> HasContractAddresses(params ReadOnlySpan<Address> contractAddresses)
    {
        AssertNoContractAddresses();

        _contractAddresses = new string[contractAddresses.Length];

        for(int i = 0; i < _contractAddresses.Length; i++)
        {
            _contractAddresses[i] = contractAddresses[i].String;
        }

        return this;
    }

    public ILogsApi<TEvent> HasTopic(string topic, int index = 0)
    {
        AssertNoTopics(index);
        _topics[index] = [topic];
        return this;
    }
    public ILogsApi<TEvent> HasTopics(int index = 0, params ReadOnlySpan<string> topics)
    {
        AssertNoTopics(index);
        _topics[index] = topics.ToArray();
        return this;
    }

    public async Task<TEvent[]> GetAllAsync(TargetBlockNumber fromBlock = default, TargetBlockNumber toBlock = default, string? blockHash = null)
    {
        if(fromBlock == default)
        {
            fromBlock = TargetBlockNumber.Earliest;
        }

        var rawResults = await _rpcClient.EthGetLogsAsync(fromBlock, toBlock, _contractAddresses, CreateTopicsArray(), blockHash);

        if(typeof(TEvent) == typeof(Log))
        {
            return (rawResults as TEvent[])
                ?? throw new ImpossibleException();
        }
        //
        return [.. rawResults.Select(TEvent.Decode)];
    }

    public async Task<IEventFilter<TEvent>> CreateFilterAsync(TargetBlockNumber fromBlock = default, TargetBlockNumber toBlock = default)
    {
        var filter = new EventFilter<TEvent>(_rpcClient, fromBlock, toBlock, _contractAddresses, CreateTopicsArray());
        await filter.InitializeAsync(default);
        return filter;
    }

    public async Task<IEventSubscription<TEvent>> CreateSubscriptionAsync()
    {
        var subscription = new EventSubscription<TEvent>(_rpcClient, _contractAddresses, CreateTopicsArray());
        await subscription.InitializeAsync(default);
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
