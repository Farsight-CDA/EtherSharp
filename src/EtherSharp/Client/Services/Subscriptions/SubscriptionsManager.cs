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

    private readonly Lock _lock = new Lock();
    //Every subscription owned by the manager, removing one uninstalls it
    private readonly Dictionary<ISubscription, SubscriptionStatus> _subscriptions = [];
    //Routes notifications of active subscriptions by their node side id
    private readonly Dictionary<string, ISubscription> _activeSubscriptions = [];
    //Some nodes (e.g. Nethermind, Besu) can send notifications before the eth_subscribe response, those are parked until claimed
    private readonly Dictionary<string, List<byte[]>> _parkedNotifications = [];
    //Subscription ids are scoped to a connection, installations that span a connection loss are discarded
    private int _connectionGeneration;
    private bool _isClosed;

    private readonly ObservableUpDownCounter<int>? _subscriptionsCounter;

    public SubscriptionsManager(IRPCTransport rpcTransport, IEthRpcModule ethRpcModule, IServiceProvider serviceProvider)
    {
        _rpcTransport = rpcTransport;
        _ethRpcModule = ethRpcModule;
        _logger = serviceProvider.GetService<ILoggerFactory>()?.CreateLogger<SubscriptionsManager>();

        _subscriptionsCounter = serviceProvider.CreateOTELObservableUpDownCounter("active_wss_subscriptions", () => _activeSubscriptions.Count);

        _rpcTransport.OnConnectionLost += HandleConnectionLost;
        _rpcTransport.OnSubscriptionMessage += HandleSubscriptionMessage;
    }

    public async Task InstallSubscriptionAsync(
        ISubscription subscription, RpcRequestOptions requestOptions, CancellationToken cancellationToken)
    {
        int connectionGeneration;

        lock(_lock)
        {
            ObjectDisposedException.ThrowIf(_isClosed, this);
            _subscriptions.Add(subscription, SubscriptionStatus.Installing);
            connectionGeneration = _connectionGeneration;
        }

        try
        {
            await subscription.InstallAsync(requestOptions, cancellationToken);
        }
        catch
        {
            _ = CompleteInstallation(subscription, connectionGeneration, false);
            throw;
        }

        if(CompleteInstallation(subscription, connectionGeneration, true))
        {
            return;
        }

        Exception exception = _isClosed
            ? new ObjectDisposedException(GetType().FullName)
            : new RPCTransportException("The connection was lost while installing the subscription.");

        subscription.Close();
        throw exception;
    }

    //Returns whether the installed subscription was activated
    private bool CompleteInstallation(ISubscription subscription, int connectionGeneration, bool isInstalled)
    {
        bool isActivated = false;
        List<string> subscriptionIdsToUninstall = [];

        lock(_lock)
        {
            if(isInstalled && !_isClosed && connectionGeneration == _connectionGeneration)
            {
                if(_subscriptions.ContainsKey(subscription))
                {
                    _subscriptions[subscription] = SubscriptionStatus.Active;
                    _activeSubscriptions.Add(subscription.Id, subscription);
                    isActivated = true;

                    if(_parkedNotifications.Remove(subscription.Id, out var parkedNotifications))
                    {
                        foreach(byte[] payload in parkedNotifications)
                        {
                            Dispatch(subscription, payload);
                        }
                    }
                }
                else
                {
                    //Uninstalled while installing, the node side subscription is no longer needed
                    subscriptionIdsToUninstall.Add(subscription.Id);
                }
            }
            else
            {
                //Failed or installed on a lost connection
                _ = _subscriptions.Remove(subscription);
            }

            if(!_subscriptions.ContainsValue(SubscriptionStatus.Installing) && _parkedNotifications.Count > 0)
            {
                subscriptionIdsToUninstall.AddRange(_parkedNotifications.Keys);
                _parkedNotifications.Clear();
            }
        }

        foreach(string subscriptionId in subscriptionIdsToUninstall)
        {
            _ = UninstallUnknownSubscriptionAsync(subscriptionId);
        }

        return isActivated;
    }

    public async Task UninstallSubscription(ISubscription subscription)
    {
        bool wasActive;

        lock(_lock)
        {
            wasActive = _subscriptions.Remove(subscription, out var status) && status == SubscriptionStatus.Active;

            if(wasActive)
            {
                _ = _activeSubscriptions.Remove(subscription.Id);
            }
        }

        try
        {
            if(wasActive)
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
        lock(_lock)
        {
            if(_isClosed)
            {
                return;
            }

            _isClosed = true;

            foreach(var subscription in _subscriptions.Keys)
            {
                subscription.Close();
            }

            _subscriptions.Clear();
            _activeSubscriptions.Clear();
            _parkedNotifications.Clear();
        }

        _rpcTransport.OnConnectionLost -= HandleConnectionLost;
        _rpcTransport.OnSubscriptionMessage -= HandleSubscriptionMessage;
    }

    private void HandleConnectionLost(Exception connectionError)
    {
        lock(_lock)
        {
            if(_isClosed)
            {
                return;
            }

            _connectionGeneration++;

            if(_activeSubscriptions.Count > 0)
            {
                _logger?.LogWarning(connectionError, "Connection lost, interrupting {count} subscriptions", _activeSubscriptions.Count);
            }

            foreach(var subscription in _activeSubscriptions.Values)
            {
                subscription.Interrupt(new SubscriptionInterrupted(SubscriptionInterruptionReason.ConnectionLost, connectionError.Message));
                _subscriptions[subscription] = SubscriptionStatus.Interrupted;
            }

            _activeSubscriptions.Clear();
            _parkedNotifications.Clear();
        }
    }

    private void HandleSubscriptionMessage(string subscriptionId, ReadOnlySpan<byte> payload)
    {
        lock(_lock)
        {
            if(_isClosed)
            {
                return;
            }

            if(_activeSubscriptions.TryGetValue(subscriptionId, out var subscription))
            {
                Dispatch(subscription, payload);
                return;
            }

            if(_subscriptions.ContainsValue(SubscriptionStatus.Installing))
            {
                if(!_parkedNotifications.TryGetValue(subscriptionId, out var parkedNotifications))
                {
                    parkedNotifications = [];
                    _parkedNotifications.Add(subscriptionId, parkedNotifications);
                }

                parkedNotifications.Add(payload.ToArray());
                return;
            }
        }

        _ = UninstallUnknownSubscriptionAsync(subscriptionId);
    }

    private void Dispatch(ISubscription subscription, ReadOnlySpan<byte> payload)
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
                        subscription.Id,
                        subscription,
                        notification.Error
                    );

                    if(_activeSubscriptions.Remove(subscription.Id))
                    {
                        _ = _subscriptions.Remove(subscription);
                        subscription.Interrupt(new SubscriptionInterrupted(SubscriptionInterruptionReason.TerminatedByNode, notification.Error));
                        subscription.Close();
                    }
                    break;
                case SubscriptionNotification.Kind.Malformed:
                    _logger?.LogWarning(
                        "Received notification without result for subscription id {id} of type {type}", subscription.Id, subscription
                    );
                    break;
            }
        }
        catch(Exception ex)
        {
            _logger?.LogWarning(ex, "Failed to process payload for subscription id {id} of type {type}", subscription.Id, subscription);
        }
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

    private enum SubscriptionStatus
    {
        //Waiting for the eth_subscribe response
        Installing,
        //Receiving notifications, routed through _activeSubscriptions
        Active,
        //Lost its node side subscription
        Interrupted,
    }
}
