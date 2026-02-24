using EtherSharp.Numerics;
using System.Buffers.Binary;

namespace EtherSharp.Query.Operations;

internal class GetBlockTimestampQueryOperation : IQuery, IQuery<DateTimeOffset>
{
    public int CallDataLength => 1;
    public UInt256 EthValue => 0;
    IReadOnlyList<IQuery> IQuery<DateTimeOffset>.Queries => [this];

    public void Encode(Span<byte> buffer)
        => buffer[0] = (byte) QueryOperationId.GetBlockTimestamp;
    public int ParseResultLength(ReadOnlySpan<byte> resultData)
        => 8;
    DateTimeOffset IQuery<DateTimeOffset>.ReadResultFrom(params ReadOnlySpan<ReadOnlyMemory<byte>> queryResults)
    {
        long seconds = (long) BinaryPrimitives.ReadUInt64BigEndian(queryResults[0].Span);
        return DateTimeOffset.FromUnixTimeSeconds(seconds);
    }
}
