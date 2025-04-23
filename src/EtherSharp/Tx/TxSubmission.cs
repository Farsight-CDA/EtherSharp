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
    where TTxGasParams : class, ITxGasParams<TTxGasParams>;