using EtherSharp.Common;
using EtherSharp.Crypto;
using EtherSharp.Numerics;
using EtherSharp.RLP;
using EtherSharp.Tx.Types;
using EtherSharp.Types;
using EtherSharp.Wallet;
using System.Buffers;

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
public sealed record EIP1559Transaction(
    ulong ChainId,
    ulong Gas,
    ulong Nonce,
    ITxInput Input,
    UInt256 MaxFeePerGas,
    UInt256 MaxPriorityFeePerGas,
    StateAccess[] AccessList
) : ITransaction<EIP1559Transaction, EIP1559TxParams, EIP1559GasParams>
{
    private static readonly int _maxSignatureLength = TxRLPEncoder.GetMaxEncodedSignatureLength(1);

    /// <summary>
    /// Gets the number of nested RLP lists required to encode this transaction.
    /// </summary>
    public static int NestedListCount => 2;

    /// <summary>
    /// Gets the EIP-2718 typed transaction prefix for EIP-1559 transactions.
    /// </summary>
    public static byte PrefixByte => 0x02;

    /// <inheritdoc/>
    public static EIP1559Transaction Create(
        ulong chainId, ulong nonce, ITxInput input, EIP1559TxParams txParams, EIP1559GasParams gasParams
    )
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(txParams);
        ArgumentNullException.ThrowIfNull(gasParams);

        return new EIP1559Transaction(
            chainId, gasParams.GasLimit, nonce, input, gasParams.MaxFeePerGas, gasParams.MaxPriorityFeePerGas, txParams.AccessList
        );
    }

    /// <inheritdoc/>
    public EIP1559Transaction WithGasParams(EIP1559GasParams gasParams)
    {
        ArgumentNullException.ThrowIfNull(gasParams);

        return this with
        {
            Gas = gasParams.GasLimit,
            MaxFeePerGas = gasParams.MaxFeePerGas,
            MaxPriorityFeePerGas = gasParams.MaxPriorityFeePerGas
        };
    }

    /// <inheritdoc/>
    public EIP1559Transaction WithNonce(ulong nonce)
        => this with { Nonce = nonce };

    /// <inheritdoc/>
    public EIP1559Transaction IncrementGasFeeByFactor(UInt256 multiplier, UInt256 divider, UInt256 minimumIncrement)
        => multiplier < divider
            ? throw new ArgumentException("Multiplier must be larger than divider")
            : this with
            {
                MaxFeePerGas = UInt256.Max(MaxFeePerGas + minimumIncrement, MaxFeePerGas * multiplier / divider),
                MaxPriorityFeePerGas = UInt256.Max(MaxPriorityFeePerGas + minimumIncrement, MaxPriorityFeePerGas * multiplier / divider)
            };

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
            TxRLPEncoder.GetAddressStringSize(Input.To) +
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
    {
        var encoder = new RLPEncoder(destination)
            .EncodeList(listLengths[0])
            .EncodeInt(ChainId)
            .EncodeInt(Nonce)
            .EncodeInt(MaxPriorityFeePerGas)
            .EncodeInt(MaxFeePerGas)
            .EncodeInt(Gas);

        encoder = Input.To is { } to
            ? encoder.EncodeAddress(to)
            : encoder.EncodeString();

        encoder
            .EncodeInt(Input.Value)
            .EncodeString(Input.Data.Span)
            .EncodeList(listLengths[1])
            .EncodeAccessList(AccessList);
    }

    async ValueTask<SignedTransaction> ITransaction.SignAsync(IEtherSigner signer, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(signer);

        int[] lengthBuffer = ArrayPool<int>.Shared.Rent(NestedListCount);
        byte[]? rented = null;

        try
        {
            int txTemplateLength = GetEncodedSize(lengthBuffer);
            int txBufferLength = 2 + txTemplateLength + _maxSignatureLength;

            rented = ArrayPool<byte>.Shared.Rent(txBufferLength);
            var txBuffer = rented.AsSpan(0, txBufferLength);
            var txTemplateBuffer = txBuffer[1..(txTemplateLength + 2)];

            Encode(lengthBuffer, txTemplateBuffer[1..]);
            txTemplateBuffer[0] = PrefixByte;

            var signingHash = Keccak256.HashData(txTemplateBuffer);
            var signature = await signer.SignRecoverableAsync(signingHash, cancellationToken).ConfigureAwait(false);

            txBuffer = rented.AsSpan(0, txBufferLength);
            var signatureBuffer = txBuffer[^_maxSignatureLength..];
            EncodeSignature(signature, signatureBuffer, out int signatureLength);

            int oldLengthBytes = RLPEncoder.GetPrefixLength(lengthBuffer[0]);
            int newLengthBytes = RLPEncoder.GetPrefixLength(lengthBuffer[0] + signatureLength);

            if(newLengthBytes == oldLengthBytes)
            {
                //Dont need the extra byte for the length increase
                txBuffer = txBuffer[1..];
            }
            else
            {
                txBuffer[0] = PrefixByte;
            }

            _ = new RLPEncoder(txBuffer[1..]).EncodeList(lengthBuffer[0] + signatureLength);

            var signedTxBuffer = txBuffer[..^(_maxSignatureLength - signatureLength)];

            Span<byte> txHashBuffer = stackalloc byte[32];
            return !Keccak256.TryHashData(signedTxBuffer, txHashBuffer)
                ? throw new InvalidOperationException("Failed to calculate tx hash")
                : new SignedTransaction(
                    HexUtils.ToPrefixedHexString(signedTxBuffer),
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
            rawSignatureBuffer[..64], parityByte, out encodedSignatureLength
        );
    }
}
