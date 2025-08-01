﻿using EtherSharp.Client.Services.RPC;
using EtherSharp.Client.Services.Subscriptions;
using EtherSharp.Common;
using EtherSharp.Types;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Channels;

namespace EtherSharp.Realtime.Events.Subscription;
internal class EventSubscription<TEvent>(IRpcClient client, SubscriptionsManager subscriptionsManager, Address[]? contractAddresses, string[]?[]? topics)
    : IEventSubscription<TEvent>, ISubscription
    where TEvent : ITxEvent<TEvent>
{
    public string Id { get; private set; } = null!;

    private readonly IRpcClient _client = client;
    private readonly SubscriptionsManager _subscriptionsManager = subscriptionsManager;

    private readonly Address[]? _contractAddresses = contractAddresses;
    private readonly string[]?[]? _topics = topics;

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

    public async Task InstallAsync(CancellationToken cancellationToken = default)
        => Id = await _client.EthSubscribeLogsAsync(_contractAddresses, _topics, cancellationToken);

    private record LogParams(LogResponse Params);
    private record LogResponse(Log Result);
    public bool HandleSubscriptionMessage(ReadOnlySpan<byte> payload)
    {
        var p = JsonSerializer.Deserialize<LogParams>(payload, ParsingUtils.EvmSerializerOptions)!;
        _channel.Writer.TryWrite(p.Params.Result);
        return true;
    }

    public async ValueTask DisposeAsync()
        => await _subscriptionsManager.UninstallSubscription(this);
}
