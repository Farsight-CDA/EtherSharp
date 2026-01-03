using System.Buffers.Binary;
using System.Numerics;

namespace EtherSharp.Query.Operations;

internal class GetBlockGasLimitQueryOperation : IQuery, IQuery<ulong>
{
    public int CallDataLength => 1;
    public BigInteger EthValue => 0;
    IReadOnlyList<IQuery> IQuery<ulong>.Queries => [this];

    public void Encode(Span<byte> buffer)
        => buffer[0] = (byte) QueryOperationId.GetBlockGasLimit;
    public int ParseResultLength(ReadOnlySpan<byte> resultData)
        => 8;

    ulong IQuery<ulong>.ReadResultFrom(params ReadOnlySpan<byte[]> queryResults)
        => BinaryPrimitives.ReadUInt64BigEndian(queryResults[0]);
}
