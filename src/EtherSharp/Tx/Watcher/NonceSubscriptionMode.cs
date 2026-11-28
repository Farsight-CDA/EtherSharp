namespace EtherSharp.Tx.Watcher;

/// <summary>
/// How a nonce subscription takes part in nonce reads.
/// </summary>
public enum NonceSubscriptionMode
{
    /// <summary>
    /// Only receives readings made for active subscriptions, never causes reads of its own.
    /// </summary>
    Passive,

    /// <summary>
    /// Keeps the sender's nonce being read while the subscription exists.
    /// </summary>
    Active,
}
