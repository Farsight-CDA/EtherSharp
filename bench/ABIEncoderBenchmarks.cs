using BenchmarkDotNet.Attributes;
using EtherSharp.ABI;

namespace EtherSharp.Bench;

[MemoryDiagnoser]
public class ABIEncoderBenchmarks
{
    private AbiEncoder _abiEncoder = null!;
    private byte[] _buffer = null!;

    [GlobalSetup]
    public void Setup()
    {
        _abiEncoder = new AbiEncoder()
            .UInt8(Byte.MaxValue)
            .UInt8(Byte.MaxValue)
            .UInt8(Byte.MaxValue)
            .UInt8(Byte.MaxValue)
            .UInt8(Byte.MaxValue);
        ;
        _buffer = new byte[32 * 5];
    }

    [Benchmark]
    public void AbiEncoder_Build()
        => _abiEncoder.TryWritoTo(_buffer);
}
