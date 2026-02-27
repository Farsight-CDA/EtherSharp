using EtherSharp.Numerics;
namespace EtherSharp.Types;

/// <summary>
/// Block payload returned by <c>eth_getBlockByNumber</c> when transaction hashes are requested.
/// </summary>
/// <param name="BaseFeePerGas">EIP-1559 base fee for this block.</param>
/// <param name="BlobGasUsed">Total blob gas consumed in this block (EIP-4844).</param>
/// <param name="Difficulty">Reported PoW difficulty for legacy networks.</param>
/// <param name="ExcessBlobGas">Excess blob gas carried forward for blob base fee calculation.</param>
/// <param name="ExtraData">Validator/miner extra data field.</param>
/// <param name="GasLimit">Gas limit for the block.</param>
/// <param name="GasUsed">Total gas consumed by transactions in the block.</param>
/// <param name="Hash">The block hash.</param>
/// <param name="L1BlockNumber">Rollup-reported L1 origin block number, when available.</param>
/// <param name="LogsBloom">Bloom filter aggregating all logs in the block.</param>
/// <param name="Miner">Block producer address when available.</param>
/// <param name="MixHash">Mix hash / prevRandao value for this block.</param>
/// <param name="Nonce">Block nonce for PoW-style blocks.</param>
/// <param name="Number">Block height, or <see langword="null"/> when unavailable.</param>
/// <param name="ParentBeaconBlockRoot">Beacon chain root linked to this execution block.</param>
/// <param name="ParentHash">Parent block hash.</param>
/// <param name="ReceiptsRoot">Merkle root of the transaction receipts trie.</param>
/// <param name="SendCount">Rollup-specific send count metadata.</param>
/// <param name="SendRoot">Rollup-specific send root metadata.</param>
/// <param name="Sha3Uncles">Hash of the ommers/uncles list.</param>
/// <param name="Size">Block size in bytes.</param>
/// <param name="StateRoot">State trie root after executing this block.</param>
/// <param name="Timestamp">Block production timestamp.</param>
/// <param name="TotalDifficulty">Cumulative chain difficulty at this block.</param>
/// <param name="Transactions">Transaction hashes included in the block.</param>
/// <param name="TransactionsRoot">Merkle root of the transactions trie.</param>
/// <param name="Withdrawals">Consensus-layer withdrawals included in this block.</param>
/// <param name="WithdrawalsRoot">Merkle root of the withdrawals list when provided by the node.</param>
public record Block(
    UInt256? BaseFeePerGas,
    UInt256? BlobGasUsed,
    UInt256? Difficulty,
    UInt256? ExcessBlobGas,
    string ExtraData,
    ulong GasLimit,
    ulong GasUsed,
    Bytes32 Hash,
    ulong? L1BlockNumber,
    string LogsBloom,
    Address? Miner,
    Bytes32 MixHash,
    ulong? Nonce,
    ulong? Number,
    Bytes32? ParentBeaconBlockRoot,
    Bytes32 ParentHash,
    Bytes32 ReceiptsRoot,
    ulong? SendCount,
    Bytes32? SendRoot,
    Bytes32 Sha3Uncles,
    ulong Size,
    Bytes32 StateRoot,
    DateTimeOffset Timestamp,
    UInt256? TotalDifficulty,
    Bytes32[] Transactions,
    Bytes32 TransactionsRoot,
    Withdrawal[]? Withdrawals,
    Bytes32? WithdrawalsRoot
);
