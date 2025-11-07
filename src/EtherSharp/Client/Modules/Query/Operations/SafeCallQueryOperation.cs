
using EtherSharp.Tx;
using EtherSharp.Types;
using System.Buffers.Binary;

namespace EtherSharp.Client.Modules.Query.Operations;

internal class SafeCallQueryOperation<T>(ITxInput<T> txInput) : IQuery, IQuery<QueryResult<T>>
{
    private readonly ITxInput<T> _txInput = txInput;

    public int CallDataLength => 4 + 20 + _txInput.Data.Length;
    IReadOnlyList<IQuery> IQuery<QueryResult<T>>.Queries => [this];

    public void Encode(Span<byte> buffer)
    {
        BinaryPrimitives.WriteUInt32BigEndian(buffer[0..4], (uint) _txInput.Data.Length);
        _txInput.To.Bytes.CopyTo(buffer[4..24]);
        _txInput.Data.CopyTo(buffer[24..]);

        if(buffer[0] > 128)
        {
            throw new InvalidOperationException("Calldata too large");
        }
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
        var callResults = new TxCallResult[queryResults.Length];

        for(int i = 0; i < queryResults.Length; i++)
        {
            bool success = queryResults[i][0] == 0x01;
            byte[] returnData = queryResults[i][4..];
            callResults[i] = success switch
            {
                true => new TxCallResult.Success(returnData),
                false => new TxCallResult.Reverted(returnData)
            };
        }

        return QueryResult<T>.Parse(callResults[0], x => _txInput.ReadResultFrom(x.Data));
    }
}
