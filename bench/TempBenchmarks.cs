using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using EtherSharp.ABI;

namespace EtherSharp.Bench;

[ShortRunJob(RuntimeMoniker.Net80)]
[ShortRunJob(RuntimeMoniker.Net90)]
[MemoryDiagnoser]
public class TempBenchmarks
{
    private AbiEncoder _encoder = null!;

    [GlobalSetup]
    public void Setup()
        => _encoder = new AbiEncoder();

    [Benchmark]
    public byte[] Encode_Build()
        => _encoder
            .Int32(16)
            .String("Hello")
            .Array(x => x.Int256Array(1, 353535))
            .Build();

    [Benchmark]
    public void Encode_WriteTo()
    {
        var encoder = _encoder
            .Int32(16)
            .String("Hello")
            .Array(x => x.Int256Array(1, 353535));
        Span<byte> buffer = stackalloc byte[encoder.Size];
        _ = encoder.TryWritoTo(buffer);
    }
}
