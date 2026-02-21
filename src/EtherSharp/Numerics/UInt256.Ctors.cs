// SPDX-FileCopyrightText: 2025 Demerzel Solutions Limited
// SPDX-License-Identifier: MIT

#pragma warning disable CS1591

using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace EtherSharp.Numerics;

public readonly partial struct UInt256
{
    public UInt256(ulong u0 = 0, ulong u1 = 0, ulong u2 = 0, ulong u3 = 0)
    {
        if(Vector256.IsHardwareAccelerated)
        {
            Unsafe.SkipInit(out _u0);
            Unsafe.SkipInit(out _u1);
            Unsafe.SkipInit(out _u2);
            Unsafe.SkipInit(out _u3);
            Unsafe.As<ulong, Vector256<ulong>>(ref _u0) = Vector256.Create(u0, u1, u2, u3);
        }
        else
        {
            _u0 = u0;
            _u1 = u1;
            _u2 = u2;
            _u3 = u3;
        }
    }

    internal UInt256(in ReadOnlySpan<byte> bytes, bool isBigEndian = false)
    {
        if(bytes.Length != 32)
        {
            throw new ArgumentOutOfRangeException(nameof(bytes));
        }

        if(isBigEndian)
        {
            if(Avx2.IsSupported)
            {
                Unsafe.SkipInit(out _u0);
                Unsafe.SkipInit(out _u1);
                Unsafe.SkipInit(out _u2);
                Unsafe.SkipInit(out _u3);
                var data = Unsafe.ReadUnaligned<Vector256<byte>>(ref MemoryMarshal.GetReference(bytes));
                var shuffle = Vector256.Create(
                    0x18191a1b1c1d1e1ful,
                    0x1011121314151617ul,
                    0x08090a0b0c0d0e0ful,
                    0x0001020304050607ul
                ).AsByte();
                if(Avx512Vbmi.VL.IsSupported)
                {
                    var convert = Avx512Vbmi.VL.PermuteVar32x8(data, shuffle);
                    Unsafe.As<ulong, Vector256<byte>>(ref _u0) = convert;
                }
                else
                {
                    var convert = Avx2.Shuffle(data, shuffle);
                    var permute = Avx2.Permute4x64(Unsafe.As<Vector256<byte>, Vector256<ulong>>(ref convert), 0b_01_00_11_10);
                    Unsafe.As<ulong, Vector256<ulong>>(ref _u0) = permute;
                }
            }
            else
            {
                _u3 = BinaryPrimitives.ReadUInt64BigEndian(bytes[..8]);
                _u2 = BinaryPrimitives.ReadUInt64BigEndian(bytes.Slice(8, 8));
                _u1 = BinaryPrimitives.ReadUInt64BigEndian(bytes.Slice(16, 8));
                _u0 = BinaryPrimitives.ReadUInt64BigEndian(bytes.Slice(24, 8));
            }
        }
        else
        {
            if(Vector256.IsHardwareAccelerated)
            {
                Unsafe.SkipInit(out _u0);
                Unsafe.SkipInit(out _u1);
                Unsafe.SkipInit(out _u2);
                Unsafe.SkipInit(out _u3);
                Unsafe.As<ulong, Vector256<byte>>(ref _u0) = Vector256.Create(bytes);
            }
            else
            {
                _u0 = BinaryPrimitives.ReadUInt64LittleEndian(bytes[..8]);
                _u1 = BinaryPrimitives.ReadUInt64LittleEndian(bytes.Slice(8, 8));
                _u2 = BinaryPrimitives.ReadUInt64LittleEndian(bytes.Slice(16, 8));
                _u3 = BinaryPrimitives.ReadUInt64LittleEndian(bytes.Slice(24, 8));
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static UInt256 Create(ulong u0, ulong u1, ulong u2, ulong u3)
    {
        if(Vector256.IsHardwareAccelerated)
        {
            var v = Vector256.Create(u0, u1, u2, u3);
            return Unsafe.As<Vector256<ulong>, UInt256>(ref v);
        }
        else
        {
            Unsafe.SkipInit(out UInt256 r);
            ref ulong p = ref Unsafe.As<UInt256, ulong>(ref r);
            p = u0;
            Unsafe.Add(ref p, 1) = u1;
            Unsafe.Add(ref p, 2) = u2;
            Unsafe.Add(ref p, 3) = u3;
            return r;
        }
    }
}
