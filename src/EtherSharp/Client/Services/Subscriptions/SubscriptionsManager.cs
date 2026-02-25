using EtherSharp.Common.Exceptions;
using EtherSharp.Common.Extensions;
using EtherSharp.Common.Instrumentation;
using EtherSharp.Realtime;
using EtherSharp.RPC;
using EtherSharp.RPC.Modules.Eth;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics.Metrics;

namespace EtherSharp.Client.Services.Subscriptions;

internal class SubscriptionsManager : ISubscriptionsManager
{
    private readonly IRpcClient _rpcClient;
    private readonly IEthRpcModule _ethRpcModule;

    private readonly ILogger? _logger;

    private readonly Lock _subscriptionsLock = new Lock();
    private readonly List<ISubscription> _subscriptions = [];

    private readonly ObservableUpDownCounter<int>? _subscriptionsCounter;

    public SubscriptionsManager(IRpcClient rpcClient, IEthRpcModule ethRpcModule, IServiceProvider serviceProvider)
    {
        _rpcClient = rpcClient;
        _ethRpcModule = ethRpcModule;
        _logger = serviceProvider.GetService<ILoggerFactory>()?.CreateLogger<SubscriptionsManager>();

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

        await _ethRpcModule.UnsubscribeAsync(subscription.Id);
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
                        if(_logger?.IsEnabled(LogLevel.Debug) == true)
                        {
                            string subscriptionType = subscription.GetType().Name;
                            _logger.LogDebug("Reinstalling subscription of type {type}, oldId={oldId}", subscriptionType, subscription.Id);
                        }

                        await subscription.InstallAsync();
                    }
                    catch(Exception ex)
                    {
                        if(_logger?.IsEnabled(LogLevel.Critical) == true)
                        {
                            string subscriptionType = subscription.GetType().Name;
                            _logger.LogCritical(ex, "Failed to reinstall subscription id {id} of type {type}", subscription.Id, subscriptionType);
                        }
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
            try
            {
                _ = subscription.HandleSubscriptionMessage(payload);
            }
            catch(Exception ex)
            {
                if(_logger?.IsEnabled(LogLevel.Warning) == true)
                {
                    string subscriptionType = subscription.GetType().Name;
                    _logger.LogWarning(ex,
                        "Failed to process payload for subscription id {id} of type {type}",
                        subscription.Id,
                        subscriptionType
                    );
                }
            }

            return;
        }

        _ = Task.Run(async () =>
        {
            try
            {
                if(_logger?.IsEnabled(LogLevel.Debug) == true)
                {
                    _logger.LogDebug("Uninstalling unknown active subscription with id {id}", subscriptionId);
                }

                await _ethRpcModule.UnsubscribeAsync(subscriptionId);
            }
            catch(RPCException ex) when(ex.Message.Contains("not found"))
            {
                return;
            }
            catch(Exception ex)
            {
                if(_logger?.IsEnabled(LogLevel.Critical) == true)
                {
                    _logger.LogCritical(ex, "Failed to uninstall unknown subscription with id {id}", subscriptionId);
                }
            }
        });
    }
}
