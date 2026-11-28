using EtherSharp.RPC.Transport;
using EtherSharp.Tx.Watcher;
using EtherSharp.Types;

namespace EtherSharp.Tx.Nonce;

/// <summary>
/// Allocates the sender's next free on-chain nonce as last observed through <see cref="INonceWatcher"/>, without reserving it.
/// Repeated allocations return the same nonce until a higher on-chain nonce is observed. Never triggers reads of its own.
/// </summary>
public sealed class OnChainNonceAllocator : INonceAllocator
{
    private readonly Lock _lock = new Lock();
    private readonly INonceSubscription _subscription;

    private ulong _nextNonce;
    private bool _isDisposed;

    /// <summary>
    /// Creates a new <see cref="OnChainNonceAllocator"/>.
    /// </summary>
    public OnChainNonceAllocator(
        INonceWatcher nonceWatcher,
        in Address sender,
        ulong initialNonce
    )
    {
        ArgumentNullException.ThrowIfNull(nonceWatcher);

        _nextNonce = initialNonce;
        _subscription = nonceWatcher.Subscribe(in sender, NonceSubscriptionMode.Passive, initialNonce);
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
            return ValueTask.FromResult(_nextNonce);
        }
    }

    /// <summary>
    /// Nothing is reserved, so releasing succeeds as long as <paramref name="nonce"/> was not observed as consumed.
    /// </summary>
    public ValueTask<bool> TryReleaseAsync(
        ulong nonce,
        CancellationToken cancellationToken = default
    )
    {
        lock(_lock)
        {
            ObjectDisposedException.ThrowIf(_isDisposed, this);
            return ValueTask.FromResult(nonce >= _nextNonce);
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
