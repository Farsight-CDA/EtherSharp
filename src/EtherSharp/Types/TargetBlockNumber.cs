using System.Diagnostics.CodeAnalysis;
using System.Numerics;
namespace EtherSharp.Types;

public readonly struct TargetBlockNumber
{
    private readonly string? _value;
    private TargetBlockNumber(string value)
    {
        _value = value;
    }
    public static TargetBlockNumber Latest { get; } = new(null!);
    public static TargetBlockNumber Earliest { get; } = new("earliest");
    public static TargetBlockNumber Safe { get; } = new("safe");
    public static TargetBlockNumber Finalized { get; } = new("finalized");
    public static TargetBlockNumber Pending { get; } = new("pending");
    public static TargetBlockNumber Height(BigInteger number) => new($"0x{number.ToString("X").TrimStart('0')}");
    public override string ToString() => _value ?? "latest";

    public override bool Equals([NotNullWhen(true)] object? obj)
        => obj is TargetBlockNumber tbn && _value == tbn._value;

    public static bool operator ==(TargetBlockNumber left, TargetBlockNumber right)
        => left.Equals(right);
    public static bool operator !=(TargetBlockNumber left, TargetBlockNumber right)
        => !(left == right);

    public override int GetHashCode()
        => HashCode.Combine(_value);
}