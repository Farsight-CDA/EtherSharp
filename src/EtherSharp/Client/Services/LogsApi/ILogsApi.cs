using EtherSharp.Contract;
using EtherSharp.Filters;
using EtherSharp.Types;

namespace EtherSharp.Client.Services.LogsApi;
public interface ILogsApi
{    
    public ILogsApi HasTopic(string topic);
    public ILogsApi HasTopics(params ReadOnlySpan<string> topics);
    
    public ILogsApi HasContractAddress(string contractAddress);
    public ILogsApi HasContract(IEVMContract contract);
    public ILogsApi HasContractAddresses(params ReadOnlySpan<string> contractAddresses);
    public ILogsApi HasContracts(params ReadOnlySpan<IEVMContract> contracts);

    public Task<Log[]> GetAllAsync(TargetBlockNumber fromBlock = default, TargetBlockNumber toBlock = default, byte[]? blockHash = null);
    public Task<IEventFilter> ToFilterAsync(TargetBlockNumber fromBlock = default, TargetBlockNumber toBlock = default);
}
