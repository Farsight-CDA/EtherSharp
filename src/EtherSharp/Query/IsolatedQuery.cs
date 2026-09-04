using EtherSharp.Common.Exceptions;
using EtherSharp.Numerics;
using EtherSharp.Query.Operations;
using EtherSharp.Types;
using System.Buffers.Binary;

namespace EtherSharp.Query;

internal sealed class IsolatedQuery<T> : IQuery, IQuery<T>
{
    private const int HEADER_LENGTH = 4 + Bytes32.BYTE_LENGTH;
    private const byte TERMINATOR = Byte.MaxValue;

    private readonly IQuery<T> _query;
    private readonly QueryPlan _innerPlan;
    private readonly int _innerCallDataLength;

    public IsolatedQuery(IQuery<T> query)
    {
        _query = query;
        _innerPlan = new QueryPlan(query.OperationCount);
        _innerPlan.Add(query);

        foreach(var operation in _innerPlan.Queries)
        {
            _innerCallDataLength = checked(_innerCallDataLength + operation.CallDataLength);
            EthValue += operation.EthValue;
        }
    }

    public int CallDataLength
        => HEADER_LENGTH + _innerCallDataLength + 1;

    public UInt256 EthValue { get; private set; }

    int IQuery<T>.OperationCount => 1;

    void IQuery<T>.AddTo(IQueryPlan plan)
    {
        if(_innerPlan.StateOverrides is { } innerOverrides)
        {
            foreach(var (address, accountOverride) in innerOverrides)
            {
                plan.AddStateOverride(address, accountOverride);
            }
        }

        plan.AddOperation(this);
    }

    public void Encode(Span<byte> buffer)
    {
        int innerLength = _innerCallDataLength + 1;
        BinaryPrimitives.WriteUInt32BigEndian(buffer[0..4], (uint) innerLength);
        if(buffer[0] != 0)
        {
            throw new InvalidOperationException("Calldata too large");
        }

        buffer[0] = (byte) QueryOperationId.Isolate;
        BinaryPrimitives.WriteUInt256BigEndian(buffer[4..HEADER_LENGTH], EthValue);

        var callData = buffer.Slice(HEADER_LENGTH, innerLength);
        var operationBuffer = callData;
        foreach(var operation in _innerPlan.Queries)
        {
            operation.Encode(operationBuffer);
            operationBuffer = operationBuffer[operation.CallDataLength..];
        }

        callData[^1] = TERMINATOR;
    }

    public int ParseResultLength(ReadOnlySpan<byte> resultData)
        => CallQueryEncoding.ParseResultLength(resultData);

    T IQuery<T>.ReadResultFrom(params scoped ReadOnlySpan<ReadOnlyMemory<byte>> queryResults)
    {
        var (success, returnData) = CallQueryEncoding.ParseResult(queryResults[0]);
        if(success)
        {
            throw new CallParsingException.MalformedReturnDataException(
                returnData,
                "Isolated query returned without reverting");
        }

        var innerResults = new ReadOnlyMemory<byte>[_innerPlan.Count];
        int offset = 0;

        try
        {
            for(int i = 0; i < _innerPlan.Count; i++)
            {
                int length = _innerPlan.Queries[i].ParseResultLength(returnData.Span[offset..]);
                innerResults[i] = returnData.Slice(offset, length);
                offset += length;
            }
        }
        catch(Exception exception) when(exception is ArgumentException or ArgumentOutOfRangeException or IndexOutOfRangeException)
        {
            throw new CallParsingException.MalformedReturnDataException(returnData, exception);
        }

        return offset != returnData.Length
            ? throw new CallParsingException.RemainingReturnDataException(returnData, offset)
            : _query.ReadResultFrom(innerResults);
    }
}
