using System.Numerics;

namespace EtherSharp.Types;
public record Transaction(
    string Hash,
    string BlockHash,
    ulong BlockNumber,
    string? ContractAddress,
    BigInteger GasPrice,
    BigInteger MaxFeePerGas,
    BigInteger MaxPriorityFeePerGas,
    ulong Gas,
    ulong Nonce,
    Address From,
    Address To,
    byte[] Input,
    BigInteger Value
);