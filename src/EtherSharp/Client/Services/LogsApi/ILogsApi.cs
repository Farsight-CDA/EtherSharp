using EtherSharp.Contract;
using EtherSharp.Realtime.Events;
using EtherSharp.Types;

namespace EtherSharp.Client.Services.LogsApi;
public interface ILogsApi<TEvent> : IConfiguredLogsApi<TEvent>
    where TEvent : ITxEvent<TEvent>
{
    public ILogsApi<TEvent> HasTopic(string topic, int index = 0);
    public ILogsApi<TEvent> HasTopics(int index = 0, params ReadOnlySpan<string> topics);

    public ILogsApi<TEvent> HasContractAddress(string contractAddress);
    public ILogsApi<TEvent> HasContractAddresses(params ReadOnlySpan<string> contractAddresses);

    public ILogsApi<TEvent> HasContractAddress(Address contractAddress);
    public ILogsApi<TEvent> HasContractAddresses(params ReadOnlySpan<Address> contractAddresses);

    public ILogsApi<TEvent> HasContract(IEVMContract contract);
    public ILogsApi<TEvent> HasContracts(params ReadOnlySpan<IEVMContract> contracts);
}
