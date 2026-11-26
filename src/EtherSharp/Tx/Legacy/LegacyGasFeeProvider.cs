using EtherSharp.Client;
using EtherSharp.Client.Services.GasFeeProvider;
using EtherSharp.Numerics;
using EtherSharp.RPC.Modules.Eth;
using EtherSharp.RPC.Transport;
using EtherSharp.Types;

namespace EtherSharp.Tx.Legacy;

/// <summary>
/// Provides gas fee estimates for legacy transactions.
/// </summary>
public sealed class LegacyGasFeeProvider : IGasFeeProvider<LegacyTxParams, LegacyGasParams>
{
    private readonly IEthRpcModule _ethRpcModule;

    private readonly int _gasPriceOffsetPercentage;
    private readonly ulong _gasLimitOffsetPercentage;

    /// <summary>
    /// Creates a new <see cref="LegacyGasFeeProvider"/>.
    /// </summary>
    /// <param name="ethRpcModule">RPC module used to query fee and gas data.</param>
    /// <param name="options">Gas estimation tuning values.</param>
    public LegacyGasFeeProvider(IEthRpcModule ethRpcModule, GasFeeOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        options.Validate(nameof(options));

        _ethRpcModule = ethRpcModule;
        _gasPriceOffsetPercentage = options.Legacy.GasPriceOffsetPercentage;
        _gasLimitOffsetPercentage = options.GasLimitOffsetPercentage;
    }

    /// <inheritdoc/>
    public async Task<LegacyGasParams> EstimateGasParamsAsync(
        ITxInput txInput, LegacyTxParams txParams, Address? from,
        RpcRequestOptions requestOptions, CancellationToken cancellationToken)
    {
        var gasUsedTask = _ethRpcModule.EstimateGasAsync(
            txInput.To,
            txInput.Value,
            txInput.Data,
            new CallOptions { From = from },
            requestOptions,
            cancellationToken
        );
        var gasPriceTask = _ethRpcModule.GasPriceAsync(requestOptions, cancellationToken);

        ulong gasUsed = await gasUsedTask;
        var gasPrice = await gasPriceTask;

        var adjustedGasPrice = gasPrice * (UInt256) (100 + _gasPriceOffsetPercentage) / 100;

        return new LegacyGasParams(
            gasUsed * (100 + _gasLimitOffsetPercentage) / 100,
            adjustedGasPrice
        );
    }
}
