using EtherSharp.ABI.Types;
using EtherSharp.RPC.Modules.Eth;
using EtherSharp.Types;
using Microsoft.Extensions.Logging;
using System.Buffers.Binary;

namespace EtherSharp.Client.Modules.Query;

internal partial class QueryBuilder<TQuery>(IEthRpcModule rpc, ILogger? logger) : IQueryBuilder<TQuery>
{
    private readonly static byte[] _querierBytecode = Convert.FromHexString(
        "608060405234610095576102a880380380610019816100ad565b928339810190602081830312610095578051906001600160401b038211610095570181601f820112156100955780519061005a610055836100d7565b6100ad565b9282845260208383010111610095575f5b82811061008057835f60208583010152610192565b8060208092840101518282870101520161006b565b5f80fd5b634e487b7160e01b5f52604160045260245ffd5b6040519190601f01601f191682016001600160401b038111838210176100d257604052565b610099565b6001600160401b0381116100d257601f01601f191660200190565b61600090616020610102816100ad565b838152928391906001600160401b03106100d257601f190190369060200137565b634e487b7160e01b5f52601160045260245ffd5b906017820180921161014557565b610123565b906018820180921161014557565b601801908160181161014557565b600401908160041161014557565b9190820180921161014557565b620f423f1981019190821161014557565b805180156102a5576101a26100f2565b5f925f92602082016020840194621e8480925b8082106101c4575b8787818852f35b9091929396828801926034602085015160601c94015160e01c90836101f16101ec8484610174565b610137565b1015610095575f91818361022161021161020c83968c610174565b61014a565b9261021b85610158565b90610174565b9761022b5a610181565bf1903d865a1061029d5761023e81610166565b9261600061024c8585610174565b1161029457905f6004849361026561028a97968e610174565b9081538360101c60018201538360081c600282015360ff84166003820153013e610174565b96939291906101b5565b505097506101bd565b5097506101bd565b00fe"
    );
    private const int MAX_PAYLOAD_SIZE = 32 * 1024;

    private readonly IEthRpcModule _rpc = rpc;
    private readonly ILogger? _logger = logger;

    private readonly List<ICallInput> _calls = [];
    private readonly List<(int, Func<ReadOnlySpan<TxCallResult>, TQuery>)> _resultSelectorFunctions = [];

    private static byte[] EncodeCalls(IEnumerable<ICallInput> calls)
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

    public List<TQuery> ReadResultFrom(ReadOnlySpan<TxCallResult> callResults)
    {
        var results = new List<TQuery>();
        foreach(var (offset, selectorFunc) in _resultSelectorFunctions)
        {
            results.Add(selectorFunc(callResults[offset..]));
        }
        return results;
    }

    public IEnumerable<ICallInput> GetQueryInputs()
        => _calls;

    public IQueryBuilder<TQuery> AddQuery(IQueryable<TQuery> c)
    {
        int resultIndex = _calls.Count;
        _calls.AddRange(c.GetQueryInputs());
        _resultSelectorFunctions.Add((resultIndex, c.ReadResultFrom));

        return this;
    }
}