using BenchmarkDotNet.Attributes;
using EtherSharp.Client;
using EtherSharp.RPC;
using EtherSharp.RPC.Transport;
using EtherSharp.Types;
using Microsoft.Extensions.DependencyInjection;

namespace EtherSharp.Bench.Rpc;

[MemoryDiagnoser]
[ShortRunJob]
public class RpcClientPathBenchmarks
{
    private readonly IRpcClient _noMiddlewareClient;
    private readonly IRpcClient _oneMiddlewareClient;

    public RpcClientPathBenchmarks()
    {
        _noMiddlewareClient = BuildClient(new BenchTransport());
        _oneMiddlewareClient = BuildClient(new BenchTransport(), new PassthroughMiddleware());
    }

    [Benchmark(Baseline = true)]
    public Task<RpcResult<int>> Typed_NoMiddleware_TwoParams()
        => _noMiddlewareClient.SendRpcRequestAsync<string, int, int>(
            "eth_test",
            "0xabc",
            7,
            TargetBlockNumber.Latest,
            cancellationToken: default
        );

    [Benchmark]
    public Task<RpcResult<int>> Typed_OneMiddleware_TwoParams()
        => _oneMiddlewareClient.SendRpcRequestAsync<string, int, int>(
            "eth_test",
            "0xabc",
            7,
            TargetBlockNumber.Latest,
            cancellationToken: default
        );

    private static IRpcClient BuildClient(IRPCTransport transport, IRpcMiddleware? middleware = null)
    {
        var builder = EtherClientBuilder
            .CreateEmpty()
            .WithRPCTransport(transport);

        if(middleware is not null)
        {
            builder.AddRPCMiddleware(middleware);
        }

        var client = builder.BuildReadClient();
        return client.AsInternal().Provider.GetRequiredService<IRpcClient>();
    }
}
