using EtherSharp.ABI;
using EtherSharp.Types;
using System.Numerics;

namespace EtherSharp.Tx;
/// <summary>
/// Represents a contract call transaction payload.
/// </summary>
public interface IContractCall : ITxInput
{
    /// <summary>
    /// Creates an IContractCall for a contract call that returns no result.
    /// </summary>
    /// <param name="contractAddress"></param>
    /// <param name="value"></param>
    /// <param name="functionSignature"></param>
    /// <param name="encoder"></param>
    /// <returns></returns>
    public static IContractCall ForContractCall(Address contractAddress, BigInteger value, ReadOnlyMemory<byte> functionSignature, AbiEncoder encoder)
    {
        byte[] data = new byte[functionSignature.Length + encoder.Size];
        functionSignature.CopyTo(data);
        encoder.TryWritoTo(data.AsSpan()[functionSignature.Length..]);
        return new TxInput(contractAddress, value, data);
    }
}

/// <summary>
/// Represents a contract call transaction payload with a return value.
/// </summary>
public interface IContractCall<T> : IContractCall, ITxInput<T>
{
    /// <summary>
    /// Creates an IContractCall for a contract call that returns a result of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="contractAddress"></param>
    /// <param name="value"></param>
    /// <param name="functionSignature"></param>
    /// <param name="encoder"></param>
    /// <param name="decoder"></param>
    /// <returns></returns>
    public static IContractCall<T> ForContractCall(Address contractAddress, BigInteger value, ReadOnlyMemory<byte> functionSignature, AbiEncoder encoder, Func<AbiDecoder, T> decoder)
    {
        byte[] data = new byte[functionSignature.Length + encoder.Size];
        functionSignature.CopyTo(data);
        encoder.TryWritoTo(data.AsSpan()[functionSignature.Length..]);
        return new TxInput<T>(contractAddress, value, data, decoder);
    }
}
