using EtherSharp.Client.Services.RPC;
using EtherSharp.Common;
using EtherSharp.Types;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Channels;

namespace EtherSharp.Realtime.Blocks.Subscription;
internal class BlocksSubscription(IRpcClient client) : IBlocksSubscription
{
    public string Id { get; private set; } = null!;

    private readonly IRpcClient _client = client;

    private readonly Channel<BlockHeader> _channel = Channel.CreateUnbounded<BlockHeader>(new UnboundedChannelOptions()
    {
        SingleReader = true,
        SingleWriter = true,
    });

    public async IAsyncEnumerable<BlockHeader> ListenAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        while(await _channel.Reader.WaitToReadAsync(cancellationToken))
        {
            yield return await _channel.Reader.ReadAsync(cancellationToken);
        }
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        await InstallAsync(cancellationToken);

        _client.OnConnectionEstablished += HandleReconnect;
        _client.OnSubscriptionMessage += HandleSubscriptionMessage;
    }

    private async Task InstallAsync(CancellationToken cancellationToken = default)
        => Id = await _client.EthSubscribeNewHeadsAsync( cancellationToken);

    private void HandleReconnect()
        => _ = Task.Run(() => InstallAsync());

    private record HeadsParams(HeadsResponse Params);
    private record HeadsResponse(BlockHeader Result);
    private void HandleSubscriptionMessage(string subscriptionId, ReadOnlySpan<byte> payload)
    {
        if(Id != subscriptionId)
        {
            return;
        }

        var p = JsonSerializer.Deserialize<HeadsParams>(payload, ParsingUtils.EvmSerializerOptions)!;
        _channel.Writer.TryWrite(p.Params.Result);
    }

    public async ValueTask DisposeAsync()
    {
        _client.OnConnectionEstablished -= HandleReconnect;
        _client.OnSubscriptionMessage -= HandleSubscriptionMessage;

        await _client.EthUnsubscribeAsync(Id);
    }
}
