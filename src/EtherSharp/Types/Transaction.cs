using System.Numerics;

namespace EtherSharp.Types;
public record Transaction(
    string BlockHash,
    ulong BlockNumber,
    string? ContractAddress,
    BigInteger EffectiveGasPrice,
    string From,
    ulong GasUsed,
    BigInteger? L1Fee    
);