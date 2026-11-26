using EtherSharp.RPC.Transport;

namespace EtherSharp.Tx.Nonce;

/// <summary>
/// Allocates sequential transaction nonces for a sender account.
/// </summary>
public interface INonceAllocator : IDisposable
{
    /// <summary>
    /// Allocates the next nonce for a transaction.
    /// </summary>
    public ValueTask<ulong> AllocateAsync(
        RpcRequestOptions requestOptions = default,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Releases a previously allocated nonce if no higher nonce has been allocated since.
    /// </summary>
    public ValueTask<bool> TryReleaseAsync(
        ulong nonce,
        CancellationToken cancellationToken = default
    );
}
