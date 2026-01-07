using BenchmarkDotNet.Attributes;
using EtherSharp.Crypto;
using Nethereum.Util;

namespace EtherSharp.Bench;

[ShortRunJob]
[MemoryDiagnoser]
public class Keccak256Benchmarks
{
    private const int SEED = 123456789;
    private static readonly int[] _inputDataLenghts = [0, 32, 512, 4096, 32768];
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
    public byte[] EtherSharp_Keccak256_Output_Heap()
        => Keccak256.HashData(InputData);

    [Benchmark]
    public bool EtherSharp_Keccak256_Output_Stack()
    {
        Span<byte> buffer = stackalloc byte[32];
        return Keccak256.TryHashData(InputData, buffer);
    }

    [Benchmark]
    public byte[] NEthereum_Keccak256()
        => Sha3Keccack.Current.CalculateHash(InputData);
}
