using EtherSharp.ABI.Types.Base;
using System.Buffers.Binary;
using System.Text;

namespace EtherSharp.ABI.Types;

public static partial class AbiTypes
{
    public class String : DynamicType<string>, IPackedEncodeType
    {
        public override uint PayloadSize => (((uint) PackedSize + 31) / 32 * 32) + 32;
        /// <inheritdoc/>
        public int PackedSize { get; }

        internal String(string value)
            : base(value)
        {
            ArgumentNullException.ThrowIfNull(value);
            PackedSize = Encoding.UTF8.GetByteCount(value);
        }

        public override void Encode(Span<byte> metadata, Span<byte> payload, uint payloadOffset)
        {
            BinaryPrimitives.WriteUInt32BigEndian(metadata[28..32], payloadOffset);
            BinaryPrimitives.WriteUInt32BigEndian(payload[28..32], (uint) Value.Length);

            if(!Encoding.UTF8.TryGetBytes(Value, payload[32..], out _))
            {
                throw new InvalidOperationException("Failed to write bytes");
            }
        }
        void IPackedEncodeType.EncodePacked(Span<byte> buffer)
        {
            if(!Encoding.UTF8.TryGetBytes(Value, buffer, out _))
            {
                throw new InvalidOperationException("Failed to write bytes");
            }
        }

        public static string Decode(ReadOnlySpan<byte> bytes, uint metaDataOffset)
        {
            uint bytesOffset = BinaryPrimitives.ReadUInt32BigEndian(bytes[((int) metaDataOffset + 28)..((int) metaDataOffset + 32)]);
            
            uint stringLength = BinaryPrimitives.ReadUInt32BigEndian(bytes[((int) bytesOffset + 28)..((int) bytesOffset + 32)]);
            var stringBytes = bytes[((int) bytesOffset + 32)..((int) bytesOffset + 32 + (int) stringLength)];

            return Encoding.UTF8.GetString(stringBytes);
        }
    }
}
