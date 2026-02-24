using EtherSharp.Client.Services.GasFeeProvider;
using EtherSharp.Common;
using EtherSharp.Numerics;
using EtherSharp.RPC.Modules.Eth;
using EtherSharp.Types;
using EtherSharp.Wallet;

namespace EtherSharp.Tx.EIP1559;

/// <summary>
/// Provides gas fee estimates for EIP-1559 transactions.
/// </summary>
public class EIP1559GasFeeProvider : IGasFeeProvider<EIP1559TxParams, EIP1559GasParams>
{
    /// <summary>
    /// Configuration values used to tune EIP-1559 gas estimation.
    /// </summary>
    public sealed class Configuration
    {
        /// <summary>
        /// Number of recent blocks used when querying fee history.
        /// </summary>
        public int FeeHistoryRange { get; set; } = 5;

        /// <summary>
        /// Reward percentile used to estimate the priority fee.
        /// </summary>
        public double PriorityFeePercentile { get; set; } = 25;

        /// <summary>
        /// Percentage added to the estimated base fee.
        /// </summary>
        public int BaseFeeOffsetPercentage { get; set; } = 20;

        /// <summary>
        /// Percentage added to the estimated priority fee.
        /// </summary>
        public int PriorityFeeOffsetPercentage { get; set; } = 20;

        /// <summary>
        /// Percentage added to the estimated gas limit.
        /// </summary>
        public ulong GasWantedOffsetPercentage { get; set; } = 15;
    }

    private readonly IEthRpcModule _ethRpcModule;
    private readonly IEtherSigner _signer;

    private readonly int _feeHistoryRange;
    private readonly double _priorityFeePercentile;
    private readonly int _baseFeeOffsetPercentage;
    private readonly int _priorityFeeOffsetPercentage;
    private readonly ulong _gasWantedOffsetPercentage;

    /// <summary>
    /// Creates a new <see cref="EIP1559GasFeeProvider"/>.
    /// </summary>
    /// <param name="ethRpcModule">RPC module used to query fee and gas data.</param>
    /// <param name="signer">Signer used as the sender context for gas estimation.</param>
    /// <param name="configuration">Optional gas estimation tuning values.</param>
    public EIP1559GasFeeProvider(
        IEthRpcModule ethRpcModule,
        IEtherSigner signer,
        EIP1559GasFeeProvider.Configuration? configuration = null
    )
    {
        var resolvedConfiguration = configuration ?? new EIP1559GasFeeProvider.Configuration();

        _ethRpcModule = ethRpcModule;
        _signer = signer;
        _feeHistoryRange = resolvedConfiguration.FeeHistoryRange;
        _priorityFeePercentile = resolvedConfiguration.PriorityFeePercentile;
        _baseFeeOffsetPercentage = resolvedConfiguration.BaseFeeOffsetPercentage;
        _priorityFeeOffsetPercentage = resolvedConfiguration.PriorityFeeOffsetPercentage;
        _gasWantedOffsetPercentage = resolvedConfiguration.GasWantedOffsetPercentage;
    }

    /// <inheritdoc/>
    public async Task<EIP1559GasParams> EstimateGasParamsAsync(ITxInput txInput, EIP1559TxParams txParams, CancellationToken cancellationToken)
    {
        var gasEstimationTask = _ethRpcModule.EstimateGasAsync(
            _signer.Address, txInput.To, txInput.Value, HexUtils.ToPrefixedHexString(txInput.Data.Span), cancellationToken);
        var feeHistoryTask = _ethRpcModule.GetFeeHistoryAsync(_feeHistoryRange, TargetHeight.Latest, [_priorityFeePercentile], cancellationToken);

        ulong gasEstimation = await gasEstimationTask;
        var feeHistory = await feeHistoryTask;

        UInt256 baseFee;
        UInt256 priorityFee;

        if(feeHistory.BaseFeePerGas.Length == 0)
        {
            baseFee = await _ethRpcModule.GasPriceAsync(cancellationToken);
        }
        else
        {
            var summedBaseFees = feeHistory.BaseFeePerGas.Aggregate(UInt256.Zero, (prev, curr) => prev + curr);
            baseFee = summedBaseFees / (uint) feeHistory.BaseFeePerGas.Length;
        }

        var nonZeroRewards = feeHistory.Reward.Where(x => x[0] != 0).ToArray();
        if(nonZeroRewards.Length == 0)
        {
            priorityFee = await _ethRpcModule.MaxPriorityFeePerGasAsync(cancellationToken);
        }
        else
        {
            var summedPriorityFees = nonZeroRewards.Aggregate(UInt256.Zero, (prev, curr) => prev + curr[0]);
            priorityFee = summedPriorityFees / (uint) nonZeroRewards.Length;
        }

        var adjustedBaseFee = baseFee * (uint) ((100 + _baseFeeOffsetPercentage) / 100);
        var adjustedPriorityFee = priorityFee * (uint) ((100 + _priorityFeeOffsetPercentage) / 100);

        return new EIP1559GasParams(
            gasEstimation * (100 + _gasWantedOffsetPercentage) / 100,
            adjustedBaseFee + adjustedPriorityFee,
            adjustedPriorityFee
        );
    }
}
