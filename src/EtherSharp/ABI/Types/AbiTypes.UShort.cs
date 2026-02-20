using EtherSharp.ABI.Types.Base;
using System.Buffers.Binary;

namespace EtherSharp.ABI.Types;

public static partial class AbiTypes
{
    public class UShort : FixedType<ushort>, IPackedEncodeType
    {
        /// <inheritdoc/>
        public int PackedSize => 2;

        internal UShort(ushort value) : base(value)
        {

        }

        public override void Encode(Span<byte> buffer)
            => EncodeInto(Value, buffer);
        void IPackedEncodeType.EncodePacked(Span<byte> buffer)
            => EncodeInto(Value, buffer);

        public static void EncodeInto(ushort value, Span<byte> buffer)
            => BinaryPrimitives.WriteUInt16BigEndian(buffer[(buffer.Length - 2)..], value);

        public static ushort Decode(ReadOnlySpan<byte> bytes)
            => BinaryPrimitives.ReadUInt16BigEndian(bytes[(32 - 2)..]);

    }
}
