using EtherSharp.ABI.Types;
using EtherSharp.Common.Exceptions;
using EtherSharp.RPC.Modules.Eth;
using EtherSharp.Tx;
using EtherSharp.Types;
using System.Buffers.Binary;

namespace EtherSharp.Client.Modules.Query;

internal class QueryBuilder<TQuery>(IEthRpcModule rpc) : IQueryBuilder<TQuery>
{
    private readonly static byte[] _querierBytecode = Convert.FromHexString(
        "608060405234610085576103a7803803809161001a8261009d565b6080396020816080019112610085576080516001600160401b0381116100855781609f82011215610085578060800151610053816100eb565b9261006160405194856100c8565b81845260a08284010111610085576100809160a0602085019101610106565b610253565b5f80fd5b634e487b7160e01b5f52604160045260245ffd5b6080601f91909101601f19168101906001600160401b038211908210176100c357604052565b610089565b601f909101601f19168101906001600160401b038211908210176100c357604052565b6001600160401b0381116100c357601f01601f191660200190565b5f5b8381106101175750505f910152565b8181015183820152602001610108565b90610131826100eb565b61013e60405191826100c8565b828152809261014f601f19916100eb565b0190602036910137565b634e487b7160e01b5f52601160045260245ffd5b906018820180921161017b57565b610159565b9190820180921161017b57565b90815181101561019e570160200190565b634e487b7160e01b5f52603260045260245ffd5b63ffffffff166018019063ffffffff821161017b57565b620249ef1981019190821161017b57565b3d15610204573d906101eb826100eb565b916101f960405193846100c8565b82523d5f602084013e565b606090565b602090610220600596959382815194859201610106565b0191151560f81b825263ffffffff60e01b9060e01b16600182015261024e8251809360208685019101610106565b010190565b620493e0905f9060605b815183101561039d57828201926034602085015160601c94015160e01c9061028482610127565b915f5b8181106103345750916102b25f93926102ac6102a386956101b2565b63ffffffff1690565b90610180565b95826102bd5a6101c9565b9160208451940192f1906102cf6101da565b91855a106103295761030b906102fd6102ec855163ffffffff1690565b946040519586938660208601610209565b03601f1981018452836100c8565b61600082511161031b575061025d565b93505050505b602081519101f35b509350505050610321565b8061038261035c61035660019461035188999b97989c9a9c61016d565b610180565b8561018d565b517fff000000000000000000000000000000000000000000000000000000000000001690565b5f1a61038e828961018d565b53019593959492919094610287565b9250505061032156fe"
    );

    private readonly IEthRpcModule _rpc = rpc;
    private readonly List<ITxInput> _calls = [];
    private readonly List<(int, Func<ReadOnlySpan<byte[]>, TQuery>)> _resultSelectorFunctions = [];

    IEnumerable<ITxInput> ICallable<List<TQuery>>.GetCalls()
        => _calls;
    Func<ReadOnlySpan<byte[]>, List<TQuery>> ICallable<List<TQuery>>.GetResultSelector()
        => outputs =>
           {
               var results = new List<TQuery>();

               foreach(var (offset, selector) in _resultSelectorFunctions)
               {
                   var selectorOutputs = outputs[offset..];
                   results.Add(selector.Invoke(selectorOutputs));
               }

               return results;
           };

    public IQueryBuilder<TQuery> AddQuery(ICallable<TQuery> callable)
    {
        int resultIndex = _calls.Count;
        _calls.AddRange(callable.GetCalls());
        _resultSelectorFunctions.Add((resultIndex, callable.GetResultSelector()));

        return this;
    }

    public IQueryBuilder<TQuery> AddQueries(params IEnumerable<ICallable<TQuery>> callables)
    {
        foreach(var callable in callables)
        {
            AddQuery(callable);
        }
        return this;
    }

    public IQueryBuilder<TQuery> AddQuery<T1>(ICallable<T1> c1, Func<T1, TQuery> mapping)
    {
        int resultIndex = _calls.Count;
        _calls.AddRange(c1.GetCalls());
        var resultSelector = c1.GetResultSelector();
        _resultSelectorFunctions.Add((resultIndex, x => mapping.Invoke(resultSelector(x))));
        return this;
    }

    public IQueryBuilder<TQuery> AddQuery<T1, T2>(ITxInput<T1> c1, ITxInput<T2> c2, Func<T1, T2, TQuery> mapping)
    {
        int resultIndex = _calls.Count;
        _calls.Add(c1);
        _calls.Add(c2);
        _resultSelectorFunctions.Add((
            resultIndex,
            x => mapping(c1.ReadResultFrom(x[0]), c2.ReadResultFrom(x[1]))
        ));
        return this;
    }

    public IQueryBuilder<TQuery> AddQuery<T1, T2, T3>(ITxInput<T1> c1, ITxInput<T2> c2, ITxInput<T3> c3, Func<T1, T2, T3, TQuery> mapping)
    {
        int resultIndex = _calls.Count;
        _calls.Add(c1);
        _calls.Add(c2);
        _calls.Add(c3);
        _resultSelectorFunctions.Add((
            resultIndex,
            x => mapping(c1.ReadResultFrom(x[0]), c2.ReadResultFrom(x[1]), c3.ReadResultFrom(x[2]))
        ));
        return this;
    }

    private static byte[] EncodeCalls(IEnumerable<ITxInput> calls)
    {
        int dataLength = 0;

        foreach(var call in calls)
        {
            dataLength += 20 + 4 + call.Data.Length;
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
        }

        return arr;
    }

    private List<TQuery> ParseResults(byte[][] outputs)
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
        byte[][] outputs = new byte[_calls.Count][];

        for(int i = 0; i < _calls.Count; i++)
        {
            if(buffer.Length == 0)
            {
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

        return ParseResults(outputs);
    }
}
