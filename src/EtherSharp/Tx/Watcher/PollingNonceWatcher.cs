using EtherSharp.Common.Extensions;
using EtherSharp.RPC;
using EtherSharp.Types;
using Microsoft.Extensions.Logging;
using System.Threading.Channels;

namespace EtherSharp.Tx.Watcher;

/// <summary>
/// Polls sender nonces through a <see cref="NonceSource"/> while any subscription is active,
/// and delivers observations to all subscribers.
/// </summary>
public sealed class PollingNonceWatcher : INonceWatcher, IAsyncDisposable
{
    private static readonly TimeSpan _pollInterval = TimeSpan.FromSeconds(2);

    private readonly NonceSource _nonceSource;
    private readonly NonceReadTrigger _readTrigger;
    private readonly TimeSpan _retryDelay;
    private readonly TimeSpan _maxErrorBackoff;
    private readonly ILogger<PollingNonceWatcher>? _logger;

    private readonly Lock _stateLock = new Lock();
    private readonly Dictionary<Address, SenderTracker> _senders = [];
    private readonly CancellationTokenSource _cts = new CancellationTokenSource();
    private readonly Task _heartbeatTask;

    private NonceSource.Request[] _requestBuffer = [];
    private bool _isDisposed;

    private bool BatchesSenders => _nonceSource switch
    {
        StateOverrideNonceSource => true,
        ConstructorCallNonceSource => false,
    };

    /// <summary>
    /// Creates a new <see cref="PollingNonceWatcher"/>.
    /// </summary>
    public PollingNonceWatcher(
        NonceSource nonceSource,
        RpcRetryOptions? retryOptions = null,
        ILogger<PollingNonceWatcher>? logger = null
    )
    {
        ArgumentNullException.ThrowIfNull(nonceSource);

        var resolvedRetryOptions = retryOptions ?? new RpcRetryOptions();
        resolvedRetryOptions.Validate(nameof(retryOptions));

        _nonceSource = nonceSource;
        _readTrigger = new NonceReadTrigger.Interval(_pollInterval);
        _retryDelay = resolvedRetryOptions.RetryDelay;
        _maxErrorBackoff = resolvedRetryOptions.MaxErrorBackoff;
        _logger = logger;
        _heartbeatTask = Task.Run(() => RunHeartbeatAsync(_cts.Token));
    }

    /// <inheritdoc/>
    public INonceSubscription Subscribe(
        in Address sender,
        NonceSubscriptionMode mode,
        ulong startNonce = 0
    )
    {
        using var scope = _stateLock.EnterScope();
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        if(!_senders.TryGetValue(sender, out var tracker))
        {
            tracker = new SenderTracker(sender, startNonce);
            _senders.Add(sender, tracker);
        }

        var handle = new NonceSubscriptionHandle(this, sender, mode);
        tracker.AddHandleLocked(handle, startNonce);
        return handle;
    }

    private void Unregister(NonceSubscriptionHandle handle)
    {
        using var scope = _stateLock.EnterScope();

        if(_isDisposed || !_senders.TryGetValue(handle.Sender, out var tracker))
        {
            return;
        }

        if(!tracker.RemoveHandleLocked(handle))
        {
            return;
        }

        if(tracker.Handles.Count == 0)
        {
            _ = _senders.Remove(handle.Sender);
        }
    }

    private async Task RunHeartbeatAsync(CancellationToken cancellationToken)
    {
        while(true)
        {
            await _readTrigger.WaitAsync(cancellationToken);
            await ReadUntilSucceededAsync(cancellationToken);
        }
    }

    // Retries a failed read with backoff instead of waiting for the next trigger.
    private async Task ReadUntilSucceededAsync(CancellationToken cancellationToken)
    {
        var errorBackoffDelay = _retryDelay;

        while(GetReadRequests() is { IsEmpty: false } requests)
        {
            try
            {
                var (blockNumber, nonces) = await _nonceSource.ReadAsync(requests, cancellationToken);

                lock(_stateLock)
                {
                    var requestSpan = requests.Span;
                    for(int i = 0; i < requestSpan.Length; i++)
                    {
                        if(_senders.TryGetValue(requestSpan[i].Sender, out var tracker))
                        {
                            tracker.ObserveNonceLocked(nonces[i], blockNumber);
                        }
                    }
                }

                return;
            }
            catch(Exception ex) when(!cancellationToken.IsCancellationRequested)
            {
                _logger?.TxWatcher.NonceReadFailed(ex, requests.Length);
                await Task.Delay(errorBackoffDelay, cancellationToken);
                errorBackoffDelay = RpcRetryOptions.NextBackoff(errorBackoffDelay, _maxErrorBackoff);
            }
        }
    }

    private ReadOnlyMemory<NonceSource.Request> GetReadRequests()
    {
        using var scope = _stateLock.EnterScope();

        if(_requestBuffer.Length < _senders.Count)
        {
            _requestBuffer = new NonceSource.Request[_senders.Count];
        }

        bool hasActive = false;
        int count = 0;
        foreach(var tracker in _senders.Values)
        {
            hasActive |= tracker.IsActive;

            if(BatchesSenders || tracker.IsActive)
            {
                _requestBuffer[count++] = new NonceSource.Request(tracker.Sender, tracker.LastKnownNonce);
            }
        }

        return hasActive
            ? _requestBuffer.AsMemory(0, count)
            : ReadOnlyMemory<NonceSource.Request>.Empty;
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        lock(_stateLock)
        {
            if(_isDisposed)
            {
                return;
            }

            _isDisposed = true;

            foreach(var tracker in _senders.Values)
            {
                foreach(var handle in tracker.Handles)
                {
                    handle.Complete();
                }

                tracker.Handles.Clear();
            }

            _senders.Clear();
        }

        try
        {
            await _cts.CancelAsync();

            try
            {
                await _heartbeatTask;
            }
            catch(OperationCanceledException)
            {
            }
            catch(Exception ex)
            {
                _logger?.TxWatcher.HeartbeatShutdownFailed(ex);
            }
        }
        finally
        {
            _cts.Dispose();
        }
    }

    private sealed class SenderTracker(Address sender, ulong startNonce)
    {
        private int _activeHandleCount;

        public Address Sender { get; } = sender;
        public List<NonceSubscriptionHandle> Handles { get; } = [];
        public ulong LastKnownNonce { get; private set; } = startNonce;
        public bool IsActive => _activeHandleCount > 0;

        public void AddHandleLocked(NonceSubscriptionHandle handle, ulong startNonce)
        {
            if(Handles.Count == 0 || startNonce < LastKnownNonce)
            {
                LastKnownNonce = startNonce;
            }

            Handles.Add(handle);
            if(handle.Mode == NonceSubscriptionMode.Active)
            {
                _activeHandleCount++;
            }
        }

        public bool RemoveHandleLocked(NonceSubscriptionHandle handle)
        {
            if(!Handles.Remove(handle))
            {
                return false;
            }

            if(handle.Mode == NonceSubscriptionMode.Active)
            {
                _activeHandleCount--;
            }

            handle.Complete();
            return true;
        }

        public void ObserveNonceLocked(ulong nonce, ulong blockNumber)
        {
            LastKnownNonce = nonce;

            var observation = new NonceObservation(nonce, blockNumber);
            foreach(var handle in Handles)
            {
                handle.Publish(observation);
            }
        }
    }

    private sealed class NonceSubscriptionHandle(
        PollingNonceWatcher watcher,
        Address sender,
        NonceSubscriptionMode mode
    ) : INonceSubscription
    {
        private readonly Channel<NonceObservation> _channel = Channel.CreateBounded<NonceObservation>(
            new BoundedChannelOptions(1)
            {
                FullMode = BoundedChannelFullMode.DropOldest,
                SingleReader = true,
                SingleWriter = true,
            }
        );

        public Address Sender { get; } = sender;
        public NonceSubscriptionMode Mode { get; } = mode;

        IAsyncEnumerable<NonceObservation> INonceSubscription.ListenAsync(CancellationToken cancellationToken)
            => _channel.Reader.ReadAllAsync(cancellationToken);

        void IDisposable.Dispose()
            => watcher.Unregister(this);

        public void Publish(NonceObservation observation)
            => _ = _channel.Writer.TryWrite(observation);

        public void Complete()
            => _ = _channel.Writer.TryComplete();
    }
}
