using EtherSharp.RPC.Transport;
using EtherSharp.Tx.Watcher;
using EtherSharp.Types;

namespace EtherSharp.Tx.Nonce;

/// <summary>
/// Allocates nonces from a local cursor kept in sync through <see cref="INonceWatcher"/>.
/// </summary>
public sealed class LocalNonceAllocator : INonceAllocator
{
    /// <summary>
    /// Configuration for <see cref="LocalNonceAllocator"/>.
    /// </summary>
    public sealed class Options
    {
        /// <summary>
        /// Maximum interval between background nonce reads when idle, or <see cref="TimeSpan.Zero"/> to only update from free reads.
        /// </summary>
        public TimeSpan SyncInterval { get; set; } = TimeSpan.Zero;
    }

    private readonly Lock _lock = new Lock();
    private readonly INonceSubscription _subscription;

    private ulong _nextNonce;
    private bool _isDisposed;

    /// <summary>
    /// Creates a new <see cref="LocalNonceAllocator"/>.
    /// </summary>
    public LocalNonceAllocator(
        INonceWatcher nonceWatcher,
        in Address sender,
        ulong initialNonce,
        Options? options = null
    )
    {
        ArgumentNullException.ThrowIfNull(nonceWatcher);

        var resolved = options ?? new Options();
        ArgumentOutOfRangeException.ThrowIfLessThan(resolved.SyncInterval, TimeSpan.Zero, nameof(options));

        _nextNonce = initialNonce;
        _subscription = nonceWatcher.Subscribe(in sender, resolved.SyncInterval, initialNonce);
        _ = Task.Run(ObserveNoncesAsync);
    }

    /// <inheritdoc/>
    public ValueTask<ulong> AllocateAsync(
        RpcRequestOptions requestOptions = default,
        CancellationToken cancellationToken = default
    )
    {
        lock(_lock)
        {
            ObjectDisposedException.ThrowIf(_isDisposed, this);
            return ValueTask.FromResult(_nextNonce++);
        }
    }

    /// <inheritdoc/>
    public ValueTask<bool> TryReleaseAsync(
        ulong nonce,
        CancellationToken cancellationToken = default
    )
    {
        lock(_lock)
        {
            ObjectDisposedException.ThrowIf(_isDisposed, this);
            if(_nextNonce != nonce + 1)
            {
                return ValueTask.FromResult(false);
            }

            _nextNonce--;
            return ValueTask.FromResult(true);
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        lock(_lock)
        {
            if(_isDisposed)
            {
                return;
            }

            _isDisposed = true;
            _subscription.Dispose();
        }
    }

    private async Task ObserveNoncesAsync()
    {
        await foreach(var (observedNonce, _) in _subscription.ListenAsync())
        {
            lock(_lock)
            {
                if(observedNonce > _nextNonce)
                {
                    _nextNonce = observedNonce;
                }
            }
        }
    }
}
