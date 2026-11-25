using EtherSharp.Types;

namespace EtherSharp.Tx.Watcher;

/// <summary>
/// Final outcome of watching a <c>(sender, nonce)</c> slot via <see cref="ITxWatch"/>.
/// </summary>
public closed record TxWatchResult
{
    /// <summary>
    /// One of the watch's tracked transactions was included in a block.
    /// </summary>
    public sealed record Confirmed(Bytes32 TransactionHash, ulong BlockNumber, Bytes32 BlockHash) : TxWatchResult;

    /// <summary>
    /// A transaction outside this watch's tracked hashes consumed the <c>(sender, nonce)</c> slot.
    /// </summary>
    public sealed record Replaced(Bytes32 TransactionHash, ulong BlockNumber, Bytes32 BlockHash) : TxWatchResult;

    /// <summary>
    /// The nonce was already consumed on-chain and the including transaction could not be located.
    /// </summary>
    public sealed record NonceAlreadyUsed() : TxWatchResult;
}
