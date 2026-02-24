using BenchmarkDotNet.Attributes;
using EtherSharp.Common;
using EtherSharp.Types;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace EtherSharp.Bench.Types;

[MemoryDiagnoser]
public class Hash32CoreBenchmarks
{
    private static readonly JsonSerializerOptions _options = ParsingUtils.EvmSerializerOptions;

    private const string HEX_A = "0x00112233445566778899aabbccddeeff00112233445566778899aabbccddeeff";
    private const string HEX_B = "0x00112233445566778899aabbccddeeff00112233445566778899aabbccddee00";
    private const string HEX_C = "0x00112233445566778899aabbccddeeff00112233445566778899aabbccddef00";

    private Hash32 _hashA;
    private Hash32 _equalHash;
    private Hash32 _differentHash;
    private Hash32 _lowerHash;
    private Hash32 _higherHash;
    private string _jsonA = null!;

    [GlobalSetup]
    public void Setup()
    {
        _hashA = Hash32.Parse(HEX_A);
        _equalHash = _hashA;
        _differentHash = Hash32.Parse(HEX_B);
        _lowerHash = Hash32.Parse(HEX_B);
        _higherHash = Hash32.Parse(HEX_C);
        _jsonA = JsonSerializer.Serialize(_hashA, _options);
    }

    [Benchmark]
    public Hash32 ParseHex()
        => Hash32.Parse(HEX_A);

    [Benchmark]
    public string SerializeHex()
        => _hashA.ToString();

    [Benchmark]
    public string SerializeJson()
        => JsonSerializer.Serialize(_hashA, _options);

    [Benchmark]
    public Hash32 ParseJson()
        => JsonSerializer.Deserialize<Hash32>(_jsonA, _options);

    [Benchmark]
    public int OrderCompare_AgainstHigherValue()
        => CompareValues(_hashA, _higherHash);

    [Benchmark]
    public int OrderCompare_AgainstLowerValue()
        => CompareValues(_hashA, _lowerHash);

    [Benchmark]
    public bool EqualityCheck_SameValue()
        => AreEqual(_hashA, _equalHash);

    [Benchmark]
    public bool EqualityCheck_DifferentValue()
        => AreEqual(_hashA, _differentHash);

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static bool AreEqual(Hash32 left, Hash32 right)
        => left == right;

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static int CompareValues(Hash32 left, Hash32 right)
        => left.CompareTo(right);
}
