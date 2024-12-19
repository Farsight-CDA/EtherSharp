using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using EtherSharp.RLP;
using System.Buffers.Binary;
using System.Numerics;
using System.Text;

namespace EtherSharp.Bench;

[MemoryDiagnoser]
[ShortRunJob]
public class RLPEncoderBenchmarks
{
    private readonly uint _nonce = 0;
    private readonly BigInteger _gasPrice = BigInteger.Zero;
    private readonly ulong _gas = 0;
    private readonly byte[] _to = Encoding.UTF8.GetBytes("D7B9798cE43Ca0A6aa8Be304a5EC5f88225f8bBA");
    private readonly BigInteger _value = BigInteger.Zero;
    private readonly byte[] _data = new byte[200];

    private readonly Consumer _consumer = new Consumer();

    [Benchmark]
    public byte[] EtherSharp_RLPEncode_Heap()
    {
        int bufferSize = RLPEncoder.GetListSize(
            RLPEncoder.GetIntSize(_nonce) +
            RLPEncoder.GetIntSize(_gasPrice) +
            RLPEncoder.GetIntSize(_gas) +
            RLPEncoder.GetStringSize(_to) +
            RLPEncoder.GetIntSize(_value) +
            RLPEncoder.GetStringSize(_data)
        );

        byte[] rlpBuffer = new byte[bufferSize];

        var encoder = new RLPEncoder(rlpBuffer);
        _ = encoder.EncodeList(bufferSize)
            .EncodeInt(_nonce)
            .EncodeInt(_gasPrice)
            .EncodeInt(_gas)
            .EncodeString(_to)
            .EncodeInt(_value)
            .EncodeString(_data);

        return rlpBuffer;
    }

    [Benchmark]
    public void EtherSharp_RLPEncode_Stack()
    {
        int bufferSize = RLPEncoder.GetListSize(
            RLPEncoder.GetIntSize(_nonce) +
            RLPEncoder.GetIntSize(_gasPrice) +
            RLPEncoder.GetIntSize(_gas) +
            RLPEncoder.GetStringSize(_to) +
            RLPEncoder.GetIntSize(_value) +
            RLPEncoder.GetStringSize(_data)
        );

        Span<byte> rlpBuffer = stackalloc byte[bufferSize];

        var encoder = new RLPEncoder(rlpBuffer);
        _ = encoder.EncodeList(bufferSize)
            .EncodeInt(_nonce)
            .EncodeInt(_gasPrice)
            .EncodeInt(_gas)
            .EncodeString(_to)
            .EncodeInt(_value)
            .EncodeString(_data);

        byte dummy = rlpBuffer[0];
        GC.KeepAlive(dummy);
    }

    [Benchmark]
    public byte[] NEthereum_RLPEncode()
    {
        byte[] nonceBytes = new byte[4];
        byte[] gasBytes = new byte[8];

        BinaryPrimitives.WriteUInt32BigEndian(nonceBytes, _nonce);
        BinaryPrimitives.WriteUInt64BigEndian(gasBytes, _gas);

        if(BitConverter.IsLittleEndian)
        {
            nonceBytes.AsSpan().Reverse();
            gasBytes.AsSpan().Reverse();
        }

        return _ = Nethereum.RLP.RLP.EncodeList(
            Nethereum.RLP.RLP.EncodeElement(nonceBytes),
            Nethereum.RLP.RLP.EncodeElement(_gasPrice.ToByteArray(true, true)),
            Nethereum.RLP.RLP.EncodeElement(gasBytes),
            Nethereum.RLP.RLP.EncodeElement(_to),
            Nethereum.RLP.RLP.EncodeElement(_value.ToByteArray(true, true)),
            Nethereum.RLP.RLP.EncodeElement(_data)
        );
    }
}
