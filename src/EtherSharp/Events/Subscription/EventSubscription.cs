using EtherSharp.Client.Services.RPC;
using EtherSharp.Types;
using System.Runtime.CompilerServices;
using System.Threading.Channels;

namespace EtherSharp.Events.Subscription;
internal class EventSubscription<TEvent>(string[]? contractAddresses, string[]? topics)
    : IEventSubscription<TEvent>, ISubscriptionHandler<Log>
    where TEvent : ITxEvent<TEvent>
{
    private readonly string[]? _contractAddresses = contractAddresses;
    private readonly string[]? _topics = topics;

    private readonly Channel<Log> _channel = Channel.CreateUnbounded<Log>(new UnboundedChannelOptions()
    {
        SingleReader = true,
        SingleWriter = true,
    });

    public Task<string> InstallAsync(IRpcClient client)
        => client.EthSubscribeLogsAsync(_contractAddresses, _topics);

    public void HandlePayload(Log payload)
        => _channel.Writer.TryWrite(payload);

    public async IAsyncEnumerable<TEvent> ListenAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        while(await _channel.Reader.WaitToReadAsync(cancellationToken))
        {
            var log = await _channel.Reader.ReadAsync(cancellationToken);
            yield return TEvent.Decode(log);
        }
    }
}
