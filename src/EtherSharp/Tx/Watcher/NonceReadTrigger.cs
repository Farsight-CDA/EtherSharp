namespace EtherSharp.Tx.Watcher;

/// <summary>
/// Decides when <see cref="PollingNonceWatcher"/> reads the nonces of actively watched senders.
/// </summary>
internal closed class NonceReadTrigger
{
    /// <summary>
    /// Triggers a read a fixed interval after the previous one.
    /// </summary>
    public sealed class Interval(TimeSpan interval) : NonceReadTrigger
    {
        private readonly TimeSpan _interval = interval > TimeSpan.Zero
            ? interval
            : throw new ArgumentOutOfRangeException(nameof(interval));

        public override Task WaitAsync(CancellationToken cancellationToken)
            => Task.Delay(_interval, cancellationToken);
    }

    /// <summary>
    /// Waits until the next read is due. Called after each read completes.
    /// </summary>
    public abstract Task WaitAsync(CancellationToken cancellationToken);
}
