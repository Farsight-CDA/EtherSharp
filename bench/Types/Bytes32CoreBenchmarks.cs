using BenchmarkDotNet.Attributes;
using EtherSharp.Common;
using EtherSharp.Types;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace EtherSharp.Bench.Types;

[MemoryDiagnoser]
public class Bytes32CoreBenchmarks
{
    private static readonly JsonSerializerOptions _options = ParsingUtils.EvmSerializerOptions;

    private const string HEX_A = "0x00112233445566778899aabbccddeeff00112233445566778899aabbccddeeff";
    private const string HEX_B = "0x00112233445566778899aabbccddeeff00112233445566778899aabbccddee00";
    private const string HEX_C = "0x00112233445566778899aabbccddeeff00112233445566778899aabbccddef00";

    private Bytes32 _hashA;
    private Bytes32 _equalHash;
    private Bytes32 _differentHash;
    private Bytes32 _lowerHash;
    private Bytes32 _higherHash;
    private string _jsonA = null!;

    [GlobalSetup]
    public void Setup()
    {
        _hashA = Bytes32.Parse(HEX_A);
        _equalHash = _hashA;
        _differentHash = Bytes32.Parse(HEX_B);
        _lowerHash = Bytes32.Parse(HEX_B);
        _higherHash = Bytes32.Parse(HEX_C);
        _jsonA = JsonSerializer.Serialize(_hashA, _options);
    }

    [Benchmark]
    public Bytes32 ParseHex()
        => Bytes32.Parse(HEX_A);

    [Benchmark]
    public string SerializeHex()
        => _hashA.ToString();

    [Benchmark]
    public string SerializeJson()
        => JsonSerializer.Serialize(_hashA, _options);

    [Benchmark]
    public Bytes32 ParseJson()
        => JsonSerializer.Deserialize<Bytes32>(_jsonA, _options);

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
    private static bool AreEqual(Bytes32 left, Bytes32 right)
        => left == right;

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static int CompareValues(Bytes32 left, Bytes32 right)
        => left.CompareTo(right);
}
