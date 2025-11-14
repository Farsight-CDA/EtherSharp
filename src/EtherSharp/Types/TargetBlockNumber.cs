using System.Diagnostics.CodeAnalysis;
using System.Globalization;
namespace EtherSharp.Types;

/// <summary>
/// A struct used to specify the block number to use for certain client calls.
/// </summary>
/// <remarks>
/// Not all available TargetBlockNumbers are supported on every blockchain network.
/// </remarks>
public readonly struct TargetBlockNumber
{
    private readonly string? _value;
    private TargetBlockNumber(string value)
    {
        _value = value;
    }

    /// <summary>
    /// Targets the latest block.
    /// </summary>
    public static TargetBlockNumber Latest { get; } = new(null!);
    /// <summary>
    /// Targets the earliest available block on the node.
    /// </summary>
    public static TargetBlockNumber Earliest { get; } = new("earliest");
    /// <summary>
    /// Targets the latest safe block.
    /// </summary>
    public static TargetBlockNumber Safe { get; } = new("safe");
    /// <summary>
    /// Targets the latest finalized block.
    /// </summary>
    public static TargetBlockNumber Finalized { get; } = new("finalized");
    /// <summary>
    /// Targets the pending block.
    /// </summary>
    public static TargetBlockNumber Pending { get; } = new("pending");
    /// <summary>
    /// Targets the block at a given height.
    /// </summary>
    public static TargetBlockNumber Height(ulong number)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(number, (ulong) 1);
        return new($"0x{number.ToString("X", CultureInfo.InvariantCulture).TrimStart('0')}");
    }

    /// <inheritdoc/>
    public override string ToString() => _value ?? Latest._value!;

    /// <inheritdoc/>
    public override bool Equals([NotNullWhen(true)] object? obj)
        => obj is TargetBlockNumber tbn && _value == tbn._value;

    /// <inheritdoc/>
    public override int GetHashCode()
        => HashCode.Combine(_value);

    /// <inheritdoc/>
    public static bool operator ==(TargetBlockNumber left, TargetBlockNumber right)
        => left.Equals(right);
    /// <inheritdoc/>
    public static bool operator !=(TargetBlockNumber left, TargetBlockNumber right)
        => !(left == right);
}