using EtherSharp.Tx;
using EtherSharp.Tx.Types;
using EtherSharp.Types;
using System.Numerics;

namespace EtherSharp.Client.Services.ResiliencyLayer;
public record TxSubmissionStorage(
    string TxHash,
    string SignedTx,
    Address To,
    BigInteger Value,
    byte[] CallData,
    byte[] TxParams,
    byte[] TxGasParams
)
{
    public TxSubmission<TTxParams, TTxGasParams> ToTxSubmission<TTxParams, TTxGasParams>()
        where TTxParams : class, ITxParams<TTxParams>
        where TTxGasParams : class, ITxGasParams<TTxGasParams> 
        => new TxSubmission<TTxParams, TTxGasParams>(
            TxHash,
            SignedTx,
            new TxInput(To, Value, CallData),
            TTxParams.Decode(TxParams),
            TTxGasParams.Decode(TxGasParams)
        );
}
