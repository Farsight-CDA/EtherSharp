namespace EtherSharp.Client.Services.TxScheduler;

/// <summary>
/// Controls how the tx scheduler synchronizes account nonce allocation.
/// </summary>
public enum NonceMode
{
    /// <summary>
    /// Uses only the scheduler's local nonce cursor.
    /// </summary>
    ExclusiveLocal,

    /// <summary>
    /// Periodically refreshes the confirmed nonce from the node even when the scheduler is idle.
    /// </summary>
    BackgroundSync,

    /// <summary>
    /// Refreshes the node's pending nonce before every local nonce allocation.
    /// </summary>
    RefreshOnAllocate,
}
