namespace EtherSharp.Common.Exceptions;

/// <summary>
/// Thrown by a subscription stream when the subscription was interrupted and events may have been missed.
/// </summary>
/// <param name="reason">The reason the subscription was interrupted.</param>
/// <param name="details">Additional details appended to the message, if any.</param>
/// <param name="innerException">The error that caused the interruption, if any.</param>
public sealed class SubscriptionInterruptedException(
    SubscriptionInterruptionReason reason, string? details = null, Exception? innerException = null
) : Exception(CreateMessage(reason, details), innerException)
{
    /// <summary>
    /// Gets the reason the subscription was interrupted.
    /// </summary>
    public SubscriptionInterruptionReason Reason { get; } = reason;

    private static string CreateMessage(SubscriptionInterruptionReason reason, string? details)
    {
        string message = reason switch
        {
            SubscriptionInterruptionReason.ConnectionLost => "The connection was lost",
            SubscriptionInterruptionReason.TerminatedByNode => "The subscription was terminated by the node",
            _ => "The subscription was interrupted",
        };

        return details is null
            ? $"{message}."
            : $"{message}: {details}";
    }
}
