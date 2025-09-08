using EtherSharp.Realtime.Events;
using EtherSharp.Realtime.Events.Filter;
using EtherSharp.Realtime.Events.Subscription;
using EtherSharp.Types;

namespace EtherSharp.Client.Modules.Events;
public interface IConfiguredEventsModule<TLog>
    where TLog : ITxLog<TLog>
{
    public Task<TLog[]> GetAllAsync(
        TargetBlockNumber fromBlock = default, TargetBlockNumber toBlock = default, string? blockHash = null, CancellationToken cancellationToken = default);
    public Task<IEventFilter<TLog>> CreateFilterAsync(
        TargetBlockNumber fromBlock = default, TargetBlockNumber toBlock = default, CancellationToken cancellationToken = default);
    public Task<IEventSubscription<TLog>> CreateSubscriptionAsync(CancellationToken cancellationToken = default);
}
