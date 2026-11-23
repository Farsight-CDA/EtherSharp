using EtherSharp.Common.Exceptions;
using EtherSharp.Common.Extensions;
using EtherSharp.Common.Instrumentation;
using EtherSharp.Realtime;
using EtherSharp.Realtime.Subscription;
using EtherSharp.RPC.Modules.Eth;
using EtherSharp.RPC.Transport;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics.Metrics;

namespace EtherSharp.Client.Services.Subscriptions;

internal sealed class SubscriptionsManager : ISubscriptionsManager
{
    private readonly IRPCTransport _rpcTransport;
    private readonly IEthRpcModule _ethRpcModule;

    private readonly ILogger? _logger;

    private readonly Lock _subscriptionsLock = new Lock();
    private readonly List<ISubscription> _subscriptions = [];
    private int _installationsInProgress;
    private int _connectionLossCount;
    private bool _subscriptionsClosed;

    private readonly ObservableUpDownCounter<int>? _subscriptionsCounter;

    public SubscriptionsManager(IRPCTransport rpcTransport, IEthRpcModule ethRpcModule, IServiceProvider serviceProvider)
    {
        _rpcTransport = rpcTransport;
        _ethRpcModule = ethRpcModule;
        _logger = serviceProvider.GetService<ILoggerFactory>()?.CreateLogger<SubscriptionsManager>();

        _subscriptionsCounter = serviceProvider.CreateOTELObservableUpDownCounter("active_wss_subscriptions", () => _subscriptions.Count);

        _rpcTransport.OnConnectionLost += HandleConnectionLost;
        _rpcTransport.OnSubscriptionMessage += HandleSubscriptionMessage;
    }

    public async Task InstallSubscriptionAsync(
        ISubscription subscription, RpcRequestOptions requestOptions, CancellationToken cancellationToken)
    {
        int connectionLossCount;

        lock(_subscriptionsLock)
        {
            ThrowIfClosed();
            _installationsInProgress++;
            connectionLossCount = _connectionLossCount;
        }

        try
        {
            await subscription.InstallAsync(requestOptions, cancellationToken);

            lock(_subscriptionsLock)
            {
                ThrowIfClosed();

                if(_connectionLossCount == connectionLossCount)
                {
                    _subscriptions.Add(subscription);
                    return;
                }
            }

            var exception = new SubscriptionInterruptedException(SubscriptionInterruptionReason.ConnectionLost);
            subscription.Close(exception);
            throw exception;
        }
        finally
        {
            lock(_subscriptionsLock)
            {
                _installationsInProgress--;
            }
        }
    }

    public async Task UninstallSubscription(ISubscription subscription)
    {
        bool shouldUnsubscribe;

        lock(_subscriptionsLock)
        {
            shouldUnsubscribe = _subscriptions.Remove(subscription);
        }

        try
        {
            if(shouldUnsubscribe)
            {
                await _ethRpcModule.UnsubscribeAsync(subscription.Id, subscription.RequestOptions);
            }
        }
        finally
        {
            subscription.Close();
        }
    }

    public void CloseSubscriptions()
    {
        ISubscription[] subscriptions;

        lock(_subscriptionsLock)
        {
            if(_subscriptionsClosed)
            {
                return;
            }

            _subscriptionsClosed = true;
            subscriptions = [.. _subscriptions];
            _subscriptions.Clear();
        }

        _rpcTransport.OnConnectionLost -= HandleConnectionLost;
        _rpcTransport.OnSubscriptionMessage -= HandleSubscriptionMessage;

        foreach(var subscription in subscriptions)
        {
            subscription.Close();
        }
    }

    private void HandleConnectionLost(Exception connectionError)
    {
        ISubscription[] subscriptions;

        lock(_subscriptionsLock)
        {
            if(_subscriptionsClosed)
            {
                return;
            }

            _connectionLossCount++;
            subscriptions = [.. _subscriptions];
            _subscriptions.Clear();
        }

        if(subscriptions.Length == 0)
        {
            return;
        }

        _logger?.LogWarning("Connection lost, closing {count} subscriptions", subscriptions.Length);

        foreach(var subscription in subscriptions)
        {
            subscription.Close(
                new SubscriptionInterruptedException(SubscriptionInterruptionReason.ConnectionLost, innerException: connectionError)
            );
        }
    }

    private void HandleSubscriptionMessage(string subscriptionId, ReadOnlySpan<byte> payload)
    {
        if(_subscriptionsClosed)
        {
            return;
        }

        ISubscription? subscription = null;
        bool shouldUnsubscribe;

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

            shouldUnsubscribe = _installationsInProgress == 0;
        }

        if(subscription is not null)
        {
            try
            {
                var notification = SubscriptionNotification.Parse(payload);

                switch(notification.Type)
                {
                    case SubscriptionNotification.Kind.Result:
                        subscription.HandleSubscriptionMessage(notification.Result);
                        break;
                    case SubscriptionNotification.Kind.Terminated:
                        _logger?.LogWarning(
                            "Subscription id {id} of type {type} was terminated by the node. Reason: {reason}",
                            subscriptionId,
                            subscription,
                            notification.Error
                        );

                        bool wasActive;
                        lock(_subscriptionsLock)
                        {
                            wasActive = _subscriptions.Remove(subscription);
                        }

                        if(wasActive)
                        {
                            subscription.Close(
                                new SubscriptionInterruptedException(SubscriptionInterruptionReason.TerminatedByNode, notification.Error)
                            );
                        }
                        break;
                    case SubscriptionNotification.Kind.Malformed:
                        _logger?.LogWarning(
                            "Received notification without result for subscription id {id} of type {type}", subscriptionId, subscription
                        );
                        break;
                }
            }
            catch(Exception ex)
            {
                _logger?.LogWarning(ex, "Failed to process payload for subscription id {id} of type {type}", subscriptionId, subscription);
            }

            return;
        }

        if(!shouldUnsubscribe)
        {
            return;
        }

        _ = UninstallUnknownSubscriptionAsync(subscriptionId);
    }

    private async Task UninstallUnknownSubscriptionAsync(string subscriptionId)
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
            _logger?.LogCritical(ex, "Failed to uninstall unknown subscription with id {id}", subscriptionId);
        }
    }

    private void ThrowIfClosed()
        => ObjectDisposedException.ThrowIf(_subscriptionsClosed, this);
}
