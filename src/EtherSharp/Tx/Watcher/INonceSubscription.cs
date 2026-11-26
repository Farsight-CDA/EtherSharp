using EtherSharp.Types;

namespace EtherSharp.Tx.Watcher;

/// <summary>
/// Subscription to observed nonces for a sender address.
/// </summary>
public interface INonceSubscription : IDisposable
{
    /// <summary>
    /// Sender account address being observed.
    /// </summary>
    public Address Sender { get; }

    /// <summary>
    /// Listens for newly observed nonces.
    /// </summary>
    public IAsyncEnumerable<NonceObservation> ListenAsync(CancellationToken cancellationToken = default);
}
