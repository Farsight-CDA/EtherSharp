
using EtherSharp.Tx;
using System.Buffers.Binary;
using System.Numerics;

namespace EtherSharp.Client.Modules.Query.Operations;

internal class SafeFlashCallQueryOperation<T>(IContractDeployment deployment, IContractCall<T> txInput) : IQuery, IQuery<QueryResult<T>>
{
    private readonly ITxInput<T> _txInput = txInput;
    private readonly IContractDeployment _deployment = deployment;

    public int CallDataLength => 1 + 8 + _deployment.ByteCode.Length + _txInput.Data.Length;
    public BigInteger EthValue => _deployment.Value + _txInput.Value;
    IReadOnlyList<IQuery> IQuery<QueryResult<T>>.Queries => [this];

    public void Encode(Span<byte> buffer)
    {
        buffer[0] = (byte) QueryOperationId.FlashCall;
        buffer = buffer[1..];
        BinaryPrimitives.WriteUInt32BigEndian(buffer[0..4], (uint) _deployment.ByteCode.Length);
        BinaryPrimitives.WriteUInt32BigEndian(buffer[4..8], (uint) _txInput.Data.Length);
        _deployment.ByteCode.ByteCode.Span.CopyTo(buffer[8..(8 + _deployment.ByteCode.Length)]);
        _txInput.Data.CopyTo(buffer[(8 + _deployment.ByteCode.Length)..]);
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
