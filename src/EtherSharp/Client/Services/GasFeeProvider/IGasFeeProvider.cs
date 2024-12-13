using EtherSharp.Tx;
using EtherSharp.Tx.Types;

namespace EtherSharp.Client.Services.GasFeeProvider;
public interface IGasFeeProvider
{
    public Task<ulong> EstimateGasAsync(
        ITxInput txInput, ReadOnlySpan<byte> data, CancellationToken cancellationToken = default); 
    public Task<ITxGasParams> CalculateGasParamsAsync<TTxParams>(
        ITxInput txInput, TTxParams txParams, ulong gas, CancellationToken cancellationToken = default)
        where TTxParams : ITxParams;
}
