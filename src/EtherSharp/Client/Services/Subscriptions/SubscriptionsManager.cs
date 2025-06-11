using EtherSharp.Client.Services.RPC;
using EtherSharp.Realtime;
using EtherSharp.Realtime.Blocks.Subscription;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EtherSharp.Client.Services.Subscriptions;
internal class SubscriptionsManager
{
    private readonly IRpcClient _rpcClient;
    private readonly ILogger _logger;

    private readonly Lock _subscriptionsLock = new Lock();
    private readonly List<ISubscription> _subscriptions = [];

    public SubscriptionsManager(IRpcClient rpcClient, IServiceProvider serviceProvider)
    {
        _rpcClient = rpcClient;
        _logger = serviceProvider.GetRequiredService<ILogger<SubscriptionsManager>>();

        _rpcClient.OnConnectionEstablished += HandleConnectionEstablished;
        _rpcClient.OnSubscriptionMessage += HandleSubscriptionMessage;
    }

    public async Task InstallSubscriptionAsync(ISubscription subscription, CancellationToken cancellationToken)
    {
        await subscription.InstallAsync(cancellationToken);

        lock(_subscriptionsLock)
        {
            _subscriptions.Add(subscription);
        }
    }

    public async Task UninstallSubscription(ISubscription subscription)
    {
        lock(_subscriptionsLock)
        {
            _subscriptions.Remove(subscription);
        }

        await _rpcClient.EthUnsubscribeAsync(subscription.Id);
    }

    private void HandleConnectionEstablished()
    {
        lock(_subscriptionsLock)
        {
            foreach(var subscription in _subscriptions)
            {
                _ = Task.Run(async () =>
                {
                    try
                    {
                        _logger.LogDebug("Reinstalling subscription of type {type}, oldId={oldId}", subscription.GetType().Name, subscription.Id);
                        await subscription.InstallAsync();
                    }
                    catch(Exception ex)
                    {
                        _logger.LogCritical(ex, "Failed to reinstall subscription id {id} of type {type}", subscription.Id, subscription.GetType().Name);
                    }
                });
            }
        }
    }

    private void HandleSubscriptionMessage(string subscriptionId, ReadOnlySpan<byte> payload)
    {
        ISubscription? subscription = null;

        lock(_subscriptionsLock)
        {
            foreach(var s in _subscriptions)
            {
                if(s.Id == subscriptionId)
                {
                    subscription = s;
                    break;
                }
            }
        }

        if(subscription is not null)
        {
            subscription.HandleSubscriptionMessage(payload);
            return;
        }

        _ = Task.Run(async () =>
        {
            try
            {
                _logger.LogDebug("Uninstalling unknown active subscription with id {id}", subscriptionId);
                await _rpcClient.EthUnsubscribeAsync(subscriptionId);
            }
            catch(Exception ex)
            {
                _logger.LogCritical(ex, "Failed to uninstall unknown subscription with id {id}", subscriptionId);
            }
        });
    }
}
