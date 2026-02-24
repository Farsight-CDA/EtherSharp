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
/// <param name="rpcClient">RPC client used by the tx handler lifecycle.</param>
public sealed class EIP1559TxTypeHandler(IEtherSigner signer, IRpcClient rpcClient)
    : IInitializableService, ITxTypeHandler<EIP1559Transaction, EIP1559TxParams, EIP1559GasParams>
{
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

    /// <inheritdoc/>
    string ITxTypeHandler<EIP1559Transaction, EIP1559TxParams, EIP1559GasParams>.EncodeTxToBytes(
        ITxInput txInput, EIP1559TxParams txParams, EIP1559GasParams txGasParams, uint nonce, out Hash32 txHash)
    {
        if(!_isInitialized)
        {
            throw new InvalidOperationException("Not initialized");
        }

        var tx = EIP1559Transaction.Create(_chainId, txParams, txGasParams, txInput, nonce);

        Span<int> lengthBuffer = stackalloc int[EIP1559Transaction.NestedListCount];
        int txTemplateLength = tx.GetEncodedSize(lengthBuffer);
        int txBufferLength = 2 + txTemplateLength + TxRLPEncoder.MaxEncodedSignatureLength;

        byte[]? rented = null;
        var txBuffer = txBufferLength <= 4096
            ? stackalloc byte[txBufferLength]
            : (rented = ArrayPool<byte>.Shared.Rent(txBufferLength)).AsSpan(0, txBufferLength);

        try
        {
            var txTemplateBuffer = txBuffer[1..(txTemplateLength + 2)];
            var signatureBuffer = txBuffer[^TxRLPEncoder.MaxEncodedSignatureLength..];

            tx.Encode(lengthBuffer, txTemplateBuffer[1..]);
            txTemplateBuffer[0] = EIP1559Transaction.PrefixByte;

            SignAndEncode(txTemplateBuffer, signatureBuffer, out int signatureLength);

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
            if(!Keccak256.TryHashData(signedTxBuffer, txHashBuffer))
            {
                throw new InvalidOperationException("Failed to calculate tx hash");
            }

            txHash = Hash32.FromBytes(txHashBuffer);
            return HexUtils.ToPrefixedHexString(signedTxBuffer);
        }
        finally
        {
            if(rented is not null)
            {
                ArrayPool<byte>.Shared.Return(rented);
            }
        }
    }

    private void SignAndEncode(Span<byte> txTemplateBuffer, Span<byte> signatureBuffer, out int encodedSignatureLength)
    {
        Span<byte> hashBuffer = stackalloc byte[32];
        _ = Keccak256.TryHashData(txTemplateBuffer, hashBuffer);

        Span<byte> rawSignatureBuffer = stackalloc byte[65];
        if(!_signer!.TrySignRecoverable(hashBuffer, rawSignatureBuffer))
        {
            throw new NotImplementedException();
        }

        _ = new RLPEncoder(signatureBuffer).EncodeSignature(rawSignatureBuffer, out encodedSignatureLength);
    }
}
