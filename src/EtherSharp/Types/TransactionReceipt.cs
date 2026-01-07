using EtherSharp.Numerics;

namespace EtherSharp.Types;

public record TransactionReceipt(
    string TransactionHash,
    int Status,
    string BlockHash,
    ulong BlockNumber,
    Address From,
    Address? ContractAddress,
    ulong GasUsed,
    ulong CumulativeGasUsed,
    UInt256 EffectiveGasPrice,
    Log[] Logs,
    string? LogsBloom,
    Address? To,
    int? TransactionIndex,
    int? Type  //todo: enum ?
)
{
    public bool Success => Status == 1;
}
