using EtherSharp.Types;

namespace EtherSharp.Filters;
public interface IEventFilter
{
    public Task<EventFilterChangesResult[]> GetChangesAsync();
}
