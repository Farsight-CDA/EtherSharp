using EtherSharp.Tx;
using EtherSharp.Tx.Types;

namespace EtherSharp.Client.Services.TxTypeHandler;
public interface ITxTypeHandler
{
    public Task<ITxGasParams> CalculateGasParamsAsync(
        ITxInput txInput, ITxParams txParams, 
        ReadOnlySpan<byte> inputData, CancellationToken cancellationToken = default
    );

    public string EncodeTxToBytes(
        ITxInput txInput, ITxParams txParams, ITxGasParams gasParams,
        ReadOnlySpan<byte> inputData, uint nonce
    );
}

public interface ITxTypeHandler<TTransaction, TTxParams, TTxGasParams> : ITxTypeHandler
    where TTransaction : class, ITransaction<TTransaction, TTxParams, TTxGasParams>
    where TTxParams : ITxParams
    where TTxGasParams : ITxGasParams
{
    
}
