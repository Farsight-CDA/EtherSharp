namespace EtherSharp.Realtime;

/// <summary>
/// An item received from a subscription, or a <see cref="SubscriptionInterruption"/> marking that items may have been missed.
/// </summary>
/// <typeparam name="T">The type of the subscription items.</typeparam>
public readonly union SubscriptionEvent<T>(T, SubscriptionInterruption)
    where T : notnull;
