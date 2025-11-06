using System.Buffers.Binary;

namespace EtherSharp.Client.Modules.Query.Operations;

internal class GetBlockTimestampQueryOperation : IQuery, IQuery<DateTimeOffset>
{
    public int CallDataLength => 1;

    public void Encode(Span<byte> buffer)
        => buffer[0] = 132;
    public int ParseResultLength(ReadOnlySpan<byte> resultData)
        => 8;
    IEnumerable<IQuery> IQuery<DateTimeOffset>.GetQueries()
    {
        yield return this;
    }
    DateTimeOffset IQuery<DateTimeOffset>.ReadResultFrom(params ReadOnlySpan<byte[]> queryResults)
    {
        var seconds = (long) BinaryPrimitives.ReadUInt64BigEndian(queryResults[0]);
        return DateTimeOffset.FromUnixTimeSeconds(seconds);
    }
}
