using EtherSharp.Common.Extensions;
using EtherSharp.RPC;
using EtherSharp.Types;
using Microsoft.Extensions.Logging;
using System.Threading.Channels;

namespace EtherSharp.Tx.Watcher;

/// <summary>
/// Polls sender nonces through a <see cref="NonceSource"/> and delivers observations to active subscribers.
/// </summary>
public sealed class PollingNonceWatcher : INonceWatcher, IAsyncDisposable
{
    private readonly NonceSource _nonceSource;
    private readonly TimeSpan _retryDelay;
    private readonly TimeSpan _maxErrorBackoff;
    private readonly ILogger<PollingNonceWatcher>? _logger;

    private readonly Lock _stateLock = new Lock();
    private readonly Dictionary<Address, SenderTracker> _senders = [];
    private readonly CancellationTokenSource _cts = new CancellationTokenSource();
    private readonly SemaphoreSlim _wakeSignal = new SemaphoreSlim(0, 1);
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
        _retryDelay = resolvedRetryOptions.RetryDelay;
        _maxErrorBackoff = resolvedRetryOptions.MaxErrorBackoff;
        _logger = logger;
        _heartbeatTask = Task.Run(() => RunHeartbeatAsync(_cts.Token));
    }

    /// <inheritdoc/>
    public INonceSubscription Subscribe(
        in Address sender,
        TimeSpan refreshInterval = default,
        ulong startNonce = 0
    )
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(refreshInterval, TimeSpan.Zero, nameof(refreshInterval));

        using var scope = _stateLock.EnterScope();
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        var now = DateTimeOffset.UtcNow;
        if(!_senders.TryGetValue(sender, out var tracker))
        {
            tracker = new SenderTracker(sender, startNonce, now);
            _senders.Add(sender, tracker);
        }

        var handle = new NonceSubscriptionHandle(this, sender, refreshInterval);
        tracker.AddHandleLocked(handle, startNonce, now);

        if(refreshInterval > TimeSpan.Zero && _wakeSignal.CurrentCount == 0)
        {
            _ = _wakeSignal.Release();
        }

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

        if(handle.RefreshInterval > TimeSpan.Zero && _wakeSignal.CurrentCount == 0)
        {
            _ = _wakeSignal.Release();
        }
    }

    private async Task RunHeartbeatAsync(CancellationToken cancellationToken)
    {
        var errorBackoffDelay = _retryDelay;

        while(true)
        {
            var requests = await WaitForDueRequestsAsync(cancellationToken);

            try
            {
                var (blockNumber, nonces) = await _nonceSource.ReadAsync(requests, cancellationToken);

                lock(_stateLock)
                {
                    var completedAt = DateTimeOffset.UtcNow;
                    var requestSpan = requests.Span;
                    for(int i = 0; i < requestSpan.Length; i++)
                    {
                        if(_senders.TryGetValue(requestSpan[i].Sender, out var tracker))
                        {
                            tracker.ObserveNonceLocked(nonces[i], blockNumber, completedAt);
                        }
                    }
                }

                errorBackoffDelay = _retryDelay;
            }
            catch(Exception ex) when(!cancellationToken.IsCancellationRequested)
            {
                _logger?.TxWatcher.NonceReadFailed(ex, requests.Length);
                await Task.Delay(errorBackoffDelay, cancellationToken);
                errorBackoffDelay = RpcRetryOptions.NextBackoff(errorBackoffDelay, _maxErrorBackoff);
            }
        }
    }

    private async Task<ReadOnlyMemory<NonceSource.Request>> WaitForDueRequestsAsync(CancellationToken cancellationToken)
    {
        while(true)
        {
            TimeSpan waitDelay;

            lock(_stateLock)
            {
                if(_requestBuffer.Length < _senders.Count)
                {
                    _requestBuffer = new NonceSource.Request[_senders.Count];
                }

                var now = DateTimeOffset.UtcNow;
                DateTimeOffset? earliestDue = null;
                bool hasDue = false;
                int count = 0;

                foreach(var tracker in _senders.Values)
                {
                    bool isDue = false;
                    if(tracker.NextDueAt is { } due)
                    {
                        if(due <= now)
                        {
                            isDue = true;
                            hasDue = true;
                        }
                        else if(earliestDue is null || due < earliestDue.Value)
                        {
                            earliestDue = due;
                        }
                    }

                    if(BatchesSenders || isDue)
                    {
                        _requestBuffer[count++] = new NonceSource.Request(tracker.Sender, tracker.LastKnownNonce);
                    }
                }

                if(hasDue)
                {
                    return _requestBuffer.AsMemory(0, count);
                }

                waitDelay = earliestDue is { } nextDue
                    ? nextDue - now
                    : Timeout.InfiniteTimeSpan;
                _ = _wakeSignal.Wait(0, cancellationToken);
            }

            _ = await _wakeSignal.WaitAsync(waitDelay, cancellationToken);
        }
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
            _wakeSignal.Dispose();
        }
    }

    private sealed class SenderTracker(Address sender, ulong startNonce, DateTimeOffset createdAt)
    {
        private DateTimeOffset _lastObservedAt = createdAt;

        public Address Sender { get; } = sender;
        public List<NonceSubscriptionHandle> Handles { get; } = [];
        public ulong LastKnownNonce { get; private set; } = startNonce;
        public DateTimeOffset? NextDueAt { get; private set; }

        public void AddHandleLocked(NonceSubscriptionHandle handle, ulong startNonce, DateTimeOffset now)
        {
            if(Handles.Count == 0 || startNonce < LastKnownNonce)
            {
                LastKnownNonce = startNonce;
            }

            Handles.Add(handle);
            if(handle.RefreshInterval > TimeSpan.Zero)
            {
                var candidateDueAt = now + handle.RefreshInterval;
                if(NextDueAt is null || candidateDueAt < NextDueAt.Value)
                {
                    NextDueAt = candidateDueAt;
                }
            }
        }

        public bool RemoveHandleLocked(NonceSubscriptionHandle handle)
        {
            if(!Handles.Remove(handle))
            {
                return false;
            }

            handle.Complete();

            if(Handles.Count > 0 && handle.RefreshInterval > TimeSpan.Zero)
            {
                var minInterval = GetMinRefreshIntervalLocked();
                NextDueAt = minInterval is { } interval
                    ? _lastObservedAt + interval
                    : null;
            }

            return true;
        }

        public void ObserveNonceLocked(ulong nonce, ulong blockNumber, DateTimeOffset observedAt)
        {
            LastKnownNonce = nonce;
            _lastObservedAt = observedAt;

            var minInterval = GetMinRefreshIntervalLocked();
            NextDueAt = minInterval is { } interval
                ? observedAt + interval
                : null;

            var observation = new NonceObservation(nonce, blockNumber);
            foreach(var handle in Handles)
            {
                handle.Publish(observation);
            }
        }

        private TimeSpan? GetMinRefreshIntervalLocked()
        {
            TimeSpan? min = null;
            foreach(var handle in Handles)
            {
                if(handle.RefreshInterval > TimeSpan.Zero && (min is null || handle.RefreshInterval < min.Value))
                {
                    min = handle.RefreshInterval;
                }
            }

            return min;
        }
    }

    private sealed class NonceSubscriptionHandle(
        PollingNonceWatcher watcher,
        Address sender,
        TimeSpan refreshInterval
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
        public TimeSpan RefreshInterval { get; } = refreshInterval;

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
