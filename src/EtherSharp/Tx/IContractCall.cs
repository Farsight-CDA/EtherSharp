using EtherSharp.ABI;
using EtherSharp.Types;
using System.Numerics;

namespace EtherSharp.Tx;
/// <summary>
/// Represents a contract call transaction payload.
/// </summary>
public interface IContractCall : ITxInput
{
    private readonly static Address _create2DeployerAddress = "0x4e59b44847b379578588920ca78fbf26c0b4956c";

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

    /// <summary>
    /// Creates an IContractCall for a raw undecoded contract call.
    /// </summary>
    /// <param name="contractAddress"></param>
    /// <param name="value"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public static IContractCall<ReadOnlyMemory<byte>> ForRawContractCall(Address contractAddress, BigInteger value, ReadOnlyMemory<byte> data)
        => new RawTxInput(contractAddress, value, data);

    /// <summary>
    /// Creates an IContractCall for calling the Create2 deployer factory.
    /// </summary>
    /// <param name="byteCode"></param>
    /// <param name="salt"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static IContractCall<Address> ForCreate2Call(EVMByteCode byteCode, ReadOnlySpan<byte> salt, BigInteger value)
    {
        if(salt.Length != 32)
        {
            throw new ArgumentException("Salt must be 32 bytes", nameof(salt));
        }

        byte[] buffer = new byte[32 + byteCode.Length];
        salt.CopyTo(buffer);
        byteCode.ByteCode.Span.CopyTo(buffer.AsSpan(32));

        return new TxInput<Address>(
            _create2DeployerAddress,
            value,
            buffer,
            x => Address.FromBytes(x.Span)
        );
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
        return new TxInput<T>(contractAddress, value, data, x => decoder(new AbiDecoder(data)));
    }
}
