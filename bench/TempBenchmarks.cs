using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using EtherSharp.ABI;

namespace EtherSharp.Bench;

[ShortRunJob(RuntimeMoniker.Net80)]
[ShortRunJob(RuntimeMoniker.Net90)]
[MemoryDiagnoser]
public class TempBenchmarks
{

    [Benchmark]
    public byte[] Encode_Build() 
        => new AbiEncoder()
            .Int32(16)
            .String("Hello")
            .Array(x => x.Int256Array(1, 353535))
            .Build();

    [Benchmark]
    public void Encode_WriteTo()
    {
        var encoder = new AbiEncoder()
            .Int32(16)
            .String("Hello")
            .Array(x => x.Int256Array(1, 353535));
        Span<byte> buffer = stackalloc byte[(int) encoder.Size];
        encoder.TryWritoTo(buffer);
    }
}
