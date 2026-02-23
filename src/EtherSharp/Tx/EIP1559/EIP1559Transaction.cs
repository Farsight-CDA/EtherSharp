using EtherSharp.Numerics;
using EtherSharp.RLP;
using EtherSharp.Tx.Types;

namespace EtherSharp.Tx.EIP1559;

/// <summary>
/// Represents an unsigned EIP-1559 transaction payload that can be RLP-encoded for signing and submission.
/// </summary>
/// <param name="ChainId">Target chain identifier used for replay protection.</param>
/// <param name="Gas">Gas limit to attach to the transaction.</param>
/// <param name="Nonce">Sender account nonce.</param>
/// <param name="Input">Transaction destination, value, and calldata.</param>
/// <param name="MaxFeePerGas">Maximum total fee per gas unit.</param>
/// <param name="MaxPriorityFeePerGas">Maximum miner tip per gas unit.</param>
/// <param name="AccessList">Optional state access list (EIP-2930 style).</param>
public record EIP1559Transaction(
    ulong ChainId,
    ulong Gas,
    uint Nonce,
    ITxInput Input,
    UInt256 MaxFeePerGas,
    UInt256 MaxPriorityFeePerGas,
    StateAccess[] AccessList
) : ITransaction<EIP1559Transaction, EIP1559TxParams, EIP1559GasParams>
{
    /// <summary>
    /// Gets the number of nested RLP lists required to encode this transaction.
    /// </summary>
    public static int NestedListCount => 2;

    /// <summary>
    /// Gets the EIP-2718 typed transaction prefix for EIP-1559 transactions.
    /// </summary>
    public static byte PrefixByte => 0x02;

    /// <summary>
    /// Creates an EIP-1559 transaction from tx and gas parameter objects.
    /// </summary>
    /// <param name="chainId">Target chain identifier.</param>
    /// <param name="txParams">Transaction-level parameters such as access list.</param>
    /// <param name="gasParams">Gas and fee parameters.</param>
    /// <param name="txInput">Destination, calldata, and value.</param>
    /// <param name="nonce">Sender nonce.</param>
    /// <returns>A transaction value built from the provided parameters.</returns>
    public static EIP1559Transaction Create(ulong chainId, EIP1559TxParams txParams, EIP1559GasParams gasParams, ITxInput txInput, uint nonce)
        => new EIP1559Transaction(
            chainId,
            gasParams.GasLimit,
            nonce,
            txInput,
            gasParams.MaxFeePerGas,
            gasParams.MaxPriorityFeePerGas,
            txParams.AccessList
        );

    /// <summary>
    /// Computes the RLP-encoded transaction size and stores nested list lengths.
    /// </summary>
    /// <param name="listLengths">Buffer used to store outer and nested list content lengths.</param>
    /// <returns>Total RLP-encoded byte count of the unsigned payload.</returns>
    public int GetEncodedSize(Span<int> listLengths)
    {
        listLengths[1] = TxRLPEncoder.GetAccessListLength(AccessList);

        int contentSize =
            RLPEncoder.GetIntSize(ChainId) +
            RLPEncoder.GetIntSize(Nonce) +
            RLPEncoder.GetIntSize(MaxPriorityFeePerGas) +
            RLPEncoder.GetIntSize(MaxFeePerGas) +
            RLPEncoder.GetIntSize(Gas) +
            RLPEncoder.GetStringSize(Input.To is null ? [] : Input.To.Bytes) +
            RLPEncoder.GetIntSize(Input.Value) +
            RLPEncoder.GetStringSize(Input.Data.Span) +
            RLPEncoder.GetListSize(listLengths[1]);

        listLengths[0] = contentSize;

        return RLPEncoder.GetListSize(contentSize);
    }

    /// <summary>
    /// Writes the RLP-encoded unsigned transaction payload into the destination buffer.
    /// </summary>
    /// <param name="listLengths">Precomputed list content lengths from <see cref="GetEncodedSize"/>.</param>
    /// <param name="destination">Destination span to receive encoded bytes.</param>
    public void Encode(ReadOnlySpan<int> listLengths, Span<byte> destination)
        => new RLPEncoder(destination)
            .EncodeList(listLengths[0])
                .EncodeInt(ChainId)
                .EncodeInt(Nonce)
                .EncodeInt(MaxPriorityFeePerGas)
                .EncodeInt(MaxFeePerGas)
                .EncodeInt(Gas)
                .EncodeString(Input.To is null ? [] : Input.To.Bytes)
                .EncodeInt(Input.Value)
                .EncodeString(Input.Data.Span)
                .EncodeList(listLengths[1])
                    .EncodeAccessList(AccessList);
}
