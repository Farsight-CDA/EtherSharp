using EtherSharp.Client.Services.Subscriptions;
using EtherSharp.RPC.Transport;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Channels;

namespace EtherSharp.Realtime;

internal abstract class SubscriptionBase<T>(ISubscriptionsManager subscriptionsManager, JsonTypeInfo<T> resultTypeInfo) : ISubscription
    where T : notnull
{
    public string Id { get; private set; } = null!;
    public RpcRequestOptions RequestOptions { get; private set; }

    private readonly ISubscriptionsManager _subscriptionsManager = subscriptionsManager;
    private readonly JsonTypeInfo<T> _resultTypeInfo = resultTypeInfo;
    private readonly Channel<SubscriptionEvent<T>> _channel = Channel.CreateUnbounded<SubscriptionEvent<T>>(new UnboundedChannelOptions()
    {
        SingleReader = true,
        SingleWriter = true,
    });
    private readonly Lock _statusLock = new Lock();
    private bool _isClosed;
    private bool _isDisposing;
    private bool _isInterrupted;

    protected abstract Task<string> SubscribeAsync(RpcRequestOptions requestOptions, CancellationToken cancellationToken);

    protected IAsyncEnumerable<SubscriptionEvent<T>> ReadAsync(CancellationToken cancellationToken)
        => _channel.Reader.ReadAllAsync(cancellationToken);

    public async Task InstallAsync(RpcRequestOptions requestOptions = default, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_isClosed, this);
        RequestOptions = requestOptions;
        Id = await SubscribeAsync(requestOptions, cancellationToken);
    }

    public void HandleSubscriptionMessage(ReadOnlySpan<byte> result)
        => _channel.Writer.TryWrite(JsonSerializer.Deserialize(result, _resultTypeInfo)!);

    public void Interrupt(SubscriptionInterrupted interruption)
    {
        if(!_isInterrupted)
        {
            _isInterrupted = true;
            _ = _channel.Writer.TryWrite(interruption);
        }
    }

    public void Resume()
    {
        if(_isInterrupted)
        {
            _isInterrupted = false;
            _ = _channel.Writer.TryWrite(SubscriptionResumed.Instance);
        }
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
}
