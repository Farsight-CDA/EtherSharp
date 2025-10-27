using EtherSharp.ABI.Types;
using EtherSharp.RPC.Modules.Eth;
using EtherSharp.Tx;
using EtherSharp.Types;
using Microsoft.Extensions.Logging;
using System.Buffers.Binary;

namespace EtherSharp.Client.Modules.Query;

internal class QueryBuilder<TQuery>(IEthRpcModule rpc, ILogger? logger) : IQueryBuilder<TQuery>
{
    private readonly static byte[] _querierBytecode = Convert.FromHexString(
        "608060405234610095576102a880380380610019816100ad565b928339810190602081830312610095578051906001600160401b038211610095570181601f820112156100955780519061005a610055836100d7565b6100ad565b9282845260208383010111610095575f5b82811061008057835f60208583010152610192565b8060208092840101518282870101520161006b565b5f80fd5b634e487b7160e01b5f52604160045260245ffd5b6040519190601f01601f191682016001600160401b038111838210176100d257604052565b610099565b6001600160401b0381116100d257601f01601f191660200190565b61600090616020610102816100ad565b838152928391906001600160401b03106100d257601f190190369060200137565b634e487b7160e01b5f52601160045260245ffd5b906017820180921161014557565b610123565b906018820180921161014557565b601801908160181161014557565b600401908160041161014557565b9190820180921161014557565b620f423f1981019190821161014557565b805180156102a5576101a26100f2565b5f925f92602082016020840194621e8480925b8082106101c4575b8787818852f35b9091929396828801926034602085015160601c94015160e01c90836101f16101ec8484610174565b610137565b1015610095575f91818361022161021161020c83968c610174565b61014a565b9261021b85610158565b90610174565b9761022b5a610181565bf1903d865a1061029d5761023e81610166565b9261600061024c8585610174565b1161029457905f6004849361026561028a97968e610174565b9081538360101c60018201538360081c600282015360ff84166003820153013e610174565b96939291906101b5565b505097506101bd565b5097506101bd565b00fe"
    );
    private const int MAX_PAYLOAD_SIZE = 32 * 1024;

    private readonly IEthRpcModule _rpc = rpc;
    private readonly ILogger? _logger = logger;

    private readonly List<ITxInput> _calls = [];
    private readonly List<(int, Func<ReadOnlySpan<TxCallResult>, TQuery>)> _resultSelectorFunctions = [];

    public IQueryBuilder<TQuery> AddQuery(ITxInput<TQuery> c)
    {
        int resultIndex = _calls.Count;
        _calls.Add(c);
        _resultSelectorFunctions.Add((resultIndex, x => c.ReadResultFrom(x[0].Unwrap(c.To))));

        return this;
    }

    internal QueryBuilder<TQuery> AddCallable<T1>(ICallable<T1> c, Func<T1, TQuery> mapping)
    {
        if(c is ITxInput<T1> t)
        {
            AddQuery(t, mapping);
            return this;
        }
        else if(c is QueryBuilder<T1> q)
        {
            int resultIndex = _calls.Count;
            _calls.AddRange(q._calls);

            foreach(var (index, selector) in q._resultSelectorFunctions)
            {
                _resultSelectorFunctions.Add((
                    index + resultIndex,
                    x => mapping(selector(x))
                ));
            }

            return this;
        }
        else
        {
            throw new NotSupportedException();
        }
    }

    public IQueryBuilder<TQuery> AddSafeQuery(ITxInput<TQuery> c, Func<QueryResult<TQuery>, TQuery> mapping)
    {
        int resultIndex = _calls.Count;
        _calls.Add(c);

        TQuery selector(ReadOnlySpan<TxCallResult> x) => mapping.Invoke(
            QueryResult<TQuery>.Parse(x[0], c.ReadResultFrom)
        );

        _resultSelectorFunctions.Add((resultIndex, selector));
        return this;
    }

    public IQueryBuilder<TQuery> AddQuery<T1>(ITxInput<T1> c1, Func<T1, TQuery> mapping)
    {
        int resultIndex = _calls.Count;
        _calls.AddRange(c1);

        TQuery selector(ReadOnlySpan<TxCallResult> x) => mapping.Invoke(
            c1.ReadResultFrom(x[0].Unwrap(c1.To))
        );

        _resultSelectorFunctions.Add((resultIndex, selector));
        return this;
    }

    public IQueryBuilder<TQuery> AddSafeQuery<T1>(
        ITxInput<T1> c1,
        Func<QueryResult<T1>, TQuery> mapping)
    {
        int resultIndex = _calls.Count;
        _calls.AddRange(c1);

        TQuery selector(ReadOnlySpan<TxCallResult> x) => mapping.Invoke(
            QueryResult<T1>.Parse(x[0], c1.ReadResultFrom)
        );

        _resultSelectorFunctions.Add((resultIndex, selector));
        return this;
    }

    public IQueryBuilder<TQuery> AddQuery<T1, T2>(
        ITxInput<T1> c1, ITxInput<T2> c2,
        Func<T1, T2, TQuery> mapping)
    {
        int resultIndex = _calls.Count;
        _calls.AddRange(c1, c2);

        TQuery selector(ReadOnlySpan<TxCallResult> x) => mapping.Invoke(
            c1.ReadResultFrom(x[0].Unwrap(c1.To)),
            c2.ReadResultFrom(x[1].Unwrap(c2.To))
        );

        _resultSelectorFunctions.Add((resultIndex, selector));
        return this;
    }

    public IQueryBuilder<TQuery> AddSafeQuery<T1, T2>(
        ITxInput<T1> c1, ITxInput<T2> c2,
        Func<QueryResult<T1>, QueryResult<T2>, TQuery> mapping)
    {
        int resultIndex = _calls.Count;
        _calls.AddRange(c1, c2);

        TQuery selector(ReadOnlySpan<TxCallResult> x) => mapping.Invoke(
            QueryResult<T1>.Parse(x[0], c1.ReadResultFrom),
            QueryResult<T2>.Parse(x[1], c2.ReadResultFrom)
        );

        _resultSelectorFunctions.Add((resultIndex, selector));
        return this;
    }

    public IQueryBuilder<TQuery> AddQuery<T1, T2, T3>(
        ITxInput<T1> c1, ITxInput<T2> c2, ITxInput<T3> c3,
        Func<T1, T2, T3, TQuery> mapping)
    {
        int resultIndex = _calls.Count;
        _calls.AddRange(c1, c2, c3);

        TQuery selector(ReadOnlySpan<TxCallResult> x) => mapping.Invoke(
            c1.ReadResultFrom(x[0].Unwrap(c1.To)),
            c2.ReadResultFrom(x[1].Unwrap(c2.To)),
            c3.ReadResultFrom(x[2].Unwrap(c3.To))
        );

        _resultSelectorFunctions.Add((resultIndex, selector));
        return this;
    }

    public IQueryBuilder<TQuery> AddSafeQuery<T1, T2, T3>(
        ITxInput<T1> c1, ITxInput<T2> c2, ITxInput<T3> c3,
        Func<QueryResult<T1>, QueryResult<T2>, QueryResult<T3>, TQuery> mapping)
    {
        int resultIndex = _calls.Count;
        _calls.AddRange(c1, c2, c3);

        TQuery selector(ReadOnlySpan<TxCallResult> x) => mapping.Invoke(
            QueryResult<T1>.Parse(x[0], c1.ReadResultFrom),
            QueryResult<T2>.Parse(x[1], c2.ReadResultFrom),
            QueryResult<T3>.Parse(x[2], c3.ReadResultFrom)
        );

        _resultSelectorFunctions.Add((resultIndex, selector));
        return this;
    }

    public IQueryBuilder<TQuery> AddQuery<T1, T2, T3, T4>(
        ITxInput<T1> c1, ITxInput<T2> c2, ITxInput<T3> c3, ITxInput<T4> c4,
        Func<T1, T2, T3, T4, TQuery> mapping)
    {
        int resultIndex = _calls.Count;
        _calls.AddRange(c1, c2, c3, c4);

        TQuery selector(ReadOnlySpan<TxCallResult> x) => mapping.Invoke(
            c1.ReadResultFrom(x[0].Unwrap(c1.To)),
            c2.ReadResultFrom(x[1].Unwrap(c2.To)),
            c3.ReadResultFrom(x[2].Unwrap(c3.To)),
            c4.ReadResultFrom(x[3].Unwrap(c4.To))
        );

        _resultSelectorFunctions.Add((resultIndex, selector));
        return this;
    }

    public IQueryBuilder<TQuery> AddSafeQuery<T1, T2, T3, T4>(
        ITxInput<T1> c1, ITxInput<T2> c2, ITxInput<T3> c3, ITxInput<T4> c4,
        Func<QueryResult<T1>, QueryResult<T2>, QueryResult<T3>, QueryResult<T4>, TQuery> mapping)
    {
        int resultIndex = _calls.Count;
        _calls.AddRange(c1, c2, c3, c4);

        TQuery selector(ReadOnlySpan<TxCallResult> x) => mapping.Invoke(
            QueryResult<T1>.Parse(x[0], c1.ReadResultFrom),
            QueryResult<T2>.Parse(x[1], c2.ReadResultFrom),
            QueryResult<T3>.Parse(x[2], c3.ReadResultFrom),
            QueryResult<T4>.Parse(x[3], c4.ReadResultFrom)
        );

        _resultSelectorFunctions.Add((resultIndex, selector));
        return this;
    }

    public IQueryBuilder<TQuery> AddQuery<T1, T2, T3, T4, T5>(
         ITxInput<T1> c1, ITxInput<T2> c2, ITxInput<T3> c3, ITxInput<T4> c4, ITxInput<T5> c5,
         Func<T1, T2, T3, T4, T5, TQuery> mapping)
    {
        int resultIndex = _calls.Count;
        _calls.AddRange(c1, c2, c3, c4, c5);

        TQuery selector(ReadOnlySpan<TxCallResult> x) => mapping.Invoke(
            c1.ReadResultFrom(x[0].Unwrap(c1.To)),
            c2.ReadResultFrom(x[1].Unwrap(c2.To)),
            c3.ReadResultFrom(x[2].Unwrap(c3.To)),
            c4.ReadResultFrom(x[3].Unwrap(c4.To)),
            c5.ReadResultFrom(x[4].Unwrap(c5.To))
        );

        _resultSelectorFunctions.Add((resultIndex, selector));
        return this;
    }

    public IQueryBuilder<TQuery> AddSafeQuery<T1, T2, T3, T4, T5>(
        ITxInput<T1> c1, ITxInput<T2> c2, ITxInput<T3> c3, ITxInput<T4> c4, ITxInput<T5> c5,
        Func<QueryResult<T1>, QueryResult<T2>, QueryResult<T3>, QueryResult<T4>, QueryResult<T5>, TQuery> mapping)
    {
        int resultIndex = _calls.Count;
        _calls.AddRange(c1, c2, c3, c4, c5);

        TQuery selector(ReadOnlySpan<TxCallResult> x) => mapping.Invoke(
            QueryResult<T1>.Parse(x[0], c1.ReadResultFrom),
            QueryResult<T2>.Parse(x[1], c2.ReadResultFrom),
            QueryResult<T3>.Parse(x[2], c3.ReadResultFrom),
            QueryResult<T4>.Parse(x[3], c4.ReadResultFrom),
            QueryResult<T5>.Parse(x[4], c5.ReadResultFrom)
        );

        _resultSelectorFunctions.Add((resultIndex, selector));
        return this;
    }

    public IQueryBuilder<TQuery> AddQuery<T1, T2, T3, T4, T5, T6>(
         ITxInput<T1> c1, ITxInput<T2> c2, ITxInput<T3> c3, ITxInput<T4> c4, ITxInput<T5> c5, ITxInput<T6> c6,
         Func<T1, T2, T3, T4, T5, T6, TQuery> mapping)
    {
        int resultIndex = _calls.Count;
        _calls.AddRange(c1, c2, c3, c4, c5, c6);

        TQuery selector(ReadOnlySpan<TxCallResult> x) => mapping.Invoke(
            c1.ReadResultFrom(x[0].Unwrap(c1.To)),
            c2.ReadResultFrom(x[1].Unwrap(c2.To)),
            c3.ReadResultFrom(x[2].Unwrap(c3.To)),
            c4.ReadResultFrom(x[3].Unwrap(c4.To)),
            c5.ReadResultFrom(x[4].Unwrap(c5.To)),
            c6.ReadResultFrom(x[5].Unwrap(c6.To))
        );

        _resultSelectorFunctions.Add((resultIndex, selector));
        return this;
    }

    public IQueryBuilder<TQuery> AddSafeQuery<T1, T2, T3, T4, T5, T6>(
        ITxInput<T1> c1, ITxInput<T2> c2, ITxInput<T3> c3, ITxInput<T4> c4, ITxInput<T5> c5, ITxInput<T6> c6,
        Func<QueryResult<T1>, QueryResult<T2>, QueryResult<T3>, QueryResult<T4>, QueryResult<T5>, QueryResult<T6>, TQuery> mapping)
    {
        int resultIndex = _calls.Count;
        _calls.AddRange(c1, c2, c3, c4, c5);

        TQuery selector(ReadOnlySpan<TxCallResult> x) => mapping.Invoke(
            QueryResult<T1>.Parse(x[0], c1.ReadResultFrom),
            QueryResult<T2>.Parse(x[1], c2.ReadResultFrom),
            QueryResult<T3>.Parse(x[2], c3.ReadResultFrom),
            QueryResult<T4>.Parse(x[3], c4.ReadResultFrom),
            QueryResult<T5>.Parse(x[4], c5.ReadResultFrom),
            QueryResult<T6>.Parse(x[5], c6.ReadResultFrom)
        );

        _resultSelectorFunctions.Add((resultIndex, selector));
        return this;
    }

    public IQueryBuilder<TQuery> AddQuery<T1, T2, T3, T4, T5, T6, T7>(
    ITxInput<T1> c1, ITxInput<T2> c2, ITxInput<T3> c3, ITxInput<T4> c4, ITxInput<T5> c5, ITxInput<T6> c6, ITxInput<T7> c7,
    Func<T1, T2, T3, T4, T5, T6, T7, TQuery> mapping)
    {
        int resultIndex = _calls.Count;
        _calls.AddRange(c1, c2, c3, c4, c5, c6, c7);

        TQuery selector(ReadOnlySpan<TxCallResult> x) => mapping.Invoke(
            c1.ReadResultFrom(x[0].Unwrap(c1.To)),
            c2.ReadResultFrom(x[1].Unwrap(c2.To)),
            c3.ReadResultFrom(x[2].Unwrap(c3.To)),
            c4.ReadResultFrom(x[3].Unwrap(c4.To)),
            c5.ReadResultFrom(x[4].Unwrap(c5.To)),
            c6.ReadResultFrom(x[5].Unwrap(c6.To)),
            c7.ReadResultFrom(x[6].Unwrap(c7.To))
        );

        _resultSelectorFunctions.Add((resultIndex, selector));
        return this;
    }

    public IQueryBuilder<TQuery> AddSafeQuery<T1, T2, T3, T4, T5, T6, T7>(
        ITxInput<T1> c1, ITxInput<T2> c2, ITxInput<T3> c3, ITxInput<T4> c4, ITxInput<T5> c5, ITxInput<T6> c6, ITxInput<T7> c7,
        Func<QueryResult<T1>, QueryResult<T2>, QueryResult<T3>, QueryResult<T4>, QueryResult<T5>, QueryResult<T6>, QueryResult<T7>, TQuery> mapping)
    {
        int resultIndex = _calls.Count;
        _calls.AddRange(c1, c2, c3, c4, c5, c6, c7);

        TQuery selector(ReadOnlySpan<TxCallResult> x) => mapping.Invoke(
            QueryResult<T1>.Parse(x[0], c1.ReadResultFrom),
            QueryResult<T2>.Parse(x[1], c2.ReadResultFrom),
            QueryResult<T3>.Parse(x[2], c3.ReadResultFrom),
            QueryResult<T4>.Parse(x[3], c4.ReadResultFrom),
            QueryResult<T5>.Parse(x[4], c5.ReadResultFrom),
            QueryResult<T6>.Parse(x[5], c6.ReadResultFrom),
            QueryResult<T7>.Parse(x[6], c7.ReadResultFrom)
        );

        _resultSelectorFunctions.Add((resultIndex, selector));
        return this;
    }

    public IQueryBuilder<TQuery> AddQuery<T1, T2, T3, T4, T5, T6, T7, T8>(
        ITxInput<T1> c1, ITxInput<T2> c2, ITxInput<T3> c3, ITxInput<T4> c4, ITxInput<T5> c5, ITxInput<T6> c6, ITxInput<T7> c7, ITxInput<T8> c8,
        Func<T1, T2, T3, T4, T5, T6, T7, T8, TQuery> mapping)
    {
        int resultIndex = _calls.Count;
        _calls.AddRange(c1, c2, c3, c4, c5, c6, c7, c8);

        TQuery selector(ReadOnlySpan<TxCallResult> x) => mapping.Invoke(
            c1.ReadResultFrom(x[0].Unwrap(c1.To)),
            c2.ReadResultFrom(x[1].Unwrap(c2.To)),
            c3.ReadResultFrom(x[2].Unwrap(c3.To)),
            c4.ReadResultFrom(x[3].Unwrap(c4.To)),
            c5.ReadResultFrom(x[4].Unwrap(c5.To)),
            c6.ReadResultFrom(x[5].Unwrap(c6.To)),
            c7.ReadResultFrom(x[6].Unwrap(c7.To)),
            c8.ReadResultFrom(x[7].Unwrap(c8.To))
        );

        _resultSelectorFunctions.Add((resultIndex, selector));
        return this;
    }

    public IQueryBuilder<TQuery> AddSafeQuery<T1, T2, T3, T4, T5, T6, T7, T8>(
        ITxInput<T1> c1, ITxInput<T2> c2, ITxInput<T3> c3, ITxInput<T4> c4, ITxInput<T5> c5, ITxInput<T6> c6, ITxInput<T7> c7, ITxInput<T8> c8,
        Func<QueryResult<T1>, QueryResult<T2>, QueryResult<T3>, QueryResult<T4>, QueryResult<T5>, QueryResult<T6>, QueryResult<T7>, QueryResult<T8>, TQuery> mapping)
    {
        int resultIndex = _calls.Count;
        _calls.AddRange(c1, c2, c3, c4, c5, c6, c7, c8);

        TQuery selector(ReadOnlySpan<TxCallResult> x) => mapping.Invoke(
            QueryResult<T1>.Parse(x[0], c1.ReadResultFrom),
            QueryResult<T2>.Parse(x[1], c2.ReadResultFrom),
            QueryResult<T3>.Parse(x[2], c3.ReadResultFrom),
            QueryResult<T4>.Parse(x[3], c4.ReadResultFrom),
            QueryResult<T5>.Parse(x[4], c5.ReadResultFrom),
            QueryResult<T6>.Parse(x[5], c6.ReadResultFrom),
            QueryResult<T7>.Parse(x[6], c7.ReadResultFrom),
            QueryResult<T8>.Parse(x[7], c8.ReadResultFrom)
        );

        _resultSelectorFunctions.Add((resultIndex, selector));
        return this;
    }

    public IQueryBuilder<TQuery> AddQuery<T1, T2, T3, T4, T5, T6, T7, T8, T9>(
        ITxInput<T1> c1, ITxInput<T2> c2, ITxInput<T3> c3, ITxInput<T4> c4, ITxInput<T5> c5, ITxInput<T6> c6, ITxInput<T7> c7, ITxInput<T8> c8, ITxInput<T9> c9,
        Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TQuery> mapping)
    {
        int resultIndex = _calls.Count;
        _calls.AddRange(c1, c2, c3, c4, c5, c6, c7, c8, c9);

        TQuery selector(ReadOnlySpan<TxCallResult> x) => mapping.Invoke(
            c1.ReadResultFrom(x[0].Unwrap(c1.To)),
            c2.ReadResultFrom(x[1].Unwrap(c2.To)),
            c3.ReadResultFrom(x[2].Unwrap(c3.To)),
            c4.ReadResultFrom(x[3].Unwrap(c4.To)),
            c5.ReadResultFrom(x[4].Unwrap(c5.To)),
            c6.ReadResultFrom(x[5].Unwrap(c6.To)),
            c7.ReadResultFrom(x[6].Unwrap(c7.To)),
            c8.ReadResultFrom(x[7].Unwrap(c8.To)),
            c9.ReadResultFrom(x[8].Unwrap(c9.To))
        );

        _resultSelectorFunctions.Add((resultIndex, selector));
        return this;
    }

    public IQueryBuilder<TQuery> AddSafeQuery<T1, T2, T3, T4, T5, T6, T7, T8, T9>(
        ITxInput<T1> c1, ITxInput<T2> c2, ITxInput<T3> c3, ITxInput<T4> c4, ITxInput<T5> c5, ITxInput<T6> c6, ITxInput<T7> c7, ITxInput<T8> c8, ITxInput<T9> c9,
        Func<QueryResult<T1>, QueryResult<T2>, QueryResult<T3>, QueryResult<T4>, QueryResult<T5>, QueryResult<T6>, QueryResult<T7>, QueryResult<T8>, QueryResult<T9>, TQuery> mapping)
    {
        int resultIndex = _calls.Count;
        _calls.AddRange(c1, c2, c3, c4, c5, c6, c7, c8, c9);

        TQuery selector(ReadOnlySpan<TxCallResult> x) => mapping.Invoke(
            QueryResult<T1>.Parse(x[0], c1.ReadResultFrom),
            QueryResult<T2>.Parse(x[1], c2.ReadResultFrom),
            QueryResult<T3>.Parse(x[2], c3.ReadResultFrom),
            QueryResult<T4>.Parse(x[3], c4.ReadResultFrom),
            QueryResult<T5>.Parse(x[4], c5.ReadResultFrom),
            QueryResult<T6>.Parse(x[5], c6.ReadResultFrom),
            QueryResult<T7>.Parse(x[6], c7.ReadResultFrom),
            QueryResult<T8>.Parse(x[7], c8.ReadResultFrom),
            QueryResult<T9>.Parse(x[8], c9.ReadResultFrom)
        );

        _resultSelectorFunctions.Add((resultIndex, selector));
        return this;
    }

    private static byte[] EncodeCalls(IEnumerable<ITxInput> calls)
    {
        int dataLength = 0;

        int callCount = 0;
        foreach(var call in calls)
        {
            dataLength += 20 + 4 + call.Data.Length;
            callCount++;

            if(dataLength > MAX_PAYLOAD_SIZE)
            {
                break;
            }
        }

        byte[] arr = new byte[dataLength + _querierBytecode.Length + 64];
        var buffer = arr.AsSpan();

        _querierBytecode.CopyTo(buffer);
        buffer = buffer[_querierBytecode.Length..];
        AbiTypes.BigInteger.EncodeInto(32, true, buffer[0..32]);
        AbiTypes.BigInteger.EncodeInto(dataLength, true, buffer[32..64]);
        buffer = buffer[64..];

        foreach(var call in calls)
        {
            call.To.Bytes.CopyTo(buffer[0..20]);
            BinaryPrimitives.WriteUInt32BigEndian(buffer[20..24], (uint) call.Data.Length);
            call.Data.CopyTo(buffer[24..]);

            buffer = buffer[(24 + call.Data.Length)..];

            callCount--;
            if(callCount == 0)
            {
                break;
            }
        }

        return arr;
    }

    private List<TQuery> ParseResults(TxCallResult[] outputs)
    {
        var results = new List<TQuery>(_resultSelectorFunctions.Count);

        foreach(var (offset, selector) in _resultSelectorFunctions)
        {
            results.Add(selector.Invoke(outputs.AsSpan(offset)));
        }

        return results;
    }

    public async Task<List<TQuery>> QueryAsync(TargetBlockNumber targetBlockNumber, CancellationToken cancellationToken)
    {
        Span<byte> buffer = [];
        var outputs = new TxCallResult[_calls.Count];
        byte[] lengthBuffer = new byte[4];

        int requestCount = 0;

        for(int i = 0; i < _calls.Count; i++)
        {
            if(buffer.Length == 0)
            {
                requestCount++;
                var callResult = await _rpc.CallAsync(
                    Address.Zero,
                    null!,
                    null,
                    null,
                    0,
                    $"0x{Convert.ToHexString(EncodeCalls(_calls.Skip(i)))}",
                    targetBlockNumber,
                    cancellationToken
                );

                byte[] output = callResult.Unwrap(Address.Zero);

                if(output.Length == 0)
                {
                    throw new InvalidOperationException("Call is too expensive to be executed within batch");
                }

                buffer = output.AsSpan();
            }

            bool success = buffer[0] == 0x01;
            buffer[1..4].CopyTo(lengthBuffer.AsSpan(1));
            int dataLength = (int) BinaryPrimitives.ReadUInt32BigEndian(lengthBuffer);
            var data = buffer[4..(4 + dataLength)];

            outputs[i] = success switch
            {
                true => new TxCallResult.Success(data.ToArray()),
                false => new TxCallResult.Reverted(data.ToArray())
            };

            buffer = buffer[(4 + dataLength)..];
        }

        _logger?.LogTrace("Batch query processing completed using {requests} request(s)", requestCount);

        if(requestCount > 1)
        {
            _logger?.LogDebug("Batch query processing too expensive, required {requests} requests", requestCount);
        }

        return ParseResults(outputs);
    }
}
