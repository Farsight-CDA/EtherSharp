using EtherSharp.Numerics;
using System.Buffers.Binary;

namespace EtherSharp.Query.Operations;

internal class GetChainIdQueryOperation : IQuery, IQuery<ulong>
{
    public int CallDataLength => 1;
    public UInt256 EthValue => 0;
    IReadOnlyList<IQuery> IQuery<ulong>.Queries => [this];

    public void Encode(Span<byte> buffer)
        => buffer[0] = (byte) QueryOperationId.GetChainId;
    public int ParseResultLength(ReadOnlySpan<byte> resultData)
        => 8;

    ulong IQuery<ulong>.ReadResultFrom(params ReadOnlySpan<byte[]> queryResults)
        => BinaryPrimitives.ReadUInt64BigEndian(queryResults[0]);
}
