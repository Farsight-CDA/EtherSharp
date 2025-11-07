using EtherSharp.ABI.Types;
using EtherSharp.Client.Services.Subscriptions;
using EtherSharp.RPC.Modules.Eth;
using EtherSharp.Types;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EtherSharp.Client.Modules.Query.Executor;

public class ConstructorCallQueryExecutor(IEthRpcModule ethRpcModule, IServiceProvider provider) : IQueryExecutor
{
    private readonly static byte[] _querierBytecode = Convert.FromHexString(
        "6080604052346100955761044480380380610019816100ad565b928339810190602081830312610095578051906001600160401b038211610095570181601f820112156100955780519061005a610055836100d7565b6100ad565b9282845260208383010111610095575f5b82811061008057835f602085830101526101b3565b8060208092840101518282870101520161006b565b5f80fd5b634e487b7160e01b5f52604160045260245ffd5b6040519190601f01601f191682016001600160401b038111838210176100d257604052565b610099565b6001600160401b0381116100d257601f01601f191660200190565b634e487b7160e01b5f52601160045260245ffd5b906020820180921161011457565b6100f2565b906008820180921161011457565b600301908160031161011457565b906015820180921161011457565b906018820180921161011457565b601801908160181161011457565b600401908160041161011457565b9190820180921161011457565b90610187610055836100d7565b8281528092610198601f19916100d7565b0190602036910137565b6201869f1981019190821161011457565b805115610441576101c561602061017a565b602082019162030d405f805b835182101561043757825a1061043757818401602081810180518995858a01938401959492909160f81c608081116102bd575091815f61023c61022c610227839796602485985160e01c97015160601c9c61016d565b610143565b9261023685610151565b9061016d565b986102465a6101a2565bf1903d918015806102b4575b6102a65761025f8361015f565b9461600061026d878761016d565b116102975761028f959493925f92602492538360e81b6021820152013e61016d565b925b926101d1565b50505050955050505050602001f35b505050955050505050602001f35b50865a10610252565b91989650506081810361033057506021015160601c90813b916102df83610127565b946160006102ed878761016d565b116102975791839161030d95936103139795610319575b5050505061016d565b93610135565b90610291565b5f926023918560e81b905201903c5f808080610304565b92939290915060828103610373575061600061034b84610106565b11610367576021015160601c3f90526020019260150190610291565b50509450505050602001f35b608381036103ac57505061600061038983610119565b116103a1574360c01b90526008019260010190610291565b509450505050602001f35b608481036103da5750506160006103c283610119565b116103a1574260c01b90526008019260010190610291565b608581036104085750506160006103f083610119565b116103a1574560c01b90526008019260010190610291565b6086036100955761600061041b84610106565b11610367576021015160601c3190526020019260150190610291565b9450505050602001f35b00fe"
    );
    private const int MAX_PAYLOAD_SIZE = 1024 * 1024; //1MB

    private readonly IEthRpcModule _ethRpcModule = ethRpcModule;
    private readonly ILogger? _logger = provider.GetService<ILoggerFactory>()?.CreateLogger<SubscriptionsManager>();

    public async Task<TQuery> ExecuteQueryAsync<TQuery>(IQuery<TQuery> query, TargetBlockNumber targetBlockNumber, CancellationToken cancellationToken)
    {
        Span<byte> buffer = [];
        byte[][] outputs = new byte[query.Queries.Count][];
        int requestCount = 0;

        for(int i = 0; i < query.Queries.Count; i++)
        {
            var q = query.Queries[i];
            if(buffer.Length == 0)
            {
                requestCount++;
                string payload = $"0x{Convert.ToHexString(EncodeCalls(query.Queries.Skip(i)))}";
                var callResult = await _ethRpcModule.CallAsync(
                    Address.Zero,
                    null!,
                    null,
                    null,
                    0,
                    payload,
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

            int sliceLength = q.ParseResultLength(buffer);
            byte[] sliceData = buffer[0..sliceLength].ToArray();
            outputs[i] = sliceData;
            buffer = buffer[sliceLength..];
        }

        _logger?.LogTrace("Batch query processing completed using {requests} request(s)", requestCount);

        if(requestCount > 1)
        {
            _logger?.LogDebug("Batch query processing too expensive, required {requests} requests", requestCount);
        }

        return query.ReadResultFrom(outputs);
    }

    private static byte[] EncodeCalls(IEnumerable<IQuery> queries)
    {
        int dataLength = 0;
        int callCount = 0;

        foreach(var queryable in queries)
        {
            dataLength += queryable.CallDataLength;
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

        foreach(var queryable in queries)
        {
            if(callCount == 0)
            {
                break;
            }

            queryable.Encode(buffer);
            buffer = buffer[queryable.CallDataLength..];
            callCount--;
        }

        return arr;
    }
}
