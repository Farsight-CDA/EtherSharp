using EtherSharp.Numerics;

namespace EtherSharp.Types;

public record TxReceipt(
    string BlockHash,
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
    string TransactionHash,
    uint? TransactionIndex,
    TxType? Type,
    UInt256? L1Fee,
    UInt256? L1FeeScalar,
    UInt256? L1GasPrice,
    ulong? L1GasUsed
)
{
    public bool Success => Status == 1;
}
