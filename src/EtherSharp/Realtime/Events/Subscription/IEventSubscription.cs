namespace EtherSharp.Realtime.Events.Subscription;

/// <summary>
/// Represents a realtime stream of decoded transaction logs.
/// </summary>
/// <typeparam name="TLog">The decoded log type.</typeparam>
public interface IEventSubscription<TLog> : IAsyncDisposable
    where TLog : ITxLog<TLog>
{
    /// <summary>
    /// Listens for decoded realtime log events.
    /// </summary>
    /// <param name="cancellationToken">Token used to stop listening.</param>
    /// <returns>
    /// An async stream of decoded logs. A <see cref="SubscriptionInterrupted"/> is emitted when the subscription is interrupted
    /// and a <see cref="SubscriptionResumed"/> once it is live again, logs emitted in between may have been missed.
    /// Use <see cref="SubscriptionEventExtensions.IgnoreInterruptions{T}"/> if missed logs are acceptable.
    /// </returns>
    public IAsyncEnumerable<SubscriptionEvent<TLog>> ListenAsync(CancellationToken cancellationToken = default);
}
