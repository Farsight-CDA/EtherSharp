using EtherSharp.Tx;
using EtherSharp.Tx.Types;

namespace EtherSharp.Client.Services.GasFeeProvider;

public interface IGasFeeProvider<TTxParams, TTxGasParams>
    where TTxParams : class, ITxParams<TTxParams>
    where TTxGasParams : class, ITxGasParams
{
    public Task<TTxGasParams> EstimateGasParamsAsync(
        ITxInput txInput,
        TTxParams txParams,
        CancellationToken cancellationToken
    );
}
