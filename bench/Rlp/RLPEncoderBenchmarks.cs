using BenchmarkDotNet.Attributes;
using EtherSharp.Numerics;
using EtherSharp.RLP;
using Nethereum.RLP;
using System.Numerics;
using System.Text;

namespace EtherSharp.Bench.Rlp;

[MemoryDiagnoser]
[ShortRunJob]
public class RLPEncoderBenchmarks
{
    private readonly ulong _chainId = 137;
    private readonly uint _nonce = 1_000;
    private readonly UInt256 _maxPriorityFeePerGas = 10 * UInt256.Pow(10, 18);
    private readonly UInt256 _maxFeePerGas = 30 * UInt256.Zero;
    private readonly ulong _gas = 3_000_000;
    private readonly byte[] _to = Encoding.UTF8.GetBytes("D7B9798cE43Ca0A6aa8Be304a5EC5f88225f8bBA");
    private readonly UInt256 _value = 5 * UInt256.Pow(10, 18);
    private readonly byte[] _data = new byte[200];

    private readonly byte[] _shortString = new byte[32];
    private readonly byte[] _longString = new byte[4096];
    private readonly ulong _largeUInt64 = 8765432109876543210;
    private readonly UInt256 _largeUInt256 = (UInt256.One << 200) + 123456789;

    private readonly int _chainIdForNethereum;
    private readonly int _nonceForNethereum;
    private readonly BigInteger _maxPriorityFeePerGasForNethereum;
    private readonly BigInteger _maxFeePerGasForNethereum;
    private readonly long _gasForNethereum;
    private readonly BigInteger _valueForNethereum;
    private readonly long _largeInt64ForNethereum;
    private readonly BigInteger _largeUInt256ForNethereum;

    public RLPEncoderBenchmarks()
    {
        new Random(12345).NextBytes(_data);
        new Random(12345).NextBytes(_shortString);
        new Random(12345).NextBytes(_longString);

        _chainIdForNethereum = (int) _chainId;
        _nonceForNethereum = (int) _nonce;
        _maxPriorityFeePerGasForNethereum = (BigInteger) _maxPriorityFeePerGas;
        _maxFeePerGasForNethereum = (BigInteger) _maxFeePerGas;
        _gasForNethereum = (long) _gas;
        _valueForNethereum = (BigInteger) _value;
        _largeInt64ForNethereum = (long) _largeUInt64;
        _largeUInt256ForNethereum = (BigInteger) _largeUInt256;
    }

    [Benchmark]
    public byte EtherSharp_EIP1559_RLPEncode()
    {
        int contentSize =
            RLPEncoder.GetIntSize(_chainId) +
            RLPEncoder.GetIntSize(_nonce) +
            RLPEncoder.GetIntSize(_maxPriorityFeePerGas) +
            RLPEncoder.GetIntSize(_maxFeePerGas) +
            RLPEncoder.GetIntSize(_gas) +
            RLPEncoder.GetStringSize(_to) +
            RLPEncoder.GetIntSize(_value) +
            RLPEncoder.GetStringSize(_data);
        int bufferSize = RLPEncoder.GetListSize(contentSize);

        Span<byte> rlpBuffer = stackalloc byte[bufferSize];

        var encoder = new RLPEncoder(rlpBuffer);
        _ = encoder.EncodeList(contentSize)
            .EncodeInt(_chainId)
            .EncodeInt(_nonce)
            .EncodeInt(_maxPriorityFeePerGas)
            .EncodeInt(_maxFeePerGas)
            .EncodeInt(_gas)
            .EncodeString(_to)
            .EncodeInt(_value)
            .EncodeString(_data);

        return rlpBuffer[0];
    }

    [Benchmark]
    public byte[] NEthereum_EIP1559_RLPEncode()
    {
        byte[] chainIdBytes = _chainIdForNethereum.ToBytesForRLPEncoding();
        byte[] nonceBytes = _nonceForNethereum.ToBytesForRLPEncoding();
        byte[] gasBytes = _gasForNethereum.ToBytesForRLPEncoding();
        byte[] maxPriorityFeePerGasBytes = _maxPriorityFeePerGasForNethereum.ToBytesForRLPEncoding();
        byte[] maxFeePerGasBytes = _maxFeePerGasForNethereum.ToBytesForRLPEncoding();
        byte[] valueBytes = _valueForNethereum.ToBytesForRLPEncoding();

        return _ = Nethereum.RLP.RLP.EncodeList(
            Nethereum.RLP.RLP.EncodeElement(chainIdBytes),
            Nethereum.RLP.RLP.EncodeElement(nonceBytes),
            Nethereum.RLP.RLP.EncodeElement(maxPriorityFeePerGasBytes),
            Nethereum.RLP.RLP.EncodeElement(maxFeePerGasBytes),
            Nethereum.RLP.RLP.EncodeElement(gasBytes),
            Nethereum.RLP.RLP.EncodeElement(_to),
            Nethereum.RLP.RLP.EncodeElement(valueBytes),
            Nethereum.RLP.RLP.EncodeElement(_data)
        );
    }

    [Benchmark]
    public byte EtherSharp_EncodeString_Short()
    {
        int bufferSize = RLPEncoder.GetStringSize(_shortString);
        Span<byte> rlpBuffer = stackalloc byte[bufferSize];
        _ = new RLPEncoder(rlpBuffer).EncodeString(_shortString);
        return rlpBuffer[0];
    }

    [Benchmark]
    public byte[] NEthereum_EncodeString_Short()
        => Nethereum.RLP.RLP.EncodeElement(_shortString);

    [Benchmark]
    public byte EtherSharp_EncodeString_Long()
    {
        int bufferSize = RLPEncoder.GetStringSize(_longString);
        Span<byte> rlpBuffer = stackalloc byte[bufferSize];
        _ = new RLPEncoder(rlpBuffer).EncodeString(_longString);
        return rlpBuffer[0];
    }

    [Benchmark]
    public byte[] NEthereum_EncodeString_Long()
        => Nethereum.RLP.RLP.EncodeElement(_longString);

    [Benchmark]
    public byte EtherSharp_EncodeInt_UInt64_Large()
    {
        Span<byte> rlpBuffer = stackalloc byte[RLPEncoder.GetIntSize(_largeUInt64)];
        _ = new RLPEncoder(rlpBuffer).EncodeInt(_largeUInt64);
        return rlpBuffer[0];
    }

    [Benchmark]
    public byte[] NEthereum_EncodeInt_UInt64_Large()
        => Nethereum.RLP.RLP.EncodeElement(_largeInt64ForNethereum.ToBytesForRLPEncoding());

    [Benchmark]
    public byte EtherSharp_EncodeInt_UInt256_Large()
    {
        Span<byte> rlpBuffer = stackalloc byte[RLPEncoder.GetIntSize(_largeUInt256)];
        _ = new RLPEncoder(rlpBuffer).EncodeInt(_largeUInt256);
        return rlpBuffer[0];
    }

    [Benchmark]
    public byte[] NEthereum_EncodeInt_UInt256_Large()
        => Nethereum.RLP.RLP.EncodeElement(_largeUInt256ForNethereum.ToBytesForRLPEncoding());
}
