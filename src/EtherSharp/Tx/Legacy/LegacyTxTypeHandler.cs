using EtherSharp.Client.Services;
using EtherSharp.Client.Services.TxTypeHandler;
using EtherSharp.Common;
using EtherSharp.Crypto;
using EtherSharp.RLP;
using EtherSharp.RPC;
using EtherSharp.Wallet;
using System.Buffers;

namespace EtherSharp.Tx.Legacy;

public class LegacyTxTypeHandler(IEtherSigner signer, IRpcClient rpcClient)
    : IInitializableService, ITxTypeHandler<LegacyTransaction, LegacyTxParams, LegacyGasParams>
{
    private const int MAX_LEGACY_SIGNATURE_LENGTH = 10 + 32 + 32;

    private readonly IRpcClient _rpcClient = rpcClient;
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

    public string EncodeTxToBytes(ITxInput txInput, LegacyTxParams txParams, LegacyGasParams txGasParams, uint nonce, out string txHash)
    {
        if(!_isInitialized)
        {
            throw new InvalidOperationException("Not initialized");
        }

        var tx = LegacyTransaction.Create(_chainId, txParams, txGasParams, txInput, nonce);

        Span<int> lengthBuffer = stackalloc int[LegacyTransaction.NestedListCount];
        int signDataLength = tx.GetSignDataEncodedSize(lengthBuffer);
        int bufferLength = signDataLength + MAX_LEGACY_SIGNATURE_LENGTH;

        byte[]? rented = null;
        var buffer = bufferLength <= 4096
            ? stackalloc byte[bufferLength]
            : (rented = ArrayPool<byte>.Shared.Rent(bufferLength)).AsSpan(0, bufferLength);

        try
        {
            var signDataBuffer = buffer[0..signDataLength];

            tx.EncodeSignData(lengthBuffer, signDataBuffer);

            int txSizeWithoutSignature = tx.GetEncodedSize(lengthBuffer);
            int maxTxSize = 1 + txSizeWithoutSignature + MAX_LEGACY_SIGNATURE_LENGTH;

            var txBuffer = buffer[0..maxTxSize];

            var signatureBuffer = txBuffer[^MAX_LEGACY_SIGNATURE_LENGTH..];

            SignAndEncode(signDataBuffer, signatureBuffer, tx.ChainId, out int signatureLength);

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
            if(!Keccak256.TryHashData(txBuffer, txHashBuffer))
            {
                throw new InvalidOperationException("Failed to calculate tx hash");
            }

            txHash = HexUtils.ToPrefixedHexString(txHashBuffer);
            return HexUtils.ToPrefixedHexString(txBuffer);
        }
        finally
        {
            if(rented is not null)
            {
                ArrayPool<byte>.Shared.Return(rented);
            }
        }
    }

    private void SignAndEncode(ReadOnlySpan<byte> signDataBuffer, Span<byte> signatureBuffer, ulong chainId, out int encodedSignatureLength)
    {
        Span<byte> hashBuffer = stackalloc byte[32];
        _ = Keccak256.TryHashData(signDataBuffer, hashBuffer);

        Span<byte> rawSignatureBuffer = stackalloc byte[65];
        if(!_signer!.TrySignRecoverable(hashBuffer, rawSignatureBuffer))
        {
            throw new NotImplementedException();
        }

        byte v = rawSignatureBuffer[64] switch
        {
            0 => 0,
            1 => 1,
            27 => 0,
            28 => 1,
            _ => throw new NotSupportedException("Bad parity byte")
        };

        var r = rawSignatureBuffer[..32];
        var s = rawSignatureBuffer[32..64];

        ulong eip155V = (chainId * 2) + 35 + v;

        encodedSignatureLength = RLPEncoder.GetIntSize(eip155V) + RLPEncoder.GetStringSize(r) + RLPEncoder.GetStringSize(s);

        _ = new RLPEncoder(signatureBuffer)
            .EncodeInt(eip155V)
            .EncodeString(r)
            .EncodeString(s);
    }
}
