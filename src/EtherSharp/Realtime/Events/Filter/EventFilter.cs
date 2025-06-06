﻿using EtherSharp.Client.Services.RPC;
using EtherSharp.Types;

namespace EtherSharp.Realtime.Events.Filter;

internal class EventFilter<TEvent>(IRpcClient client,
    TargetBlockNumber fromBlock, TargetBlockNumber toBlock,
    Address[]? addresses, string[]?[]? topics
) : IEventFilter<TEvent>
    where TEvent : ITxEvent<TEvent>
{
    public string Id { get; private set; } = null!;

    private readonly IRpcClient _client = client;

    private readonly TargetBlockNumber _fromBlock = fromBlock;
    private readonly TargetBlockNumber _toBlock = toBlock;

    private readonly Address[]? _addresses = addresses;
    private readonly string[]?[]? _topics = topics;

    public async Task<TEvent[]> GetChangesAsync(CancellationToken cancellationToken)
    {
        var rawResults = await _client.EthGetEventFilterChangesAsync(Id, cancellationToken);
        return [.. rawResults.Select(TEvent.Decode)];
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        _client.OnConnectionEstablished += HandleReconnect;
        await InstallAsync(cancellationToken);
    }

    private async Task InstallAsync(CancellationToken cancellationToken = default)
        => Id = await _client.EthNewFilterAsync(_fromBlock, _toBlock, _addresses, _topics, cancellationToken);

    private void HandleReconnect()
        => _ = Task.Run(() => InstallAsync());

    public async ValueTask DisposeAsync()
    {
        _client.OnConnectionEstablished -= HandleReconnect;
        await _client.EthUninstallFilterAsync(Id);
    }
}
