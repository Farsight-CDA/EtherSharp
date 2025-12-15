using System.Numerics;

namespace EtherSharp.Client.Modules.Query.Operations;

internal class GetBlockBaseFeeQueryOperation : IQuery, IQuery<BigInteger>
{
    public int CallDataLength => 1;
    IReadOnlyList<IQuery> IQuery<BigInteger>.Queries => [this];

    public void Encode(Span<byte> buffer)
        => buffer[0] = (byte) QueryOperationId.GetBlockBaseFee;

    public int ParseResultLength(ReadOnlySpan<byte> resultData)
        => 32;
    BigInteger IQuery<BigInteger>.ReadResultFrom(params ReadOnlySpan<byte[]> queryResults)
        => new BigInteger(queryResults[0], true, true);
}
