namespace EtherSharp.Realtime;

/// <summary>
/// Emitted once a subscription is live again after it was interrupted.
/// Items between the last received item and this marker may have been missed.
/// </summary>
/// <param name="Reason">The reason the subscription was interrupted.</param>
/// <param name="Details">Additional details about the interruption, if any.</param>
public sealed record SubscriptionInterruption(
    SubscriptionInterruptionReason Reason,
    string? Details
);
