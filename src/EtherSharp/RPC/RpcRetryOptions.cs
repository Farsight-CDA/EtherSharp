namespace EtherSharp.RPC;

/// <summary>
/// Configuration for retrying transient RPC failures with exponential backoff.
/// </summary>
public sealed class RpcRetryOptions
{
    /// <summary>
    /// Initial delay before retrying after a transient failure, doubled on consecutive retries.
    /// </summary>
    public TimeSpan RetryDelay { get; set; } = TimeSpan.FromMilliseconds(500);

    /// <summary>
    /// Upper bound of the backoff after consecutive errors.
    /// </summary>
    public TimeSpan MaxErrorBackoff { get; set; } = TimeSpan.FromSeconds(16);

    internal void Validate(string paramName)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(RetryDelay, TimeSpan.Zero, paramName);
        ArgumentOutOfRangeException.ThrowIfLessThan(MaxErrorBackoff, RetryDelay, paramName);
    }

    internal static TimeSpan NextBackoff(TimeSpan currentDelay, TimeSpan maxErrorBackoff)
        => currentDelay * 2 < maxErrorBackoff
            ? currentDelay * 2
            : maxErrorBackoff;
}
