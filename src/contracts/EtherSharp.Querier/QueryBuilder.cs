using EtherSharp.Client;
using EtherSharp.Common.Exceptions;
using EtherSharp.Tx;
using System.Buffers.Binary;

namespace EtherSharp.Querier;

internal class QueryBuilder<TQuery>(IEtherClient client, IQuerier querier) : IQueryBuilder<TQuery>
{
    private readonly IEtherClient _client = client;
    private readonly IQuerier _querier = querier;
    private readonly List<ITxInput> _calls = [];
    private readonly List<Func<byte[][], TQuery>> _resultSelectorFunctions = [];

    public IQueryBuilder<TQuery> AddQuery(ITxInput<TQuery> call)
    {
        int resultIndex = _calls.Count;
        _calls.Add(call);
        _resultSelectorFunctions.Add(results => call.ReadResultFrom(results[resultIndex]));
        return this;
    }

    public IQueryBuilder<TQuery> AddQueries(params IEnumerable<ITxInput<TQuery>> calls)
    {
        foreach(var call in calls)
        {
            AddQuery(call);
        }
        return this;
    }

    public IQueryBuilder<TQuery> AddQuery<T1>(ITxInput<T1> call1, Func<T1, TQuery> mapping)
    {
        int resultIndex = _calls.Count;
        _calls.Add(call1);
        _resultSelectorFunctions.Add(results => mapping.Invoke(
            call1.ReadResultFrom(results[resultIndex])
        ));
        return this;
    }

    public IQueryBuilder<TQuery> AddQuery<T1, T2>(ITxInput<T1> call1, ITxInput<T2> call2, Func<T1, T2, TQuery> mapping)
    {
        int resultIndex1 = _calls.Count;
        _calls.Add(call1);
        int resultIndex2 = _calls.Count;
        _calls.Add(call2);
        _resultSelectorFunctions.Add(results => mapping.Invoke(
            call1.ReadResultFrom(results[resultIndex1]),
            call2.ReadResultFrom(results[resultIndex2])
        ));
        return this;
    }

    public IQueryBuilder<TQuery> AddQuery<T1, T2, T3>(ITxInput<T1> call1, ITxInput<T2> call2, ITxInput<T3> call3, Func<T1, T2, T3, TQuery> mapping)
    {
        int resultIndex1 = _calls.Count;
        _calls.Add(call1);
        int resultIndex2 = _calls.Count;
        _calls.Add(call2);
        int resultIndex3 = _calls.Count;
        _calls.Add(call3);
        _resultSelectorFunctions.Add(results => mapping.Invoke(
            call1.ReadResultFrom(results[resultIndex1]),
            call2.ReadResultFrom(results[resultIndex2]),
            call3.ReadResultFrom(results[resultIndex3])
        ));
        return this;
    }

    public async Task<TQuery[]> QueryAsync(CancellationToken cancellationToken)
    {
        Span<byte> buffer = [];
        byte[][] outputs = new byte[_calls.Count][];

        for(int i = 0; i < _calls.Count; i++)
        {
            if(buffer.Length == 0)
            {
                var callMsg = _querier.QueryCallsAggregated(EncodeCalls(_calls.Skip(i)));
                byte[] output = await _client.CallAsync(callMsg, cancellationToken: cancellationToken);
                buffer = output.AsSpan();
            }

            bool success = buffer[0] == 0x01;
            int dataLength = (int) BinaryPrimitives.ReadUInt32BigEndian(buffer[1..5]);
            var data = buffer[5..(5 + dataLength)];
            buffer = buffer[(5 + dataLength)..];

            if(!success)
            {
                throw CallRevertedException.Parse(_calls[i].To, data);
            }

            outputs[i] = data.ToArray();
        }

        return [.. _resultSelectorFunctions.Select(x => x.Invoke(outputs))];
    }

    private static byte[] EncodeCalls(IEnumerable<ITxInput> calls)
    {
        int totalLength = 0;

        foreach(var call in calls)
        {
            totalLength += 20 + 4 + call.Data.Length;
        }

        byte[] arr = new byte[totalLength];
        var buffer = arr.AsSpan();

        foreach(var call in calls)
        {
            call.To.Bytes.CopyTo(buffer[0..20]);
            BinaryPrimitives.WriteUInt32BigEndian(buffer[20..24], (uint) call.Data.Length);
            call.Data.CopyTo(buffer[24..]);

            buffer = buffer[(24 + call.Data.Length)..];
        }

        return arr;
    }
}
