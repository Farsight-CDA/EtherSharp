using EtherSharp.Client;
using EtherSharp.Client.Services.GasFeeProvider;
using EtherSharp.Numerics;
using EtherSharp.RPC.Modules.Eth;
using EtherSharp.RPC.Transport;
using EtherSharp.Types;

namespace EtherSharp.Tx.EIP1559;

/// <summary>
/// Provides gas fee estimates for EIP-1559 transactions.
/// </summary>
public sealed class EIP1559GasFeeProvider : IGasFeeProvider<EIP1559TxParams, EIP1559GasParams>
{
    private readonly IEthRpcModule _ethRpcModule;

    private readonly int _feeHistoryRange;
    private readonly double[] _rewardPercentiles;
    private readonly int _baseFeeOffsetPercentage;
    private readonly int _priorityFeeOffsetPercentage;
    private readonly ulong _gasLimitOffsetPercentage;

    /// <summary>
    /// Creates a new <see cref="EIP1559GasFeeProvider"/>.
    /// </summary>
    /// <param name="ethRpcModule">RPC module used to query fee and gas data.</param>
    /// <param name="options">Gas estimation tuning values.</param>
    public EIP1559GasFeeProvider(IEthRpcModule ethRpcModule, GasFeeOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        options.Validate(nameof(options));

        _ethRpcModule = ethRpcModule;
        _feeHistoryRange = options.EIP1559.FeeHistoryRange;
        _rewardPercentiles = [options.EIP1559.PriorityFeePercentile];
        _baseFeeOffsetPercentage = options.EIP1559.BaseFeeOffsetPercentage;
        _priorityFeeOffsetPercentage = options.EIP1559.PriorityFeeOffsetPercentage;
        _gasLimitOffsetPercentage = options.GasLimitOffsetPercentage;
    }

    /// <inheritdoc/>
    public async Task<EIP1559GasParams> EstimateGasParamsAsync(
        ITxInput txInput, EIP1559TxParams txParams, Address? from,
        RpcRequestOptions requestOptions, CancellationToken cancellationToken)
    {
        var gasEstimationTask = _ethRpcModule.EstimateGasAsync(
            txInput.To,
            txInput.Value,
            txInput.Data,
            new CallOptions { From = from, AccessList = txParams.AccessList },
            requestOptions,
            cancellationToken
        );
        var feeHistoryTask = _ethRpcModule.GetFeeHistoryAsync(
            _feeHistoryRange, TargetHeight.Latest, _rewardPercentiles, requestOptions, cancellationToken);

        ulong gasEstimation = await gasEstimationTask;
        var feeHistory = await feeHistoryTask;

        UInt256 baseFee;
        UInt256 priorityFee;

        if(feeHistory.BaseFeePerGas.Length == 0)
        {
            baseFee = await _ethRpcModule.GasPriceAsync(requestOptions, cancellationToken);
        }
        else
        {
            var summedBaseFees = feeHistory.BaseFeePerGas.Sum();
            baseFee = summedBaseFees / (uint) feeHistory.BaseFeePerGas.Length;
        }

        var nonZeroRewards = feeHistory.Reward.Where(x => !x[0].IsZero).ToArray();
        if(nonZeroRewards.Length == 0)
        {
            priorityFee = await _ethRpcModule.MaxPriorityFeePerGasAsync(requestOptions, cancellationToken);
        }
        else
        {
            var summedPriorityFees = nonZeroRewards.Sum(x => x[0]);
            priorityFee = summedPriorityFees / (uint) nonZeroRewards.Length;
        }

        var adjustedBaseFee = baseFee * (UInt256) (100 + _baseFeeOffsetPercentage) / 100;
        var adjustedPriorityFee = priorityFee * (UInt256) (100 + _priorityFeeOffsetPercentage) / 100;

        return new EIP1559GasParams(
            gasEstimation * (100 + _gasLimitOffsetPercentage) / 100,
            adjustedBaseFee + adjustedPriorityFee,
            adjustedPriorityFee
        );
    }
}
