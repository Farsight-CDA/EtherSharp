using System.Runtime.CompilerServices;

namespace EtherSharp.Numerics;

public readonly partial struct UInt256 : IComparable, IComparable<UInt256>, IEquatable<UInt256>
{
    /// <inheritdoc cref="Equals(UInt256)"/>
    [OverloadResolutionPriority(1)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(in UInt256 other)
        => AsUpstream(in this).Equals(in AsUpstream(in other));

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(UInt256 other)
        => Equals(in other);

    /// <inheritdoc/>
    public override bool Equals(object? obj)
        => obj is UInt256 other && Equals(in other);

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode()
        => AsUpstream(in this).GetHashCode();

    /// <inheritdoc cref="CompareTo(UInt256)"/>
    [OverloadResolutionPriority(1)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int CompareTo(in UInt256 other)
        => AsUpstream(in this).CompareTo(in AsUpstream(in other));

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int CompareTo(UInt256 other)
        => CompareTo(in other);

    /// <summary>
    /// Compares this value to a boxed <see cref="UInt256"/>.
    /// </summary>
    /// <param name="obj">The value to compare to, or null.</param>
    /// <returns>A negative value, zero, or a positive value if this value is less than, equal to, or greater than <paramref name="obj"/>; 1 if <paramref name="obj"/> is null.</returns>
    /// <exception cref="ArgumentException"><paramref name="obj"/> is not null and is not a <see cref="UInt256"/>.</exception>
    public int CompareTo(object? obj)
        => obj switch
        {
            null => 1,
            UInt256 other => CompareTo(in other),
            _ => throw new ArgumentException("Object must be of type UInt256.", nameof(obj))
        };
}
