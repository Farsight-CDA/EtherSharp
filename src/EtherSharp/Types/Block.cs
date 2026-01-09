using EtherSharp.Numerics;
namespace EtherSharp.Types;

public record BlockData(
    UInt256? Number,
    UInt256? BaseFeePerGas,
    UInt256? BlobGasUsed,
    UInt256? ExcessBlobGas,
    byte[]? ParentBeaconBlockRoot,
    string? WithdrawalsRoot,
    Withdrawal[]? Withdrawals,
    ulong? L1BlockNumber,
    ulong? SendCount,
    byte[]? SendRoot,
    byte[]? MixHash,
    UInt256? Difficulty,
    string? ExtraData,
    ulong? GasLimit,
    ulong? GasUsed,
    string Hash,
    string? LogsBloom,
    Address? Miner,
    ulong? Nonce,

    string? ParentHash,
    byte[]? ReceiptsRoot,
    byte[]? Sha3Uncles,
    ulong Size,
    byte[]? StateRoot,
    DateTimeOffset? Timestamp,
    UInt256? TotalDifficulty,
    byte[]? TransactionsRoot,
    List<TransactionData>? Transactions
);

public record BlockDataTrasactionAsString(
    ulong Number,
    DateTimeOffset Timestamp,
    UInt256? BaseFeePerGas,
    string Hash,
    string ParentHash,
    IReadOnlyList<string> Transactions
);

