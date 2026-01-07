using EtherSharp.ABI.Types;
using EtherSharp.Numerics;
using EtherSharp.Tx;
using System.Buffers.Binary;

namespace EtherSharp.Query.Operations;

internal class SafeFlashCallQueryOperation<T>(IContractDeployment deployment, IContractCall<T> txInput) : IQuery, IQuery<QueryResult<T>>
{
    private readonly ITxInput<T> _txInput = txInput;
    private readonly IContractDeployment _deployment = deployment;

    public int CallDataLength => 1 + 37 + _deployment.ByteCode.Length + _txInput.Data.Length;
    public UInt256 EthValue => _deployment.Value + _txInput.Value;
    IReadOnlyList<IQuery> IQuery<QueryResult<T>>.Queries => [this];

    public void Encode(Span<byte> buffer)
    {
        if(_deployment.Value > 0)
        {
            throw new NotSupportedException("Contract deployment cannot contain any value");
        }

        buffer[0] = (byte) QueryOperationId.FlashCall;
        buffer = buffer[1..];

        AbiTypes.UShort.EncodeInto((ushort) _deployment.ByteCode.Length, buffer[0..2]);
        AbiTypes.UInt.EncodeInto((uint) _txInput.Data.Length, buffer[2..5], true);
        BinaryPrimitives.WriteUInt256BigEndian(buffer[5..37], EthValue);

        _deployment.ByteCode.ByteCode.Span.CopyTo(buffer[37..]);
        _txInput.Data.Span.CopyTo(buffer[(37 + _deployment.ByteCode.Length)..]);
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
