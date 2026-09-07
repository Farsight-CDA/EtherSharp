using System.Runtime.CompilerServices;

namespace EtherSharp.Numerics;

public readonly partial struct Int256 : IComparable, IComparable<Int256>, IEquatable<Int256>
{
    /// <summary>
    /// <inheritdoc cref="Equals(Int256)" path="/summary/node()"/>
    /// </summary>
    [OverloadResolutionPriority(1)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(in Int256 other)
        => _value.Equals(in other._value);

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(Int256 other)
        => Equals(in other);

    /// <inheritdoc/>
    public override bool Equals(object? obj)
        => obj is Int256 other && Equals(in other);

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode()
        => _value.GetHashCode();

    /// <inheritdoc cref="CompareTo(Int256)"/>
    [OverloadResolutionPriority(1)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int CompareTo(in Int256 other)
        => AsUpstream(in this).CompareTo(in AsUpstream(in other));

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int CompareTo(Int256 other)
        => CompareTo(in other);

    /// <summary>
    /// Compares this value to a boxed <see cref="Int256"/>.
    /// </summary>
    /// <param name="obj">The value to compare to, or null.</param>
    /// <returns>A negative value, zero, or a positive value if this value is less than, equal to, or greater than <paramref name="obj"/>; 1 if <paramref name="obj"/> is null.</returns>
    /// <exception cref="ArgumentException"><paramref name="obj"/> is not null and is not an <see cref="Int256"/>.</exception>
    public int CompareTo(object? obj)
        => obj switch
        {
            null => 1,
            Int256 other => CompareTo(in other),
            _ => throw new ArgumentException("Object must be of type Int256.", nameof(obj))
        };
}
