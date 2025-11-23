
using EtherSharp.Types;

namespace EtherSharp.Client.Modules.Query.Operations;

internal class GetCodeHashQueryOperation(Address address) : IQuery, IQuery<byte[]>
{
    private readonly Address _address = address;

    public int CallDataLength => 21;
    IReadOnlyList<IQuery> IQuery<byte[]>.Queries => [this];

    public void Encode(Span<byte> buffer)
    {
        buffer[0] = (byte) QueryOperationId.GetCodeHash;
        _address.Bytes.CopyTo(buffer[1..]);
    }
    public int ParseResultLength(ReadOnlySpan<byte> resultData)
        => 32;
    byte[] IQuery<byte[]>.ReadResultFrom(params ReadOnlySpan<byte[]> queryResults)
        => queryResults[0];
}
