using System.Numerics;

namespace EtherSharp.Types;
public record TransactionReceipt(
    string TransactionHash,
    int Status,
    string BlockHash,
    uint BlockNumber,
    Address From,
    Address? ContractAddress,
    ulong GasUsed,
    ulong CumulativeGasUsed,
    BigInteger EffectiveGasPrice,
    Log[] Logs,
    string? LogsBloom,
    Address? To,
    int? TransactionIndex,
    int? Type  //todo: enum ?
)
{
    public bool Success => Status == 1;
}
