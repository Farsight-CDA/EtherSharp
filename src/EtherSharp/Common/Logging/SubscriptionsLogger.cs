using EtherSharp.Realtime;
using Microsoft.Extensions.Logging;

namespace EtherSharp.Common.Logging;

internal sealed partial class SubscriptionsLogger(ILogger logger)
{
    private readonly ILogger _logger = logger;

    [LoggerMessage(LogLevel.Warning, "Failed to reinstall subscription of type {type}, retrying")]
    public partial void ReinstallFailed(Exception exception, ISubscription type);

    [LoggerMessage(LogLevel.Warning, "Failed to unsubscribe subscription id {id} of type {type}")]
    public partial void UnsubscribeFailed(Exception exception, string id, ISubscription type);

    [LoggerMessage(LogLevel.Warning, "Connection lost, {count} subscriptions will be reinstalled once reconnected")]
    public partial void ConnectionLost(Exception exception, int count);

    [LoggerMessage(LogLevel.Warning, "Subscription id {id} of type {type} was terminated by the node, reinstalling. Reason: {reason}")]
    public partial void TerminatedByNode(string id, ISubscription type, string? reason);

    [LoggerMessage(LogLevel.Warning, "Received notification without result for subscription id {id} of type {type}")]
    public partial void MalformedNotification(string id, ISubscription type);

    [LoggerMessage(LogLevel.Warning, "Failed to process payload for subscription id {id} of type {type}")]
    public partial void PayloadProcessingFailed(Exception exception, string id, ISubscription type);

    [LoggerMessage(LogLevel.Debug, "Uninstalling unknown active subscription with id {id}")]
    public partial void UninstallingUnknown(string id);

    [LoggerMessage(LogLevel.Critical, "Failed to uninstall unknown subscription with id {id}")]
    public partial void UninstallUnknownFailed(Exception exception, string id);
}
