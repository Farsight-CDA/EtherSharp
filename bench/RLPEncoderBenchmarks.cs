using BenchmarkDotNet.Attributes;
using EtherSharp.RLP;
using System.Numerics;
using System.Text;

namespace EtherSharp.Bench;

[MemoryDiagnoser, ShortRunJob]
public class RLPEncoderBenchmarks
{

    private readonly int nonce = 0;
    private readonly byte[] nonceBytes = Encoding.UTF8.GetBytes("0x00000");

    private readonly BigInteger gasPrice = BigInteger.Zero;
    private readonly byte[] gasPriceBytes = Encoding.UTF8.GetBytes("0x0000000");

    private readonly ulong gas = 0;
    private readonly byte[] gasBytes = Encoding.UTF8.GetBytes("0xFF");

    private readonly byte[] to = Encoding.UTF8.GetBytes("D7B9798cE43Ca0A6aa8Be304a5EC5f88225f8bBA");

    private readonly BigInteger value = BigInteger.Zero;
    private readonly byte[] valueBytes = Encoding.UTF8.GetBytes("0x0");

    private readonly byte[] data = new byte[200];

    [GlobalSetup]
    public void Setup()
    {

    }

    [Benchmark]
    public void RLPEncod()
    {
        int bufferSize = RLPEncoder.GetListSize(
            RLPEncoder.GetIntSize(nonce) +
            RLPEncoder.GetIntSize(gasPrice) +
            RLPEncoder.GetIntSize(gas) +
            RLPEncoder.GetStringSize(to) +
            RLPEncoder.GetIntSize(value) +
            RLPEncoder.GetStringSize(data)
        );

        Span<byte> rlpBuffer = bufferSize > 2048
            ? new byte[bufferSize]
            : stackalloc byte[bufferSize];

        var encoder = new RLPEncoder(rlpBuffer);
        _ = encoder.EncodeList(bufferSize)
            .EncodeInt(nonce)
        .EncodeInt(gasPrice)
        .EncodeInt(gas)
            .EncodeString(to)
            .EncodeInt(value)
            .EncodeString(data);
    }

    [Benchmark]
    public void RLPEncod_Nethereum() => _ = Nethereum.RLP.RLP.EncodeDataItemsAsElementOrListAndCombineAsList(
            [nonceBytes, gasBytes, gasPriceBytes, to, valueBytes, data]
            );
}
