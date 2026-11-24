namespace EtherSharp.Realtime;

/// <summary>
/// Emitted once a subscription is live again after a <see cref="SubscriptionInterrupted"/>, before any newly received item.
/// Items between the interruption and this marker may have been missed.
/// </summary>
public sealed class SubscriptionResumed
{
    /// <summary>
    /// The shared instance, the marker carries no data.
    /// </summary>
    public static SubscriptionResumed Instance { get; } = new SubscriptionResumed();

    private SubscriptionResumed()
    {
    }
}
