using EtherSharp.ABI.Types.Base;
using EtherSharp.ABI.Types.Interfaces;
using System.Buffers.Binary;

namespace EtherSharp.ABI.Types;
internal static partial class AbiTypes
{
    internal class UShort(ushort value) : FixedType<ushort>(value), IPackedEncodeType
    {
        public int PackedSize => 2;

        public override void Encode(Span<byte> buffer)
            => EncodeInto(Value, buffer);
        public void EncodePacked(Span<byte> buffer) 
            => EncodeInto(Value, buffer);

        public static void EncodeInto(ushort value, Span<byte> buffer)
            => BinaryPrimitives.WriteUInt16BigEndian(buffer[(buffer.Length - 2)..], value);

        public static ushort Decode(ReadOnlySpan<byte> bytes)
            => BinaryPrimitives.ReadUInt16BigEndian(bytes[(32 - 2)..]);

    }
}
