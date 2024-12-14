using EtherSharp.Events;
using EtherSharp.Events.Filter;
using EtherSharp.Events.Subscription;
using EtherSharp.Types;

namespace EtherSharp.Client.Services.LogsApi;
public interface IConfiguredLogsApi<TEvent>
    where TEvent : ITxEvent<TEvent>
{
    public Task<TEvent[]> GetAllAsync(
        TargetBlockNumber fromBlock = default, TargetBlockNumber toBlock = default, byte[]? blockHash = null);
    public Task<IEventFilter<TEvent>> CreateFilterAsync(
        TargetBlockNumber fromBlock = default, TargetBlockNumber toBlock = default);
    public Task<IEventSubscription<TEvent>> CreateSubscriptionAsync();
}
