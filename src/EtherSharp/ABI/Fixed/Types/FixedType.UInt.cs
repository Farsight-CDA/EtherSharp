using System.Buffers.Binary;

namespace EtherSharp.ABI.Fixed;
internal abstract partial class FixedType
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
            => BinaryPrimitives.WriteUInt32BigEndian(buffer[(32 - 4)..], value);

        public static uint Decode(ReadOnlySpan<byte> bytes) 
            => BinaryPrimitives.ReadUInt32BigEndian(bytes[(32 - 4)..]);
    }
}
