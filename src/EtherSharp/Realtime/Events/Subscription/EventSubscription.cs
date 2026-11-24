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
    private static readonly string _displayName = $"{nameof(EventSubscription<>)}<{typeof(TLog)}>".Replace('+', '.');

    public string Id { get; private set; } = null!;
    public RpcRequestOptions RequestOptions { get; private set; }

    private readonly IEthRpcModule _ethRpcModule = ethRpcModule;
    private readonly ISubscriptionsManager _subscriptionsManager = subscriptionsManager;
    private readonly EtherSharpJsonSerializerContext _jsonSerializerContext = jsonSerializerContext;

    private readonly EventFilter _eventFilter = eventFilter;
    private readonly Channel<SubscriptionEvent<Log>> _channel = Channel.CreateUnbounded<SubscriptionEvent<Log>>(new UnboundedChannelOptions()
    {
        SingleReader = true,
        SingleWriter = true,
    });
    private readonly Lock _statusLock = new Lock();
    private bool _isClosed;
    private bool _isDisposing;

    public async IAsyncEnumerable<SubscriptionEvent<TLog>> ListenAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        while(await _channel.Reader.WaitToReadAsync(cancellationToken))
        {
            while(_channel.Reader.TryRead(out var subscriptionEvent))
            {
                yield return subscriptionEvent switch
                {
                    Log log => TLog.Decode(log),
                    SubscriptionInterruption interruption => interruption,
                };
                cancellationToken.ThrowIfCancellationRequested();
            }
        }
    }

    public async Task InstallAsync(RpcRequestOptions requestOptions = default, CancellationToken cancellationToken = default)
    {
        ThrowIfClosed();
        RequestOptions = requestOptions;
        Id = await _ethRpcModule.SubscribeLogsAsync(_eventFilter, requestOptions, cancellationToken);
    }

    public void HandleSubscriptionMessage(ReadOnlySpan<byte> result)
    {
        var log = JsonSerializer.Deserialize(result, _jsonSerializerContext.Log)!;
        _channel.Writer.TryWrite(log);
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

    public override string ToString()
        => _displayName;

    private void ThrowIfClosed()
        => ObjectDisposedException.ThrowIf(_isClosed, this);
}
