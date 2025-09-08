using EtherSharp.Client.Services.GasFeeProvider;
using EtherSharp.RPC;
using EtherSharp.RPC.Modules.Eth;
using EtherSharp.Types;
using EtherSharp.Wallet;
using System.Numerics;

namespace EtherSharp.Tx.EIP1559;
public class EIP1559GasFeeProvider(IEthRpcModule ethRpcModule, IEtherSigner signer) : IGasFeeProvider<EIP1559TxParams, EIP1559GasParams>
{
    private readonly IEthRpcModule _ethRpcModule = ethRpcModule;
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
        ulong gasEstimation = await _ethRpcModule.EstimateGasAsync(
            _signer.Address, to, value, inputDataHex, cancellationToken);

        var feeHistory = await _ethRpcModule.GetFeeHistoryAsync(FeeHistoryRange, TargetBlockNumber.Latest, [PriorityFeePercentile], default);

        BigInteger baseFee;
        BigInteger priorityFee;

        if(feeHistory.BaseFeePerGas.Length == 0)
        {
            baseFee = await _ethRpcModule.GasPriceAsync(cancellationToken);
        }
        else
        {
            var summedBaseFees = feeHistory.BaseFeePerGas.Aggregate(BigInteger.Zero, (prev, curr) => prev + curr);
            baseFee = summedBaseFees / feeHistory.BaseFeePerGas.Length;
        }

        var nonZeroRewards = feeHistory.Reward.Where(x => x[0] != 0).ToArray();
        if(nonZeroRewards.Length == 0)
        {
            priorityFee = await _ethRpcModule.MaxPriorityFeePerGasAsync(cancellationToken);
        }
        else
        {
            var summedPriorityFees = nonZeroRewards.Aggregate(BigInteger.Zero, (prev, curr) => prev + curr[0]);
            priorityFee = summedPriorityFees / nonZeroRewards.Length;
        }

        var adjustedBaseFee = baseFee * (100 + BaseFeeOffsetPercentage) / 100;
        var adjustedPriorityFee = priorityFee * (100 + PriorityFeeOffsetPercentage) / 100;

        return new EIP1559GasParams(
            gasEstimation,
            adjustedBaseFee + adjustedPriorityFee,
            adjustedPriorityFee
        );
    }
}
