using EtherSharp.ABI;
using EtherSharp.Types;
using System.Numerics;

namespace EtherSharp.Client.Modules.Query;

public interface IQueryInput
{    /// <summary>
     /// Creates a IQueryInput for a contract call that returns a result of type <typeparamref name="T"/>.
     /// </summary>
     /// <typeparam name="T"></typeparam>
     /// <param name="contractAddress"></param>
     /// <param name="value"></param>
     /// <param name="functionSignature"></param>
     /// <param name="encoder"></param>
     /// <param name="decoder"></param>
     /// <returns></returns>
    public static IQueryInput<QueryResult<T>> ForContractCall<T>(Address contractAddress, BigInteger value, ReadOnlyMemory<byte> functionSignature, AbiEncoder encoder, Func<AbiDecoder, T> decoder)
    {
        byte[] data = new byte[functionSignature.Length + encoder.Size];
        functionSignature.CopyTo(data);
        encoder.TryWritoTo(data.AsSpan()[functionSignature.Length..]);
        return new QueryInput<T>(contractAddress, value, data, decoder);
    }
}

public interface IQueryInput<T> : ICallInput, IQueryable<T>
{
}
