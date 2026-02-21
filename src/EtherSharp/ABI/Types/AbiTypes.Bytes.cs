using EtherSharp.ABI.Types.Base;
using System.Buffers.Binary;

namespace EtherSharp.ABI.Types;

public static partial class AbiTypes
{
    /// <summary>
    /// Represents a dynamic ABI <c>bytes</c> value.
    /// </summary>
    public class Bytes : DynamicType<ReadOnlyMemory<byte>>, IPackedEncodeType
    {
        /// <summary>
        /// Gets the encoded payload size in bytes.
        /// </summary>
        public override int PayloadSize => ((Value.Length + 31) / 32 * 32) + 32;

        /// <summary>
        /// Gets the packed encoded size in bytes.
        /// </summary>
        public int PackedSize => Value.Length;

        internal Bytes(ReadOnlyMemory<byte> value) : base(value) { }

        /// <summary>
        /// Encodes bytes metadata and payload.
        /// </summary>
        public override void Encode(Span<byte> metadata, Span<byte> payload, int payloadOffset)
        {
            BinaryPrimitives.WriteUInt32BigEndian(metadata[28..32], (uint) payloadOffset);
            BinaryPrimitives.WriteUInt32BigEndian(payload[28..32], (uint) Value.Length);

            Value.Span.CopyTo(payload[32..]);
        }
        void IPackedEncodeType.EncodePacked(Span<byte> buffer)
            => Value.Span.CopyTo(buffer);

        /// <summary>
        /// Decodes a dynamic bytes value.
        /// </summary>
        public static ReadOnlyMemory<byte> Decode(ReadOnlyMemory<byte> bytes, int metaDataOffset)
        {
            uint bytesOffset = BinaryPrimitives.ReadUInt32BigEndian(bytes.Span[((int) metaDataOffset + 28)..((int) metaDataOffset + 32)]);

            uint valueLength = BinaryPrimitives.ReadUInt32BigEndian(bytes.Span[((int) bytesOffset + 28)..((int) bytesOffset + 32)]);
            return bytes.Slice((int) bytesOffset + 32, (int) valueLength);
        }
    }
}
