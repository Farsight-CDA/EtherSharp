using EtherSharp.Contract;
using EtherSharp.Numerics;
using EtherSharp.Types;

namespace EtherSharp.Query.Operations;

internal sealed class GetCodeQueryOperation(in Address address) : IQueryOperation, IQuery<EVMRuntimeCode>
{
    private readonly Address _address = address;

    public QueryOperationId OperationId => QueryOperationId.GetCode;
    public UInt256 EthValue => 0;

    public int GetCallDataLength(QueryEncodingContext context)
        => 21;

    public void Encode(Span<byte> buffer, QueryEncodingContext context)
    {
        buffer[0] = (byte) QueryOperationId.GetCode;
        _address.CopyTo(buffer[1..]);
    }

    public int ParseResultLength(ReadOnlySpan<byte> resultData)
        => 3 + ((resultData[0] << 16) | (resultData[1] << 8) | resultData[2]);

    EVMRuntimeCode IQuery<EVMRuntimeCode>.ReadResultFrom(params ReadOnlySpan<ReadOnlyMemory<byte>> queryResults)
        => new EVMRuntimeCode(queryResults[0][3..]);
}
