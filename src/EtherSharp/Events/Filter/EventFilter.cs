using EtherSharp.Client.Services.RPC;

namespace EtherSharp.Events.Filter;

internal class EventFilter<TEvent>(IRpcClient client, string filterId) : IEventFilter<TEvent>
    where TEvent : ITxEvent<TEvent>
{
    private readonly IRpcClient _client = client;
    private readonly string _filterId = filterId;

    public async Task<TEvent[]> GetChangesAsync()
    {
        var rawResults = await _client.EthGetEventFilterChangesAsync(_filterId);
        return rawResults.Select(TEvent.Decode).ToArray();
    }
}
