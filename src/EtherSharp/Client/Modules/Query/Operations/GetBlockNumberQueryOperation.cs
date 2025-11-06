using System.Buffers.Binary;

namespace EtherSharp.Client.Modules.Query.Operations;

internal class GetBlockNumberQueryOperation : IQuery, IQuery<ulong>
{
    public int CallDataLength => 1;

    public void Encode(Span<byte> buffer)
        => buffer[0] = 131;
    public int ParseResultLength(ReadOnlySpan<byte> resultData)
        => 8;
    IEnumerable<IQuery> IQuery<ulong>.GetQueries()
    {
        yield return this;
    }
    ulong IQuery<ulong>.ReadResultFrom(params scoped ReadOnlySpan<byte[]> queryResults)
        => BinaryPrimitives.ReadUInt64BigEndian(queryResults[0]);
}
