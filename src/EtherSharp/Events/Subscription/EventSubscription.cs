using EtherSharp.Client.Services.RPC;
using EtherSharp.Common;
using EtherSharp.Types;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Channels;

namespace EtherSharp.Events.Subscription;
internal class EventSubscription<TEvent>(IRpcClient client, string[]? contractAddresses, string[]? topics)
    : IEventSubscription<TEvent>
    where TEvent : ITxEvent<TEvent>
{
    public string Id { get; private set; } = null!;

    private readonly IRpcClient _client = client;

    private readonly string[]? _contractAddresses = contractAddresses;
    private readonly string[]? _topics = topics;

    private readonly Channel<Log> _channel = Channel.CreateUnbounded<Log>(new UnboundedChannelOptions()
    {
        SingleReader = true,
        SingleWriter = true,
    });

    public async IAsyncEnumerable<TEvent> ListenAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        while(await _channel.Reader.WaitToReadAsync(cancellationToken))
        {
            var log = await _channel.Reader.ReadAsync(cancellationToken);
            yield return TEvent.Decode(log);
        }
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        _client.OnConnectionEstablished += HandleReconnect;
        _client.OnSubscriptionMessage += HandleSubscriptionMessage;

        await InstallAsync(cancellationToken);
    }

    private async Task InstallAsync(CancellationToken cancellationToken = default) 
        => Id = await _client.EthSubscribeLogsAsync(_contractAddresses, _topics, cancellationToken);

    private void HandleReconnect()
        => _ = Task.Run(() => InstallAsync());
    private record LogParams(LogResponse Params);
    private record LogResponse(Log Result);
    private void HandleSubscriptionMessage(string subscriptionId, ReadOnlySpan<byte> payload)
    {
        if(Id != subscriptionId)
        {
            return;
        }

        var p = JsonSerializer.Deserialize<LogParams>(payload, ParsingUtils.EvmSerializerOptions)!;
        _channel.Writer.TryWrite(p.Params.Result);
    }

    public async ValueTask DisposeAsync()
    { 
        _client.OnConnectionEstablished -= HandleReconnect;
        _client.OnSubscriptionMessage -= HandleSubscriptionMessage;

        await _client.EthUninstallFilterAsync(Id);
    }
}
