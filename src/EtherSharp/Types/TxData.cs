using EtherSharp.Numerics;

namespace EtherSharp.Types;

/// <summary>
/// Transaction payload returned by <c>eth_getTransactionByHash</c>.
/// </summary>
/// <param name="Hash">Transaction hash.</param>
/// <param name="BlockHash">Containing block hash, or <see langword="null"/> while pending.</param>
/// <param name="BlockNumber">Containing block number, or <see langword="null"/> while pending.</param>
/// <param name="From">Sender address recovered from the transaction signature.</param>
/// <param name="Gas">Gas limit declared on the transaction.</param>
/// <param name="GasPrice">Effective gas price field used by legacy transactions.</param>
/// <param name="MaxfeePerGas">EIP-1559 max fee cap per gas unit.</param>
/// <param name="MaxPriorityFeePerGas">EIP-1559 miner tip cap per gas unit.</param>
/// <param name="Input">Transaction calldata bytes.</param>
/// <param name="Nonce">Sender nonce used to order transactions.</param>
/// <param name="To">Recipient address; <see langword="null"/> for contract-creation transactions.</param>
/// <param name="TransactionIndex">Position of the transaction within its block, when mined.</param>
/// <param name="Value">Native value transferred by the transaction.</param>
/// <param name="Type">Typed transaction envelope identifier.</param>
/// <param name="ChainId">Chain id used for replay protection.</param>
/// <param name="V">Signature recovery id / parity component.</param>
/// <param name="R">Signature R component.</param>
/// <param name="S">Signature S component.</param>
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
