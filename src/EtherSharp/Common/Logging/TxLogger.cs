using EtherSharp.Types;
using Microsoft.Extensions.Logging;

namespace EtherSharp.Common.Logging;

internal sealed partial class TxLogger(ILogger logger)
{
    private readonly ILogger _logger = logger;

    [LoggerMessage(LogLevel.Information, "Tx {TxId} {Sender}/{Nonce}: Prepared {TxHash}")]
    public partial void Prepared(Guid txId, Address sender, ulong nonce, Bytes32 txHash);

    [LoggerMessage(LogLevel.Information, "Tx {TxId} {Sender}/{Nonce}: Attached with {SubmissionCount} submissions, latest {TxHash}")]
    public partial void Attached(Guid txId, Address sender, ulong nonce, int submissionCount, Bytes32 txHash);

    [LoggerMessage(LogLevel.Information, "Tx {TxId} {Sender}/{Nonce}: Published submission {Sequence} {TxHash} ({Result})")]
    public partial void SubmissionPublished(Guid txId, Address sender, ulong nonce, uint sequence, Bytes32 txHash, string result);

    [LoggerMessage("Tx {TxId} {Sender}/{Nonce}: Publish of submission {Sequence} {TxHash} rejected ({Result})")]
    public partial void SubmissionRejected(
        LogLevel level, Exception? exception, Guid txId, Address sender, ulong nonce, uint sequence, Bytes32 txHash, string result
    );

    [LoggerMessage(LogLevel.Information, "Tx {TxId} {Sender}/{Nonce}: Repriced to submission {Sequence} {TxHash}")]
    public partial void Repriced(Guid txId, Address sender, ulong nonce, uint sequence, Bytes32 txHash);

    [LoggerMessage(
        LogLevel.Information,
        "Tx {TxId} {Sender}/{Nonce}: Confirmed {TxHash} in block {BlockNumber}, success {Success}, gas used {GasUsed}"
    )]
    public partial void Confirmed(Guid txId, Address sender, ulong nonce, Bytes32 txHash, ulong blockNumber, bool success, ulong gasUsed);

    [LoggerMessage(LogLevel.Warning, "Tx {TxId} {Sender}/{Nonce}: Nonce used by foreign tx {TxHash} in block {BlockNumber}")]
    public partial void Replaced(Guid txId, Address sender, ulong nonce, Bytes32 txHash, ulong blockNumber);

    [LoggerMessage(LogLevel.Warning, "Tx {TxId} {Sender}/{Nonce}: Nonce already used, no matching transaction found")]
    public partial void NonceAlreadyUsed(Guid txId, Address sender, ulong nonce);

    [LoggerMessage(
        LogLevel.Warning,
        "Tx {TxId} {Sender}/{Nonce}: Receipt for {TxHash} in block {BlockNumber} unavailable after {RetryCount} retries, watching the nonce again"
    )]
    public partial void ReceiptUnavailable(Guid txId, Address sender, ulong nonce, Bytes32 txHash, ulong blockNumber, int retryCount);

    [LoggerMessage(LogLevel.Warning, "{Sender}/{Nonce}: Nonce not released after failed preparation, later transactions may stall on the gap")]
    public partial void NonceNotReleased(Exception exception, Address sender, ulong nonce);
}
