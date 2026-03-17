using EtherSharp.Client.Services.Subscriptions;
using EtherSharp.RPC;
using EtherSharp.RPC.Modules.Eth;
using EtherSharp.Types;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Channels;

namespace EtherSharp.Realtime.Events.Subscription;

internal sealed class EventSubscription<TLog>(
    IRpcClient client, IEthRpcModule ethRpcModule, ISubscriptionsManager subscriptionsManager,
    JsonSerializerOptions jsonSerializerOptions, Address[]? contractAddresses, string[]?[]? topics
)
    : IEventSubscription<TLog>, ISubscription
    where TLog : ITxLog<TLog>
{
    public string Id { get; private set; } = null!;

    private readonly IRpcClient _client = client;
    private readonly IEthRpcModule _ethRpcModule = ethRpcModule;
    private readonly ISubscriptionsManager _subscriptionsManager = subscriptionsManager;
    private readonly JsonSerializerOptions _jsonSerializerOptions = jsonSerializerOptions;

    private readonly Address[]? _contractAddresses = contractAddresses;
    private readonly string[]?[]? _topics = topics;

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

    public async Task InstallAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfClosed();
        Id = await _ethRpcModule.SubscribeLogsAsync(_contractAddresses, _topics, cancellationToken);
    }

    private record struct LogParams(LogResponse Params);
    private record struct LogResponse(Log Result);
    public bool HandleSubscriptionMessage(ReadOnlySpan<byte> payload)
    {
        var p = JsonSerializer.Deserialize<LogParams>(payload, _jsonSerializerOptions)!;
        _channel.Writer.TryWrite(p.Params.Result);
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
