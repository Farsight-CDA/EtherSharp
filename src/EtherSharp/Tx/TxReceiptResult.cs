using EtherSharp.Types;
using System.Diagnostics.CodeAnalysis;

namespace EtherSharp.Tx;

/// <summary>
/// Outcome of waiting for a transaction's inclusion and receipt via <see cref="IPendingTx{TTxParams, TTxGasParams}"/>.
/// </summary>
public closed record TxReceiptResult
{
    /// <summary>
    /// One of this transaction's submissions was included on-chain and its receipt was retrieved.
    /// </summary>
    /// <param name="Receipt">The confirmed transaction receipt.</param>
    public sealed record Confirmed(TxReceipt Receipt) : TxReceiptResult;

    /// <summary>
    /// A different transaction consumed this transaction's nonce slot.
    /// </summary>
    /// <param name="TransactionHash">Hash of the transaction that consumed the nonce.</param>
    /// <param name="BlockNumber">Block number in which the replacing transaction was included.</param>
    /// <param name="BlockHash">Block hash in which the replacing transaction was included.</param>
    public sealed record Replaced(Bytes32 TransactionHash, ulong BlockNumber, Bytes32 BlockHash) : TxReceiptResult;

    /// <summary>
    /// The nonce was already consumed on-chain and the including transaction could not be located.
    /// </summary>
    public sealed record NonceAlreadyUsed() : TxReceiptResult;

    /// <summary>
    /// Returns the receipt for a confirmed transaction result.
    /// </summary>
    /// <returns>The confirmed transaction receipt.</returns>
    /// <exception cref="InvalidOperationException">Thrown when this result is not <see cref="Confirmed"/>.</exception>
    public TxReceipt Unwrap()
        => this switch
        {
            Confirmed confirmed => confirmed.Receipt,
            Replaced replaced => throw new InvalidOperationException(
                $"Transaction was replaced by {replaced.TransactionHash} in block {replaced.BlockNumber}"
            ),
            NonceAlreadyUsed => throw new InvalidOperationException(
                "Transaction nonce was already used on-chain"
            ),
        };
}
