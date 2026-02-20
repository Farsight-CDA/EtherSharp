namespace EtherSharp.Tx.Types;

public interface ITransaction<TSelf, TTxParams, TTxGasParams>
    where TTxParams : ITxParams<TTxParams>
    where TTxGasParams : ITxGasParams
    where TSelf : ITransaction<TSelf, TTxParams, TTxGasParams>
{
}
