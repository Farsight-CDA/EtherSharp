using EtherSharp.Numerics;
using EtherSharp.Types;

namespace EtherSharp.Query.Operations;

internal sealed class HasCodeQueryOperation(in Address address) : IQuery, IQuery<bool>
{
    private readonly Address _address = address;

    public int CallDataLength => 21;
    public UInt256 EthValue => 0;
    IReadOnlyList<IQuery> IQuery<bool>.Queries => [this];

    public void Encode(Span<byte> buffer)
    {
        buffer[0] = (byte) QueryOperationId.HasCode;
        _address.CopyTo(buffer[1..]);
    }

    public int ParseResultLength(ReadOnlySpan<byte> resultData)
        => 1;
    bool IQuery<bool>.ReadResultFrom(params scoped ReadOnlySpan<ReadOnlyMemory<byte>> queryResults)
        => queryResults[0].Span[0] != 0;
}
