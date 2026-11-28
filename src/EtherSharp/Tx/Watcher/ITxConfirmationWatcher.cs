using EtherSharp.Types;

namespace EtherSharp.Tx.Watcher;

/// <summary>
/// Watches pending transaction <c>(sender, nonce)</c> slots until inclusion or nonce consumption.
/// </summary>
public interface ITxConfirmationWatcher
{
    /// <summary>
    /// Starts watching a <c>(sender, nonce)</c> slot with optional initial candidate transaction hashes.
    /// Additional candidate hashes can be registered with <see cref="ITxWatch.TryAddHash"/> before broadcasting them.
    /// </summary>
    /// <param name="sender">The sender account address.</param>
    /// <param name="nonce">The sender nonce to monitor.</param>
    /// <param name="txHashes">Optional initial candidate transaction hashes associated with this slot.</param>
    /// <returns>A disposable handle representing this watch registration.</returns>
    /// <exception cref="InvalidOperationException">A hash is already watched for another <c>(sender, nonce)</c> slot.</exception>
    public ITxWatch Watch(in Address sender, ulong nonce, params ReadOnlySpan<Bytes32> txHashes);

    /// <summary>
    /// Starts watching a <c>(sender, nonce)</c> slot, seeding the inclusion search with a block at which the nonce was unused.
    /// </summary>
    /// <param name="sender">The sender account address.</param>
    /// <param name="nonce">The sender nonce to monitor.</param>
    /// <param name="unusedAtBlock">A block at which the nonce is known to be unused. A wrong value only costs extra lookups.</param>
    /// <param name="txHashes">Optional initial candidate transaction hashes associated with this slot.</param>
    /// <returns>A disposable handle representing this watch registration.</returns>
    /// <exception cref="InvalidOperationException">A hash is already watched for another <c>(sender, nonce)</c> slot.</exception>
    public ITxWatch Watch(in Address sender, ulong nonce, ulong unusedAtBlock, params ReadOnlySpan<Bytes32> txHashes);
}
