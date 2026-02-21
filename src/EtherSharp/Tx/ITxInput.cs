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
    public static ITxInput ForEthTransfer(Address receiver, UInt256 amount)
        => new TxInput(receiver, amount, Array.Empty<byte>());
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
