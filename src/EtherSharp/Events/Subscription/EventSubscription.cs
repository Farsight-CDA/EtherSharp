using EtherSharp.Client.Services.RPC;
using EtherSharp.Types;
using System.Runtime.CompilerServices;
using System.Threading.Channels;

namespace EtherSharp.Events.Subscription;
internal class EventSubscription<TEvent>(IRpcClient client, string[]? contractAddresses, string[]? topics)
    : IEventSubscription<TEvent>, ISubscriptionHandler<Log>
    where TEvent : ITxEvent<TEvent>
{
    private readonly IRpcClient _client = client;

    private readonly string[]? _contractAddresses = contractAddresses;
    private readonly string[]? _topics = topics;

    public string Id { get; private set; } = null!;

    private readonly Channel<Log> _channel = Channel.CreateUnbounded<Log>(new UnboundedChannelOptions()
    {
        SingleReader = true,
        SingleWriter = true,
    });

    public async Task<string> InstallAsync(CancellationToken cancellationToken)
        => Id = await _client.EthSubscribeLogsAsync(_contractAddresses, _topics);

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

    public async ValueTask DisposeAsync()
        => await _client.EthUninstallFilterAsync(Id);
}
