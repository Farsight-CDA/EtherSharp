using EtherSharp.Client.Services.GasFeeProvider;
using EtherSharp.Client.Services.RPC;
using EtherSharp.Types;
using EtherSharp.Wallet;
using System.Numerics;

namespace EtherSharp.Tx.EIP1559;
public class EIP1559GasFeeProvider(IRpcClient rpcClient, IEtherSigner signer) : IGasFeeProvider<EIP1559TxParams, EIP1559GasParams>
{
    private readonly IRpcClient _rpcClient = rpcClient;
    private readonly IEtherSigner _signer = signer;

    public Task<EIP1559GasParams> EstimateGasParamsAsync(
        Address to, BigInteger value, ReadOnlySpan<byte> inputData,
        EIP1559TxParams txParams, CancellationToken cancellationToken) 
        => SendEstimationRequestsAsync(
            to,
            value,
            $"0x{Convert.ToHexString(inputData)}",
            cancellationToken
        );

    private async Task<EIP1559GasParams> SendEstimationRequestsAsync(
        Address to, BigInteger value, string inputDataHex, CancellationToken cancellationToken)
    {
        ulong gasEstimation = await _rpcClient.EthEstimateGasAsync(
            _signer.Address.String, to.String, value, inputDataHex, cancellationToken);

        var gasPriceTask = _rpcClient.EthGasPriceAsync(cancellationToken);
        var priorityFeeTask = _rpcClient.EthMaxPriorityFeePerGas(cancellationToken);

        var gasPrice = await gasPriceTask;
        var priorityFee = await priorityFeeTask;

        return new EIP1559GasParams(
            gasEstimation,
            gasPrice * 12 / 10,
            priorityFee * 12 / 10
        );
    }
}
