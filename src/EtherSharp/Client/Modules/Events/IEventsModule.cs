using EtherSharp.Contract;
using EtherSharp.Realtime.Events;
using EtherSharp.Types;
using System.Runtime.CompilerServices;

namespace EtherSharp.Client.Modules.Events;

public interface IEventsModule<TLog> : IConfiguredEventsModule<TLog>
    where TLog : ITxLog<TLog>
{
    public IEventsModule<TLog> HasTopic(string topic, int index = 0);
    [OverloadResolutionPriority(1)]
    public IEventsModule<TLog> HasTopics(int index = 0, params ReadOnlySpan<string> topics);
    public IEventsModule<TLog> HasTopics(int index = 0, params IEnumerable<string> topics);

    public IEventsModule<TLog> HasContractAddress(Address contractAddress);
    [OverloadResolutionPriority(1)]
    public IEventsModule<TLog> HasContractAddresses(params ReadOnlySpan<Address> contractAddresses);
    public IEventsModule<TLog> HasContractAddresses(params IEnumerable<Address> contractAddresses);

    public IEventsModule<TLog> HasContract(IEVMContract contract);
    [OverloadResolutionPriority(1)]
    public IEventsModule<TLog> HasContracts(params ReadOnlySpan<IEVMContract> contracts);
    public IEventsModule<TLog> HasContracts(params IEnumerable<IEVMContract> contracts);
}
