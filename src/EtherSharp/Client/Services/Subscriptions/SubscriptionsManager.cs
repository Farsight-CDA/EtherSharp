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
    private readonly Dictionary<string, ISubscription> _activeSubscriptions = [];
    //Some nodes (e.g. Nethermind, Besu) can send notifications before the eth_subscribe response, those are parked until claimed
    private readonly Dictionary<string, List<byte[]>> _parkedNotifications = [];
    private int _pendingInstallationCount;
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
            _pendingInstallationCount++;
            connectionGeneration = _connectionGeneration;
        }

        try
        {
            await subscription.InstallAsync(requestOptions, cancellationToken);
        }
        catch
        {
            CompleteInstallation(null, connectionGeneration);
            throw;
        }

        if(CompleteInstallation(subscription, connectionGeneration))
        {
            return;
        }

        Exception exception = _isClosed
            ? new ObjectDisposedException(GetType().FullName)
            : new SubscriptionInterruptedException(SubscriptionInterruptionReason.ConnectionLost);

        subscription.Close(exception);
        throw exception;
    }

    //Returns whether the installed subscription was activated
    private bool CompleteInstallation(ISubscription? installedSubscription, int connectionGeneration)
    {
        bool isActivated = false;
        string[] unclaimedSubscriptionIds = [];

        lock(_lock)
        {
            _pendingInstallationCount--;

            if(installedSubscription is not null && !_isClosed && connectionGeneration == _connectionGeneration)
            {
                _activeSubscriptions.Add(installedSubscription.Id, installedSubscription);
                isActivated = true;

                if(_parkedNotifications.Remove(installedSubscription.Id, out var parkedNotifications))
                {
                    foreach(byte[] payload in parkedNotifications)
                    {
                        Dispatch(installedSubscription, payload);
                    }
                }
            }

            //No pending installation is left that could claim the remaining parked notifications
            if(_pendingInstallationCount == 0 && _parkedNotifications.Count > 0)
            {
                unclaimedSubscriptionIds = [.. _parkedNotifications.Keys];
                _parkedNotifications.Clear();
            }
        }

        foreach(string subscriptionId in unclaimedSubscriptionIds)
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
            //Id is null if the installation never completed. Ids can be reused across connections, so match on the instance as well
            wasActive = subscription.Id is { } subscriptionId
                && _activeSubscriptions.TryGetValue(subscriptionId, out var activeSubscription)
                && ReferenceEquals(activeSubscription, subscription)
                && _activeSubscriptions.Remove(subscriptionId);
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
            CloseAll(null);
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
                _logger?.LogWarning("Connection lost, closing {count} subscriptions", _activeSubscriptions.Count);
            }

            CloseAll(connectionError);
        }
    }

    private void CloseAll(Exception? connectionError)
    {
        foreach(var subscription in _activeSubscriptions.Values)
        {
            subscription.Close(connectionError is null
                ? null
                : new SubscriptionInterruptedException(SubscriptionInterruptionReason.ConnectionLost, innerException: connectionError)
            );
        }

        _activeSubscriptions.Clear();
        _parkedNotifications.Clear();
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

            if(_pendingInstallationCount > 0)
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
                        subscription.Close(
                            new SubscriptionInterruptedException(SubscriptionInterruptionReason.TerminatedByNode, notification.Error)
                        );
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
}
