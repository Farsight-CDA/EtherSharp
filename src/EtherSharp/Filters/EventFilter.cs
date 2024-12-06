using EtherSharp.Client.Services.RPC;
using EtherSharp.Types;

namespace EtherSharp.Filters;
internal class EventFilter(IRpcClient client, string filterId) : IEventFilter
{
    private readonly IRpcClient _client = client;
    private readonly string _filterId = filterId;

    public Task<EventFilterChangesResult[]> GetChangesAsync() 
        => _client.EthGetEventFilterChangesAsync(_filterId);
}
