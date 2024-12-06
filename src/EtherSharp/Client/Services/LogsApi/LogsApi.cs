using EtherSharp.Client.Services.RPC;
using EtherSharp.Contract;
using EtherSharp.Filters;
using EtherSharp.Types;

namespace EtherSharp.Client.Services.LogsApi;
internal class LogsApi(IRpcClient rpcClient) : ILogsApi
{
    private readonly IRpcClient _rpcClient = rpcClient;

    private string[]? _topics = null;
    private string[]? _contractAddresses = null;

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

    public ILogsApi HasContract(IEVMContract contract)
    {
        AssertNoContractAddresses();
        _contractAddresses = [contract.ContractAddress];
        return this;
    }
    public ILogsApi HasContractAddress(string contractAddress)
    {
        AssertNoContractAddresses();
        _contractAddresses = [contractAddress];
        return this;
    }
    public ILogsApi HasContractAddresses(params ReadOnlySpan<string> contractAddresses)
    {
        AssertNoContractAddresses();
        _contractAddresses = contractAddresses.ToArray();
        return this;
    }
    public ILogsApi HasContracts(params ReadOnlySpan<IEVMContract> contracts)
    {
        AssertNoContractAddresses();

        _contractAddresses = new string[contracts.Length];

        for (int i = 0; i < _contractAddresses.Length; i++) 
        {
            _contractAddresses[i] = contracts[i].ContractAddress;
        }

        return this;
    }

    public ILogsApi HasTopic(string topic)
    {
        AssertNoTopics();
        _topics = [topic];
        return this;
    }
    public ILogsApi HasTopics(params ReadOnlySpan<string> topics)
    {
        AssertNoTopics();
        _topics = topics.ToArray();
        return this;
    }

    public async Task<Log[]> GetAllAsync(TargetBlockNumber fromBlock = default, TargetBlockNumber toBlock = default, byte[]? blockHash = null)
    {
        if (fromBlock == default)
        {
            fromBlock = TargetBlockNumber.Earliest;
        }

        return await _rpcClient.EthGetLogsAsync(fromBlock, toBlock, _contractAddresses, _topics, blockHash);
    }

    public async Task<IEventFilter> ToFilterAsync(TargetBlockNumber fromBlock = default, TargetBlockNumber toBlock = default)
    {
        string filterId = await _rpcClient.EthNewFilterAsync(fromBlock, toBlock, _contractAddresses, _topics);
        return new EventFilter(_rpcClient, filterId);
    }
}
