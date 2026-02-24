using System.Buffers.Binary;
using System.Diagnostics.CodeAnalysis;
namespace EtherSharp.Types;

/// <summary>
/// A struct used to specify the block number to use for certain client calls.
/// </summary>
/// <remarks>
/// Not all available TargetHeights are supported on every blockchain network.
/// </remarks>
public readonly struct TargetHeight(ulong value, string rawValue)
{
    /// <summary>
    /// The block height or 0 if it is a string based one.
    /// </summary>
    public ulong Value { get; } = value;
    private readonly string? _rawValue = rawValue;

    /// <summary>
    /// Targets the latest block.
    /// </summary>
    public static TargetHeight Latest { get; } = new(0, "latest");
    /// <summary>
    /// Targets the earliest available block on the node.
    /// </summary>
    public static TargetHeight Earliest { get; } = new(0, "earliest");
    /// <summary>
    /// Targets the latest safe block.
    /// </summary>
    public static TargetHeight Safe { get; } = new(0, "safe");
    /// <summary>
    /// Targets the latest finalized block.
    /// </summary>
    public static TargetHeight Finalized { get; } = new(0, "finalized");
    /// <summary>
    /// Targets the pending block.
    /// </summary>
    public static TargetHeight Pending { get; } = new(0, "pending");
    /// <summary>
    /// Targets the block at a given height.
    /// </summary>
    public static TargetHeight Height(ulong value)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(value, (ulong) 1);

        Span<byte> byteBuffer = stackalloc byte[sizeof(ulong)];

        BinaryPrimitives.WriteUInt64BigEndian(byteBuffer, value);

        byteBuffer = byteBuffer.TrimStart((byte) 0);

        int dataIndex = byteBuffer[0] < 16 ? 1 : 2;
        Span<char> hexBuffer = stackalloc char[(byteBuffer.Length * 2) + dataIndex];

        if(!Convert.TryToHexString(byteBuffer, hexBuffer[dataIndex..], out _))
        {
            throw new InvalidOperationException("Failed to convert to hex");
        }

        hexBuffer[0] = '0';
        hexBuffer[1] = 'x';

        return new TargetHeight(value, hexBuffer.ToString());
    }

    /// <inheritdoc/>
    public override string ToString() => _rawValue ?? "latest";

    /// <inheritdoc/>
    public override bool Equals([NotNullWhen(true)] object? obj)
        => obj is TargetHeight tbn && _rawValue == tbn._rawValue;

    /// <inheritdoc/>
    public override int GetHashCode()
        => HashCode.Combine(_rawValue);

    /// <inheritdoc/>
    public static bool operator ==(TargetHeight left, TargetHeight right)
        => left.Equals(right);
    /// <inheritdoc/>
    public static bool operator !=(TargetHeight left, TargetHeight right)
        => !left.Equals(right);
}
