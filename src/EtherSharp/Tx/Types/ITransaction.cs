namespace EtherSharp.Tx.Types;
public interface ITransaction<TSelf, TTxParams, TTxGasParams>
    where TTxParams : ITxParams
    where TTxGasParams : ITxGasParams
    where TSelf : ITransaction<TSelf, TTxParams, TTxGasParams>
{
}
