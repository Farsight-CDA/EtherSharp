using EtherSharp.Numerics;
using EtherSharp.Types;

namespace EtherSharp.Query.Operations;

internal sealed class ReadStorageQueryOperation(in Bytes32 slot)
    : IQuery, IQuery<Bytes32>
{
    private readonly Bytes32 _slot = slot;

    public int CallDataLength => 1 + Bytes32.BYTE_LENGTH;
    public UInt256 EthValue => 0;

    public void Encode(Span<byte> buffer)
    {
        buffer[0] = (byte) QueryOperationId.ReadStorage;
        _slot.CopyTo(buffer[1..]);
    }

    public int ParseResultLength(ReadOnlySpan<byte> resultData)
        => Bytes32.BYTE_LENGTH;

    Bytes32 IQuery<Bytes32>.ReadResultFrom(params scoped ReadOnlySpan<ReadOnlyMemory<byte>> queryResults)
        => Bytes32.FromBytes(queryResults[0].Span);
}
