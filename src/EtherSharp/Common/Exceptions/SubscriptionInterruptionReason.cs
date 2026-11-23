namespace EtherSharp.Common.Exceptions;

/// <summary>
/// Describes why a subscription was interrupted.
/// </summary>
public enum SubscriptionInterruptionReason
{
    /// <summary>
    /// The connection to the node was lost.
    /// </summary>
    ConnectionLost,
    /// <summary>
    /// The node terminated the subscription.
    /// </summary>
    TerminatedByNode,
}
