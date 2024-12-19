using System.Buffers.Binary;

namespace EtherSharp.ABI.Dynamic;
internal abstract partial class DynamicType<T>
{
    public class Bytes(byte[] value) : DynamicType<byte[]>(value)
    {
        public override uint PayloadSize => (((uint) Value.Length + 31) / 32 * 32) + 32;

        public override void Encode(Span<byte> metadata, Span<byte> payload, uint payloadOffset)
        {
            BinaryPrimitives.WriteUInt32BigEndian(metadata[28..32], payloadOffset);
            BinaryPrimitives.WriteUInt32BigEndian(payload[28..32], (uint) Value.Length);

            Value.CopyTo(payload[32..]);
        }

        public static ReadOnlySpan<byte> Decode(ReadOnlySpan<byte> bytes, uint metaDataOffset)
        {
            uint bytesOffset = BinaryPrimitives.ReadUInt32BigEndian(bytes[(32 - 4)..]);

            long index = bytesOffset - metaDataOffset;

            if(index < 0 || index > bytes.Length)
            {
                throw new IndexOutOfRangeException("Index out of range");
            }

            uint valueLength = BinaryPrimitives.ReadUInt32BigEndian(bytes[(int) (index + 32 - 4)..(int) (index + 32)]);
            return bytes[((int) index + 32)..(int) ((int) index + 32 + valueLength)];
        }
    }
}
