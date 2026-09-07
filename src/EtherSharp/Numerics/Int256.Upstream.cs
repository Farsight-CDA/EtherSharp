using System.Runtime.CompilerServices;
using CoreInt256 = Nethermind.Int256.Int256;

namespace EtherSharp.Numerics;

public readonly partial struct Int256
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ref readonly CoreInt256 AsUpstream(in Int256 value)
        => ref Unsafe.As<Int256, CoreInt256>(ref Unsafe.AsRef(in value));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Int256 FromUpstream(CoreInt256 value)
        => Unsafe.BitCast<CoreInt256, Int256>(value);
}
