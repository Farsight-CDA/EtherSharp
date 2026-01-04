using EtherSharp.Client.Services.GasFeeProvider;
using EtherSharp.Common;
using EtherSharp.RPC.Modules.Eth;
using EtherSharp.Wallet;

namespace EtherSharp.Tx.Legacy;

public class LegacyGasFeeProvider(IEthRpcModule ethRpcModule, IEtherSigner signer) : IGasFeeProvider<LegacyTxParams, LegacyGasParams>
{
    private readonly IEthRpcModule _ethRpcModule = ethRpcModule;
    private readonly IEtherSigner _signer = signer;

    public int GasPriceOffsetPercentage { get; set; } = 15;
    public ulong GasWantedOffsetPercentage { get; set; } = 15;

    async Task<LegacyGasParams> IGasFeeProvider<LegacyTxParams, LegacyGasParams>.EstimateGasParamsAsync(ITxInput txInput, LegacyTxParams txParams, CancellationToken cancellationToken)
    {
        ulong gasUsed = await _ethRpcModule.EstimateGasAsync(_signer.Address, txInput.To, txInput.Value, HexUtils.ToPrefixedHexString(txInput.Data.Span), cancellationToken);
        var gasPrice = await _ethRpcModule.GasPriceAsync(cancellationToken);

        var adjustedGasPrice = gasPrice * (100 + GasPriceOffsetPercentage) / 100;

        return new LegacyGasParams(
            gasUsed * (100 + GasWantedOffsetPercentage) / 100,
            gasPrice + adjustedGasPrice
        );
    }
}