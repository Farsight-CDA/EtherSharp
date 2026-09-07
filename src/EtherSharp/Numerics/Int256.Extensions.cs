// SPDX-FileCopyrightText: 2025 Demerzel Solutions Limited
// SPDX-License-Identifier: MIT

using System.Buffers.Binary;
using System.Runtime.CompilerServices;

namespace EtherSharp.Numerics;

/// <summary>
/// Provides binary read/write and sequence aggregation helpers for <see cref="Int256"/> values.
/// </summary>
public static class Int256Extensions
{
    extension(BinaryPrimitives)
    {
        /// <summary>
        /// Reads an <see cref="Int256"/> from the first 32 bytes of a little-endian span using two's-complement representation.
        /// </summary>
        /// <param name="source">Source span that must be at least 32 bytes long.</param>
        /// <returns>The decoded signed 256-bit integer.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The source is shorter than 32 bytes.</exception>
        public static Int256 ReadInt256LittleEndian(ReadOnlySpan<byte> source)
            => Unsafe.BitCast<UInt256, Int256>(BinaryPrimitives.ReadUInt256LittleEndian(source));

        /// <summary>
        /// Reads an <see cref="Int256"/> from the first 32 bytes of a big-endian span using two's-complement representation.
        /// </summary>
        /// <param name="source">Source span that must be at least 32 bytes long.</param>
        /// <returns>The decoded signed 256-bit integer.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The source is shorter than 32 bytes.</exception>
        public static Int256 ReadInt256BigEndian(ReadOnlySpan<byte> source)
            => Unsafe.BitCast<UInt256, Int256>(BinaryPrimitives.ReadUInt256BigEndian(source));

        /// <summary>
        /// Writes an <see cref="Int256"/> to the first 32 bytes of a span in little-endian two's-complement representation.
        /// </summary>
        /// <param name="destination">Destination span that must be at least 32 bytes long.</param>
        /// <param name="value">Value to write.</param>
        /// <exception cref="ArgumentOutOfRangeException">The destination is shorter than 32 bytes.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteInt256LittleEndian(Span<byte> destination, in Int256 value)
            => BinaryPrimitives.WriteUInt256LittleEndian(destination, in value._value);

        /// <summary>
        /// Writes an <see cref="Int256"/> to the first 32 bytes of a span in big-endian two's-complement representation.
        /// </summary>
        /// <param name="destination">Destination span that must be at least 32 bytes long.</param>
        /// <param name="value">Value to write.</param>
        /// <exception cref="ArgumentOutOfRangeException">The destination is shorter than 32 bytes.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteInt256BigEndian(Span<byte> destination, in Int256 value)
            => BinaryPrimitives.WriteUInt256BigEndian(destination, in value._value);
    }

    extension(IEnumerable<Int256> e)
    {
        /// <summary>
        /// Computes the sum of all values in the sequence.
        /// </summary>
        /// <returns>The sum of the values in <c>e</c>.</returns>
        public Int256 Sum()
        {
            ArgumentNullException.ThrowIfNull(e);

            var result = Int256.Zero;
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
        /// Computes the sum of the projected <see cref="Int256"/> values in the sequence.
        /// </summary>
        /// <param name="selector">Projects each element of the sequence to an <see cref="Int256"/> value.</param>
        /// <returns>The sum of the projected values in <c>e</c>.</returns>
        public Int256 Sum(Func<T, Int256> selector)
        {
            ArgumentNullException.ThrowIfNull(e);
            ArgumentNullException.ThrowIfNull(selector);

            var result = Int256.Zero;
            foreach(var item in e)
            {
                result += selector(item);
            }
            return result;
        }
    }
}
