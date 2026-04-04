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
    /// Passively synchronizes allocatable nonce state from the node's pending nonce in the background.
    /// </summary>
    BackgroundSync,

    /// <summary>
    /// Refreshes the node's pending nonce before every local nonce allocation.
    /// </summary>
    RefreshOnAllocate,
}
