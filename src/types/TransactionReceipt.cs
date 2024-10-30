using System.Numerics;

namespace EVM.net.types;
public record TransactionReceipt(
    string BlockHash,
    uint BlockNumber,
    string From,
    string ContractAddress,
    uint GasUsed,
    uint CumulativeGasUsed,
    BigInteger EffectiveGasPrice,
    List<TransactionRecievedLog>? Logs,
    string? LogsBloom,
    string? Status,
    string? To,
    string? TransactionHash,
    int? TransactionIndex,
    int? Type  //todo: enum ?
);
