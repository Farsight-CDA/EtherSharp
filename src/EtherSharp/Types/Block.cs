using System.Numerics;
namespace EtherSharp.Types;

public record BlockData(
    BigInteger? BaseFeePerGas,
    BigInteger? BlobGasUsed,
    BigInteger? ExcessBlobGas,
    byte[]? ParentBeaconBlockRoot,
    string? WithdrawalsRoot,
    Withdrawal[]? Withdrawals,
    bool? L1BlockNumber,
    bool? SendCount,
    bool? SendRoot,
    byte[]? MixHash,
    BigInteger? Difficulty,
    string? ExtraData,
    BigInteger? GasLimit,
    BigInteger? GasUsed,
    string Hash,
    string? LogsBloom,
    string? Miner,
    BigInteger? Nonce,
    BigInteger? Number,

    string? ParentHash,
    byte[]? ReceiptsRoot,
    byte[]? Sha3Uncles,
    BigInteger Size,
    byte[]? StateRoot,
    DateTimeOffset? Timestamp,
    BigInteger? TotalDifficulty,
    byte[]? TransactionsRoot,
    List<TransactionData>? Transactions
);

public record BlockDataTrasactionAsString(
    ulong Number,
    DateTimeOffset Timestamp,
    BigInteger? BaseFeePerGas,
    string Hash,
    string ParentHash
);

