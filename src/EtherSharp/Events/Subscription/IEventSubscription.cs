namespace EtherSharp.Events.Subscription;
public interface IEventSubscription<TEvent> : IAsyncDisposable
    where TEvent : ITxEvent<TEvent>
{
    public IAsyncEnumerable<TEvent> ListenAsync(CancellationToken cancellationToken = default);
}
