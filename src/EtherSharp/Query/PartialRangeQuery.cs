using EtherSharp.Common.Exceptions;
using EtherSharp.Numerics;
using EtherSharp.Types;
using System.Buffers.Binary;

namespace EtherSharp.Query;

internal sealed class PartialRangeQuery<T> : IQueryOperation, IQuery<PartialRangeResult<T>>
{
    // Input: opcode (1), inner calldata byte length (3), additional gas reserve (4), inner calldata.
    // Output: operation-result byte length (3), operation results. All integers are big-endian.
    private const int HEADER_LENGTH = 2 * sizeof(uint);
    private const int RESULT_HEADER_LENGTH = 3;

    private readonly ReadOnlyMemory<IQuery<T>> _queries;
    private readonly ulong _additionalGasReserve;
    private readonly QueryPlan _innerPlan;
    private readonly int _innerCallDataLength;

    public int CallDataLength
        => HEADER_LENGTH + _innerCallDataLength;
    public UInt256 EthValue { get; private set; }
    int IQuery.OperationCount => 1;

    public PartialRangeQuery(ReadOnlyMemory<IQuery<T>> queries, ulong additionalGasReserve)
    {
        _queries = queries;
        _additionalGasReserve = additionalGasReserve;
        _innerPlan = new QueryPlan(queries.Length);

        foreach(var query in queries.Span)
        {
            _innerPlan.Add(query);
        }

        foreach(var operation in _innerPlan.Queries)
        {
            _innerCallDataLength += operation.CallDataLength;
            EthValue += operation.EthValue;
        }
    }

    void IQuery.AddTo(IQueryPlan plan)
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
        BinaryPrimitives.WriteUInt32BigEndian(buffer[..sizeof(uint)], (uint) _innerCallDataLength);
        if(buffer[0] != 0)
        {
            throw new InvalidOperationException("Calldata too large");
        }

        buffer[0] = (byte) QueryOperationId.PartialRange;
        BinaryPrimitives.WriteUInt32BigEndian(buffer[sizeof(uint)..HEADER_LENGTH], checked((uint) _additionalGasReserve));

        var operationBuffer = buffer.Slice(HEADER_LENGTH, _innerCallDataLength);
        foreach(var operation in _innerPlan.Queries)
        {
            operation.Encode(operationBuffer);
            operationBuffer = operationBuffer[operation.CallDataLength..];
        }
    }

    public int ParseResultLength(ReadOnlySpan<byte> resultData)
        => RESULT_HEADER_LENGTH + ((resultData[0] << 16) | (resultData[1] << 8) | resultData[2]);

    PartialRangeResult<T> IQuery<PartialRangeResult<T>>.ReadResultFrom(
        params scoped ReadOnlySpan<ReadOnlyMemory<byte>> queryResults
    )
    {
        var returnData = queryResults[0][RESULT_HEADER_LENGTH..];
        var innerResults = new ReadOnlyMemory<byte>[_innerPlan.Count];
        int offset = 0;
        int operationCount = 0;

        try
        {
            while(operationCount < _innerPlan.Count && offset < returnData.Length)
            {
                int length = _innerPlan.Queries[operationCount].ParseResultLength(returnData.Span[offset..]);
                innerResults[operationCount] = returnData.Slice(offset, length);
                offset += length;
                operationCount++;
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

        List<T> items = [];
        int resultOffset = 0;
        foreach(var query in _queries.Span)
        {
            int count = query.OperationCount;
            if(count > operationCount - resultOffset)
            {
                break;
            }

            items.Add(query.ReadResultFrom(innerResults.AsSpan(resultOffset, count)));
            resultOffset += count;
        }

        return new PartialRangeResult<T>(items);
    }
}
