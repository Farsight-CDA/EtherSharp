using EtherSharp.Types;

namespace EtherSharp.Tx.Watcher;

/// <summary>
/// Watches sender account nonces and delivers observed readings to subscribers.
/// </summary>
public interface INonceWatcher
{
    /// <summary>
    /// Subscribes to nonce observations for a sender address.
    /// </summary>
    /// <param name="sender">The sender account address.</param>
    /// <param name="refreshInterval">Maximum interval between reads, or <see cref="TimeSpan.Zero"/> to only piggyback on free reads.</param>
    /// <param name="startNonce">Optional initial nonce hint.</param>
    public INonceSubscription Subscribe(
        in Address sender,
        TimeSpan refreshInterval = default,
        ulong startNonce = 0
    );
}
