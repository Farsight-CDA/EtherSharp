using System.Numerics;

namespace EtherSharp.Types;
public record TransactionReceipt(
    string TransactionHash,
    int Status,
    string BlockHash,
    uint BlockNumber,
    string From,
    string ContractAddress,
    uint GasUsed,
    uint CumulativeGasUsed,
    BigInteger EffectiveGasPrice,
    Log[] Logs,
    string? LogsBloom,
    string? To,
    int? TransactionIndex,
    int? Type  //todo: enum ?
);
