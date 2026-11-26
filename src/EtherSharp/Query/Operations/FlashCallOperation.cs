using EtherSharp.Contract;
using EtherSharp.Numerics;
using EtherSharp.Tx;
using EtherSharp.Types;
using System.Buffers.Binary;

namespace EtherSharp.Query.Operations;

internal sealed class SafeFlashCallQueryOperation<T> : IQueryOperation, IQuery<CallResult<T>>
{
    // With state overrides the code is installed at its own address and encoded as a plain call.
    // Otherwise input: opcode (1), calldata length (3), value (32), initcode hash (32), calldata.
    // Output: success (1), return data length (3), return data.
    private const int HEADER_LENGTH = 1 + 3 + 32 + Bytes32.BYTE_LENGTH;

    private readonly IFlashCall<T> _txInput;
    private readonly Func<ReadOnlyMemory<byte>, T> _readResultFrom;
    private readonly EVMRuntimeCode _runtimeCode;
    private readonly Bytes32 _initCodeHash;
    private readonly Address _stateOverrideAddress;

    public QueryOperationId OperationId => QueryOperationId.FlashCall;
    public UInt256 EthValue => _txInput.Value;

    public SafeFlashCallQueryOperation(EVMRuntimeCode runtimeCode, IFlashCall<T> txInput)
    {
        _txInput = txInput;
        _readResultFrom = txInput.ReadResultFrom;
        _runtimeCode = runtimeCode;
        _initCodeHash = FlashCallEncoding.GetInitCodeHash(runtimeCode);
        _stateOverrideAddress = FlashCallEncoding.GetStateOverrideAddress(_initCodeHash);
    }

    void IQuery.AddTo(IQueryPlan plan)
    {
        plan.AddFlashCode(_stateOverrideAddress, _runtimeCode);
        plan.AddOperation(this);
    }

    public int GetCallDataLength(QueryEncodingContext context)
        => context.UsesStateOverrides
            ? CallQueryEncoding.GetCallDataLength(_txInput.Data.Length)
            : HEADER_LENGTH + _txInput.Data.Length;

    public void Encode(Span<byte> buffer, QueryEncodingContext context)
    {
        if(context.UsesStateOverrides)
        {
            _txInput.Data.Span.CopyTo(CallQueryEncoding.EncodeHeader(
                _stateOverrideAddress,
                EthValue,
                _txInput.Data.Length,
                buffer
            ));
            return;
        }

        if((uint) _txInput.Data.Length > 0xFFFFFF)
        {
            throw new InvalidOperationException("Calldata too large");
        }

        BinaryPrimitives.WriteUInt32BigEndian(buffer[..4], ((uint) QueryOperationId.FlashCall << 24) | (uint) _txInput.Data.Length);
        BinaryPrimitives.WriteUInt256BigEndian(buffer[4..36], EthValue);
        _initCodeHash.CopyTo(buffer[36..HEADER_LENGTH]);
        _txInput.Data.Span.CopyTo(buffer[HEADER_LENGTH..]);
    }

    public int ParseResultLength(ReadOnlySpan<byte> resultData)
        => 4 + ((resultData[1] << 16) | (resultData[2] << 8) | resultData[3]);

    CallResult<T> IQuery<CallResult<T>>.ReadResultFrom(params ReadOnlySpan<ReadOnlyMemory<byte>> queryResults)
    {
        var queryResult = queryResults[0];
        bool success = queryResult.Span[0] == 0x01;
        var returnData = queryResult[4..];

        return success switch
        {
            true => CallResult<T>.ParseSuccessFrom(returnData, null, _readResultFrom),
            false => new CallResult<T>.Reverted(null, returnData)
        };
    }
}
