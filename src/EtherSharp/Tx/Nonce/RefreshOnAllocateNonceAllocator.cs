using EtherSharp.RPC.Modules.Eth;
using EtherSharp.RPC.Transport;
using EtherSharp.Types;

namespace EtherSharp.Tx.Nonce;

/// <summary>
/// Allocates nonces by refreshing the sender's on-chain nonce before every allocation.
/// </summary>
public sealed class RefreshOnAllocateNonceAllocator : INonceAllocator
{
    private readonly IEthRpcModule _ethRpcModule;
    private readonly Address _sender;
    private readonly SemaphoreSlim _allocationLock = new SemaphoreSlim(1, 1);

    private ulong _nextNonce;
    private bool _isDisposed;

    /// <summary>
    /// Creates a new <see cref="RefreshOnAllocateNonceAllocator"/>.
    /// </summary>
    public RefreshOnAllocateNonceAllocator(
        IEthRpcModule ethRpcModule,
        in Address sender,
        ulong initialNonce
    )
    {
        ArgumentNullException.ThrowIfNull(ethRpcModule);

        _ethRpcModule = ethRpcModule;
        _sender = sender;
        _nextNonce = initialNonce;
    }

    /// <inheritdoc/>
    public async ValueTask<ulong> AllocateAsync(
        RpcRequestOptions requestOptions = default,
        CancellationToken cancellationToken = default
    )
    {
        await _allocationLock.WaitAsync(cancellationToken);
        try
        {
            ObjectDisposedException.ThrowIf(_isDisposed, this);

            ulong latestNonce = await _ethRpcModule.GetTransactionCountAsync(
                in _sender, TargetHeight.Latest, requestOptions, cancellationToken
            );

            if(latestNonce > _nextNonce)
            {
                _nextNonce = latestNonce;
            }

            return _nextNonce++;
        }
        finally
        {
            _ = _allocationLock.Release();
        }
    }

    /// <inheritdoc/>
    public async ValueTask<bool> TryReleaseAsync(
        ulong nonce,
        CancellationToken cancellationToken = default
    )
    {
        await _allocationLock.WaitAsync(cancellationToken);
        try
        {
            ObjectDisposedException.ThrowIf(_isDisposed, this);

            if(_nextNonce != nonce + 1)
            {
                return false;
            }

            _nextNonce--;
            return true;
        }
        finally
        {
            _ = _allocationLock.Release();
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if(_isDisposed)
        {
            return;
        }

        _isDisposed = true;
        _allocationLock.Dispose();
    }
}
