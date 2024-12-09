namespace EtherSharp.Events.Subscription;
public interface IEventSubscription<TEvent>
    where TEvent : ITxEvent<TEvent>
{
    public IAsyncEnumerable<TEvent> ListenAsync();
}
