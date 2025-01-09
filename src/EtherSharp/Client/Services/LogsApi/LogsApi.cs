using EtherSharp.Client.Services.RPC;
using EtherSharp.Common.Exceptions;
using EtherSharp.Contract;
using EtherSharp.Events;
using EtherSharp.Events.Filter;
using EtherSharp.Events.Subscription;
using EtherSharp.Types;

namespace EtherSharp.Client.Services.LogsApi;
internal class LogsApi<TEvent>(IRpcClient rpcClient) : ILogsApi<TEvent>
    where TEvent : ITxEvent<TEvent>
{
    protected readonly IRpcClient _rpcClient = rpcClient;
    protected string[]? _topics = null;
    protected string[]? _contractAddresses = null;

    private void AssertNoTopics()
    {
        if(_topics is null)
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

    public ILogsApi<TEvent> HasTopic(string topic)
    {
        AssertNoTopics();
        _topics = [topic];
        return this;
    }
    public ILogsApi<TEvent> HasTopics(params ReadOnlySpan<string> topics)
    {
        AssertNoTopics();
        _topics = topics.ToArray();
        return this;
    }

    public async Task<TEvent[]> GetAllAsync(TargetBlockNumber fromBlock = default, TargetBlockNumber toBlock = default, byte[]? blockHash = null)
    {
        if(fromBlock == default)
        {
            fromBlock = TargetBlockNumber.Earliest;
        }

        var rawResults = await _rpcClient.EthGetLogsAsync(fromBlock, toBlock, _contractAddresses, _topics, blockHash);

        if(typeof(TEvent) == typeof(Log))
        {
            return (rawResults as TEvent[])
                ?? throw new ImpossibleException();
        }
        //
        return rawResults.Select(TEvent.Decode).ToArray();
    }

    public async Task<IEventFilter<TEvent>> CreateFilterAsync(TargetBlockNumber fromBlock = default, TargetBlockNumber toBlock = default)
    {
        var filter = new EventFilter<TEvent>(_rpcClient, fromBlock, toBlock, _contractAddresses, _topics);
        await filter.InitializeAsync(default);
        return filter;
    }

    public async Task<IEventSubscription<TEvent>> CreateSubscriptionAsync()
    {
        var subscription = new EventSubscription<TEvent>(_rpcClient, _contractAddresses, _topics);
        await subscription.InitializeAsync(default);
        return subscription;
    }
}
