// SPDX-FileCopyrightText: 2025 Demerzel Solutions Limited
// SPDX-License-Identifier: MIT

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace EtherSharp.Numerics;

public readonly partial struct Int256
{
    // Ported from Nethermind.Int256 55ca462. Counts >= 256 saturate to the sign fill.
    // Negative multiples of 64 also saturate; other negative counts use n & 63.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void RightShift(in Int256 x, int n, out Int256 res)
    {
        // Read all limbs before writing: the result may alias the input.
        ulong x0 = x._value._u0, x1 = x._value._u1, x2 = x._value._u2, x3 = x._value._u3;
        long top = unchecked((long) x3);
        int wordShift = n >> 6;
        if((uint) wordShift >= 4)
        {
            if(wordShift >= 0 || (n & 63) == 0)
            {
                ulong saturated = (ulong) (top >> 63);
                SetLimbs(out res, saturated, saturated, saturated, saturated);
                return;
            }
            wordShift = 0;
        }

        int bitShift = n & 63;
        int carryShift = 63 - bitShift;
        if(Avx2.IsSupported)
        {
            var v = Unsafe.BitCast<UInt256, Vector256<ulong>>(x._value);
            var fill = Avx2.Permute4x64(Vector256.LessThan(v.AsInt64(), Vector256<long>.Zero).AsUInt64(), 0xFF);
            Vector256<ulong> lo, hi;
            if(Avx512F.VL.IsSupported)
            {
                ref var windows = ref Unsafe.As<ulong, Vector256<ulong>>(ref MemoryMarshal.GetReference(ShiftWindows));
                lo = Avx512F.VL.PermuteVar4x64x2(v, Unsafe.Add(ref windows, wordShift), fill);
                hi = Avx512F.VL.PermuteVar4x64x2(v, Unsafe.Add(ref windows, wordShift + 1), fill);
            }
            else
            {
                ref var windows = ref Unsafe.As<uint, Vector256<uint>>(ref MemoryMarshal.GetReference(ShiftWindowPairs));
                ref var vacated = ref Unsafe.As<ulong, Vector256<ulong>>(ref MemoryMarshal.GetReference(ShiftVacated));
                var value = v.AsUInt32();
                lo = Vector256.ConditionalSelect(Unsafe.Add(ref vacated, wordShift), fill,
                    Avx2.PermuteVar8x32(value, Unsafe.Add(ref windows, wordShift)).AsUInt64()
                );
                hi = Vector256.ConditionalSelect(Unsafe.Add(ref vacated, wordShift + 1), fill,
                    Avx2.PermuteVar8x32(value, Unsafe.Add(ref windows, wordShift + 1)).AsUInt64()
                );
            }

            Unsafe.SkipInit(out res);
            Unsafe.As<Int256, Vector256<ulong>>(ref res) =
                Avx2.ShiftRightLogical(lo, Vector128.CreateScalar((ulong) bitShift))
                | Avx2.ShiftLeftLogical(Avx2.ShiftLeftLogical(hi, 1), Vector128.CreateScalar((ulong) carryShift));
            return;
        }

        // Splitting the carry shift avoids C# masking a shift of 64 to zero.
        if(wordShift == 0)
        {
            SetLimbs(out res,
                (x0 >> bitShift) | (x1 << 1 << carryShift),
                (x1 >> bitShift) | (x2 << 1 << carryShift),
                (x2 >> bitShift) | (x3 << 1 << carryShift),
                (ulong) (top >> bitShift)
            );
        }
        else if(wordShift == 1)
        {
            SetLimbs(out res,
                (x1 >> bitShift) | (x2 << 1 << carryShift),
                (x2 >> bitShift) | (x3 << 1 << carryShift),
                (ulong) (top >> bitShift),
                (ulong) (top >> 63)
            );
        }
        else if(wordShift == 2)
        {
            ulong fill = (ulong) (top >> 63);
            SetLimbs(out res,
                (x2 >> bitShift) | (x3 << 1 << carryShift),
                (ulong) (top >> bitShift), fill, fill
            );
        }
        else
        {
            ulong fill = (ulong) (top >> 63);
            SetLimbs(out res, (ulong) (top >> bitShift), fill, fill, fill);
        }
    }

    // Each window selects four limbs; indices past limb 3 select the sign fill.
    private static ReadOnlySpan<ulong> ShiftWindows => [
        0, 1, 2, 3,
        1, 2, 3, 4,
        2, 3, 4, 5,
        3, 4, 5, 6,
        4, 5, 6, 7
    ];

    // AVX2 uses paired 32-bit lanes and replaces wrapped lanes with sign fill.
    private static ReadOnlySpan<uint> ShiftWindowPairs => [
        0, 1, 2, 3, 4, 5, 6, 7,
        2, 3, 4, 5, 6, 7, 0, 1,
        4, 5, 6, 7, 0, 1, 2, 3,
        6, 7, 0, 1, 2, 3, 4, 5,
        0, 1, 2, 3, 4, 5, 6, 7
    ];

    private static ReadOnlySpan<ulong> ShiftVacated => [
        0, 0, 0, 0,
        0, 0, 0, UInt64.MaxValue,
        0, 0, UInt64.MaxValue, UInt64.MaxValue,
        0, UInt64.MaxValue, UInt64.MaxValue, UInt64.MaxValue,
        UInt64.MaxValue, UInt64.MaxValue, UInt64.MaxValue, UInt64.MaxValue
    ];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void SetLimbs(out Int256 res, ulong z0, ulong z1, ulong z2, ulong z3)
    {
        Unsafe.SkipInit(out res);
        Unsafe.As<Int256, Vector256<ulong>>(ref res) = Vector256.Create(z0, z1, z2, z3);
    }
}
