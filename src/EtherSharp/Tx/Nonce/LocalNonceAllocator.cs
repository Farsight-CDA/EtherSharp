using EtherSharp.RPC.Transport;
using EtherSharp.Tx.Watcher;
using EtherSharp.Types;

namespace EtherSharp.Tx.Nonce;

/// <summary>
/// Allocates nonces from a local cursor that catches up with nonce reads <see cref="INonceWatcher"/> makes anyway,
/// e.g. while transactions of the sender are pending. Never triggers reads of its own.
/// </summary>
public sealed class LocalNonceAllocator : INonceAllocator
{
    private readonly Lock _lock = new Lock();
    private readonly INonceSubscription _subscription;

    private ulong _nextNonce;
    private ulong _minNonce;
    private bool _isDisposed;

    /// <summary>
    /// Creates a new <see cref="LocalNonceAllocator"/>.
    /// </summary>
    public LocalNonceAllocator(
        INonceWatcher nonceWatcher,
        in Address sender,
        ulong initialNonce
    )
    {
        ArgumentNullException.ThrowIfNull(nonceWatcher);

        _nextNonce = initialNonce;
        _minNonce = initialNonce;
        _subscription = nonceWatcher.Subscribe(in sender, TimeSpan.Zero, initialNonce);
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
            if(nonce < _minNonce || _nextNonce <= _minNonce || _nextNonce - 1 != nonce)
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
                if(observedNonce > _minNonce)
                {
                    _minNonce = observedNonce;
                }

                if(observedNonce > _nextNonce)
                {
                    _nextNonce = observedNonce;
                }
            }
        }
    }
}
