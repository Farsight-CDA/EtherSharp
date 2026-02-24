using BenchmarkDotNet.Attributes;
using EtherSharp.Crypto;
using EtherSharp.Types;
using Nethereum.Util;

namespace EtherSharp.Bench.Crypto;

[MemoryDiagnoser]
public class Keccak256Benchmarks
{
    private const int SEED = 123456789;
    private static readonly int[] _inputDataLenghts = [0, 128, 4096, 65536];
    public static IEnumerable<byte[]> Values
        => _inputDataLenghts.Select(size =>
        {
            byte[] data = new byte[size];
            new Random(SEED).NextBytes(data);
            return data;
        });

    [ParamsSource(nameof(Values))]
    public byte[] InputData = null!;

    [Benchmark]
    public Hash32 EtherSharp_Keccak256_Output_Hash32()
        => Keccak256.HashData(InputData);

    [Benchmark]
    public byte[] EtherSharp_Keccak256_Output_Hash32_ToArray()
        => Keccak256.HashData(InputData).ToArray();

    [Benchmark]
    public byte[] NEthereum_Keccak256()
        => Sha3Keccack.Current.CalculateHash(InputData);
}
