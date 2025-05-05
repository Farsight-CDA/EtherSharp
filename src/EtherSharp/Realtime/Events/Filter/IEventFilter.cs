namespace EtherSharp.Realtime.Events.Filter;
public interface IEventFilter<TEvent> : IAsyncDisposable
    where TEvent : ITxEvent<TEvent>
{
    public Task<TEvent[]> GetChangesAsync(CancellationToken cancellationToken = default);
}