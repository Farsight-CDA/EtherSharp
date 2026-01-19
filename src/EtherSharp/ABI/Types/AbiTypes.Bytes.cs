using EtherSharp.ABI.Types.Base;
using System.Buffers.Binary;

namespace EtherSharp.ABI.Types;

public static partial class AbiTypes
{
    public class Bytes : DynamicType<byte[]>, IPackedEncodeType
    {
        public override int PayloadSize => ((Value.Length + 31) / 32 * 32) + 32;
        /// <inheritdoc/>
        public int PackedSize => Value.Length;

        internal Bytes(byte[] value) : base(value) { }

        public override void Encode(Span<byte> metadata, Span<byte> payload, int payloadOffset)
        {
            BinaryPrimitives.WriteUInt32BigEndian(metadata[28..32], (uint) payloadOffset);
            BinaryPrimitives.WriteUInt32BigEndian(payload[28..32], (uint) Value.Length);

            Value.CopyTo(payload[32..]);
        }
        void IPackedEncodeType.EncodePacked(Span<byte> buffer)
            => Value.CopyTo(buffer);

        public static ReadOnlySpan<byte> Decode(ReadOnlySpan<byte> bytes, int metaDataOffset)
        {
            uint bytesOffset = BinaryPrimitives.ReadUInt32BigEndian(bytes[((int) metaDataOffset + 28)..((int) metaDataOffset + 32)]);

            uint valueLength = BinaryPrimitives.ReadUInt32BigEndian(bytes[((int) bytesOffset + 28)..((int) bytesOffset + 32)]);
            return bytes[((int) bytesOffset + 32)..((int) bytesOffset + 32 + (int) valueLength)];
        }
    }
}
