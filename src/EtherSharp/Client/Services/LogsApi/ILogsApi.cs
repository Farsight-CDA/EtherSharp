using EtherSharp.Contract;
using EtherSharp.Realtime.Events;
using EtherSharp.Types;
using System.Runtime.CompilerServices;

namespace EtherSharp.Client.Services.LogsApi;
public interface ILogsApi<TEvent> : IConfiguredLogsApi<TEvent>
    where TEvent : ITxEvent<TEvent>
{
    public ILogsApi<TEvent> HasTopic(string topic, int index = 0);
    [OverloadResolutionPriority(1)]
    public ILogsApi<TEvent> HasTopics(int index = 0, params ReadOnlySpan<string> topics);
    public ILogsApi<TEvent> HasTopics(int index = 0, params IEnumerable<string> topics);

    public ILogsApi<TEvent> HasContractAddress(Address contractAddress);
    [OverloadResolutionPriority(1)]
    public ILogsApi<TEvent> HasContractAddresses(params ReadOnlySpan<Address> contractAddresses);
    public ILogsApi<TEvent> HasContractAddresses(params IEnumerable<Address> contractAddresses);

    public ILogsApi<TEvent> HasContract(IEVMContract contract);
    [OverloadResolutionPriority(1)]
    public ILogsApi<TEvent> HasContracts(params ReadOnlySpan<IEVMContract> contracts);
    public ILogsApi<TEvent> HasContracts(params IEnumerable<IEVMContract> contracts);
}
