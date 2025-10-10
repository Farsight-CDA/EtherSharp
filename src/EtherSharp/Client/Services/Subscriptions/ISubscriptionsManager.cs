using EtherSharp.Realtime;

namespace EtherSharp.Client.Services.Subscriptions;

public interface ISubscriptionsManager
{
    public Task InstallSubscriptionAsync(ISubscription subscription, CancellationToken cancellationToken);
    public Task UninstallSubscription(ISubscription subscription);
}
