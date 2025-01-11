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

    public int FeeHistoryRange { get; set; } = 5;
    public double PriorityFeePercentile { get; set; } = 25;
    public int BaseFeeOffsetPercentage { get; set; } = 20;
    public int PriorityFeeOffsetPercentage { get; set; } = 20;

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

        var feeHistory = await _rpcClient.EthGetFeeHistory(FeeHistoryRange, TargetBlockNumber.Latest, [PriorityFeePercentile], default);

        BigInteger baseFee;
        BigInteger priorityFee;

        if (feeHistory.BaseFeePerGas.Length == 0)
        {
            baseFee = await _rpcClient.EthGasPriceAsync(cancellationToken);
        }
        else
        {
            var summedBaseFees = feeHistory.BaseFeePerGas.Aggregate(BigInteger.Zero, (prev, curr) => prev + curr);
            baseFee = summedBaseFees / feeHistory.BaseFeePerGas.Length; 
        }

        if (feeHistory.Reward.Length == 0)
        {
            priorityFee = await _rpcClient.EthMaxPriorityFeePerGas(cancellationToken);
        }
        else
        {
            var summedPriorityFees = feeHistory.Reward.Aggregate(BigInteger.Zero, (prev, curr) => prev + curr[0]);
            priorityFee = summedPriorityFees / feeHistory.Reward.Length;
        }

        return new EIP1559GasParams(
            gasEstimation,
            baseFee * (100 + BaseFeeOffsetPercentage) / 100,
            priorityFee * (100 + PriorityFeeOffsetPercentage) / 100
        );
    }
}
