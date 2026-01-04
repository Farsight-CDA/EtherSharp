using EtherSharp.ABI.Types;
using EtherSharp.Tx;
using System.Buffers.Binary;
using System.Numerics;

namespace EtherSharp.Query.Operations;

internal class CallQueryOperation<T>(IContractCall<T> txInput) : IQuery, IQuery<QueryResult<T>>
{
    private readonly IContractCall<T> _txInput = txInput;

    public int CallDataLength => 4 + 20 + 32 + _txInput.Data.Length;
    public BigInteger EthValue => _txInput.Value;
    IReadOnlyList<IQuery> IQuery<QueryResult<T>>.Queries => [this];

    public void Encode(Span<byte> buffer)
    {
        BinaryPrimitives.WriteUInt32BigEndian(buffer[0..4], (uint) _txInput.Data.Length);

        if(buffer[0] != 0)
        {
            throw new InvalidOperationException("Calldata too large");
        }

        buffer[0] = (byte) QueryOperationId.Call;

        _txInput.To.Bytes.CopyTo(buffer[4..24]);
        AbiTypes.BigInteger.EncodeInto(EthValue, true, buffer[24..56]);
        _txInput.Data.Span.CopyTo(buffer[56..]);
    }
    public int ParseResultLength(ReadOnlySpan<byte> resultData)
    {
        Span<byte> lengthBuffer = stackalloc byte[4];
        resultData[1..4].CopyTo(lengthBuffer[1..4]);
        int dataLength = (int) BinaryPrimitives.ReadUInt32BigEndian(lengthBuffer);
        return dataLength + 4;
    }
    QueryResult<T> IQuery<QueryResult<T>>.ReadResultFrom(params ReadOnlySpan<byte[]> queryResults)
    {
        byte[] queryResult = queryResults[0];
        bool success = queryResult[0] == 0x01;
        byte[] returnData = queryResult[4..];

        return success switch
        {
            true => new QueryResult<T>.Success(_txInput.ReadResultFrom(returnData)),
            false => new QueryResult<T>.Reverted(returnData)
        };
    }
}
