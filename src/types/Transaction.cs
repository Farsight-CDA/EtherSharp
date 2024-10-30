using System.Numerics;

namespace EVM.net.types;

public record Transaction(
    string BlockHash,
    BigInteger BlockNumber,
    string From,
    uint Gas,
    BigInteger GasPrice,
    BigInteger MaxFeePerGas,
    BigInteger MaxPriorityFeePerGas,
    string Hash,
    string Input,
    BigInteger Nonce,
    string To,
    uint TransactionIndex,
    BigInteger? Value,
    uint Type,  // enum 
    List<AccessItem?> AccessList,  //todo: this is a complex type
    uint ChainId,
    Signature Signature,
    uint YParity
);