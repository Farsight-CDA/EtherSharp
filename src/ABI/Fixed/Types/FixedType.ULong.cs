using System.Buffers.Binary;

namespace EtherSharp.ABI.Fixed;
internal abstract partial class FixedType<T>
{
    internal class ULong : FixedType<ulong>
    {
        public ULong(ulong value, int length) : base(value)
        {
            if(length < 32 || length > 64 || length % 8 != 0)
            {
                throw new ArgumentException("Invalid bit size for fixed type", nameof(length));
            }
            if(length != 64 && value >> length != 0)
            {
                throw new ArgumentException($"Value is too large to fit in a {length}-bit unsigned integer", nameof(value));
            }
        }

        public override void Encode(Span<byte> buffer)
            => EncodeInto(Value, buffer);

        public static void EncodeInto(ulong value, Span<byte> buffer)
        {
            if(!BitConverter.TryWriteBytes(buffer[(32 - 8)..], value))
            {
                throw new InvalidOperationException("Could Not Wryte Bytes");
            }
            if(BitConverter.IsLittleEndian)
            {
                buffer[(32 - 8)..].Reverse();
            }
        }

        public static ulong Decode(Span<byte> bytes)
        {
            ulong value = BitConverter.ToUInt64(bytes[..8]);

            if(BitConverter.IsLittleEndian)
            {
                value = BinaryPrimitives.ReverseEndianness(value);
            }
            return value;
        }
    }
}
