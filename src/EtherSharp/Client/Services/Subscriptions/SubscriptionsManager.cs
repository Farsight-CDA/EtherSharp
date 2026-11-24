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
    //Subscription ids are scoped to a connection, installations that span a connection loss are not activated
    private int _connectionGeneration;
    private bool _isConnected = true;
    private bool _isClosed;

    private readonly ObservableUpDownCounter<int>? _subscriptionsCounter;

    //Must be called while holding _lock. Notifications can only be claimed while an eth_subscribe response is pending
    private bool IsInstalling
        => _subscriptions.Values.Any(status => status is SubscriptionStatus.Installing or SubscriptionStatus.Reinstalling);

    public SubscriptionsManager(IRPCTransport rpcTransport, IEthRpcModule ethRpcModule, IServiceProvider serviceProvider)
    {
        _rpcTransport = rpcTransport;
        _ethRpcModule = ethRpcModule;
        _logger = serviceProvider.GetService<ILoggerFactory>()?.CreateLogger<SubscriptionsManager>();

        _subscriptionsCounter = serviceProvider.CreateOTELObservableUpDownCounter("active_wss_subscriptions", () => _activeSubscriptions.Count);

        _rpcTransport.OnConnectionEstablished += HandleConnectionEstablished;
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
            _ = CompleteInstallation(subscription, connectionGeneration, InstallationOutcome.Failed);
            throw;
        }

        if(CompleteInstallation(subscription, connectionGeneration, InstallationOutcome.Installed))
        {
            return;
        }

        Exception exception = _isClosed
            ? new ObjectDisposedException(GetType().FullName)
            : new RPCTransportException("The connection was lost while installing the subscription.");

        subscription.Close();
        throw exception;
    }

    private async Task ReinstallAsync(ISubscription subscription, int connectionGeneration)
    {
        try
        {
            await subscription.InstallAsync(subscription.RequestOptions, CancellationToken.None);
        }
        catch(Exception ex)
        {
            _logger?.LogWarning(ex, "Failed to reinstall subscription of type {type}, retrying once reconnected", subscription);
            _ = CompleteInstallation(subscription, connectionGeneration, InstallationOutcome.Failed);
            return;
        }

        _ = CompleteInstallation(subscription, connectionGeneration, InstallationOutcome.Installed);
    }

    private bool CompleteInstallation(ISubscription subscription, int connectionGeneration, InstallationOutcome outcome)
    {
        bool isActivated = false;
        List<string> subscriptionIdsToUninstall = [];

        lock(_lock)
        {
            bool isWanted = _subscriptions.TryGetValue(subscription, out var status);
            bool isLive = outcome == InstallationOutcome.Installed && connectionGeneration == _connectionGeneration;

            switch((IsWanted: isWanted, IsLive: isLive, Status: status))
            {
                case { IsWanted: true, IsLive: true }:
                    subscription.Resume();
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
                    break;
                case { IsWanted: true, Status: SubscriptionStatus.Reinstalling }:
                    _subscriptions[subscription] = SubscriptionStatus.Interrupted;

                    //Reconnected while reinstalling, so the reconnect did not pick it up
                    if(_isConnected && connectionGeneration != _connectionGeneration)
                    {
                        StartReinstall(subscription);
                    }
                    break;
                case { IsWanted: true }:
                    //Failed or installed on a lost connection
                    _ = _subscriptions.Remove(subscription);
                    break;
                case { IsWanted: false, IsLive: true } when !_isClosed:
                    //Uninstalled while installing, the node side subscription is no longer needed
                    subscriptionIdsToUninstall.Add(subscription.Id);
                    break;
            }

            if(!IsInstalling && _parkedNotifications.Count > 0)
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

    //Must be called while holding _lock
    private void StartReinstall(ISubscription subscription)
    {
        _subscriptions[subscription] = SubscriptionStatus.Reinstalling;
        int connectionGeneration = _connectionGeneration;
        _ = Task.Run(() => ReinstallAsync(subscription, connectionGeneration));
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

        _rpcTransport.OnConnectionEstablished -= HandleConnectionEstablished;
        _rpcTransport.OnConnectionLost -= HandleConnectionLost;
        _rpcTransport.OnSubscriptionMessage -= HandleSubscriptionMessage;
    }

    private void HandleConnectionEstablished()
    {
        lock(_lock)
        {
            _isConnected = true;

            //Only interrupted ones, a subscription that is still reinstalling is never installed twice
            ISubscription[] interruptedSubscriptions = [.. _subscriptions
                .Where(x => x.Value == SubscriptionStatus.Interrupted)
                .Select(x => x.Key)
            ];

            foreach(var subscription in interruptedSubscriptions)
            {
                StartReinstall(subscription);
            }
        }
    }

    private void HandleConnectionLost(Exception connectionError)
    {
        lock(_lock)
        {
            if(_isClosed)
            {
                return;
            }

            _isConnected = false;
            _connectionGeneration++;

            if(_activeSubscriptions.Count > 0)
            {
                _logger?.LogWarning(
                    connectionError, "Connection lost, {count} subscriptions will be reinstalled once reconnected", _activeSubscriptions.Count
                );
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

            if(IsInstalling)
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

    private enum InstallationOutcome
    {
        Installed,
        Failed,
    }

    private enum SubscriptionStatus
    {
        Installing,
        Reinstalling,
        Active,
        Interrupted,
    }
}
