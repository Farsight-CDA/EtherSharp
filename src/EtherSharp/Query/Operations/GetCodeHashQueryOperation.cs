using EtherSharp.Numerics;
using EtherSharp.Types;

namespace EtherSharp.Query.Operations;

internal sealed class GetCodeHashQueryOperation(in Address address) : IQueryOperation, IQuery<byte[]>
{
    private readonly Address _address = address;

    public QueryOperationId OperationId => QueryOperationId.GetCodeHash;
    public UInt256 EthValue => 0;

    public int GetCallDataLength(QueryEncodingContext context)
        => 21;

    public void Encode(Span<byte> buffer, QueryEncodingContext context)
    {
        buffer[0] = (byte) QueryOperationId.GetCodeHash;
        _address.CopyTo(buffer[1..]);
    }
    public int ParseResultLength(ReadOnlySpan<byte> resultData)
        => 32;
    byte[] IQuery<byte[]>.ReadResultFrom(params ReadOnlySpan<ReadOnlyMemory<byte>> queryResults)
        => queryResults[0].ToArray();
}
