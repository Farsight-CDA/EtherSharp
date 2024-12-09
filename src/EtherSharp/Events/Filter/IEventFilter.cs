namespace EtherSharp.Events.Filter;
public interface IEventFilter<TEvent>
    where TEvent : ITxEvent<TEvent>
{
    public Task<TEvent[]> GetChangesAsync();
}