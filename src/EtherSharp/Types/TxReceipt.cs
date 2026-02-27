using EtherSharp.Numerics;

namespace EtherSharp.Types;

/// <summary>
/// Execution receipt returned by <c>eth_getTransactionReceipt</c> after a transaction is mined.
/// </summary>
/// <param name="BlockHash">Hash of the block that included the transaction.</param>
/// <param name="BlockNumber">Number of the block that included the transaction.</param>
/// <param name="ContractAddress">Created contract address for deployment transactions; otherwise <see langword="null"/>.</param>
/// <param name="CumulativeGasUsed">Total gas used in the block up to and including this transaction.</param>
/// <param name="EffectiveGasPrice">Per-unit gas price actually paid by this transaction.</param>
/// <param name="From">Sender address.</param>
/// <param name="GasUsed">Gas consumed by this transaction execution.</param>
/// <param name="Logs">Event logs emitted during execution.</param>
/// <param name="LogsBloom">Bloom filter summarizing emitted logs when provided by the node.</param>
/// <param name="Status">Execution status code where <c>1</c> is success and <c>0</c> is revert/failure.</param>
/// <param name="To">Recipient address; <see langword="null"/> for contract-creation transactions.</param>
/// <param name="TransactionHash">Hash of the transaction this receipt belongs to.</param>
/// <param name="TransactionIndex">Position of the transaction within its block.</param>
/// <param name="Type">Transaction envelope type, when returned by the node.</param>
/// <param name="L1Fee">Layer-1 data fee reported by OP Stack compatible chains.</param>
/// <param name="L1FeeScalar">Chain-specific scaling factor used in L1 fee calculation.</param>
/// <param name="L1GasPrice">Layer-1 gas price used for L1 data fee accounting.</param>
/// <param name="L1GasUsed">Layer-1 gas units attributed to this transaction.</param>
public record TxReceipt(
    Bytes32 BlockHash,
    ulong BlockNumber,
    Address? ContractAddress,
    ulong CumulativeGasUsed,
    UInt256 EffectiveGasPrice,
    Address From,
    ulong GasUsed,
    Log[] Logs,
    string? LogsBloom,
    uint Status,
    Address? To,
    Bytes32 TransactionHash,
    uint? TransactionIndex,
    TxType? Type,
    UInt256? L1Fee,
    UInt256? L1FeeScalar,
    UInt256? L1GasPrice,
    ulong? L1GasUsed
)
{
    /// <summary>
    /// Indicates whether the transaction executed successfully.
    /// </summary>
    public bool Success => Status == 1;
}
