using System.Numerics;

namespace EtherSharp.Client.Modules.Query.Operations;

internal class GetBlockGasLimitQueryOperation : IQuery, IQuery<BigInteger>
{
    public int CallDataLength => 1;

    public void Encode(Span<byte> buffer)
        => buffer[0] = 133;
    public int ParseResultLength(ReadOnlySpan<byte> resultData)
        => 8;
    IEnumerable<IQuery> IQuery<BigInteger>.GetQueries()
    {
        yield return this;
    }
    BigInteger IQuery<BigInteger>.ReadResultFrom(params ReadOnlySpan<byte[]> queryResults)
        => new BigInteger(queryResults[0], true, true);
}
