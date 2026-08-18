using EtherSharp.Client.Services;
using EtherSharp.Client.Services.TxTypeHandler;
using EtherSharp.Common;
using EtherSharp.Crypto;
using EtherSharp.RLP;
using EtherSharp.RPC;
using EtherSharp.RPC.Transport;
using EtherSharp.Types;
using EtherSharp.Wallet;
using System.Buffers;

namespace EtherSharp.Tx.Legacy;

/// <summary>
/// Encodes and signs legacy Ethereum transactions.
/// </summary>
/// <param name="signer">Signer used to produce recoverable transaction signatures.</param>
public sealed class LegacyTxTypeHandler(IEtherSigner signer)
    : IInitializableService, ITxTypeHandler<LegacyTransaction, LegacyTxParams, LegacyGasParams>
{
    private const int MAX_LEGACY_SIGNATURE_LENGTH = 10 + 32 + 32;

    private readonly IEtherSigner _signer = signer;

    private bool _isInitialized;
    private ulong _chainId;

    /// <inheritdoc/>
    public ValueTask InitializeAsync(
        ulong chainId, RpcRequestOptions _ = default, CancellationToken cancellationToken = default)
    {
        _chainId = chainId;
        _isInitialized = true;
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc/>
    public async ValueTask<SignedTransaction> EncodeTxAsync(
        ITxInput txInput,
        LegacyTxParams txParams,
        LegacyGasParams txGasParams,
        uint nonce,
        CancellationToken cancellationToken = default)
    {
        if(!_isInitialized)
        {
            throw new InvalidOperationException("Not initialized");
        }

        var tx = LegacyTransaction.Create(_chainId, txGasParams, txInput, nonce);
        int[] lengthBuffer = ArrayPool<int>.Shared.Rent(LegacyTransaction.NestedListCount);
        byte[]? rented = null;

        try
        {
            int signDataLength = tx.GetSignDataEncodedSize(lengthBuffer);
            int bufferLength = signDataLength + MAX_LEGACY_SIGNATURE_LENGTH;

            rented = ArrayPool<byte>.Shared.Rent(bufferLength);
            var signDataBuffer = rented.AsSpan(0, signDataLength);

            tx.EncodeSignData(lengthBuffer, signDataBuffer);

            int txSizeWithoutSignature = tx.GetEncodedSize(lengthBuffer);
            int maxTxSize = 1 + txSizeWithoutSignature + MAX_LEGACY_SIGNATURE_LENGTH;

            var signingHash = Keccak256.HashData(signDataBuffer);
            var signature = await _signer.SignRecoverableAsync(signingHash, cancellationToken).ConfigureAwait(false);

            var txBuffer = rented.AsSpan(0, maxTxSize);
            var signatureBuffer = txBuffer[^MAX_LEGACY_SIGNATURE_LENGTH..];
            EncodeSignature(signature, signatureBuffer, out int signatureLength);

            if(signatureLength < MAX_LEGACY_SIGNATURE_LENGTH)
            {
                txBuffer = txBuffer[..^(MAX_LEGACY_SIGNATURE_LENGTH - signatureLength)];
            }

            int oldLengthBytes = RLPEncoder.GetPrefixLength(lengthBuffer[0]);
            int newLengthBytes = RLPEncoder.GetPrefixLength(lengthBuffer[0] + signatureLength);

            if(newLengthBytes == oldLengthBytes)
            {
                //Dont need the extra byte for the length increase
                txBuffer = txBuffer[1..];
            }

            tx.Encode(lengthBuffer, txBuffer, signatureLength);

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

    private void EncodeSignature(
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
        ulong eip155V = (_chainId * 2) + 35 + parityByte;

        _ = new RLPEncoder(signatureBuffer).EncodeSignature(
            rawSignatureBuffer[..64], eip155V, out encodedSignatureLength);
    }
}
