using System.Runtime.CompilerServices;

namespace EtherSharp.Numerics;

public readonly partial struct Int256 : IEquatable<Int256>
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
}
