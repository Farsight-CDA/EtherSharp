using EtherSharp.Common.Exceptions;
using EtherSharp.Contract;
using EtherSharp.Crypto;
using EtherSharp.Numerics;
using EtherSharp.Query.Operations;
using EtherSharp.Types;

namespace EtherSharp.Query;

internal sealed class WithCallerQuery<T> : IQuery, IQuery<T>
{
    private static Address QuerierAddress { get; } = GetTransientAddress(IQuerier.Code.London.Runtime.ByteCode.Span);

    private readonly Address _caller;
    private readonly IQuery<T> _query;
    private readonly QueryPlan _innerPlan;
    private readonly EVMByteCode? _originalByteCode;
    private readonly int _innerCallDataLength;

    public WithCallerQuery(in Address caller, IQuery<T> query, EVMByteCode? originalByteCode)
    {
        _caller = caller;
        _query = query;
        _originalByteCode = originalByteCode;
        _innerPlan = new QueryPlan(query.OperationCount);
        _innerPlan.Add(query);

        foreach(var operation in _innerPlan.Queries)
        {
            _innerCallDataLength += operation.CallDataLength;
            EthValue += operation.EthValue;
        }
    }

    public int CallDataLength
        => CallQueryEncoding.GetCallDataLength(IQuerierDelegate.Functions.Query.GetCallDataLength(_innerCallDataLength));

    public UInt256 EthValue { get; private set; }

    int IQuery<T>.OperationCount => 1;

    void IQuery<T>.AddTo(IQueryPlan plan)
    {
        var querierAddress = QuerierAddress;

        if(_innerPlan.StateOverrides is { } innerOverrides)
        {
            foreach(var (address, accountOverride) in innerOverrides)
            {
                plan.AddStateOverride(address, accountOverride);
            }
        }

        plan.AddStateOverride(querierAddress, new AccountOverride(code: IQuerier.Code.London.Runtime.ByteCode));

        if(_originalByteCode is not { } originalByteCode)
        {
            var delegateCode = IQuerierDelegate.Code.Create(in querierAddress);
            plan.AddStateOverride(_caller, new AccountOverride(code: delegateCode.ByteCode));
        }
        else
        {
            var originalCodeAddress = GetTransientAddress(originalByteCode.ByteCode.Span);
            var delegateCode = IQuerierDelegate.Code.CreatePreserving(in querierAddress, in originalCodeAddress);
            plan.AddStateOverride(
                _caller,
                new AccountOverride(code: delegateCode.ByteCode));
            plan.AddStateOverride(originalCodeAddress, new AccountOverride(code: originalByteCode.ByteCode));
        }

        plan.AddOperation(this);
    }

    public void Encode(Span<byte> buffer)
    {
        var callData = CallQueryEncoding.EncodeHeader(
            _caller,
            EthValue,
            IQuerierDelegate.Functions.Query.GetCallDataLength(_innerCallDataLength),
            buffer);
        IQuerierDelegate.Functions.Query.Encode(callData, _innerPlan.Queries);
    }

    public int ParseResultLength(ReadOnlySpan<byte> resultData)
        => CallQueryEncoding.ParseResultLength(resultData);

    T IQuery<T>.ReadResultFrom(params scoped ReadOnlySpan<ReadOnlyMemory<byte>> queryResults)
    {
        var (success, returnData) = CallQueryEncoding.ParseResult(queryResults[0]);
        if(!success)
        {
            throw CallRevertedException.Parse(_caller, returnData.Span);
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
            throw new CallParsingException.MalformedCallDataException(returnData, exception);
        }

        return offset != returnData.Length
            ? throw new CallParsingException.RemainingCallDataException(returnData, offset)
            : _query.ReadResultFrom(innerResults);
    }

    private static Address GetTransientAddress(ReadOnlySpan<byte> code)
    {
        var codeHash = Keccak256.HashData(code);
        return Address.FromBytes(codeHash.DangerousGetReadOnlySpan()[^Address.BYTES_LENGTH..]);
    }
}
