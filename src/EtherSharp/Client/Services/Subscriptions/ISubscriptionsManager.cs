using EtherSharp.Realtime;

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
    /// <param name="cancellationToken">A token used to cancel installation.</param>
    /// <returns>A task that completes when the subscription is active.</returns>
    public Task InstallSubscriptionAsync(ISubscription subscription, CancellationToken cancellationToken);

    /// <summary>
    /// Stops and removes the provided subscription.
    /// </summary>
    /// <param name="subscription">The subscription instance to uninstall.</param>
    /// <returns>A task that completes when the subscription has been removed.</returns>
    public Task UninstallSubscription(ISubscription subscription);
}
