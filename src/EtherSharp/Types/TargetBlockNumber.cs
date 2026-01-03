using System.Buffers.Binary;
using System.Diagnostics.CodeAnalysis;
namespace EtherSharp.Types;

/// <summary>
/// A struct used to specify the block number to use for certain client calls.
/// </summary>
/// <remarks>
/// Not all available TargetBlockNumbers are supported on every blockchain network.
/// </remarks>
public readonly struct TargetBlockNumber
{
    /// <summary>
    /// The block height or 0 if it is a string based one.
    /// </summary>
    public ulong Value { get; }
    private readonly string? _rawValue;

    private TargetBlockNumber(ulong value, string rawValue)
    {
        Value = value;
        _rawValue = rawValue;
    }

    /// <summary>
    /// Targets the latest block.
    /// </summary>
    public static TargetBlockNumber Latest { get; } = new(0, "latest");
    /// <summary>
    /// Targets the earliest available block on the node.
    /// </summary>
    public static TargetBlockNumber Earliest { get; } = new(0, "earliest");
    /// <summary>
    /// Targets the latest safe block.
    /// </summary>
    public static TargetBlockNumber Safe { get; } = new(0, "safe");
    /// <summary>
    /// Targets the latest finalized block.
    /// </summary>
    public static TargetBlockNumber Finalized { get; } = new(0, "finalized");
    /// <summary>
    /// Targets the pending block.
    /// </summary>
    public static TargetBlockNumber Pending { get; } = new(0, "pending");
    /// <summary>
    /// Targets the block at a given height.
    /// </summary>
    public static TargetBlockNumber Height(ulong value)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(value, (ulong) 1);

        Span<byte> bytesBuffer = stackalloc byte[8];
        BinaryPrimitives.WriteUInt64BigEndian(bytesBuffer, value);

        Span<char> charBuffer = stackalloc char[16];

        Convert.TryToHexString(bytesBuffer, charBuffer, out _);
        int nonZeroIndex = charBuffer.IndexOfAnyExcept('0');

        if(nonZeroIndex < 2)
        {
            throw new ArgumentOutOfRangeException(nameof(value), "Exceeds max block size");
        }

        charBuffer[nonZeroIndex - 1] = 'x';
        charBuffer[nonZeroIndex - 2] = '0';

        return new TargetBlockNumber(value, charBuffer[(nonZeroIndex - 2)..].ToString());
    }

    /// <inheritdoc/>
    public override string ToString() => _rawValue ?? "latest";

    /// <inheritdoc/>
    public override bool Equals([NotNullWhen(true)] object? obj)
        => obj is TargetBlockNumber tbn && _rawValue == tbn._rawValue;

    /// <inheritdoc/>
    public override int GetHashCode()
        => HashCode.Combine(_rawValue);

    /// <inheritdoc/>
    public static bool operator ==(TargetBlockNumber left, TargetBlockNumber right)
        => left.Equals(right);
    /// <inheritdoc/>
    public static bool operator !=(TargetBlockNumber left, TargetBlockNumber right)
        => !(left == right);
}