using EtherSharp.Client.Services.EtherApi;
using EtherSharp.Tx;
using EtherSharp.Tx.EIP1559;
using EtherSharp.Tx.Types;

namespace EtherSharp.Client;
public interface IEtherTxClient : IEtherClient
{
    public new IEtherTxApi ETH { get; }

    public IPendingTxHandler PrepareTx(ITxInput call, EIP1559TxParams? txParams = default, EIP1559GasParams? txGasParams = default)
        => PrepareTx<EIP1559Transaction, EIP1559TxParams, EIP1559GasParams>(call, txParams, txGasParams);

    public IPendingTxHandler PrepareTx<TTransaction, TTxParams, TTxGasParams>(
        ITxInput call, TTxParams? txParams = default, TTxGasParams? txGasParams = default
    )
        where TTransaction : class, ITransaction<TTransaction, TTxParams, TTxGasParams>
        where TTxParams : class, ITxParams<TTxParams>
        where TTxGasParams : class, ITxGasParams<TTxGasParams>;

    public Task<IPendingTxHandler> AttachPendingTxAsync(uint nonce)
        => AttachPendingTxAsync<EIP1559Transaction, EIP1559TxParams, EIP1559GasParams>(nonce);

    public Task<IPendingTxHandler> AttachPendingTxAsync<TTransaction, TTxParams, TTxGasParams>(uint nonce)
        where TTransaction : class, ITransaction<TTransaction, TTxParams, TTxGasParams>
        where TTxParams : class, ITxParams<TTxParams>
        where TTxGasParams : class, ITxGasParams<TTxGasParams>;
}
