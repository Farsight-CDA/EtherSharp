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
            .UInt8(byte.MaxValue)
            .UInt8(byte.MaxValue)
            .UInt8(byte.MaxValue)
            .UInt8(byte.MaxValue)
            .UInt8(byte.MaxValue);
        ;
        _buffer = new byte[32 * 5];
    }

    [Benchmark]
    public void AbiEncoder_Build()
        => _abiEncoder.Build(_buffer);
}
