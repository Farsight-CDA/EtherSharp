﻿using EtherSharp.Client.Services.RPC;
using EtherSharp.Common.Exceptions;
using EtherSharp.Common.Extensions;
using EtherSharp.Common.Instrumentation;
using EtherSharp.Realtime;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics.Metrics;

namespace EtherSharp.Client.Services.Subscriptions;
internal class SubscriptionsManager
{
    private readonly IRpcClient _rpcClient;
    private readonly ILogger? _logger;

    private readonly Lock _subscriptionsLock = new Lock();
    private readonly List<ISubscription> _subscriptions = [];

    private readonly OTELCounter<long>? _subscriptionMessageCounter;
    private readonly ObservableUpDownCounter<int>? _subscriptionsCounter;

    public SubscriptionsManager(IRpcClient rpcClient, IServiceProvider serviceProvider)
    {
        _rpcClient = rpcClient;
        _logger = serviceProvider.GetService<ILoggerFactory>()?.CreateLogger<SubscriptionsManager>();

        _subscriptionMessageCounter = serviceProvider.CreateOTELCounter<long>("subscription_messages_received");
        _subscriptionsCounter = serviceProvider.CreateOTELObservableUpDownCounter("active_wss_subscriptions", () => _subscriptions.Count);

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
                        _logger?.LogDebug("Reinstalling subscription of type {type}, oldId={oldId}", subscription.GetType().Name, subscription.Id);
                        await subscription.InstallAsync();
                    }
                    catch(Exception ex)
                    {
                        _logger?.LogCritical(ex, "Failed to reinstall subscription id {id} of type {type}", subscription.Id, subscription.GetType().Name);
                    }
                });
            }
        }
    }

    private void HandleSubscriptionMessage(string subscriptionId, ReadOnlySpan<byte> payload)
    {
        _subscriptionMessageCounter?.Add(1);

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
                _logger?.LogDebug("Uninstalling unknown active subscription with id {id}", subscriptionId);
                await _rpcClient.EthUnsubscribeAsync(subscriptionId);
            }
            catch(RPCException ex) when(ex.Message.Contains("not found"))
            {
                return;
            }
            catch(Exception ex)
            {
                _logger?.LogCritical(ex, "Failed to uninstall unknown subscription with id {id}", subscriptionId);
            }
        });
    }
}
