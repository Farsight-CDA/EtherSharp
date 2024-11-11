using System.Buffers.Binary;

namespace EtherSharp.ABI.Fixed;
internal abstract partial class FixedType<T>
{
    internal class UInt : FixedType<uint>
    {
        public UInt(uint value, int length) : base(value)
        {
            if(length < 24 || length > 32 || length % 8 != 0)
            {
                throw new ArgumentException("Invalid bit size for fixed type", nameof(length));
            }
            if(length != 32 && value >> length != 0)
            {
                throw new ArgumentException($"Value is too large to fit in a {length}-bit unsigned integer", nameof(value));
            }
        }

        public override void Encode(Span<byte> buffer)
            => EncodeInto(Value, buffer);

        public static void EncodeInto(uint value, Span<byte> buffer)
        {
            if(!BitConverter.TryWriteBytes(buffer[(32 - 4)..], value))
            {
                throw new InvalidOperationException("Could Not Wryte Bytes");
            }
            if(BitConverter.IsLittleEndian)
            {
                buffer[(32 - 4)..].Reverse();
            }
        }

        public static uint Decode(ReadOnlySpan<byte> bytes)
        {
            uint value = BitConverter.ToUInt32(bytes[(32 - 4)..]);

            if(BitConverter.IsLittleEndian)
            {
                value = BinaryPrimitives.ReverseEndianness(value);
            }
            return value;
        }
    }
}
