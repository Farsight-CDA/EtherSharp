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
    /// <returns>An async stream of decoded logs.</returns>
    public IAsyncEnumerable<TLog> ListenAsync(CancellationToken cancellationToken = default);
}
