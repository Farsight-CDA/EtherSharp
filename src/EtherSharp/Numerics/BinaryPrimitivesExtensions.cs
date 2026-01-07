using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace EtherSharp.Numerics;

public static class BinaryPrimitivesExtensions
{
    extension(BinaryPrimitives)
    {
        public static UInt256 ReadUInt256LittleEndian(ReadOnlySpan<byte> source)
            => new UInt256(source, false);
        public static UInt256 ReadUInt256BigEndian(ReadOnlySpan<byte> source)
            => new UInt256(source, true);

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

        public static Int256 ReaInt256dLittleEndian(ReadOnlySpan<byte> source)
            => new Int256(new UInt256(source, false));
        public static Int256 ReadInt256BigEndian(ReadOnlySpan<byte> source)
            => new Int256(new UInt256(source, true));
        public static void WriteInt256LittleEndian(Span<byte> destination, Int256 value)
            => WriteUInt256LittleEndian(destination, value._value);
        public static void WriteInt256BigEndian(Span<byte> destination, Int256 value)
            => WriteUInt256BigEndian(destination, value._value);
    }
}
