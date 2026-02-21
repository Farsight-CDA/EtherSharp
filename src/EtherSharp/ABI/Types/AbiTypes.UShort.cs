using EtherSharp.ABI.Types.Base;
using System.Buffers.Binary;

namespace EtherSharp.ABI.Types;

public static partial class AbiTypes
{
    /// <summary>
    /// Represents an ABI unsigned 16-bit value.
    /// </summary>
    public class UShort : FixedType<ushort>, IPackedEncodeType
    {
        /// <summary>
        /// Gets the packed encoded size in bytes.
        /// </summary>
        public int PackedSize => 2;

        internal UShort(ushort value) : base(value)
        {

        }

        /// <summary>
        /// Writes the value into the target buffer.
        /// </summary>
        public override void Encode(Span<byte> buffer)
            => EncodeInto(Value, buffer);
        void IPackedEncodeType.EncodePacked(Span<byte> buffer)
            => EncodeInto(Value, buffer);

        /// <summary>
        /// Encodes an unsigned short value.
        /// </summary>
        public static void EncodeInto(ushort value, Span<byte> buffer)
            => BinaryPrimitives.WriteUInt16BigEndian(buffer[(buffer.Length - 2)..], value);

        /// <summary>
        /// Decodes an unsigned short from an ABI word.
        /// </summary>
        public static ushort Decode(ReadOnlySpan<byte> bytes)
            => BinaryPrimitives.ReadUInt16BigEndian(bytes[(32 - 2)..]);
    }
}
