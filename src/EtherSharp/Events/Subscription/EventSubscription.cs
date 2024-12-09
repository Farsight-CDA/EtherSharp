
namespace EtherSharp.Events.Subscription;
internal class EventSubscription<TEvent> : IEventSubscription<TEvent>
    where TEvent : ITxEvent<TEvent>
{
    public IAsyncEnumerable<TEvent> ListenAsync() => throw new NotImplementedException();
}
