using System.Runtime.CompilerServices;
using CoreUInt256 = Nethermind.Int256.UInt256;

namespace EtherSharp.Numerics;

public readonly partial struct UInt256
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ref readonly CoreUInt256 AsUpstream(in UInt256 value)
        => ref Unsafe.As<UInt256, CoreUInt256>(ref Unsafe.AsRef(in value));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static UInt256 FromUpstream(CoreUInt256 value)
        => Unsafe.BitCast<CoreUInt256, UInt256>(value);
}
