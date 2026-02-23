namespace EtherSharp.Tx.Types;

/// <summary>
/// Represents a transaction payload type and its associated tx-parameter and gas-parameter types.
/// </summary>
/// <typeparam name="TSelf">Concrete transaction payload type.</typeparam>
/// <typeparam name="TTxParams">Concrete transaction parameter type.</typeparam>
/// <typeparam name="TTxGasParams">Concrete gas parameter type.</typeparam>
public interface ITransaction<TSelf, TTxParams, TTxGasParams>
    where TTxParams : ITxParams<TTxParams>
    where TTxGasParams : ITxGasParams
    where TSelf : ITransaction<TSelf, TTxParams, TTxGasParams>
{
}
