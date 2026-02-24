using EtherSharp.Numerics;

namespace EtherSharp.Types;

public record Transaction(
    string Hash,
    string BlockHash,
    ulong BlockNumber,
    string? ContractAddress,
    UInt256 GasPrice,
    UInt256 MaxFeePerGas,
    UInt256 MaxPriorityFeePerGas,
    ulong Gas,
    ulong Nonce,
    Address From,
    Address? To,
    byte[] Input,
    UInt256 Value
);
