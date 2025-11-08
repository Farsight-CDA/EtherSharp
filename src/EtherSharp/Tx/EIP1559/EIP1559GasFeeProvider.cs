using EtherSharp.Client.Services.GasFeeProvider;
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
    public ulong GasWantedOffsetPercentage { get; set; } = 15;

    Task<EIP1559GasParams> IGasFeeProvider<EIP1559TxParams, EIP1559GasParams>.EstimateGasParamsAsync(ITxInput txInput, EIP1559TxParams txParams, CancellationToken cancellationToken)
        => SendEstimationRequestsAsync(
            txInput,
            cancellationToken
        );

    private async Task<EIP1559GasParams> SendEstimationRequestsAsync(ITxInput txInput, CancellationToken cancellationToken)
    {
        ulong gasEstimation = await _ethRpcModule.EstimateGasAsync(
            _signer.Address, txInput.To, txInput.Value, $"0x{Convert.ToHexString(txInput.Data)}", cancellationToken);

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
            gasEstimation * GasWantedOffsetPercentage / 100,
            adjustedBaseFee + adjustedPriorityFee,
            adjustedPriorityFee
        );
    }
}
