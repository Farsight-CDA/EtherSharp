using EtherSharp.Numerics;
using EtherSharp.Tx;
using EtherSharp.Types;
using System.Buffers.Binary;

namespace EtherSharp.Query.Operations;

internal sealed class CallQueryOperation<T>(IContractCall<T> txInput) : IQuery, IQuery<CallResult<T>>
{
    private readonly IContractCall<T> _txInput = txInput;

    public int CallDataLength => 4 + 20 + 32 + _txInput.Data.Length;
    public UInt256 EthValue => _txInput.Value;
    IReadOnlyList<IQuery> IQuery<CallResult<T>>.Queries => [this];

    public void Encode(Span<byte> buffer)
    {
        BinaryPrimitives.WriteUInt32BigEndian(buffer[0..4], (uint) _txInput.Data.Length);

        if(buffer[0] != 0)
        {
            throw new InvalidOperationException("Calldata too large");
        }

        buffer[0] = (byte) QueryOperationId.Call;

        _txInput.To.CopyTo(buffer[4..24]);
        BinaryPrimitives.WriteUInt256BigEndian(buffer[24..56], EthValue);
        _txInput.Data.Span.CopyTo(buffer[56..]);
    }
    public int ParseResultLength(ReadOnlySpan<byte> resultData)
    {
        Span<byte> lengthBuffer = stackalloc byte[4];
        resultData[1..4].CopyTo(lengthBuffer[1..4]);
        int dataLength = (int) BinaryPrimitives.ReadUInt32BigEndian(lengthBuffer);
        return dataLength + 4;
    }
    CallResult<T> IQuery<CallResult<T>>.ReadResultFrom(params ReadOnlySpan<ReadOnlyMemory<byte>> queryResults)
    {
        var queryResult = queryResults[0];
        bool success = queryResult.Span[0] == 0x01;
        var returnData = queryResult[4..];

        return success switch
        {
            true => CallResult<T>.ParseSuccessFrom(returnData, _txInput.ReadResultFrom),
            false => new CallResult<T>.Reverted(returnData)
        };
    }
}
