using EtherSharp.Client.Services.Subscriptions;
using EtherSharp.RPC.Modules.Eth;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Channels;

namespace EtherSharp.Realtime.Blocks.Subscription;

internal class BlocksSubscription(IEthRpcModule ethRpcModule, ISubscriptionsManager subscriptionsManager,
    JsonSerializerOptions jsonSerializerOptions) : IBlocksSubscription, ISubscription
{
    public string Id { get; private set; } = null!;

    private readonly IEthRpcModule _ethRpcModule = ethRpcModule;
    private readonly ISubscriptionsManager _subscriptionsManager = subscriptionsManager;
    private readonly JsonSerializerOptions _jsonSerializerOptions = jsonSerializerOptions;

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

    public async Task InstallAsync(CancellationToken cancellationToken = default)
        => Id = await _ethRpcModule.SubscribeNewHeadsAsync(cancellationToken);

    public async ValueTask DisposeAsync()
        => await _subscriptionsManager.UninstallSubscription(this);

    private record struct HeadsParams(HeadsResponse Params);
    private record struct HeadsResponse(BlockHeader Result);
    public bool HandleSubscriptionMessage(ReadOnlySpan<byte> payload)
    {
        var p = JsonSerializer.Deserialize<HeadsParams>(payload, _jsonSerializerOptions)!;
        _channel.Writer.TryWrite(p.Params.Result);
        return true;
    }
}
