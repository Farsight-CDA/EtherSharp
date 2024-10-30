using System.Numerics;
namespace EtherSharp.Types;

public readonly struct TargetBlockNumber
{
    private readonly string _value;
    private TargetBlockNumber(string value)
    {
        _value = value;
    }
    public static TargetBlockNumber Latest => new("latest");
    public static TargetBlockNumber Earliest => new("earliest");
    public static TargetBlockNumber Pending => new("pending");
    public static TargetBlockNumber Height(BigInteger number) => new($"0x{number:X}");
    public override string ToString() => _value;
}