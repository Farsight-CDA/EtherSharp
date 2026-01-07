using EtherSharp.Numerics;
using EtherSharp.Types;
using System.Buffers.Binary;

namespace EtherSharp.Query.Operations;

internal class GetBalanceQueryOperation(Address user) : IQuery, IQuery<UInt256>
{
    private readonly Address _user = user;

    public int CallDataLength => 21;
    public UInt256 EthValue => 0;
    IReadOnlyList<IQuery> IQuery<UInt256>.Queries => [this];

    public void Encode(Span<byte> buffer)
    {
        buffer[0] = (byte) QueryOperationId.GetBalance;
        _user.Bytes.CopyTo(buffer[1..]);
    }

    public int ParseResultLength(ReadOnlySpan<byte> resultData)
        => 32;
    UInt256 IQuery<UInt256>.ReadResultFrom(params ReadOnlySpan<byte[]> queryResults)
        => BinaryPrimitives.ReadUInt256BigEndian(queryResults[0]);
}
