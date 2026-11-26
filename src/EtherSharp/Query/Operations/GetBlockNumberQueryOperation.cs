using EtherSharp.Numerics;
using System.Buffers.Binary;

namespace EtherSharp.Query.Operations;

internal sealed class GetBlockNumberQueryOperation : IQueryOperation, IQuery<ulong>
{
    public static GetBlockNumberQueryOperation Instance { get; } = new();

    public QueryOperationId OperationId => QueryOperationId.GetBlockNumber;

    private GetBlockNumberQueryOperation()
    {
    }

    public UInt256 EthValue => 0;

    public int GetCallDataLength(QueryEncodingContext context)
        => 1;

    public void Encode(Span<byte> buffer, QueryEncodingContext context)
        => buffer[0] = (byte) QueryOperationId.GetBlockNumber;
    public int ParseResultLength(ReadOnlySpan<byte> resultData)
        => 8;
    ulong IQuery<ulong>.ReadResultFrom(params scoped ReadOnlySpan<ReadOnlyMemory<byte>> queryResults)
        => BinaryPrimitives.ReadUInt64BigEndian(queryResults[0].Span);
}
