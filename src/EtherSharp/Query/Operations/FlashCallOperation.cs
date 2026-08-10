using EtherSharp.ABI.Types;
using EtherSharp.Contract;
using EtherSharp.Numerics;
using EtherSharp.Tx;
using EtherSharp.Types;
using System.Buffers.Binary;

namespace EtherSharp.Query.Operations;

internal sealed class SafeFlashCallQueryOperation<T> : IQuery, IQuery<CallResult<T>>
{
    private readonly IFlashCall<T> _txInput;
    private readonly EVMByteCode _initCode;

    public SafeFlashCallQueryOperation(IFlashCode code, IFlashCall<T> txInput)
    {
        if(code is IContractDeployment deployment && deployment.Value > 0)
        {
            throw new NotSupportedException("Contract deployment cannot contain any value");
        }

        _txInput = txInput;
        _initCode = code.IsRuntimeCode
            ? IFlashCode.CreateInitCode(code.ByteCode)
            : code.ByteCode;

        if(_initCode.Length > EVMByteCode.MAX_INIT_LENGTH)
        {
            throw new InvalidOperationException($"Maximum initcode length exceeded, {_initCode.Length} > {EVMByteCode.MAX_INIT_LENGTH}");
        }
    }

    public int CallDataLength => 1 + 37 + _initCode.Length + _txInput.Data.Length;
    public UInt256 EthValue => _txInput.Value;

    public void Encode(Span<byte> buffer)
    {
        buffer[0] = (byte) QueryOperationId.FlashCall;
        buffer = buffer[1..];

        AbiTypes.UShort.EncodeInto((ushort) _initCode.Length, buffer[0..2]);
        AbiTypes.UInt.EncodeInto((uint) _txInput.Data.Length, buffer[2..5], true);
        BinaryPrimitives.WriteUInt256BigEndian(buffer[5..37], EthValue);

        _initCode.ByteCode.Span.CopyTo(buffer[37..]);
        _txInput.Data.Span.CopyTo(buffer[(37 + _initCode.Length)..]);
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
            true => CallResult<T>.ParseSuccessFrom(returnData, null, _txInput.ReadResultFrom),
            false => new CallResult<T>.Reverted(null, returnData)
        };
    }
}
