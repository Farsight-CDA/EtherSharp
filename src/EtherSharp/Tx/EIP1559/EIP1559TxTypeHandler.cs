using EtherSharp.Client.Services;
using EtherSharp.Client.Services.RPC;
using EtherSharp.Client.Services.TxTypeHandler;
using EtherSharp.Crypto;
using EtherSharp.RLP;
using EtherSharp.Wallet;

namespace EtherSharp.Tx.EIP1559;
public sealed class EIP1559TxTypeHandler(IEtherSigner signer, IRpcClient rpcClient) 
    : BaseTxTypeHandler<EIP1559Transaction, EIP1559TxParams, EIP1559GasParams>(signer), IInitializableService
{
    private readonly IRpcClient _rpcClient = rpcClient;

    protected override async Task<EIP1559GasParams> CalculateGasParamsAsync(
        ITxInput txInput, EIP1559TxParams txParams, 
        string inputDataHex, CancellationToken cancellationToken)
    {
        ulong gasEstimation = await _rpcClient.EthEstimateGasAsync(
            _signer.Address.String, txInput.To.String, txInput.Value, inputDataHex, cancellationToken);

        var gasPriceTask = _rpcClient.EthGasPriceAsync(cancellationToken);
        var priorityFeeTask = _rpcClient.EthMaxPriorityFeePerGas(cancellationToken);

        var gasPrice = await gasPriceTask;
        var priorityFee = await priorityFeeTask;

        return new EIP1559GasParams(
            gasEstimation,
            gasPrice,
            priorityFee
        );
    }

    protected override string EncodeTxToBytes(
        ITxInput txInput, EIP1559TxParams txParams, EIP1559GasParams txGasParams, 
        ReadOnlySpan<byte> inputData, uint nonce)
    {
        var tx = EIP1559Transaction.Create(_chainId, txParams, txGasParams, txInput, nonce);

        Span<int> lengthBuffer = stackalloc int[EIP1559Transaction.NestedListCount];

        int txTemplateLength = tx.GetEncodedSize(inputData, lengthBuffer);

        int txBufferLength = 2 + txTemplateLength + TxRLPEncoder.MaxEncodedSignatureLength;

        Span<byte> txBuffer = txBufferLength > 4096
            ? new byte[txBufferLength]
            : stackalloc byte[txBufferLength];

        var txTemplateBuffer = txBuffer[1..(txTemplateLength + 2)];
        var signatureBuffer = txBuffer[^TxRLPEncoder.MaxEncodedSignatureLength..];

        tx.Encode(lengthBuffer, inputData, txTemplateBuffer[1..]);
        txTemplateBuffer[0] = EIP1559Transaction.PrefixByte;

        SignAndEncode(txTemplateBuffer, signatureBuffer, out int signatureLength);

        int oldLengthBytes = RLPEncoder.GetPrefixLength(lengthBuffer[0]);
        int newLengthBytes = RLPEncoder.GetPrefixLength(lengthBuffer[0] + signatureLength);

        if(newLengthBytes == oldLengthBytes)
        {
            //Dont need the extra byte for the length increase
            txBuffer = txBuffer[1..];
        }

        txBuffer[0] = EIP1559Transaction.PrefixByte;
        _ = new RLPEncoder(txBuffer[1..]).EncodeList(lengthBuffer[0] + signatureLength);

        var signedTxBuffer = txBuffer[..^(TxRLPEncoder.MaxEncodedSignatureLength - signatureLength)];

        return $"0x{Convert.ToHexString(signedTxBuffer)}";
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
