using EtherSharp.ABI;
using EtherSharp.Types;
using System.Numerics;

namespace EtherSharp.Client.Modules.Query;

internal class QueryInput<T>(Address to, BigInteger value, byte[] data, Func<AbiDecoder, T> decoder) : IQueryInput<QueryResult<T>>
{
    public Address To { get; } = to;
    public BigInteger Value { get; } = value;

    private readonly byte[] _data = data;
    public ReadOnlySpan<byte> Data => _data;
    public Func<AbiDecoder, T> Decoder { get; } = decoder;

    QueryResult<T> IQueryable<QueryResult<T>>.ReadResultFrom(ReadOnlySpan<TxCallResult> callResults)
        => QueryResult<T>.Parse(callResults[0], Decoder);
    IEnumerable<ICallInput> IQueryable<QueryResult<T>>.GetQueryInputs()
    {
        yield return this;
    }
}
