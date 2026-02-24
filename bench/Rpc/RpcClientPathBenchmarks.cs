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
    private readonly IRpcClient _twoMiddlewareClient;
    private readonly IRpcClient _threeMiddlewareClient;
    private readonly IRpcClient _fourMiddlewareClient;
    private readonly IRpcClient _fiveMiddlewareClient;

    public RpcClientPathBenchmarks()
    {
        _noMiddlewareClient = BuildClient(new BenchTransport());
        _oneMiddlewareClient = BuildClient(new BenchTransport(), new PassthroughMiddleware());
        _twoMiddlewareClient = BuildClient(new BenchTransport(), new PassthroughMiddleware(), new PassthroughMiddleware());
        _threeMiddlewareClient = BuildClient(
            new BenchTransport(),
            new PassthroughMiddleware(),
            new PassthroughMiddleware(),
            new PassthroughMiddleware()
        );
        _fourMiddlewareClient = BuildClient(
            new BenchTransport(),
            new PassthroughMiddleware(),
            new PassthroughMiddleware(),
            new PassthroughMiddleware(),
            new PassthroughMiddleware()
        );
        _fiveMiddlewareClient = BuildClient(
            new BenchTransport(),
            new PassthroughMiddleware(),
            new PassthroughMiddleware(),
            new PassthroughMiddleware(),
            new PassthroughMiddleware(),
            new PassthroughMiddleware()
        );
    }

    [Benchmark(Baseline = true)]
    public Task<RpcResult<int>> Typed_NoMiddleware_TwoParams()
        => _noMiddlewareClient.SendRpcRequestAsync<string, int, int>(
            "eth_test",
            "0xabc",
            7,
            TargetHeight.Latest,
            cancellationToken: default
        );

    [Benchmark]
    public Task<RpcResult<int>> Typed_OneMiddleware_TwoParams()
        => _oneMiddlewareClient.SendRpcRequestAsync<string, int, int>(
            "eth_test",
            "0xabc",
            7,
            TargetHeight.Latest,
            cancellationToken: default
        );

    [Benchmark]
    public Task<RpcResult<int>> Typed_TwoMiddlewares_TwoParams()
        => _twoMiddlewareClient.SendRpcRequestAsync<string, int, int>(
            "eth_test",
            "0xabc",
            7,
            TargetHeight.Latest,
            cancellationToken: default
        );

    [Benchmark]
    public Task<RpcResult<int>> Typed_ThreeMiddlewares_TwoParams()
        => _threeMiddlewareClient.SendRpcRequestAsync<string, int, int>(
            "eth_test",
            "0xabc",
            7,
            TargetHeight.Latest,
            cancellationToken: default
        );

    [Benchmark]
    public Task<RpcResult<int>> Typed_FourMiddlewares_TwoParams()
        => _fourMiddlewareClient.SendRpcRequestAsync<string, int, int>(
            "eth_test",
            "0xabc",
            7,
            TargetHeight.Latest,
            cancellationToken: default
        );

    [Benchmark]
    public Task<RpcResult<int>> Typed_FiveMiddlewares_TwoParams()
        => _fiveMiddlewareClient.SendRpcRequestAsync<string, int, int>(
            "eth_test",
            "0xabc",
            7,
            TargetHeight.Latest,
            cancellationToken: default
        );

    private static IRpcClient BuildClient(IRPCTransport transport, params IRpcMiddleware[] middlewares)
    {
        var builder = EtherClientBuilder
            .CreateEmpty()
            .WithRPCTransport(transport);

        foreach(var middleware in middlewares)
        {
            builder.AddRPCMiddleware(middleware);
        }

        var client = builder.BuildReadClient();
        return client.AsInternal().Provider.GetRequiredService<IRpcClient>();
    }
}
