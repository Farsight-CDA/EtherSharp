using EtherSharp.Types;
using Microsoft.Extensions.Logging;

namespace EtherSharp.Common.Logging;

internal sealed partial class TxWatcherLogger(ILogger logger)
{
    private readonly ILogger _logger = logger;

    [LoggerMessage(LogLevel.Warning, "Heartbeat: Reading nonces of {SenderCount} senders failed")]
    public partial void NonceReadFailed(Exception exception, int senderCount);

    [LoggerMessage(LogLevel.Warning, "Heartbeat: newHeads subscription unavailable, polling nonces every {PollInterval} instead")]
    public partial void NewHeadsUnavailable(Exception exception, TimeSpan pollInterval);

    [LoggerMessage(LogLevel.Debug, "Dispose: Heartbeat failed during shutdown")]
    public partial void HeartbeatShutdownFailed(Exception exception);

    [LoggerMessage(LogLevel.Debug, "Dispose: In-flight work failed during shutdown")]
    public partial void InFlightWorkShutdownFailed(Exception exception);

    [LoggerMessage(
        LogLevel.Warning,
        "Nonce {Nonce} of {Sender} is outside the search range starting at {SearchStart}; read it with eth_getTransactionCount at block {BlockNumber}"
    )]
    public partial void NonceOutsideSearchRange(ulong nonce, Address sender, ulong searchStart, ulong blockNumber);

    [LoggerMessage(LogLevel.Warning, "Slot {Sender}/{Nonce}: Block {BlockNumber} unavailable after retries, waiting for the nonce again")]
    public partial void SlotBlockUnavailable(Address sender, ulong nonce, ulong blockNumber);

    [LoggerMessage(LogLevel.Warning, "Slot {Sender}/{Nonce}: Block {BlockNumber} has no matching transaction")]
    public partial void SlotBlockWithoutMatchingTransaction(Address sender, ulong nonce, ulong blockNumber);

    [LoggerMessage(LogLevel.Warning, "Slot {Sender}/{Nonce}: Search failed")]
    public partial void SlotSearchFailed(Exception exception, Address sender, ulong nonce);

    [LoggerMessage(LogLevel.Warning, "Slot {Sender}/{Nonce}: Unconfirmed after {SearchCount} searches, treating the nonce as used")]
    public partial void SlotUnconfirmed(Address sender, ulong nonce, int searchCount);

    [LoggerMessage("Slot {Sender}/{Nonce}: Used at block {ObservedBlock} without usable unused block {UnusedBlock}, looking back")]
    public partial void SlotLookingBack(LogLevel level, Address sender, ulong nonce, ulong observedBlock, ulong? unusedBlock);

    [LoggerMessage(LogLevel.Warning, "Slot {Sender}/{Nonce}: Lookback failed, looking up hashes")]
    public partial void SlotLookbackFailed(Exception exception, Address sender, ulong nonce);

    [LoggerMessage(LogLevel.Warning, "Slot {Sender}/{Nonce}: Used more than {MaxLookbackBlocks} blocks before block {ObservedBlock}, looking up hashes")]
    public partial void SlotBeyondLookback(Address sender, ulong nonce, ulong maxLookbackBlocks, ulong observedBlock);

    [LoggerMessage(LogLevel.Warning, "Slot {Sender}/{Nonce}: Hash {TxHash} belongs to {TxSender}/{TxNonce}, ignoring it")]
    public partial void SlotHashMismatch(Address sender, ulong nonce, Bytes32 txHash, Address txSender, ulong txNonce);
}
