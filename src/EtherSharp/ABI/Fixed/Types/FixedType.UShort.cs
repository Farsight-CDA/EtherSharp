using System.Buffers.Binary;

namespace EtherSharp.ABI.Fixed;
internal abstract partial class FixedType<T>
{
    internal class UShort(ushort value) : FixedType<ushort>(value)
    {
        public override void Encode(Span<byte> buffer)
            => EncodeInto(Value, buffer);

        public static void EncodeInto(ushort value, Span<byte> buffer) 
            => BinaryPrimitives.WriteUInt16BigEndian(buffer[(32 - 2)..], value);

        public static ushort Decode(ReadOnlySpan<byte> bytes) 
            => BinaryPrimitives.ReadUInt16BigEndian(bytes[(32 - 2)..]);
    }
}
