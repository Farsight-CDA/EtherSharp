using EtherSharp.Tx;
using EtherSharp.Tx.Types;

namespace EtherSharp.Client.Services.TxTypeHandler;
public interface ITxTypeHandler<TTransaction, TTxParams, TTxGasParams>
    where TTransaction : class, ITransaction<TTransaction, TTxParams, TTxGasParams>
    where TTxParams : ITxParams<TTxParams>
    where TTxGasParams : ITxGasParams
{
    public string EncodeTxToBytes(
        ITxInput txInput, TTxParams txParams, TTxGasParams txGasParams,
        ReadOnlySpan<byte> inputData, uint nonce
    );
}
