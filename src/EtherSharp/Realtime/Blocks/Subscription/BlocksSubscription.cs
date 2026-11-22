using EtherSharp.Client.Services.Subscriptions;
using EtherSharp.Common;
using EtherSharp.RPC.Modules.Eth;
using EtherSharp.RPC.Transport;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Text.Json;
using System.Threading.Channels;

namespace EtherSharp.Realtime.Blocks.Subscription;

internal sealed class BlocksSubscription(IEthRpcModule ethRpcModule, ISubscriptionsManager subscriptionsManager,
    EtherSharpJsonSerializerContext jsonSerializerContext) : IBlocksSubscription, ISubscription
{
    public string Id { get; private set; } = null!;
    public RpcRequestOptions RequestOptions { get; private set; }

    private readonly IEthRpcModule _ethRpcModule = ethRpcModule;
    private readonly ISubscriptionsManager _subscriptionsManager = subscriptionsManager;
    private readonly EtherSharpJsonSerializerContext _jsonSerializerContext = jsonSerializerContext;
    private readonly Channel<BlockHeader> _channel = Channel.CreateUnbounded<BlockHeader>(new UnboundedChannelOptions()
    {
        SingleReader = true,
        SingleWriter = true,
    });
    private readonly Lock _statusLock = new Lock();
    private bool _isClosed;
    private bool _isDisposing;
    private Exception? _closeError;

    public async IAsyncEnumerable<BlockHeader> ListenAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        while(await _channel.Reader.WaitToReadAsync(cancellationToken))
        {
            yield return await _channel.Reader.ReadAsync(cancellationToken);
        }

        if(_closeError is not null)
        {
            ExceptionDispatchInfo.Throw(_closeError);
        }
    }

    public async Task InstallAsync(RpcRequestOptions requestOptions = default, CancellationToken cancellationToken = default)
    {
        ThrowIfClosed();
        RequestOptions = requestOptions;
        Id = await _ethRpcModule.SubscribeNewHeadsAsync(requestOptions, cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        lock(_statusLock)
        {
            if(_isClosed || _isDisposing)
            {
                return;
            }

            _isDisposing = true;
        }

        try
        {
            await _subscriptionsManager.UninstallSubscription(this);
        }
        finally
        {
            Close();
        }
    }

    public void Close(Exception? error = null)
    {
        lock(_statusLock)
        {
            if(_isClosed)
            {
                return;
            }

            _isClosed = true;
            _closeError = error;
        }

        _channel.Writer.TryComplete();
    }

    public void HandleSubscriptionMessage(ReadOnlySpan<byte> result)
    {
        var header = JsonSerializer.Deserialize(result, _jsonSerializerContext.BlockHeader)!;
        _channel.Writer.TryWrite(header);
    }

    public override string ToString()
        => nameof(BlocksSubscription);

    private void ThrowIfClosed()
        => ObjectDisposedException.ThrowIf(_isClosed, this);
}
