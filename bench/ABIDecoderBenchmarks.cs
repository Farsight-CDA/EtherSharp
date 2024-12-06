using BenchmarkDotNet.Attributes;
using EtherSharp.ABI;

namespace EtherSharp.Bench;
public class ABIDecoderBenchmarks
{

    private AbiEncoder _abiEncoder = null!;
    private byte[] _buffer = null!;
    private AbiDecoder _abiDecoder = null!;

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

        _buffer = _abiEncoder.Build();

        _abiDecoder = new AbiDecoder(_buffer);
    }

    [Benchmark]
    public void AbiDecoder_Build() => _abiDecoder.UInt8(out _)
            .UInt8(out _)
            .UInt8(out _)
            .UInt8(out _)
            .UInt8(out _);
}
