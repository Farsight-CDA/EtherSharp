namespace EtherSharp.Realtime;

/// <summary>
/// An item received from a subscription, or a <see cref="SubscriptionInterrupted"/> and <see cref="SubscriptionResumed"/>
/// marking the start and end of a gap in which items may have been missed.
/// </summary>
/// <typeparam name="T">The type of the subscription items.</typeparam>
public readonly union SubscriptionEvent<T>(T, SubscriptionInterrupted, SubscriptionResumed)
    where T : notnull;
