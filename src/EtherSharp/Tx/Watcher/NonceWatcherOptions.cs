namespace EtherSharp.Tx.Watcher;

/// <summary>
/// Configures when sender nonces are read to detect transaction confirmations.
/// </summary>
public sealed class NonceWatcherOptions
{
    /// <summary>
    /// Whether reads are triggered by a newHeads subscription when the transport supports subscriptions.
    /// </summary>
    public bool UseNewHeads { get; set; } = true;

    /// <summary>
    /// Interval between reads when polling.
    /// </summary>
    public TimeSpan PollInterval { get; set; } = TimeSpan.FromSeconds(2);

    /// <summary>
    /// Minimum time between the starts of two reads triggered by new heads. Zero reads on every head.
    /// </summary>
    public TimeSpan MinHeadSpacing { get; set; } = TimeSpan.FromMilliseconds(500);

    internal void Validate(string paramName)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(PollInterval, TimeSpan.Zero, paramName);
        ArgumentOutOfRangeException.ThrowIfLessThan(MinHeadSpacing, TimeSpan.Zero, paramName);
    }
}
