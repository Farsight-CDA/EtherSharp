using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace EtherSharp.Numerics;

/// <summary>
/// Adds 256-bit integer read/write helpers to <see cref="BinaryPrimitives"/>.
/// </summary>
public static class BinaryPrimitivesExtensions
{
    extension(BinaryPrimitives)
    {
        /// <summary>
        /// Reads a <see cref="UInt256"/> from a 32-byte little-endian span.
        /// </summary>
        /// <param name="source">Source bytes to read from.</param>
        /// <returns>The decoded unsigned 256-bit integer.</returns>
        public static UInt256 ReadUInt256LittleEndian(ReadOnlySpan<byte> source)
            => new UInt256(source, false);

        /// <summary>
        /// Reads a <see cref="UInt256"/> from a 32-byte big-endian span.
        /// </summary>
        /// <param name="source">Source bytes to read from.</param>
        /// <returns>The decoded unsigned 256-bit integer.</returns>
        public static UInt256 ReadUInt256BigEndian(ReadOnlySpan<byte> source)
            => new UInt256(source, true);

        /// <summary>
        /// Writes a <see cref="UInt256"/> to a 32-byte span in little-endian order.
        /// </summary>
        /// <param name="destination">Destination span that must be exactly 32 bytes long.</param>
        /// <param name="value">Value to write.</param>
        public static void WriteUInt256LittleEndian(Span<byte> destination, UInt256 value)
        {
            if(destination.Length != 32)
            {
                throw new NotSupportedException();
            }

            if(Avx.IsSupported)
            {
                Unsafe.As<byte, Vector256<ulong>>(ref MemoryMarshal.GetReference(destination)) = Unsafe.As<ulong, Vector256<ulong>>(ref Unsafe.AsRef(in value._u0));
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
        /// Writes a <see cref="UInt256"/> to a 32-byte span in big-endian order.
        /// </summary>
        /// <param name="destination">Destination span that must be exactly 32 bytes long.</param>
        /// <param name="value">Value to write.</param>
        public static void WriteUInt256BigEndian(Span<byte> destination, UInt256 value)
        {
            if(destination.Length != 32)
            {
                throw new NotSupportedException();
            }

            BinaryPrimitives.WriteUInt64BigEndian(destination[..8], value._u3);
            BinaryPrimitives.WriteUInt64BigEndian(destination.Slice(8, 8), value._u2);
            BinaryPrimitives.WriteUInt64BigEndian(destination.Slice(16, 8), value._u1);
            BinaryPrimitives.WriteUInt64BigEndian(destination.Slice(24, 8), value._u0);
        }

        /// <summary>
        /// Reads an <see cref="Int256"/> from a 32-byte little-endian span.
        /// </summary>
        /// <param name="source">Source bytes to read from.</param>
        /// <returns>The decoded signed 256-bit integer.</returns>
        public static Int256 ReaInt256dLittleEndian(ReadOnlySpan<byte> source)
            => new Int256(new UInt256(source, false));

        /// <summary>
        /// Reads an <see cref="Int256"/> from a 32-byte big-endian span.
        /// </summary>
        /// <param name="source">Source bytes to read from.</param>
        /// <returns>The decoded signed 256-bit integer.</returns>
        public static Int256 ReadInt256BigEndian(ReadOnlySpan<byte> source)
            => new Int256(new UInt256(source, true));

        /// <summary>
        /// Writes an <see cref="Int256"/> to a 32-byte span in little-endian order.
        /// </summary>
        /// <param name="destination">Destination span that must be exactly 32 bytes long.</param>
        /// <param name="value">Value to write.</param>
        public static void WriteInt256LittleEndian(Span<byte> destination, Int256 value)
            => WriteUInt256LittleEndian(destination, value._value);

        /// <summary>
        /// Writes an <see cref="Int256"/> to a 32-byte span in big-endian order.
        /// </summary>
        /// <param name="destination">Destination span that must be exactly 32 bytes long.</param>
        /// <param name="value">Value to write.</param>
        public static void WriteInt256BigEndian(Span<byte> destination, Int256 value)
            => WriteUInt256BigEndian(destination, value._value);
    }
}
