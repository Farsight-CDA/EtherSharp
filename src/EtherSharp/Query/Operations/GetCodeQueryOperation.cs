using EtherSharp.Query;
using EtherSharp.Types;
using System.Buffers.Binary;
using System.Numerics;

namespace EtherSharp.Query.Operations;

internal class GetCodeQueryOperation(Address address) : IQuery, IQuery<EVMByteCode>
{
    private readonly Address _address = address;

    public int CallDataLength => 21;
    public BigInteger EthValue => 0;
    IReadOnlyList<IQuery> IQuery<EVMByteCode>.Queries => [this];

    public void Encode(Span<byte> buffer)
    {
        buffer[0] = (byte) QueryOperationId.GetCode;
        _address.Bytes.CopyTo(buffer[1..]);
    }
    public int ParseResultLength(ReadOnlySpan<byte> resultData)
    {
        Span<byte> lengthBuffer = stackalloc byte[4];
        resultData[0..3].CopyTo(lengthBuffer[1..4]);
        int dataLength = (int) BinaryPrimitives.ReadUInt32BigEndian(lengthBuffer);
        return dataLength + 3;
    }
    EVMByteCode IQuery<EVMByteCode>.ReadResultFrom(params ReadOnlySpan<byte[]> queryResults)
        => new EVMByteCode(queryResults[0].AsMemory()[3..]);
}
