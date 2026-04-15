using EtherSharp.ABI.Types;
using EtherSharp.Numerics;
using EtherSharp.Tx;
using EtherSharp.Types;
using System.Buffers.Binary;

namespace EtherSharp.Query.Operations;

internal sealed class SafeFlashCallQueryOperation<T>(IContractDeployment deployment, IFlashCall<T> txInput) : IQuery, IQuery<CallResult<T>>
{
    private readonly IFlashCall<T> _txInput = txInput;
    private readonly IContractDeployment _deployment = deployment;

    public int CallDataLength => 1 + 37 + _deployment.ByteCode.Length + _txInput.Data.Length;
    public UInt256 EthValue => _deployment.Value + _txInput.Value;
    IReadOnlyList<IQuery> IQuery<CallResult<T>>.Queries => [this];

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
