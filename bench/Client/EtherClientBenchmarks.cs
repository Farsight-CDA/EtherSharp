using BenchmarkDotNet.Attributes;
using EtherSharp.Bench.Common;
using EtherSharp.Client;
using EtherSharp.Contract;

namespace EtherSharp.Bench.Client;

[MemoryDiagnoser]
public class EtherClientBenchmarks
{
    private readonly IEtherClient _client;

    public EtherClientBenchmarks()
    {
        _client = EtherClientBuilder
            .CreateForHttpRpc("https://optimism.llamarpc.com")
            .Build();

        _client.InitializeAsync().GetAwaiter().GetResult();
    }

    [Benchmark]
    public IEVMContract CreateContractInstance()
        => _client.Contract<IERC20>("0x94b008aa00579c1307b0ef2c499ad98a8ce58e58");
}
