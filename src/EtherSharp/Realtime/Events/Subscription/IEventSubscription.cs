namespace EtherSharp.Realtime.Events.Subscription;

/// <summary>
/// Represents a realtime stream of decoded transaction logs.
/// </summary>
/// <typeparam name="TLog">The decoded log type.</typeparam>
public interface IEventSubscription<TLog> : IAsyncDisposable
    where TLog : ITxLog<TLog>
{
    public IAsyncEnumerable<TLog> ListenAsync(CancellationToken cancellationToken = default);
}
