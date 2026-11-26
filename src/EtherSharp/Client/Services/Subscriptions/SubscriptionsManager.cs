using EtherSharp.Common.Exceptions;
using EtherSharp.Common.Extensions;
using EtherSharp.Common.Instrumentation;
using EtherSharp.Realtime;
using EtherSharp.Realtime.Subscription;
using EtherSharp.RPC.Modules.Eth;
using EtherSharp.RPC.Transport;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using System.Diagnostics.Metrics;

namespace EtherSharp.Client.Services.Subscriptions;

internal sealed class SubscriptionsManager : ISubscriptionsManager
{
    private static readonly TimeSpan _minReinstallDelay = TimeSpan.FromSeconds(1);
    private static readonly TimeSpan _maxReinstallDelay = TimeSpan.FromSeconds(60);
    private static readonly TimeSpan _stableDuration = TimeSpan.FromMinutes(1);

    private readonly IRPCTransport _rpcTransport;
    private readonly IEthRpcModule _ethRpcModule;

    private readonly ILogger? _logger;

    private readonly Lock _lock = new Lock();
    //Every subscription owned by the manager, removing one uninstalls it
    private readonly Dictionary<ISubscription, SubscriptionState> _subscriptions = [];
    //Routes notifications of active subscriptions by their node side id
    private readonly Dictionary<string, ISubscription> _activeSubscriptions = [];
    //Some nodes (e.g. Nethermind, Besu) can send notifications before the eth_subscribe response, those are parked until claimed
    private readonly Dictionary<string, List<byte[]>> _parkedNotifications = [];
    //Subscription ids are scoped to a connection, installations that span a connection loss are reinstalled
    private int _connectionGeneration;
    private bool _isConnected = true;
    private bool _isClosed;

    private readonly ObservableUpDownCounter<int>? _subscriptionsCounter;
    private readonly OTELCounter<long>? _interruptionsCounter;

    //Must be called while holding _lock. Notifications can only be claimed while an eth_subscribe response is pending
    private bool IsInstalling
        => _subscriptions.Values.Any(state => state.Status == SubscriptionStatus.Installing);

    //Observed from the metrics thread
    private Measurement<int>[] ObserveSubscriptions()
    {
        lock(_lock)
        {
            int interrupted = _subscriptions.Values.Count(
                state => state.Status is SubscriptionStatus.Interrupted or SubscriptionStatus.ReinstallScheduled
            );

            return [
                new Measurement<int>(_activeSubscriptions.Count, new KeyValuePair<string, object?>("status", "active")),
                new Measurement<int>(interrupted, new KeyValuePair<string, object?>("status", "interrupted")),
            ];
        }
    }

    public SubscriptionsManager(IRPCTransport rpcTransport, IEthRpcModule ethRpcModule, IServiceProvider serviceProvider)
    {
        _rpcTransport = rpcTransport;
        _ethRpcModule = ethRpcModule;
        _logger = serviceProvider.GetService<ILoggerFactory>()?.CreateLogger<SubscriptionsManager>();

        _subscriptionsCounter = serviceProvider.CreateOTELObservableUpDownCounter<int>("wss_subscriptions", ObserveSubscriptions);
        _interruptionsCounter = serviceProvider.CreateOTELCounter<long>("wss_subscription_interruptions");

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
            _subscriptions.Add(subscription, new SubscriptionState(SubscriptionStatus.Installing));
            connectionGeneration = _connectionGeneration;
        }

        try
        {
            await subscription.InstallAsync(requestOptions, cancellationToken);
        }
        catch
        {
            //A new subscription that failed to install is dropped, the caller gets the exception
            lock(_lock)
            {
                _ = _subscriptions.Remove(subscription);
            }

            CompleteInstallation(subscription, connectionGeneration, InstallationOutcome.Failed);
            throw;
        }

        CompleteInstallation(subscription, connectionGeneration, InstallationOutcome.Installed);
    }

    //Must be called while holding _lock, only for interrupted subscriptions
    private void StartReinstall(ISubscription subscription)
    {
        var state = _subscriptions[subscription];
        var delay = state.ReinstallDelay;

        state.Status = SubscriptionStatus.ReinstallScheduled;
        state.ReinstallDelay = TimeSpan.FromSeconds(
            Math.Clamp(delay.TotalSeconds * 2, _minReinstallDelay.TotalSeconds, _maxReinstallDelay.TotalSeconds)
        );

        _ = Task.Run(() => ReinstallAsync(subscription, delay));
    }

    private async Task ReinstallAsync(ISubscription subscription, TimeSpan delay)
    {
        await Task.Delay(delay);

        int connectionGeneration;

        lock(_lock)
        {
            //Uninstalled while waiting
            if(!_subscriptions.TryGetValue(subscription, out var state))
            {
                return;
            }

            //Disconnected while waiting, the reconnect starts it again
            if(!_isConnected)
            {
                state.Status = SubscriptionStatus.Interrupted;
                return;
            }

            state.Status = SubscriptionStatus.Installing;
            connectionGeneration = _connectionGeneration;
        }

        try
        {
            await subscription.InstallAsync(subscription.RequestOptions, CancellationToken.None);
        }
        catch(Exception ex)
        {
            _logger?.LogWarning(ex, "Failed to reinstall subscription of type {type}, retrying", subscription);
            CompleteInstallation(subscription, connectionGeneration, InstallationOutcome.Failed);
            return;
        }

        CompleteInstallation(subscription, connectionGeneration, InstallationOutcome.Installed);
    }

    private void CompleteInstallation(ISubscription subscription, int connectionGeneration, InstallationOutcome outcome)
    {
        List<string> subscriptionIdsToUninstall = [];

        lock(_lock)
        {
            bool isWanted = _subscriptions.TryGetValue(subscription, out var state);
            bool isLive = outcome == InstallationOutcome.Installed && connectionGeneration == _connectionGeneration && !_isClosed;

            switch((IsWanted: isWanted, IsLive: isLive))
            {
                case { IsWanted: true, IsLive: true }:
                    subscription.Resume();
                    state!.Status = SubscriptionStatus.Active;
                    _activeSubscriptions.Add(subscription.Id, subscription);

                    if(_parkedNotifications.Remove(subscription.Id, out var parkedNotifications))
                    {
                        foreach(byte[] payload in parkedNotifications)
                        {
                            Dispatch(subscription, payload);
                        }
                    }
                    break;
                case { IsWanted: true, IsLive: false }:
                    subscription.Interrupt(new SubscriptionInterrupted(
                        SubscriptionInterruptionReason.ConnectionLost, "The connection was lost while installing the subscription."
                    ));
                    state!.Status = SubscriptionStatus.Interrupted;

                    if(_isConnected)
                    {
                        StartReinstall(subscription);
                    }
                    break;
                case { IsWanted: false, IsLive: true }:
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
    }

    public async Task UninstallSubscription(ISubscription subscription)
    {
        bool wasActive;

        lock(_lock)
        {
            wasActive = _subscriptions.Remove(subscription, out var state) && state.Status == SubscriptionStatus.Active;

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
        catch(Exception ex)
        {
            //Either gone with the connection, or uninstalled once its next notification arrives as unknown
            _logger?.LogWarning(ex, "Failed to unsubscribe subscription id {id} of type {type}", subscription.Id, subscription);
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

            foreach(var (subscription, state) in _subscriptions)
            {
                if(state.Status == SubscriptionStatus.Interrupted)
                {
                    StartReinstall(subscription);
                }
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
                InterruptSubscription(
                    subscription,
                    new SubscriptionInterrupted(SubscriptionInterruptionReason.ConnectionLost, connectionError.Message)
                );
            }

            _activeSubscriptions.Clear();
            _parkedNotifications.Clear();
        }
    }

    //Must be called while holding _lock
    private void InterruptSubscription(ISubscription subscription, SubscriptionInterrupted interruption)
    {
        subscription.Interrupt(interruption);
        _interruptionsCounter?.Add(1, new KeyValuePair<string, object?>("reason", interruption.Reason.ToString()));
        var state = _subscriptions[subscription];
        state.Status = SubscriptionStatus.Interrupted;

        //Reinstalled right away, unless it keeps getting interrupted shortly after becoming live
        if(DateTimeOffset.UtcNow - subscription.ActiveSince >= _stableDuration)
        {
            state.ReinstallDelay = TimeSpan.Zero;
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
                        "Subscription id {id} of type {type} was terminated by the node, reinstalling. Reason: {reason}",
                        subscription.Id,
                        subscription,
                        notification.Error
                    );

                    if(_activeSubscriptions.Remove(subscription.Id))
                    {
                        InterruptSubscription(
                            subscription,
                            new SubscriptionInterrupted(SubscriptionInterruptionReason.TerminatedByNode, notification.Error)
                        );
                        StartReinstall(subscription);
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
        catch(RpcErrorException ex) when(ex.Message.Contains("not found"))
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

    private sealed class SubscriptionState(SubscriptionStatus status)
    {
        public SubscriptionStatus Status { get; set; } = status;
        public TimeSpan ReinstallDelay { get; set; }
    }

    private enum SubscriptionStatus
    {
        Installing,
        ReinstallScheduled,
        Active,
        Interrupted,
    }
}
