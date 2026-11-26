using EtherSharp.Numerics;
using System.Buffers.Binary;

namespace EtherSharp.Query.Operations;

internal sealed class GetChainIdQueryOperation : IQueryOperation, IQuery<ulong>
{
    public static GetChainIdQueryOperation Instance { get; } = new();

    public QueryOperationId OperationId => QueryOperationId.GetChainId;

    private GetChainIdQueryOperation()
    {
    }

    public UInt256 EthValue => 0;

    public int GetCallDataLength(QueryEncodingContext context)
        => 1;

    public void Encode(Span<byte> buffer, QueryEncodingContext context)
        => buffer[0] = (byte) QueryOperationId.GetChainId;
    public int ParseResultLength(ReadOnlySpan<byte> resultData)
        => 8;

    ulong IQuery<ulong>.ReadResultFrom(params ReadOnlySpan<ReadOnlyMemory<byte>> queryResults)
        => BinaryPrimitives.ReadUInt64BigEndian(queryResults[0].Span);
}
