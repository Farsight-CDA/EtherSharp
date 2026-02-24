using EtherSharp.Numerics;

namespace EtherSharp.Types;

public record TxData(
    string Hash,
    string? BlockHash,
    ulong? BlockNumber,
    Address From,
    ulong Gas,
    UInt256 GasPrice,
    UInt256 MaxfeePerGas,
    UInt256 MaxPriorityFeePerGas,
    byte[] Input,
    ulong Nonce,
    Address? To,
    uint? TransactionIndex,
    UInt256 Value,
    TxType Type,
    ulong ChainId,
    ulong V,
    byte[] R,
    byte[] S
);
