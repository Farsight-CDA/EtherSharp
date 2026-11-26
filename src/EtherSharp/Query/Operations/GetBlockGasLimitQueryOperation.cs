using EtherSharp.Numerics;
using System.Buffers.Binary;

namespace EtherSharp.Query.Operations;

internal sealed class GetBlockGasLimitQueryOperation : IQueryOperation, IQuery<ulong>
{
    public static GetBlockGasLimitQueryOperation Instance { get; } = new();

    public QueryOperationId OperationId => QueryOperationId.GetBlockGasLimit;

    private GetBlockGasLimitQueryOperation()
    {
    }

    public UInt256 EthValue => 0;

    public int GetCallDataLength(QueryEncodingContext context)
        => 1;

    public void Encode(Span<byte> buffer, QueryEncodingContext context)
        => buffer[0] = (byte) QueryOperationId.GetBlockGasLimit;
    public int ParseResultLength(ReadOnlySpan<byte> resultData)
        => 8;

    ulong IQuery<ulong>.ReadResultFrom(params ReadOnlySpan<ReadOnlyMemory<byte>> queryResults)
        => BinaryPrimitives.ReadUInt64BigEndian(queryResults[0].Span);
}
