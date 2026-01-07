using EtherSharp.Numerics;

namespace EtherSharp.Types;

public record TransactionData(
    string BlockHash,
    string BlockNumber,
    Address From,
    ulong Gas,
    UInt256 GasPrice,
    ulong GasUsed,
    string Input,
    string Logs,
    string Nonce,
    string PublicKey,
    string R,
    string S,
    Address To,
    string TransactionHash,
    UInt256 Value,
    string V
);
