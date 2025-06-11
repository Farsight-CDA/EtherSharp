using EtherSharp.Realtime.Events;
using EtherSharp.Realtime.Events.Filter;
using EtherSharp.Realtime.Events.Subscription;
using EtherSharp.Types;

namespace EtherSharp.Client.Services.LogsApi;
public interface IConfiguredLogsApi<TEvent>
    where TEvent : ITxEvent<TEvent>
{
    public Task<TEvent[]> GetAllAsync(
        TargetBlockNumber fromBlock = default, TargetBlockNumber toBlock = default, string? blockHash = null, CancellationToken cancellationToken = default);
    public Task<IEventFilter<TEvent>> CreateFilterAsync(
        TargetBlockNumber fromBlock = default, TargetBlockNumber toBlock = default, CancellationToken cancellationToken = default);
    public Task<IEventSubscription<TEvent>> CreateSubscriptionAsync(CancellationToken cancellationToken = default);
}
