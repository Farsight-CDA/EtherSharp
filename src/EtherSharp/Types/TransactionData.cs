namespace EtherSharp.Types;
public record TransactionData(
    string BlockHash,
    string BlockNumber,
    string From,
    string Gas,
    string GasPrice,
    string GasUsed,
    string Input,
    string Logs,
    string Nonce,
    string PublicKey,
    string R,
    string S,
    string To,
    string TransactionHash,
    string Value,
    string V
);
