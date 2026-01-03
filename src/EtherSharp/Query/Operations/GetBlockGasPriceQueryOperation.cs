using System.Numerics;

namespace EtherSharp.Query.Operations;

internal class GetBlockGasPriceQueryOperation : IQuery, IQuery<BigInteger>
{
    public int CallDataLength => 1;
    public BigInteger EthValue => 0;
    IReadOnlyList<IQuery> IQuery<BigInteger>.Queries => [this];

    public void Encode(Span<byte> buffer)
        => buffer[0] = (byte) QueryOperationId.GetBlockGasPrice;

    public int ParseResultLength(ReadOnlySpan<byte> resultData)
        => 32;
    BigInteger IQuery<BigInteger>.ReadResultFrom(params ReadOnlySpan<byte[]> queryResults)
        => new BigInteger(queryResults[0], true, true);
}
