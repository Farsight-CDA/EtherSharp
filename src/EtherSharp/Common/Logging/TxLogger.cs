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

    [LoggerMessage(LogLevel.Information, "Tx {TxId} {Sender}/{Nonce}: Confirmed {TxHash} in block {BlockNumber}, gas used {GasUsed}")]
    public partial void Confirmed(Guid txId, Address sender, ulong nonce, Bytes32 txHash, ulong blockNumber, ulong gasUsed);
}
