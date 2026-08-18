using EtherSharp.Realtime;
using EtherSharp.RPC.Transport;

namespace EtherSharp.Client.Services.Subscriptions;

/// <summary>
/// Manages lifecycle operations for realtime subscriptions used by the client.
/// </summary>
public interface ISubscriptionsManager
{
    /// <summary>
    /// Installs and starts the provided subscription.
    /// </summary>
    /// <param name="subscription">The subscription instance to install.</param>
    /// <param name="requestOptions">Transport-specific request options.</param>
    /// <param name="cancellationToken">A token used to cancel installation.</param>
    /// <returns>A task that completes when the subscription is active.</returns>
    public Task InstallSubscriptionAsync(
        ISubscription subscription, RpcRequestOptions requestOptions = default, CancellationToken cancellationToken = default);

    /// <summary>
    /// Stops and removes the provided subscription.
    /// </summary>
    /// <param name="subscription">The subscription instance to uninstall.</param>
    /// <returns>A task that completes when the subscription has been removed.</returns>
    public Task UninstallSubscription(ISubscription subscription);

    /// <summary>
    /// Closes all active subscriptions locally without sending unsubscribe RPCs.
    /// </summary>
    public void CloseSubscriptions();
}
