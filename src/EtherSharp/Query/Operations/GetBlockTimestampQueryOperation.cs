using EtherSharp.Numerics;
using System.Buffers.Binary;

namespace EtherSharp.Query.Operations;

internal sealed class GetBlockTimestampQueryOperation : IQueryOperation, IQuery<DateTimeOffset>
{
    public static GetBlockTimestampQueryOperation Instance { get; } = new();

    public QueryOperationId OperationId => QueryOperationId.GetBlockTimestamp;

    private GetBlockTimestampQueryOperation()
    {
    }

    public UInt256 EthValue => 0;

    public int GetCallDataLength(QueryEncodingContext context)
        => 1;

    public void Encode(Span<byte> buffer, QueryEncodingContext context)
        => buffer[0] = (byte) QueryOperationId.GetBlockTimestamp;
    public int ParseResultLength(ReadOnlySpan<byte> resultData)
        => 8;
    DateTimeOffset IQuery<DateTimeOffset>.ReadResultFrom(params ReadOnlySpan<ReadOnlyMemory<byte>> queryResults)
    {
        long seconds = (long) BinaryPrimitives.ReadUInt64BigEndian(queryResults[0].Span);
        return DateTimeOffset.FromUnixTimeSeconds(seconds);
    }
}
