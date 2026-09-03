using EtherSharp.Tx;

namespace EtherSharp.Tx.Types;

/// <summary>
/// Represents the common fields of an unsigned transaction payload.
/// </summary>
public interface ITransaction
{
    /// <summary>Gets the target chain identifier.</summary>
    public ulong ChainId { get; }
    /// <summary>Gets the transaction gas limit.</summary>
    public ulong Gas { get; }
    /// <summary>Gets the sender nonce.</summary>
    public ulong Nonce { get; }
    /// <summary>Gets the transaction destination, value, and calldata.</summary>
    public ITxInput Input { get; }
}

/// <summary>
/// Represents a transaction payload type and its associated tx-parameter and gas-parameter types.
/// </summary>
/// <typeparam name="TSelf">Concrete transaction payload type.</typeparam>
/// <typeparam name="TTxParams">Concrete transaction parameter type.</typeparam>
/// <typeparam name="TTxGasParams">Concrete gas parameter type.</typeparam>
public interface ITransaction<TSelf, TTxParams, TTxGasParams>
    : ITransaction
    where TTxParams : ITxParams<TTxParams>
    where TTxGasParams : ITxGasParams
    where TSelf : ITransaction<TSelf, TTxParams, TTxGasParams>
{
}
