using EtherSharp.Client.Services.Subscriptions;
using EtherSharp.Common;
using EtherSharp.RPC;
using EtherSharp.RPC.Modules.Eth;
using EtherSharp.Types;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Channels;

namespace EtherSharp.Realtime.Blocks.Subscription;
internal class BlocksSubscription(IEthRpcModule ethRpcModule, SubscriptionsManager subscriptionsManager) : IBlocksSubscription, ISubscription
{
    public string Id { get; private set; } = null!;

    private readonly IEthRpcModule _ethRpcModule = ethRpcModule;
    private readonly SubscriptionsManager _subscriptionsManager = subscriptionsManager;

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

    private record HeadsParams(HeadsResponse Params);
    private record HeadsResponse(BlockHeader Result);
    public bool HandleSubscriptionMessage(ReadOnlySpan<byte> payload)
    {
        var p = JsonSerializer.Deserialize<HeadsParams>(payload, ParsingUtils.EvmSerializerOptions)!;
        _channel.Writer.TryWrite(p.Params.Result);
        return true;
    }
}
