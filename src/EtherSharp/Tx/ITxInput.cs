using EtherSharp.Contract;
using EtherSharp.Numerics;
using EtherSharp.Types;

namespace EtherSharp.Tx;
/// <summary>
/// Represents a transaction payload.
/// </summary>
public interface ITxInput
{
    /// <summary>
    /// The To field of the transaction.
    /// </summary>
    public Address? To { get; }

    /// <summary>
    /// The ETH Value of the transaction.
    /// </summary>
    public UInt256 Value { get; }

    /// <summary>
    /// The calldata of the transaction.
    /// </summary>
    public ReadOnlyMemory<byte> Data { get; }

    /// <summary>
    /// Creates a ITxInput for an eth transfer with no calldata.
    /// </summary>
    /// <param name="receiver"></param>
    /// <param name="amount"></param>
    /// <returns></returns>
    public static ITxInput ForEthTransfer(in Address receiver, UInt256 amount)
        => new TxInput(receiver, amount, Array.Empty<byte>());

    /// <summary>
    /// Creates a ITxInput that deploys a contract from <paramref name="initCode"/>.
    /// </summary>
    /// <param name="initCode">The initcode run by contract creation.</param>
    /// <param name="value">The ETH value sent to the new contract.</param>
    /// <returns>A payload whose eth_call result is the runtime code the deployment would store.</returns>
    public static ITxInput<EVMRuntimeCode> ForContractDeployment(EVMInitCode initCode, UInt256 value)
        => new ContractDeployment(initCode, value);
}

/// <summary>
/// Represents a transaction payload that returns a result of type <typeparamref name="T"/> when eth_call'ed.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface ITxInput<T> : ITxInput
{
    /// <summary>
    /// Parses the result of type T from the given call result data.
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public T ReadResultFrom(ReadOnlyMemory<byte> data);
}
