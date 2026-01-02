using EtherSharp.Tx;
using EtherSharp.Types;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Numerics;

namespace EtherSharp.Client.Modules.Query.Executor;

public class FlashCallQueryExecutor(IEtherClient client, IServiceProvider provider) : IQueryExecutor
{
    private static readonly EVMByteCode _querierCode = new EVMByteCode(
        Convert.FromHexString(
            "608080604052346015576105b0908161001a8239f35b5f80fdfe608060405261000e363661049c565b610019616020610552565b5f9062030d40825b8093855182101561043657825a10610436576001602083880101515f1a9201948601602181018286019060208201908515801561042c575b1561014b575f80915160e81c602486015160601c61008560388801519c61007f84610537565b90610545565b9b60585a9801916201869f198901f1925a9003903d93801580610142575b6101325760016100b2866104f1565b97149182610122575b6160006100c88989610545565b1161011157926020926100ff98979695925f95538560e81b602184015284146101085760c01b6024820152600c905b01013e610545565b915b9192610021565b506004906100f7565b505050505050945050506020915001f35b9661012c906104ff565b966100bb565b5050505050945050506020915001f35b50875a106100a3565b91929094608281999899145f146101ce5750505160601c90813b9161016f8361051b565b9461600061017d8787610545565b116101c05791839161019d95936101a397956101a9575b50505050610545565b92610529565b92610101565b5f926023918560e81b905201903c5f808080610194565b505050509450505050602001f35b9394939192909160838103610213575050506160006101ec8461050d565b1161020857916020916014935160601c3f905201920192610101565b505093505050602001f35b969796608c810361024d575050505061600061022e836104ff565b11610242574360c01b905260080191610101565b509350506020915001f35b608d81036102795750505050616000610265836104ff565b11610242574260c01b905260080191610101565b608e81036102a55750505050616000610291836104ff565b11610242574560c01b905260080191610101565b608f81036102ce57505050506160006102bd8361050d565b11610242573a905260200191610101565b609081036102f757505050506160006102e68361050d565b116102425748905260200191610101565b9697966096810361032d575050506160006103118461050d565b1161020857916020916014935160601c31905201920192610101565b9697969192909160a08103610360575050505061600061034c836104ff565b11610242574660c01b905260080191610101565b9297939493919260aa03610428575f9182915160e81c98896046600161039f602485015160e81c9561007f8761039a6027890151976104de565b610545565b9c806047860188f094010101916201869f195a01f1903d9180158061041f575b610411576103cc836104f1565b946160006103da8787610545565b11610402576103fc959493925f92602492538360e81b6021820152013e610545565b91610101565b50505050945050506020915001f35b505050945050506020915001f35b50855a106103bf565b5f80fd5b5060018614610059565b945050506020915001f35b634e487b7160e01b5f52604160045260245ffd5b6040519190601f01601f1916820167ffffffffffffffff81118382101761047b57604052565b610441565b67ffffffffffffffff811161047b57601f01601f191660200190565b9190916104b06104ab82610480565b610455565b928184528111610428576020815f92838387013784010152565b634e487b7160e01b5f52601160045260245ffd5b60260190816026116104ec57565b6104ca565b60040190816004116104ec57565b90600882018092116104ec57565b90602082018092116104ec57565b60030190816003116104ec57565b90601482018092116104ec57565b60370190816037116104ec57565b919082018092116104ec57565b9061055f6104ab83610480565b8281528092610570601f1991610480565b019060203691013756fea2646970667358221220033b463e0f88d64f8c8c1f4c96181e85254688b24a406555e93639fed4f0b9db64736f6c634300081e0033"
        )
    );

    private const int MAX_PAYLOAD_SIZE = 48 * 1024; //EIP-3860

    private readonly IEtherClient _client = client;
    private readonly ILogger? _logger = provider.GetService<ILoggerFactory>()?.CreateLogger<FlashCallQueryExecutor>();

    public async Task<TQuery> ExecuteQueryAsync<TQuery>(IQuery<TQuery> query, TargetBlockNumber targetBlockNumber, CancellationToken cancellationToken)
    {
        ReadOnlySpan<byte> buffer = [];
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

                var callResult = await _client.SafeFlashCallAsync(
                    IContractDeployment.Create(_querierCode, 0),
                    IContractCall.ForRawContractCall(null!, ethValue, payloadBytes),
                    targetBlockNumber,
                    cancellationToken
                );

                var output = callResult.Unwrap(Address.Zero);

                if(output.Length == 0)
                {
                    throw new InvalidOperationException("Call is too expensive to be executed within batch");
                }

                buffer = output.Span;
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

            if(newDataLength + _querierCode.Length + 2 > MAX_PAYLOAD_SIZE)
            {
                break;
            }

            dataLength = newDataLength;
            callCount++;
            ethValue += queryable.EthValue;
        }

        encodedCallCount = callCount;

        byte[] arr = new byte[dataLength];
        var buffer = arr.AsSpan();

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
