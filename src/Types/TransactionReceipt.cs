using System.Numerics;

namespace EtherSharp.Types;
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
