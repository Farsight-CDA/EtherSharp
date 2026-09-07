using System.Runtime.CompilerServices;

namespace EtherSharp.Numerics;

public readonly partial struct UInt256 : IEquatable<UInt256>
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
}
