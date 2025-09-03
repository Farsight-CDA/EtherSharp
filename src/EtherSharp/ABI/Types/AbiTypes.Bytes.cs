using EtherSharp.ABI.Types.Base;
using System.Buffers.Binary;

namespace EtherSharp.ABI.Types;
public static partial class AbiTypes
{
    public class Bytes : DynamicType<byte[]>, IPackedEncodeType
    {
        public override uint PayloadSize => (((uint) Value.Length + 31) / 32 * 32) + 32;
        /// <inheritdoc/>
        public int PackedSize => Value.Length;

        internal Bytes(byte[] value) : base(value) { }

        public override void Encode(Span<byte> metadata, Span<byte> payload, uint payloadOffset)
        {
            BinaryPrimitives.WriteUInt32BigEndian(metadata[28..32], payloadOffset);
            BinaryPrimitives.WriteUInt32BigEndian(payload[28..32], (uint) Value.Length);

            Value.CopyTo(payload[32..]);
        }
        void IPackedEncodeType.EncodePacked(Span<byte> buffer)
            => Value.CopyTo(buffer);

        public static ReadOnlySpan<byte> Decode(ReadOnlySpan<byte> bytes, uint metaDataOffset)
        {
            uint bytesOffset = BinaryPrimitives.ReadUInt32BigEndian(bytes[(32 - 4)..]);

            long index = bytesOffset - metaDataOffset;

            ArgumentOutOfRangeException.ThrowIfLessThan(index, 0, nameof(metaDataOffset));
            ArgumentOutOfRangeException.ThrowIfGreaterThan(index, bytes.Length, nameof(metaDataOffset));

            uint valueLength = BinaryPrimitives.ReadUInt32BigEndian(bytes[(int) (index + 32 - 4)..(int) (index + 32)]);
            return bytes[((int) index + 32)..(int) ((int) index + 32 + valueLength)];
        }
    }
}
