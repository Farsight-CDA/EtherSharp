using EtherSharp.Contract;
using EtherSharp.Numerics;
using EtherSharp.Types;

namespace EtherSharp.Query.Operations;

internal sealed class GetCodeQueryOperation(in Address address) : IQueryOperation, IQuery<EVMByteCode>
{
    private readonly Address _address = address;

    public int CallDataLength => 21;
    public UInt256 EthValue => 0;

    public void Encode(Span<byte> buffer)
    {
        buffer[0] = (byte) QueryOperationId.GetCode;
        _address.CopyTo(buffer[1..]);
    }
    public int ParseResultLength(ReadOnlySpan<byte> resultData)
        => 3 + ((resultData[0] << 16) | (resultData[1] << 8) | resultData[2]);
    EVMByteCode IQuery<EVMByteCode>.ReadResultFrom(params ReadOnlySpan<ReadOnlyMemory<byte>> queryResults)
        => new EVMByteCode(queryResults[0][3..]);
}
