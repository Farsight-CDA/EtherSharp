using EtherSharp.ABI.Types;
using EtherSharp.Client.Services.Subscriptions;
using EtherSharp.RPC.Modules.Eth;
using EtherSharp.Types;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Numerics;

namespace EtherSharp.Client.Modules.Query.Executor;

public class ConstructorCallQueryExecutor(IEthRpcModule ethRpcModule, IServiceProvider provider) : IQueryExecutor
{
    private readonly static byte[] _querierBytecode = Convert.FromHexString(
        "608060405261073c80380380610014816100a8565b928339810190602081830312610090578051906001600160401b038211610090570181601f8201121561009057805190610055610050836100d2565b6100a8565b9282845260208383010111610090575f5b82811061007b57835f602085830101526100f4565b80602080928401015182828701015201610066565b5f80fd5b634e487b7160e01b5f52604160045260245ffd5b6040519190601f01601f191682016001600160401b038111838210176100cd57604052565b610094565b6001600160401b0381116100cd57601f01601f191660200190565b1561009057565b6040516105be8082016001600160401b038111838210176100cd57829161017e833903905ff080156101725781515f91829190602085019034906001600160a01b03165af13d156101675761015f3d91610150610050846100d2565b9283523d5f602085013e6100ed565b602081519101f35b61015f6060916100ed565b6040513d5f823e3d90fdfe608080604052346015576105a4908161001a8239f35b5f80fdfe60806040523615610433576100143636610490565b61001f616020610546565b905f9062030d40825b825184101561042957815a106104295783830190808601916020830192602082015160f81c936001602184019801978515801561041f575b15610153575f80915160e81c602486015160601c61008c60388801519c6100868461052b565b90610539565b9b60585a9801916201869f198901f1925a9003903d9380158061014a575b6101395760016100b9866104e5565b97149182610129575b6160006100cf8989610539565b11610117579260209261010698979695925f95538560e81b6021840152841461010e5760c01b6024820152600c905b01013e610539565b915b91610028565b506004906100fe565b50505050505094925050506020915001f35b96610133906104f3565b966100c2565b505050505094925050506020915001f35b50875a106100aa565b91929094608281999899145f146101d85750505160601c90813b916101778361050f565b946160006101858787610539565b116101c8579183916101a595936101ab97956101b1575b50505050610539565b9261051d565b92610108565b5f926023918560e81b905201903c5f80808061019c565b5050505094925050506020915001f35b9394939192916083810361021e575050506160006101f584610501565b1161021157916020916014935160601c3f905201920192610108565b5050939150506020915001f35b969796608c81036102585750505050616000610239836104f3565b1161024d574360c01b905260080191610108565b509360200192505050f35b608d81036102845750505050616000610270836104f3565b1161024d574260c01b905260080191610108565b608e81036102b0575050505061600061029c836104f3565b1161024d574560c01b905260080191610108565b608f81036102d957505050506160006102c883610501565b1161024d573a905260200191610108565b6090810361030257505050506160006102f183610501565b1161024d5748905260200191610108565b969796609681036103385750505061600061031c84610501565b1161021157916020916014935160601c31905201920192610108565b9697969394939192909160a0810361036f57505050509061600061035b836104f3565b1161024d574660c01b905260080191610108565b60aa0361041b5761039d5f928360296103a26100869c83965160e01c9d8e602587015160e01c9788916104d2565b610539565b9b8082850184f0930101916201869f195a01f1903d91801580610412575b610403576103cd836104e5565b946160006103db8787610539565b116101c8576103fd959493925f92602492538360e81b6021820152013e610539565b91610108565b50505094925050506020915001f35b50855a106103c0565b5f80fd5b5060018614610060565b9360200192505050f35b005b634e487b7160e01b5f52604160045260245ffd5b6040519190601f01601f1916820167ffffffffffffffff81118382101761046f57604052565b610435565b67ffffffffffffffff811161046f57601f01601f191660200190565b9190916104a461049f82610474565b610449565b92818452811161041b576020815f92838387013784010152565b634e487b7160e01b5f52601160045260245ffd5b60080190816008116104e057565b6104be565b60040190816004116104e057565b90600882018092116104e057565b90602082018092116104e057565b60030190816003116104e057565b90601482018092116104e057565b60370190816037116104e057565b919082018092116104e057565b9061055361049f83610474565b8281528092610564601f1991610474565b019060203691013756fea264697066735822122023fef1559fd47cfe4561f514c4feb528fbd6554c160e9cc9c7147a7b587b4c8b64736f6c634300081e0033"
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
                byte[] payloadBytes = EncodeCalls(query.Queries.Skip(i), out int callCount, out var ethValue);

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
                    ethValue,
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

    private static byte[] EncodeCalls(IEnumerable<IQuery> queries, out int encodedCallCount, out BigInteger ethValue)
    {
        ethValue = 0;
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
            ethValue += queryable.EthValue;
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
