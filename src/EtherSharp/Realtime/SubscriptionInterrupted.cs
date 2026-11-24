namespace EtherSharp.Realtime;

/// <summary>
/// Emitted as soon as a subscription is interrupted. Items may be missed until a <see cref="SubscriptionResumed"/> follows.
/// </summary>
/// <param name="Reason">The reason the subscription was interrupted.</param>
/// <param name="Details">Additional details about the interruption, if any.</param>
public sealed record SubscriptionInterrupted(
    SubscriptionInterruptionReason Reason,
    string? Details
);
