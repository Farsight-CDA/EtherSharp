using EtherSharp.Common.Exceptions;
using EtherSharp.Numerics;
using EtherSharp.Query.Operations;
using EtherSharp.Types;
using System.Buffers.Binary;

namespace EtherSharp.Query;

internal sealed class IsolatedQuery<T> : IQueryOperation, IQuery<T>
{
    private const int HEADER_LENGTH = 4 + Bytes32.BYTE_LENGTH;
    private const byte TERMINATOR = Byte.MaxValue;

    private readonly IQuery<T> _query;
    private readonly QueryPlan _innerPlan;

    public QueryOperationId OperationId => QueryOperationId.Isolate;
    public UInt256 EthValue { get; private set; }
    int IQuery.OperationCount => 1;

    public IsolatedQuery(IQuery<T> query)
    {
        _query = query;
        _innerPlan = new QueryPlan(query.OperationCount);
        _innerPlan.Add(query);

        foreach(var operation in _innerPlan.Queries)
        {
            EthValue += operation.EthValue;
        }
    }

    void IQuery.AddTo(IQueryPlan plan)
    {
        plan.AddRequirements(
            _innerPlan.StateOverrides,
            _innerPlan.FlashCodes,
            _innerPlan.RequiresQuerierStateOverride,
            _innerPlan.RequiredOperationIds
        );
        plan.AddOperation(this);
    }

    public int GetCallDataLength(QueryEncodingContext context)
        => HEADER_LENGTH + _innerPlan.GetCallDataLength(context) + 1;

    public void Encode(Span<byte> buffer, QueryEncodingContext context)
    {
        int innerLength = buffer.Length - HEADER_LENGTH;
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
            int length = operation.GetCallDataLength(context);
            operation.Encode(operationBuffer[..length], context);
            operationBuffer = operationBuffer[length..];
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
