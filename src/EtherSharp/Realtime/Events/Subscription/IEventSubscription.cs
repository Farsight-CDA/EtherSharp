namespace EtherSharp.Realtime.Events.Subscription;

public interface IEventSubscription<TLog> : IAsyncDisposable
    where TLog : ITxLog<TLog>
{
    public IAsyncEnumerable<TLog> ListenAsync(CancellationToken cancellationToken = default);
}
