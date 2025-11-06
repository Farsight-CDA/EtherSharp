using EtherSharp.Client.Modules.Query.Operations;
using EtherSharp.Tx;
using EtherSharp.Types;
using System.Numerics;

namespace EtherSharp.Client.Modules.Query;

public interface IQuery
{
    public int CallDataLength { get; }
    public void Encode(Span<byte> buffer);
    public int ParseResultLength(ReadOnlySpan<byte> resultData);

    public static IQuery<T> Call<T>(ITxInput<T> input)
        => new CallQueryOperation<T>(input);

    public static IQuery<QueryResult<T>> SafeCall<T>(ITxInput<T> input)
        => new SafeCallQueryOperation<T>(input);

    public static IQuery<byte[]> GetCode(Address contract)
        => new GetCodeQueryOperation(contract);

    public static IQuery<byte[]> GetCodeHash(Address contract)
        => new GetCodeHashQueryOperation(contract);

    public static IQuery<ulong> GetBlockNumber()
        => new GetBlockNumberQueryOperation();

    public static IQuery<DateTimeOffset> GetBlockTimestamp()
        => new GetBlockTimestampQueryOperation();

    public static IQuery<BigInteger> GetBlockGasLimit()
        => new GetBlockGasLimitQueryOperation();

    public static IQuery<BigInteger> GetBalance(Address user)
        => new GetBalanceQueryOperation(user);
}

/// <summary>
/// Represents a call payload that returns a result of type <typeparamref name="T"/> when eth_call'ed.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IQuery<T>
{
    internal IEnumerable<IQuery> GetQueries();
    internal T ReadResultFrom(params ReadOnlySpan<byte[]> queryResults);
}
