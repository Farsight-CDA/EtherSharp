using EtherSharp.Contract;
using EtherSharp.Events;

namespace EtherSharp.Client.Services.LogsApi;
public interface ILogsApi<TEvent> : IConfiguredLogsApi<TEvent>
    where TEvent : ITxEvent<TEvent>
{
    public ILogsApi<TEvent> HasTopic(string topic);
    public ILogsApi<TEvent> HasTopics(params ReadOnlySpan<string> topics);

    public ILogsApi<TEvent> HasContractAddress(string contractAddress);
    public ILogsApi<TEvent> HasContract(IEVMContract contract);
    public ILogsApi<TEvent> HasContractAddresses(params ReadOnlySpan<string> contractAddresses);
    public ILogsApi<TEvent> HasContracts(params ReadOnlySpan<IEVMContract> contracts);
}
