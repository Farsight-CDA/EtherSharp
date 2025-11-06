using EtherSharp.Types;
using System.Numerics;

namespace EtherSharp.Client.Modules.Query.Operations;

internal class GetBalanceQueryOperation(Address user) : IQuery, IQuery<BigInteger>
{
    private readonly Address _user = user;

    public int CallDataLength => 21;

    public void Encode(Span<byte> buffer)
    {
        buffer[0] = 134;
        _user.Bytes.CopyTo(buffer[1..]);
    }

    public int ParseResultLength(ReadOnlySpan<byte> resultData)
        => 32;
    IEnumerable<IQuery> IQuery<BigInteger>.GetQueries()
    {
        yield return this;
    }
    BigInteger IQuery<BigInteger>.ReadResultFrom(params ReadOnlySpan<byte[]> queryResults)
        => new BigInteger(queryResults[0], true, true);
}
