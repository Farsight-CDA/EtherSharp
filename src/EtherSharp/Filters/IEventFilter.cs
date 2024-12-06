using EtherSharp.Events;
using EtherSharp.Types;

namespace EtherSharp.Filters;
public interface IEventFilter
{
    public Task<EventFilterChangesResult[]> GetChangesAsync();
}

public interface IEventFilter<TEvent>
    where TEvent : ITxEvent<TEvent>
{
    public Task<TEvent[]> GetChangesAsync();
}