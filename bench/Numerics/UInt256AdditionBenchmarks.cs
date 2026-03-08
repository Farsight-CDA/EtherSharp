using BenchmarkDotNet.Attributes;
using EtherSharp.Numerics;

namespace EtherSharp.Bench.Numerics;

[MemoryDiagnoser]
[ShortRunJob]
public class UInt256AdditionBenchmarks
{
    private const int OPERATION_COUNT = 1024;
    private const string U64_NO_CARRY = "U64NoCarry";
    private const string U64_CARRY_INTO_LIMB1 = "U64CarryIntoLimb1";
    private const string WIDE_ADD_SMALL_NO_CARRY = "WideAddSmallNoCarry";
    private const string WIDE_ADD_SMALL_CARRY_CHAIN = "WideAddSmallCarryChain";
    private const string WIDE_NO_CARRY = "WideNoCarry";
    private const string WIDE_CARRY_CHAIN = "WideCarryChain";
    private const string WIDE_OVERFLOW = "WideOverflow";

    private UInt256[] _left = null!;
    private UInt256[] _right = null!;

    public static IEnumerable<string> Scenarios => [
            U64_NO_CARRY,
            U64_CARRY_INTO_LIMB1,
            WIDE_ADD_SMALL_NO_CARRY,
            WIDE_ADD_SMALL_CARRY_CHAIN,
            WIDE_NO_CARRY,
            WIDE_CARRY_CHAIN,
            WIDE_OVERFLOW,
        ];

    [ParamsSource(nameof(Scenarios))]
    public string Scenario { get; set; } = null!;

    [GlobalSetup]
    public void Setup()
    {
        _left = new UInt256[OPERATION_COUNT];
        _right = new UInt256[OPERATION_COUNT];

        for(int i = 0; i < OPERATION_COUNT; i++)
        {
            (_left[i], _right[i]) = CreateOperands(i);
        }
    }

    [Benchmark(OperationsPerInvoke = OPERATION_COUNT)]
    public UInt256 OperatorAdd()
    {
        var acc = default(UInt256);
        var left = _left;
        var right = _right;

        for(int i = 0; i < OPERATION_COUNT; i++)
        {
            acc ^= left[i] + right[i];
        }

        return acc;
    }

    private (UInt256 Left, UInt256 Right) CreateOperands(int index)
        => Scenario switch
        {
            U64_NO_CARRY =>
                (
                    new UInt256(0x0123_4567_89ab_cdefUL + (uint) index),
                    new UInt256(0x0011_2233_4455_6677UL + (uint) (index & 0xff))),
            U64_CARRY_INTO_LIMB1 =>
                (
                    new UInt256(UInt64.MaxValue - (ulong) (index & 0x7)),
                    new UInt256((ulong) ((index & 0x7) + 9))),
            WIDE_ADD_SMALL_NO_CARRY =>
                (
                    new UInt256(
                        0x0123_4567_89ab_0000UL + (uint) index,
                        0x1111_1111_1111_1111,
                        0x2222_2222_2222_2222,
                        0x3333_3333_3333_3333),
                    new UInt256((ulong) ((index & 0xf) + 1))),
            WIDE_ADD_SMALL_CARRY_CHAIN =>
                (
                    new UInt256(
                        UInt64.MaxValue - (ulong) (index & 0x7),
                        UInt64.MaxValue,
                        UInt64.MaxValue,
                        0x0123_4567_89ab_cdef),
                    new UInt256((ulong) ((index & 0x7) + 9))),
            WIDE_NO_CARRY =>
                (
                    new UInt256(
                        0x0123_4567_89ab_0000UL + (uint) index,
                        0x1111_2222_3333_4444,
                        0x2222_3333_4444_5555,
                        0x3333_4444_5555_6666),
                    new UInt256(
                        0x0011_2233_4455_0000UL + (uint) (index << 1),
                        0x0101_0202_0303_0404,
                        0x0202_0303_0404_0505,
                        0x0303_0404_0505_0606)),
            WIDE_CARRY_CHAIN =>
                (
                    new UInt256(
                        UInt64.MaxValue,
                        UInt64.MaxValue,
                        UInt64.MaxValue,
                        0x7fff_ffff_ffff_f000UL + (uint) index),
                    new UInt256(1, 0, 0, 2)),
            WIDE_OVERFLOW =>
                (
                    new UInt256(
                        UInt64.MaxValue - (ulong) (index & 0x7),
                        UInt64.MaxValue,
                        UInt64.MaxValue,
                        UInt64.MaxValue),
                    new UInt256((ulong) ((index & 0x7) + 1))),
            _ => throw new InvalidOperationException($"Unknown scenario '{Scenario}'."),
        };
}
