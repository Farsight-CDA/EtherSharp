namespace EtherSharp.Realtime.Events.Filter;

/// <summary>
/// Represents a polling-based event filter that returns decoded log changes.
/// </summary>
/// <typeparam name="TLog">The decoded log type.</typeparam>
public interface IEventFilter<TLog> : IAsyncDisposable
    where TLog : ITxLog<TLog>
{
    public Task<TLog[]> GetChangesAsync(CancellationToken cancellationToken = default);
}
