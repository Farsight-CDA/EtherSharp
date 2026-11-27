using EtherSharp.Common;
using EtherSharp.Crypto;
using EtherSharp.Numerics;
using EtherSharp.RLP;
using EtherSharp.Tx.Types;
using EtherSharp.Types;
using EtherSharp.Wallet;
using System.Buffers;

namespace EtherSharp.Tx.Legacy;

/// <summary>
/// Represents an unsigned legacy (type-0) Ethereum transaction payload.
/// </summary>
/// <param name="ChainId">Target chain identifier used for replay protection.</param>
/// <param name="Gas">Gas limit to attach to the transaction.</param>
/// <param name="Nonce">Sender account nonce.</param>
/// <param name="Input">Transaction destination, value, and calldata.</param>
/// <param name="GasPrice">Gas price to pay for each gas unit.</param>
public sealed record LegacyTransaction(
    ulong ChainId,
    ulong Gas,
    ulong Nonce,
    ITxInput Input,
    UInt256 GasPrice
) : ITransaction<LegacyTransaction, LegacyTxParams, LegacyGasParams>
{
    /// <summary>
    /// Gets the number of nested RLP lists required to encode this transaction.
    /// </summary>
    public static int NestedListCount => 1;

    /// <inheritdoc/>
    public static LegacyTransaction Create(
        ulong chainId, ulong nonce, ITxInput input, LegacyTxParams txParams, LegacyGasParams gasParams
    )
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(txParams);
        ArgumentNullException.ThrowIfNull(gasParams);

        return new LegacyTransaction(chainId, gasParams.GasLimit, nonce, input, gasParams.GasPrice);
    }

    /// <inheritdoc/>
    public LegacyTransaction WithGasParams(LegacyGasParams gasParams)
    {
        ArgumentNullException.ThrowIfNull(gasParams);

        return this with
        {
            Gas = gasParams.GasLimit,
            GasPrice = gasParams.GasPrice
        };
    }

    /// <inheritdoc/>
    public LegacyTransaction WithNonce(ulong nonce)
        => this with { Nonce = nonce };

    /// <inheritdoc/>
    public LegacyTransaction IncrementGasFeeByFactor(UInt256 multiplier, UInt256 divider, UInt256 minimumIncrement)
        => multiplier < divider
            ? throw new ArgumentException("Multiplier must be larger than divider")
            : this with
            {
                GasPrice = UInt256.Max(GasPrice + minimumIncrement, GasPrice * multiplier / divider)
            };

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
            TxRLPEncoder.GetAddressStringSize(Input.To) +
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
            TxRLPEncoder.GetAddressStringSize(Input.To) +
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
    {
        var encoder = new RLPEncoder(destination)
            .EncodeList(listLengths[0])
            .EncodeInt(Nonce)
            .EncodeInt(GasPrice)
            .EncodeInt(Gas);

        encoder = Input.To is { } to
            ? encoder.EncodeAddress(to)
            : encoder.EncodeString();

        encoder
            .EncodeInt(Input.Value)
            .EncodeString(Input.Data.Span)
            .EncodeInt(ChainId)
            .EncodeInt(0)
            .EncodeInt(0);
    }

    /// <summary>
    /// Writes the unsigned legacy payload fields into an RLP list sized for an appended signature.
    /// </summary>
    /// <param name="listLengths">Precomputed list content lengths from <see cref="GetEncodedSize"/>.</param>
    /// <param name="destination">Destination span to receive encoded bytes.</param>
    /// <param name="signatureLength">Encoded length of the signature fields that will be appended.</param>
    public void Encode(ReadOnlySpan<int> listLengths, Span<byte> destination, int signatureLength)
    {
        var encoder = new RLPEncoder(destination)
            .EncodeList(listLengths[0] + signatureLength)
            .EncodeInt(Nonce)
            .EncodeInt(GasPrice)
            .EncodeInt(Gas);

        encoder = Input.To is { } to
            ? encoder.EncodeAddress(to)
            : encoder.EncodeString();

        encoder
            .EncodeInt(Input.Value)
            .EncodeString(Input.Data.Span);
    }

    async ValueTask<SignedTransaction> ITransaction.SignAsync(IEtherSigner signer, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(signer);

        ulong eip155VBase = checked((ChainId * 2) + 35);
        int maxSignatureLength = TxRLPEncoder.GetMaxEncodedSignatureLength(checked(eip155VBase + 1));

        int[] lengthBuffer = ArrayPool<int>.Shared.Rent(NestedListCount);
        byte[]? rented = null;

        try
        {
            int signDataLength = GetSignDataEncodedSize(lengthBuffer);
            int bufferLength = signDataLength + maxSignatureLength;

            rented = ArrayPool<byte>.Shared.Rent(bufferLength);
            var signDataBuffer = rented.AsSpan(0, signDataLength);

            EncodeSignData(lengthBuffer, signDataBuffer);

            int txSizeWithoutSignature = GetEncodedSize(lengthBuffer);
            int maxTxSize = 1 + txSizeWithoutSignature + maxSignatureLength;

            var signingHash = Keccak256.HashData(signDataBuffer);
            var signature = await signer.SignRecoverableAsync(signingHash, cancellationToken).ConfigureAwait(false);

            var txBuffer = rented.AsSpan(0, maxTxSize);
            var signatureBuffer = txBuffer[^maxSignatureLength..];
            EncodeSignature(eip155VBase, signature, signatureBuffer, out int signatureLength);

            if(signatureLength < maxSignatureLength)
            {
                txBuffer = txBuffer[..^(maxSignatureLength - signatureLength)];
            }

            int oldLengthBytes = RLPEncoder.GetPrefixLength(lengthBuffer[0]);
            int newLengthBytes = RLPEncoder.GetPrefixLength(lengthBuffer[0] + signatureLength);

            if(newLengthBytes == oldLengthBytes)
            {
                //Dont need the extra byte for the length increase
                txBuffer = txBuffer[1..];
            }

            Encode(lengthBuffer, txBuffer, signatureLength);

            Span<byte> txHashBuffer = stackalloc byte[32];
            return !Keccak256.TryHashData(txBuffer, txHashBuffer)
                ? throw new InvalidOperationException("Failed to calculate tx hash")
                : new SignedTransaction(
                    HexUtils.ToPrefixedHexString(txBuffer),
                    Bytes32.FromBytes(txHashBuffer)
                );
        }
        finally
        {
            if(rented is not null)
            {
                ArrayPool<byte>.Shared.Return(rented);
            }

            ArrayPool<int>.Shared.Return(lengthBuffer);
        }
    }

    private static void EncodeSignature(
        ulong eip155VBase,
        in RecoverableEtherSignature signature,
        Span<byte> signatureBuffer,
        out int encodedSignatureLength
    )
    {
        Span<byte> rawSignatureBuffer = stackalloc byte[65];
        signature.CopyTo(rawSignatureBuffer);

        ulong parityByte = rawSignatureBuffer[64] switch
        {
            0 => 0,
            1 => 1,
            27 => 0,
            28 => 1,
            _ => throw new NotSupportedException("Bad parity byte")
        };

        _ = new RLPEncoder(signatureBuffer).EncodeSignature(
            rawSignatureBuffer[..64], eip155VBase + parityByte, out encodedSignatureLength
        );
    }
}
