using EtherSharp.Types;

namespace EtherSharp.Tx.Watcher;

/// <summary>
/// Represents an active watch registration for a specific <c>(sender, nonce)</c> slot.
/// </summary>
public interface ITxWatch : IDisposable
{
    /// <summary>
    /// Gets the sender account address being watched.
    /// </summary>
    public Address Sender { get; }

    /// <summary>
    /// Gets the sender nonce being watched.
    /// </summary>
    public ulong Nonce { get; }

    /// <summary>
    /// Adds a candidate transaction hash to this watch. Calling this with an already-tracked hash is idempotent.
    /// </summary>
    /// <param name="txHash">The candidate transaction hash to track.</param>
    /// <returns>
    /// <see langword="true"/> if the watch is still active and the hash is tracked;
    /// <see langword="false"/> if the watch has already resolved or been disposed.
    /// </returns>
    /// <exception cref="InvalidOperationException">The hash is already watched for another <c>(sender, nonce)</c> slot.</exception>
    public bool TryAddHash(in Bytes32 txHash);

    /// <summary>
    /// Removes a previously tracked transaction hash from this watch.
    /// </summary>
    /// <param name="txHash">The transaction hash to stop tracking.</param>
    /// <returns>
    /// <see langword="true"/> if the watch is active and the hash was removed;
    /// <see langword="false"/> if the hash was not tracked or the watch has already resolved or been disposed.
    /// </returns>
    public bool TryRemoveHash(in Bytes32 txHash);

    /// <summary>
    /// Waits for the watched <c>(sender, nonce)</c> slot to resolve.
    /// </summary>
    /// <param name="cancellationToken">
    /// Token that cancels only this wait operation; the underlying watch remains active until resolved or disposed.
    /// </param>
    /// <returns>The resolution outcome for this watch.</returns>
    public Task<TxWatchResult> WaitAsync(CancellationToken cancellationToken = default);
}
