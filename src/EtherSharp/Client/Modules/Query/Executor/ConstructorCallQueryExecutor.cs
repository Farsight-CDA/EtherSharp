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
        "6080604052346100955761071980380380610019816100ad565b928339810190602081830312610095578051906001600160401b038211610095570181601f820112156100955780519061005a610055836100d7565b6100ad565b9282845260208383010111610095575f5b82811061008057835f602085830101526100f9565b8060208092840101518282870101520161006b565b5f80fd5b634e487b7160e01b5f52604160045260245ffd5b6040519190601f01601f191682016001600160401b038111838210176100d257604052565b610099565b6001600160401b0381116100d257601f01601f191660200190565b1561009557565b6040516105968082016001600160401b038111838210176100d2578291610183833903905ff080156101775781515f91829190602085019083906001600160a01b03165af13d1561016c576101643d91610155610055846100d7565b9283523d5f602085013e6100f2565b602081519101f35b6101646060916100f2565b6040513d5f823e3d90fdfe6080806040523460155761057c908161001a8239f35b5f80fdfe6080604052346103f357361561040b576100193636610468565b61002461602061051e565b905f9062030d40825b825184101561040157815a106104015783830190808601916020830192602082015160f81c93600160218401980197851580156103f7575b15610154575f80915160e81c61008c602487015160601c9b61008683610503565b90610511565b9a8260385a9801916201869f198901f1925a9003903d9380158061014b575b61013a5760016100ba866104bd565b9714918261012a575b6160006100d08989610511565b11610118579260209261010798979695925f95538560e81b6021840152841461010f5760c01b6024820152600c905b01013e610511565b915b9161002d565b506004906100ff565b50505050505094925050506020915001f35b96610134906104cb565b966100c3565b505050505094925050506020915001f35b50875a106100ab565b91929094608281999899145f146101d95750505160601c90813b91610178836104e7565b946160006101868787610511565b116101c9579183916101a695936101ac97956101b2575b50505050610511565b926104f5565b92610109565b5f926023918560e81b905201903c5f80808061019d565b5050505094925050506020915001f35b9394939192916083810361021f575050506160006101f6846104d9565b1161021257916020916014935160601c3f905201920192610109565b5050939150506020915001f35b969796608c8103610259575050505061600061023a836104cb565b1161024e574360c01b905260080191610109565b509360200192505050f35b608d81036102855750505050616000610271836104cb565b1161024e574260c01b905260080191610109565b608e81036102b1575050505061600061029d836104cb565b1161024e574560c01b905260080191610109565b608f81036102da57505050506160006102c9836104d9565b1161024e573a905260200191610109565b96979660968103610310575050506160006102f4846104d9565b1161021257916020916014935160601c31905201920192610109565b9697969394939192909160a08103610347575050505090616000610333836104cb565b1161024e574660c01b905260080191610109565b60aa036103f3576103755f9283602961037a6100869c83965160e01c9d8e602587015160e01c9788916104aa565b610511565b9b8082850184f0930101916201869f195a01f1903d918015806103ea575b6103db576103a5836104bd565b946160006103b38787610511565b116101c9576103d5959493925f92602492538360e81b6021820152013e610511565b91610109565b50505094925050506020915001f35b50855a10610398565b5f80fd5b5060018614610065565b9360200192505050f35b005b634e487b7160e01b5f52604160045260245ffd5b6040519190601f01601f1916820167ffffffffffffffff81118382101761044757604052565b61040d565b67ffffffffffffffff811161044757601f01601f191660200190565b91909161047c6104778261044c565b610421565b9281845281116103f3576020815f92838387013784010152565b634e487b7160e01b5f52601160045260245ffd5b60080190816008116104b857565b610496565b60040190816004116104b857565b90600882018092116104b857565b90602082018092116104b857565b60030190816003116104b857565b90601482018092116104b857565b60170190816017116104b857565b919082018092116104b857565b9061052b6104778361044c565b828152809261053c601f199161044c565b019060203691013756fea2646970667358221220749a47da7d265fdfacbd6876f7b1a467007172aad1ed90a451487e9d2175752464736f6c634300081e0033"
    );
    private const int MAX_PAYLOAD_SIZE = 48 * 1024; //EIP-3860

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
                byte[] payloadBytes = EncodeCalls(query.Queries.Skip(i), out int callCount);

                if(callCount == 0)
                {
                    throw new InvalidOperationException("Call is too large to be executed within batch");
                }

                string payload = $"0x{Convert.ToHexString(payloadBytes)}";
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

    private static byte[] EncodeCalls(IEnumerable<IQuery> queries, out int encodedCallCount)
    {
        int dataLength = 0;
        int callCount = 0;

        foreach(var queryable in queries)
        {
            int newDataLength = dataLength + queryable.CallDataLength;

            if(newDataLength + _querierBytecode.Length + 64 > MAX_PAYLOAD_SIZE)
            {
                break;
            }

            dataLength = newDataLength;
            callCount++;
        }

        encodedCallCount = callCount;

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
