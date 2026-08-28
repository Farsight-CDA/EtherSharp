using EtherSharp.Client.Services.Subscriptions;
using EtherSharp.Common;
using EtherSharp.RPC.Modules.Eth;
using EtherSharp.RPC.Transport;
using EtherSharp.Types;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Channels;

namespace EtherSharp.Realtime.Events.Subscription;

internal sealed class EventSubscription<TLog>(
    IEthRpcModule ethRpcModule, ISubscriptionsManager subscriptionsManager,
    EtherSharpJsonSerializerContext jsonSerializerContext, EventFilter eventFilter
)
    : IEventSubscription<TLog>, ISubscription
    where TLog : ITxLog<TLog>
{
    public string Id { get; private set; } = null!;
    public RpcRequestOptions RequestOptions { get; private set; }

    private readonly IEthRpcModule _ethRpcModule = ethRpcModule;
    private readonly ISubscriptionsManager _subscriptionsManager = subscriptionsManager;
    private readonly EtherSharpJsonSerializerContext _jsonSerializerContext = jsonSerializerContext;

    private readonly EventFilter _eventFilter = eventFilter;
    private readonly Channel<Log> _channel = Channel.CreateUnbounded<Log>(new UnboundedChannelOptions()
    {
        SingleReader = true,
        SingleWriter = true,
    });
    private readonly Lock _statusLock = new Lock();
    private bool _isClosed;
    private bool _isDisposing;

    public async IAsyncEnumerable<TLog> ListenAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        while(await _channel.Reader.WaitToReadAsync(cancellationToken))
        {
            var log = await _channel.Reader.ReadAsync(cancellationToken);
            yield return TLog.Decode(log);
        }
    }

    public async Task InstallAsync(RpcRequestOptions requestOptions = default, CancellationToken cancellationToken = default)
    {
        ThrowIfClosed();
        RequestOptions = requestOptions;
        Id = await _ethRpcModule.SubscribeLogsAsync(_eventFilter, requestOptions, cancellationToken);
    }

    public bool HandleSubscriptionMessage(ReadOnlySpan<byte> payload)
    {
        var envelope = JsonSerializer.Deserialize(payload, _jsonSerializerContext.LogSubscriptionEnvelope);
        _channel.Writer.TryWrite(envelope.Params.Result);
        return true;
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

    public void Close()
    {
        lock(_statusLock)
        {
            if(_isClosed)
            {
                return;
            }

            _isClosed = true;
        }

        _channel.Writer.TryComplete();
    }

    private void ThrowIfClosed()
        => ObjectDisposedException.ThrowIf(_isClosed, this);
}
