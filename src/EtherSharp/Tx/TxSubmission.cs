using EtherSharp.Client.Services.ResiliencyLayer;
using EtherSharp.Tx.Types;

namespace EtherSharp.Tx;
public record TxSubmission<TTxParams, TTxGasParams>(
    string TxHash, 
    string SignedTx,
    ITxInput Call,
    TTxParams Params,
    TTxGasParams GasParams
) 
    where TTxParams : class, ITxParams<TTxParams>
    where TTxGasParams : class, ITxGasParams<TTxGasParams>
{
    public TxSubmissionStorage ToStorageType(uint nonce) 
        => new TxSubmissionStorage(
            nonce, 
            TxHash, 
            SignedTx, 
            Call.To.String, 
            Call.Value, 
            Call.Data.ToArray(), 
            Params.Encode(), 
            GasParams.Encode()
        );
}