using System.Buffers.Binary;

namespace EtherSharp.ABI.Fixed;
internal abstract partial class FixedType
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
            => BinaryPrimitives.WriteUInt64BigEndian(buffer[(32 - 8)..], value);

        public static ulong Decode(ReadOnlySpan<byte> bytes)
            => BinaryPrimitives.ReadUInt64BigEndian(bytes[(32 - 8)..]);
    }
}
