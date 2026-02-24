using EtherSharp.Realtime.Events;

namespace EtherSharp.Types;

/// <summary>
/// Raw Ethereum log entry returned by receipt, filter, or subscription APIs.
/// </summary>
/// <param name="Address">Contract address that emitted the log.</param>
/// <param name="BlockHash">Hash of the block containing this log.</param>
/// <param name="BlockNumber">Block number containing this log.</param>
/// <param name="LogIndex">Position of this log within the block.</param>
/// <param name="TransactionHash">Hash of the transaction that emitted this log.</param>
/// <param name="TransactionIndex">Position of the transaction within the block.</param>
/// <param name="Topics">Indexed log topics; topic 0 is typically the event signature hash.</param>
/// <param name="Data">ABI-encoded non-indexed event data payload.</param>
/// <param name="Removed"><see langword="true"/> when this log was removed due to a chain reorganization.</param>
public record Log(
    Address Address,
    Hash32 BlockHash,
    ulong BlockNumber,
    uint LogIndex,
    Hash32 TransactionHash,
    uint TransactionIndex,
    Hash32[] Topics,
    byte[] Data,
    bool Removed
) : ITxLog<Log>
{
    /// <inheritdoc/>
    Log ITxLog.Event => this;

    /// <summary>
    /// Returns the raw log unchanged for untyped event flows.
    /// </summary>
    /// <param name="log">Raw log payload.</param>
    /// <returns>The same <paramref name="log"/> instance.</returns>
    public static Log Decode(Log log) => log;
}
