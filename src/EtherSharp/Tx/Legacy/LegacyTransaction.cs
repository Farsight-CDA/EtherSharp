using EtherSharp.Numerics;
using EtherSharp.RLP;
using EtherSharp.Tx.Types;

namespace EtherSharp.Tx.Legacy;

/// <summary>
/// Represents an unsigned legacy (type-0) Ethereum transaction payload.
/// </summary>
/// <param name="ChainId">Target chain identifier used for replay protection.</param>
/// <param name="Gas">Gas limit to attach to the transaction.</param>
/// <param name="Nonce">Sender account nonce.</param>
/// <param name="Input">Transaction destination, value, and calldata.</param>
/// <param name="GasPrice">Gas price to pay for each gas unit.</param>
public record LegacyTransaction(
    ulong ChainId,
    ulong Gas,
    uint Nonce,
    ITxInput Input,
    UInt256 GasPrice
) : ITransaction<LegacyTransaction, LegacyTxParams, LegacyGasParams>
{
    /// <summary>
    /// Gets the number of nested RLP lists required to encode this transaction.
    /// </summary>
    public static int NestedListCount => 1;

    /// <summary>
    /// Creates a legacy transaction from gas parameters and transaction input.
    /// </summary>
    /// <param name="chainId">Target chain identifier.</param>
    /// <param name="gasParams">Gas limit and gas price settings.</param>
    /// <param name="txInput">Destination, calldata, and value.</param>
    /// <param name="nonce">Sender nonce.</param>
    /// <returns>A transaction value built from the provided parameters.</returns>
    public static LegacyTransaction Create(ulong chainId, LegacyGasParams gasParams, ITxInput txInput, uint nonce)
        => new LegacyTransaction(
            chainId,
            gasParams.GasLimit,
            nonce,
            txInput,
            gasParams.GasPrice
        );

    /// <summary>
    /// Computes the RLP-encoded size of signable payload data (including EIP-155 chain id fields).
    /// </summary>
    /// <param name="listLengths">Buffer used to store list content length.</param>
    /// <returns>Total RLP-encoded byte count of the signable payload.</returns>
    public int GetSignDataEncodedSize(Span<int> listLengths)
    {
        int contentSize =
            RLPEncoder.GetIntSize(Nonce) +
            RLPEncoder.GetIntSize(GasPrice) +
            RLPEncoder.GetIntSize(Gas) +
            RLPEncoder.GetStringSize(Input.To is null ? [] : Input.To.Bytes) +
            RLPEncoder.GetIntSize(Input.Value) +
            RLPEncoder.GetStringSize(Input.Data.Span) +
            RLPEncoder.GetIntSize(ChainId) +
            (2 * RLPEncoder.GetIntSize(0));

        listLengths[0] = contentSize;

        return RLPEncoder.GetListSize(contentSize);
    }

    /// <summary>
    /// Computes the RLP-encoded size of the unsigned legacy payload fields.
    /// </summary>
    /// <param name="listLengths">Buffer used to store list content length.</param>
    /// <returns>Total RLP-encoded byte count of the unsigned payload.</returns>
    public int GetEncodedSize(Span<int> listLengths)
    {
        int contentSize =
            RLPEncoder.GetIntSize(Nonce) +
            RLPEncoder.GetIntSize(GasPrice) +
            RLPEncoder.GetIntSize(Gas) +
            RLPEncoder.GetStringSize(Input.To is null ? [] : Input.To.Bytes) +
            RLPEncoder.GetIntSize(Input.Value) +
            RLPEncoder.GetStringSize(Input.Data.Span);

        listLengths[0] = contentSize;

        return RLPEncoder.GetListSize(contentSize);
    }

    /// <summary>
    /// Writes the signable RLP payload including chain id and empty signature fields.
    /// </summary>
    /// <param name="listLengths">Precomputed list content lengths from <see cref="GetSignDataEncodedSize"/>.</param>
    /// <param name="destination">Destination span to receive encoded bytes.</param>
    public void EncodeSignData(ReadOnlySpan<int> listLengths, Span<byte> destination)
        => new RLPEncoder(destination)
            .EncodeList(listLengths[0])
                .EncodeInt(Nonce)
                .EncodeInt(GasPrice)
                .EncodeInt(Gas)
                .EncodeString(Input.To is null ? [] : Input.To.Bytes)
                .EncodeInt(Input.Value)
                .EncodeString(Input.Data.Span)
                .EncodeInt(ChainId)
                .EncodeInt(0)
                .EncodeInt(0);

    /// <summary>
    /// Writes the unsigned legacy payload fields into an RLP list sized for an appended signature.
    /// </summary>
    /// <param name="listLengths">Precomputed list content lengths from <see cref="GetEncodedSize"/>.</param>
    /// <param name="destination">Destination span to receive encoded bytes.</param>
    /// <param name="signatureLength">Encoded length of the signature fields that will be appended.</param>
    public void Encode(ReadOnlySpan<int> listLengths, Span<byte> destination, int signatureLength)
        => new RLPEncoder(destination)
            .EncodeList(listLengths[0] + signatureLength)
                .EncodeInt(Nonce)
                .EncodeInt(GasPrice)
                .EncodeInt(Gas)
                .EncodeString(Input.To is null ? [] : Input.To.Bytes)
                .EncodeInt(Input.Value)
                .EncodeString(Input.Data.Span);
}
