using EtherSharp.Client.Services;
using EtherSharp.Client.Services.TxTypeHandler;
using EtherSharp.Common;
using EtherSharp.Crypto;
using EtherSharp.RLP;
using EtherSharp.RPC;
using EtherSharp.Types;
using EtherSharp.Wallet;
using System.Buffers;

namespace EtherSharp.Tx.EIP1559;

/// <summary>
/// Encodes and signs EIP-1559 transactions.
/// </summary>
/// <param name="signer">Signer used to produce recoverable transaction signatures.</param>
public sealed class EIP1559TxTypeHandler(IEtherSigner signer)
    : IInitializableService, ITxTypeHandler<EIP1559Transaction, EIP1559TxParams, EIP1559GasParams>
{
    private readonly IEtherSigner _signer = signer;

    private bool _isInitialized;
    private ulong _chainId;

    /// <inheritdoc/>
    public ValueTask InitializeAsync(ulong chainId, CancellationToken cancellationToken = default)
    {
        _chainId = chainId;
        _isInitialized = true;
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc/>
    public async ValueTask<SignedTransaction> EncodeTxAsync(
        ITxInput txInput,
        EIP1559TxParams txParams,
        EIP1559GasParams txGasParams,
        uint nonce,
        CancellationToken cancellationToken = default)
    {
        if(!_isInitialized)
        {
            throw new InvalidOperationException("Not initialized");
        }

        var tx = EIP1559Transaction.Create(_chainId, txParams, txGasParams, txInput, nonce);
        int[] lengthBuffer = ArrayPool<int>.Shared.Rent(EIP1559Transaction.NestedListCount);
        byte[]? rented = null;

        try
        {
            int txTemplateLength = tx.GetEncodedSize(lengthBuffer);
            int txBufferLength = 2 + txTemplateLength + TxRLPEncoder.MaxEncodedSignatureLength;

            rented = ArrayPool<byte>.Shared.Rent(txBufferLength);
            var txBuffer = rented.AsSpan(0, txBufferLength);
            var txTemplateBuffer = txBuffer[1..(txTemplateLength + 2)];

            tx.Encode(lengthBuffer, txTemplateBuffer[1..]);
            txTemplateBuffer[0] = EIP1559Transaction.PrefixByte;

            var signingHash = Keccak256.HashData(txTemplateBuffer);
            var signature = await _signer.SignRecoverableAsync(signingHash, cancellationToken).ConfigureAwait(false);

            txBuffer = rented.AsSpan(0, txBufferLength);
            var signatureBuffer = txBuffer[^TxRLPEncoder.MaxEncodedSignatureLength..];
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
                txBuffer[0] = EIP1559Transaction.PrefixByte;
            }

            _ = new RLPEncoder(txBuffer[1..]).EncodeList(lengthBuffer[0] + signatureLength);

            var signedTxBuffer = txBuffer[..^(TxRLPEncoder.MaxEncodedSignatureLength - signatureLength)];

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
        out int encodedSignatureLength)
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
            rawSignatureBuffer[..64], parityByte, out encodedSignatureLength);
    }
}
