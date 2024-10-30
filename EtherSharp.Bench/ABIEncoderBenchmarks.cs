using BenchmarkDotNet.Attributes;

namespace EtherSharp.Bench;

[MemoryDiagnoser]
public class ABIEncoderBenchmarks
{
    private AbiEncoder _abiEncoder = null!;

    [GlobalSetup]
    public void Setup() => _abiEncoder = new AbiEncoder();

    [Benchmark]
    public void AbiEncoder_Int8()
    {
        _abiEncoder.UInt8(byte.MaxValue);
        Span<byte> buffer = stackalloc byte[_abiEncoder.Size];
        _abiEncoder.Build(buffer);
    }
}
