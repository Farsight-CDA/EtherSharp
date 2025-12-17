using EtherSharp.ABI;
using EtherSharp.Types;
using System.Numerics;

namespace EtherSharp.Tx;
/// <summary>
/// Represents a transaction payload.
/// </summary>
public interface ITxInput : ICallInput
{
    /// <summary>
    /// Creates a ITxInput for a contract call that returns a result of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="contractAddress"></param>
    /// <param name="value"></param>
    /// <param name="functionSignature"></param>
    /// <param name="encoder"></param>
    /// <param name="decoder"></param>
    /// <returns></returns>
    public static ITxInput<T> ForContractCall<T>(Address contractAddress, BigInteger value, ReadOnlyMemory<byte> functionSignature, AbiEncoder encoder, Func<AbiDecoder, T> decoder)
    {
        byte[] data = new byte[functionSignature.Length + encoder.Size];
        functionSignature.CopyTo(data);
        encoder.TryWritoTo(data.AsSpan()[functionSignature.Length..]);
        return new TxInput<T>(contractAddress, value, data, decoder);
    }

    /// <summary>
    /// Creates a ITxInput for a contract call that returns no result.
    /// </summary>
    /// <param name="contractAddress"></param>
    /// <param name="value"></param>
    /// <param name="functionSignature"></param>
    /// <param name="encoder"></param>
    /// <returns></returns>
    public static ITxInput ForContractCall(Address contractAddress, BigInteger value, ReadOnlyMemory<byte> functionSignature, AbiEncoder encoder)
    {
        byte[] data = new byte[functionSignature.Length + encoder.Size];
        functionSignature.CopyTo(data);
        encoder.TryWritoTo(data.AsSpan()[functionSignature.Length..]);
        return new TxInput(contractAddress, value, data);
    }

    /// <summary>
    /// Creates a ITxInput for an eth transfer with no calldata.
    /// </summary>
    /// <param name="receiver"></param>
    /// <param name="amount"></param>
    /// <returns></returns>
    public static ITxInput ForEthTransfer(Address receiver, BigInteger amount)
        => new TxInput(receiver, amount, []);
}

/// <summary>
/// Represents a transaction payload that returns a result of type <typeparamref name="T"/> when eth_call'ed.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface ITxInput<T> : ITxInput
{
    public T ReadResultFrom(ReadOnlyMemory<byte> data);
}