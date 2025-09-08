namespace EtherSharp.Realtime.Events.Filter;
public interface IEventFilter<TLog> : IAsyncDisposable
    where TLog : ITxLog<TLog>
{
    public Task<TLog[]> GetChangesAsync(CancellationToken cancellationToken = default);
}