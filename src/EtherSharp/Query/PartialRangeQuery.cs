using EtherSharp.Common.Exceptions;
using EtherSharp.Numerics;
using EtherSharp.Query.Operations;
using EtherSharp.Tx;
using EtherSharp.Types;
using System.Buffers.Binary;

namespace EtherSharp.Query;

internal sealed class PartialRangeQuery<T> : IQueryOperation, IQuery<PartialRangeResult<T>>
{
    // Input: opcode (1), inner calldata byte length (3), additional gas reserve (4), memory limit (4), call records.
    // Each call: calldata byte length (3), target (20), value (32), calldata. No inner opcode.
    // Output: result byte length (3), then return-data length (3) and return data per accepted call.
    // All integers are big-endian. Failed calls are not included.
    private const int HEADER_LENGTH = 3 * sizeof(uint);
    private const int RESULT_HEADER_LENGTH = 3;

    private readonly ReadOnlyMemory<IContractCall<T>> _calls;
    private readonly uint _additionalGasReserve;
    private readonly uint _memoryLimit;
    private readonly int _innerCallDataLength;

    public QueryOperationId OperationId => QueryOperationId.PartialRange;
    public UInt256 EthValue { get; private set; }
    int IQuery.OperationCount => 1;

    public PartialRangeQuery(ReadOnlyMemory<IContractCall<T>> calls, ulong additionalGasReserve, uint memoryLimit)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(memoryLimit, 0xFFFFFFU);
        _calls = calls;
        _additionalGasReserve = checked((uint) additionalGasReserve);
        _memoryLimit = memoryLimit;

        foreach(var call in calls.Span)
        {
            _innerCallDataLength = checked(_innerCallDataLength + CallQueryEncoding.GetCallDataLength(call.Data.Length, includeOpcode: false));
            EthValue += call.Value;
        }
    }

    public int GetCallDataLength(QueryEncodingContext context)
        => HEADER_LENGTH + _innerCallDataLength;

    public void Encode(Span<byte> buffer, QueryEncodingContext context)
    {
        BinaryPrimitives.WriteUInt32BigEndian(buffer[..sizeof(uint)], (uint) _innerCallDataLength);
        buffer[0] = (byte) QueryOperationId.PartialRange;
        BinaryPrimitives.WriteUInt32BigEndian(buffer.Slice(sizeof(uint), sizeof(uint)), _additionalGasReserve);
        BinaryPrimitives.WriteUInt32BigEndian(buffer[(2 * sizeof(uint))..HEADER_LENGTH], _memoryLimit);

        var operationBuffer = buffer.Slice(HEADER_LENGTH, _innerCallDataLength);
        foreach(var call in _calls.Span)
        {
            call.Data.Span.CopyTo(CallQueryEncoding.EncodeHeader(
                call.To,
                call.Value,
                call.Data.Length,
                operationBuffer,
                includeOpcode: false
            ));
            operationBuffer = operationBuffer[CallQueryEncoding.GetCallDataLength(call.Data.Length, includeOpcode: false)..];
        }
    }

    public int ParseResultLength(ReadOnlySpan<byte> resultData)
        => RESULT_HEADER_LENGTH + ((resultData[0] << 16) | (resultData[1] << 8) | resultData[2]);

    PartialRangeResult<T> IQuery<PartialRangeResult<T>>.ReadResultFrom(
        params scoped ReadOnlySpan<ReadOnlyMemory<byte>> queryResults
    )
    {
        var returnData = queryResults[0][RESULT_HEADER_LENGTH..];
        var callResults = new ReadOnlyMemory<byte>[_calls.Length];
        int offset = 0;
        int count = 0;

        try
        {
            while(count < _calls.Length && offset < returnData.Length)
            {
                int length = ParseResultLength(returnData.Span[offset..]) - RESULT_HEADER_LENGTH;
                offset += RESULT_HEADER_LENGTH;
                callResults[count] = returnData.Slice(offset, length);
                offset += length;
                count++;
            }
        }
        catch(Exception exception) when(exception is ArgumentException or IndexOutOfRangeException)
        {
            throw new CallParsingException.MalformedReturnDataException(returnData, exception);
        }

        if(offset != returnData.Length)
        {
            throw new CallParsingException.RemainingReturnDataException(returnData, offset);
        }

        var items = new T[count];
        for(int index = 0; index < count; index++)
        {
            items[index] = _calls.Span[index].ReadResultFrom(callResults[index]);
        }

        return new PartialRangeResult<T>(items);
    }
}
