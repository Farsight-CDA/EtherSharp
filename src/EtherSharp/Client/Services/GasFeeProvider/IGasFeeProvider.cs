using EtherSharp.Tx;
using EtherSharp.Tx.Types;

namespace EtherSharp.Client.Services.GasFeeProvider;
public interface IGasFeeProvider
{
    public Task<ulong> EstimateGasAsync(ITxInput txInput, ReadOnlySpan<byte> data);
    public Task<ITxGasParams> CalculateGasParamsAsync<TTxParams>(ITxInput txInput, TTxParams txParams, ulong gas)
        where TTxParams : ITxParams;
}
