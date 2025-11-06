using EtherSharp.Types;

namespace EtherSharp.Client.Modules.Query;

public record QueryResult<T>
{
    public record Success(T Value) : QueryResult<T>;
    public record Reverted(byte[] Data) : QueryResult<T>;

    public static QueryResult<T> Parse(TxCallResult callResult, Func<TxCallResult.Success, T> parser)
        => callResult switch
        {
            TxCallResult.Success s => new QueryResult<T>.Success(parser(s)),
            TxCallResult.Reverted r => new QueryResult<T>.Reverted(r.Data),
            _ => throw new NotSupportedException()
        };
}
