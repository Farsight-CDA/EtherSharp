using Microsoft.Extensions.Logging;
using System.Text;

namespace EtherSharp.Common.Logging;

internal sealed partial class WssTransportLogger(ILogger logger)
{
    private readonly ILogger _logger = logger;

    [LoggerMessage(LogLevel.Warning, "Websocket connection lost, killing {RequestCount} pending requests...")]
    public partial void ConnectionLost(Exception exception, int requestCount);

    [LoggerMessage(LogLevel.Critical, "Websocket reconnection attempt crashed, retrying in 3s...")]
    public partial void ReconnectionCrashed(Exception exception);

    [LoggerMessage(LogLevel.Warning, "{EventName} handler threw")]
    public partial void ConnectionEventHandlerThrew(Exception exception, string eventName);

    [LoggerMessage(LogLevel.Debug, "Initiating websocket connection...")]
    public partial void Connecting();

    [LoggerMessage(LogLevel.Debug, "Connection attempt failed, retrying in 3s...")]
    public partial void ConnectionAttemptFailed(Exception exception);

    [LoggerMessage(LogLevel.Information, "Websocket connection established...")]
    public partial void Connected();

    [LoggerMessage(LogLevel.Debug, "Received response to request id that is not pending")]
    public partial void ResponseNotPending();

    public void UnidentifiedPayload(ReadOnlySpan<byte> payload)
    {
        if(_logger.IsEnabled(LogLevel.Warning))
        {
            UnidentifiedPayloadMessage(Encoding.UTF8.GetString(payload));
        }
    }

    [LoggerMessage(LogLevel.Warning, "Received unidentified websocket payload: {payload}")]
    private partial void UnidentifiedPayloadMessage(string payload);

    [LoggerMessage(LogLevel.Warning, "Subscription handler threw for subscription {SubscriptionId}")]
    public partial void SubscriptionHandlerThrew(Exception exception, string subscriptionId);

    [LoggerMessage(LogLevel.Warning, "Failed to identify payload, JSON root is not an object")]
    public partial void PayloadRootNotObject();

    [LoggerMessage(LogLevel.Warning, "Failed to identify payload with method {Method}")]
    public partial void PayloadMethodUnknown(string? method);

    [LoggerMessage(LogLevel.Warning, "Failed to identify payload, failed to proceed reading")]
    public partial void PayloadReadFailed();

    [LoggerMessage(LogLevel.Warning, "Failed to identify payload, no marker found till end of payload")]
    public partial void PayloadMarkerMissing();

    [LoggerMessage(LogLevel.Warning, "Exception while trying to identify payload")]
    public partial void PayloadIdentificationFailed(Exception exception);
}
