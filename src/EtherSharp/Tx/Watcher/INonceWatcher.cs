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
    /// <param name="mode">Whether the subscription causes reads.</param>
    /// <param name="startNonce">Optional initial nonce hint.</param>
    public INonceSubscription Subscribe(
        in Address sender,
        NonceSubscriptionMode mode,
        ulong startNonce = 0
    );
}
