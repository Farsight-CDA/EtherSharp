
using EtherSharp.Types;
using System.Buffers.Binary;

namespace EtherSharp.Client.Modules.Query.Operations;

internal class GetCodeQueryOperation(Address address) : IQuery, IQuery<EVMBytecode>
{
    private readonly Address _address = address;

    public int CallDataLength => 21;
    IReadOnlyList<IQuery> IQuery<EVMBytecode>.Queries => [this];

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
    EVMBytecode IQuery<EVMBytecode>.ReadResultFrom(params ReadOnlySpan<byte[]> queryResults)
        => new EVMBytecode(_address, queryResults[0].AsMemory()[3..]);
}
