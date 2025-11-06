
using EtherSharp.Types;
using System.Buffers.Binary;

namespace EtherSharp.Client.Modules.Query.Operations;

internal class GetCodeQueryOperation(Address address) : IQuery, IQuery<byte[]>
{
    private readonly Address _address = address;

    public int CallDataLength => 21;

    public void Encode(Span<byte> buffer)
    {
        buffer[0] = 129;
        _address.Bytes.CopyTo(buffer[1..]);
    }

    public int ParseResultLength(ReadOnlySpan<byte> resultData)
    {
        Span<byte> lengthBuffer = stackalloc byte[4];
        resultData[0..3].CopyTo(lengthBuffer[1..4]);
        int dataLength = (int) BinaryPrimitives.ReadUInt32BigEndian(lengthBuffer);
        return dataLength + 3;
    }

    IEnumerable<IQuery> IQuery<byte[]>.GetQueries()
    {
        yield return this;
    }

    byte[] IQuery<byte[]>.ReadResultFrom(params ReadOnlySpan<byte[]> queryResults)
        => [.. queryResults[0][3..]];
}
