using EtherSharp.Tx;
using EtherSharp.Tx.Types;
using EtherSharp.Types;
using System.Numerics;

namespace EtherSharp.Client.Services.GasFeeProvider;
public interface IGasFeeProvider<TTxParams, TTxGasParams> 
    where TTxParams : class, ITxParams<TTxParams>
    where TTxGasParams : class, ITxGasParams
{
    public Task<TTxGasParams> EstimateGasParamsAsync(
        Address to,
        BigInteger value,
        ReadOnlySpan<byte> inputData,
        TTxParams txParams,
        CancellationToken cancellationToken
    );
}
