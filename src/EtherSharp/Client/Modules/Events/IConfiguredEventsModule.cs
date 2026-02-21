using EtherSharp.Realtime.Events;
using EtherSharp.Realtime.Events.Filter;
using EtherSharp.Realtime.Events.Subscription;
using EtherSharp.Types;

namespace EtherSharp.Client.Modules.Events;

/// <summary>
/// Represents an event query with all optional topic/address filters already configured.
/// </summary>
/// <typeparam name="TLog">Typed log representation decoded from chain logs.</typeparam>
public interface IConfiguredEventsModule<TLog>
    where TLog : ITxLog<TLog>
{
    /// <summary>
    /// Fetches historical logs matching the configured filters.
    /// </summary>
    /// <param name="fromBlock">Start block (inclusive). Defaults to earliest when omitted.</param>
    /// <param name="toBlock">End block (inclusive). Defaults to latest when omitted.</param>
    /// <param name="blockHash">Optional block hash filter. When set, block range is ignored by the node.</param>
    /// <param name="cancellationToken">Token used to cancel the RPC request.</param>
    /// <returns>Decoded logs ordered by block/transaction/log index.</returns>
    public Task<TLog[]> GetAllAsync(
        TargetBlockNumber fromBlock = default, TargetBlockNumber toBlock = default, string? blockHash = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a poll-based event filter that can be queried incrementally.
    /// </summary>
    /// <param name="fromBlock">Start block (inclusive) for the filter.</param>
    /// <param name="toBlock">Optional end block (inclusive) for the filter.</param>
    /// <param name="cancellationToken">Token used to cancel filter creation.</param>
    /// <returns>An initialized event filter handle.</returns>
    public Task<IEventFilter<TLog>> CreateFilterAsync(
        TargetBlockNumber fromBlock = default, TargetBlockNumber toBlock = default, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a push-based subscription for matching future logs.
    /// </summary>
    /// <param name="cancellationToken">Token used to cancel subscription setup.</param>
    /// <returns>An active event subscription.</returns>
    public Task<IEventSubscription<TLog>> CreateSubscriptionAsync(CancellationToken cancellationToken = default);
}
