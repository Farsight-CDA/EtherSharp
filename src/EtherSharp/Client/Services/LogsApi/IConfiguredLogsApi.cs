using EtherSharp.Realtime.Events;
using EtherSharp.Realtime.Events.Filter;
using EtherSharp.Realtime.Events.Subscription;
using EtherSharp.Types;

namespace EtherSharp.Client.Services.LogsApi;
public interface IConfiguredLogsApi<TEvent>
    where TEvent : ITxEvent<TEvent>
{
    public Task<TEvent[]> GetAllAsync(
        TargetBlockNumber fromBlock = default, TargetBlockNumber toBlock = default, string? blockHash = null);
    public Task<IEventFilter<TEvent>> CreateFilterAsync(
        TargetBlockNumber fromBlock = default, TargetBlockNumber toBlock = default);
    public Task<IEventSubscription<TEvent>> CreateSubscriptionAsync();
}
