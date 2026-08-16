using EtherSharp.Numerics;
using System.Buffers.Binary;

namespace EtherSharp.Query.Operations;

internal sealed class GetBlockBaseFeeQueryOperation : IQuery, IQuery<UInt256>
{
    public static GetBlockBaseFeeQueryOperation Instance { get; } = new();

    private GetBlockBaseFeeQueryOperation()
    {
    }

    public int CallDataLength => 1;
    public UInt256 EthValue => 0;

    public void Encode(Span<byte> buffer)
        => buffer[0] = (byte) QueryOperationId.GetBlockBaseFee;

    public int ParseResultLength(ReadOnlySpan<byte> resultData)
        => 32;
    UInt256 IQuery<UInt256>.ReadResultFrom(params ReadOnlySpan<ReadOnlyMemory<byte>> queryResults)
        => BinaryPrimitives.ReadUInt256BigEndian(queryResults[0].Span);
}
