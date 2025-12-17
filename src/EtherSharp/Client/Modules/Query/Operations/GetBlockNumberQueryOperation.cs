using System.Buffers.Binary;
using System.Numerics;

namespace EtherSharp.Client.Modules.Query.Operations;

internal class GetBlockNumberQueryOperation : IQuery, IQuery<ulong>
{
    public int CallDataLength => 1;
    public BigInteger EthValue => 0;
    IReadOnlyList<IQuery> IQuery<ulong>.Queries => [this];

    public void Encode(Span<byte> buffer)
        => buffer[0] = (byte) QueryOperationId.GetBlockNumber;
    public int ParseResultLength(ReadOnlySpan<byte> resultData)
        => 8;
    ulong IQuery<ulong>.ReadResultFrom(params scoped ReadOnlySpan<byte[]> queryResults)
        => BinaryPrimitives.ReadUInt64BigEndian(queryResults[0]);
}
