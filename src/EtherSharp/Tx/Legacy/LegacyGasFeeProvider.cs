using EtherSharp.Client.Services.GasFeeProvider;
using EtherSharp.Common;
using EtherSharp.Numerics;
using EtherSharp.RPC.Modules.Eth;
using EtherSharp.Wallet;

namespace EtherSharp.Tx.Legacy;

/// <summary>
/// Provides gas fee estimates for legacy transactions.
/// </summary>
public class LegacyGasFeeProvider : IGasFeeProvider<LegacyTxParams, LegacyGasParams>
{
    /// <summary>
    /// Configuration values used to tune legacy gas estimation.
    /// </summary>
    public sealed class Configuration
    {
        /// <summary>
        /// Percentage added to the estimated gas price.
        /// </summary>
        public int GasPriceOffsetPercentage { get; set; } = 15;

        /// <summary>
        /// Percentage added to the estimated gas limit.
        /// </summary>
        public ulong GasWantedOffsetPercentage { get; set; } = 15;
    }

    private readonly IEthRpcModule _ethRpcModule;
    private readonly IEtherSigner _signer;

    private readonly int _gasPriceOffsetPercentage;
    private readonly ulong _gasWantedOffsetPercentage;

    /// <summary>
    /// Creates a new <see cref="LegacyGasFeeProvider"/>.
    /// </summary>
    /// <param name="ethRpcModule">RPC module used to query fee and gas data.</param>
    /// <param name="signer">Signer used as the sender context for gas estimation.</param>
    /// <param name="configuration">Optional gas estimation tuning values.</param>
    public LegacyGasFeeProvider(
        IEthRpcModule ethRpcModule,
        IEtherSigner signer,
        LegacyGasFeeProvider.Configuration? configuration = null
    )
    {
        var resolvedConfiguration = configuration ?? new LegacyGasFeeProvider.Configuration();

        _ethRpcModule = ethRpcModule;
        _signer = signer;
        _gasPriceOffsetPercentage = resolvedConfiguration.GasPriceOffsetPercentage;
        _gasWantedOffsetPercentage = resolvedConfiguration.GasWantedOffsetPercentage;
    }

    /// <inheritdoc/>
    public async Task<LegacyGasParams> EstimateGasParamsAsync(ITxInput txInput, LegacyTxParams txParams, CancellationToken cancellationToken)
    {
        ulong gasUsed = await _ethRpcModule.EstimateGasAsync(_signer.Address, txInput.To, txInput.Value, HexUtils.ToPrefixedHexString(txInput.Data.Span), cancellationToken);
        var gasPrice = await _ethRpcModule.GasPriceAsync(cancellationToken);

        var adjustedGasPrice = gasPrice * (UInt256) (100 + _gasPriceOffsetPercentage) / 100;

        return new LegacyGasParams(
            gasUsed * (100 + _gasWantedOffsetPercentage) / 100,
            adjustedGasPrice
        );
    }
}
