// SPDX-FileCopyrightText: 2025 Demerzel Solutions Limited
// SPDX-License-Identifier: MIT

using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;

namespace EtherSharp.Numerics;

/// <summary>
/// Provides binary read/write and sequence aggregation helpers for <see cref="UInt256"/> values.
/// </summary>
public static class UInt256Extensions
{
    extension(BinaryPrimitives)
    {
        /// <summary>
        /// Reads a <see cref="UInt256"/> from the first 32 bytes of a little-endian span.
        /// </summary>
        /// <param name="source">Source span that must be at least 32 bytes long.</param>
        /// <returns>The decoded unsigned 256-bit integer.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The source is shorter than 32 bytes.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt256 ReadUInt256LittleEndian(ReadOnlySpan<byte> source)
        {
            source = source[..32];

            if(Vector256.IsHardwareAccelerated)
            {
                Unsafe.SkipInit(out UInt256 result);
                Unsafe.As<UInt256, Vector256<byte>>(ref result) = Vector256.Create(source);
                return result;
            }
            //
            return new UInt256(
                BinaryPrimitives.ReadUInt64LittleEndian(source[..8]),
                BinaryPrimitives.ReadUInt64LittleEndian(source.Slice(8, 8)),
                BinaryPrimitives.ReadUInt64LittleEndian(source.Slice(16, 8)),
                BinaryPrimitives.ReadUInt64LittleEndian(source.Slice(24, 8))
            );
        }

        /// <summary>
        /// Reads a <see cref="UInt256"/> from the first 32 bytes of a big-endian span.
        /// </summary>
        /// <param name="source">Source span that must be at least 32 bytes long.</param>
        /// <returns>The decoded unsigned 256-bit integer.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The source is shorter than 32 bytes.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt256 ReadUInt256BigEndian(ReadOnlySpan<byte> source)
        {
            source = source[..32];

            if(Avx2.IsSupported)
            {
                var data = Unsafe.ReadUnaligned<Vector256<byte>>(ref MemoryMarshal.GetReference(source));
                var shuffle = Vector256.Create(
                    0x18191a1b1c1d1e1ful,
                    0x1011121314151617ul,
                    0x08090a0b0c0d0e0ful,
                    0x0001020304050607ul
                ).AsByte();

                if(Avx512Vbmi.VL.IsSupported)
                {
                    var convert = Avx512Vbmi.VL.PermuteVar32x8(data, shuffle);
                    Unsafe.SkipInit(out UInt256 result);
                    Unsafe.As<UInt256, Vector256<byte>>(ref result) = convert;
                    return result;
                }
                else
                {
                    var convert = Avx2.Shuffle(data, shuffle);
                    var permute = Avx2.Permute4x64(convert.AsUInt64(), 0b_01_00_11_10);
                    Unsafe.SkipInit(out UInt256 result);
                    Unsafe.As<UInt256, Vector256<ulong>>(ref result) = permute;
                    return result;
                }
            }
            else if(AdvSimd.Arm64.IsSupported)
            {
                ref byte src = ref MemoryMarshal.GetReference(source);
                var reversedLower = AdvSimd.ReverseElement8(Vector128.LoadUnsafe(ref src).AsUInt64());
                var reversedUpper = AdvSimd.ReverseElement8(Vector128.LoadUnsafe(ref src, 16).AsUInt64());
                var convert = Vector256.Create(
                    AdvSimd.ExtractVector128(reversedUpper, reversedUpper, 1),
                    AdvSimd.ExtractVector128(reversedLower, reversedLower, 1)
                );
                Unsafe.SkipInit(out UInt256 result);
                Unsafe.As<UInt256, Vector256<ulong>>(ref result) = convert;
                return result;
            }

            return new UInt256(
                BinaryPrimitives.ReadUInt64BigEndian(source.Slice(24, 8)),
                BinaryPrimitives.ReadUInt64BigEndian(source.Slice(16, 8)),
                BinaryPrimitives.ReadUInt64BigEndian(source.Slice(8, 8)),
                BinaryPrimitives.ReadUInt64BigEndian(source[..8])
            );
        }

        /// <summary>
        /// Writes a <see cref="UInt256"/> to the first 32 bytes of a span in little-endian order.
        /// </summary>
        /// <param name="destination">Destination span that must be at least 32 bytes long.</param>
        /// <param name="value">Value to write.</param>
        /// <exception cref="ArgumentOutOfRangeException">The destination is shorter than 32 bytes.</exception>
        public static void WriteUInt256LittleEndian(Span<byte> destination, in UInt256 value)
        {
            if(destination.Length < 32)
            {
                throw new ArgumentOutOfRangeException(nameof(destination));
            }

            if(Avx.IsSupported)
            {
                Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(destination), Unsafe.As<UInt256, Vector256<ulong>>(ref Unsafe.AsRef(in value)));
            }
            else
            {
                BinaryPrimitives.WriteUInt64LittleEndian(destination[..8], value._u0);
                BinaryPrimitives.WriteUInt64LittleEndian(destination.Slice(8, 8), value._u1);
                BinaryPrimitives.WriteUInt64LittleEndian(destination.Slice(16, 8), value._u2);
                BinaryPrimitives.WriteUInt64LittleEndian(destination.Slice(24, 8), value._u3);
            }
        }

        /// <summary>
        /// Writes a <see cref="UInt256"/> to the first 32 bytes of a span in big-endian order.
        /// </summary>
        /// <param name="destination">Destination span that must be at least 32 bytes long.</param>
        /// <param name="value">Value to write.</param>
        /// <exception cref="ArgumentOutOfRangeException">The destination is shorter than 32 bytes.</exception>
        public static void WriteUInt256BigEndian(Span<byte> destination, in UInt256 value)
        {
            if(destination.Length < 32)
            {
                throw new ArgumentOutOfRangeException(nameof(destination));
            }

            if(Avx2.IsSupported)
            {
                var data = Unsafe.As<UInt256, Vector256<byte>>(ref Unsafe.AsRef(in value));
                var shuffle = Vector256.Create(
                    0x18191a1b1c1d1e1ful,
                    0x1011121314151617ul,
                    0x08090a0b0c0d0e0ful,
                    0x0001020304050607ul
                ).AsByte();

                if(Avx512Vbmi.VL.IsSupported)
                {
                    var convert = Avx512Vbmi.VL.PermuteVar32x8(data, shuffle);
                    Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(destination), convert);
                }
                else
                {
                    var convert = Avx2.Shuffle(data, shuffle);
                    var permute = Avx2.Permute4x64(convert.AsUInt64(), 0b_01_00_11_10);
                    Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(destination), permute);
                }
            }
            else if(AdvSimd.Arm64.IsSupported)
            {
                var reversedLower = AdvSimd.ReverseElement8(Unsafe.As<ulong, Vector128<ulong>>(ref Unsafe.AsRef(in value._u0)));
                var reversedUpper = AdvSimd.ReverseElement8(Unsafe.As<ulong, Vector128<ulong>>(ref Unsafe.AsRef(in value._u2)));
                ref byte dst = ref MemoryMarshal.GetReference(destination);
                AdvSimd.ExtractVector128(reversedUpper, reversedUpper, 1).AsByte().StoreUnsafe(ref dst);
                AdvSimd.ExtractVector128(reversedLower, reversedLower, 1).AsByte().StoreUnsafe(ref dst, 16);
            }
            else
            {
                BinaryPrimitives.WriteUInt64BigEndian(destination[..8], value._u3);
                BinaryPrimitives.WriteUInt64BigEndian(destination.Slice(8, 8), value._u2);
                BinaryPrimitives.WriteUInt64BigEndian(destination.Slice(16, 8), value._u1);
                BinaryPrimitives.WriteUInt64BigEndian(destination.Slice(24, 8), value._u0);
            }
        }
    }

    extension(IEnumerable<UInt256> e)
    {
        /// <summary>
        /// Computes the sum of all values in the sequence.
        /// </summary>
        /// <returns>The sum of the values in <c>e</c>.</returns>
        public UInt256 Sum()
        {
            var result = UInt256.Zero;
            foreach(var value in e)
            {
                result += value;
            }
            return result;
        }
    }

    extension<T>(IEnumerable<T> e)
    {
        /// <summary>
        /// Computes the sum of the projected <see cref="UInt256"/> values in the sequence.
        /// </summary>
        /// <param name="selector">Projects each element of the sequence to a <see cref="UInt256"/> value.</param>
        /// <returns>The sum of the projected values in <c>e</c>.</returns>
        public UInt256 Sum(Func<T, UInt256> selector)
        {
            var result = UInt256.Zero;
            foreach(var item in e)
            {
                result += selector(item);
            }
            return result;
        }
    }
}
