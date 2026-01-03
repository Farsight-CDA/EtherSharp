using System.Buffers.Binary;
using System.Numerics;

namespace EtherSharp.Query.Operations;

internal class GetBlockTimestampQueryOperation : IQuery, IQuery<DateTimeOffset>
{
    public int CallDataLength => 1;
    public BigInteger EthValue => 0;
    IReadOnlyList<IQuery> IQuery<DateTimeOffset>.Queries => [this];

    public void Encode(Span<byte> buffer)
        => buffer[0] = (byte) QueryOperationId.GetBlockTimestamp;
    public int ParseResultLength(ReadOnlySpan<byte> resultData)
        => 8;
    DateTimeOffset IQuery<DateTimeOffset>.ReadResultFrom(params ReadOnlySpan<byte[]> queryResults)
    {
        long seconds = (long) BinaryPrimitives.ReadUInt64BigEndian(queryResults[0]);
        return DateTimeOffset.FromUnixTimeSeconds(seconds);
    }
}
