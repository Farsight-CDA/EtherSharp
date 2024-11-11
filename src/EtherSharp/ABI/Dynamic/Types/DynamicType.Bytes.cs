using System.Buffers.Binary;

namespace EtherSharp.ABI.Dynamic;
internal abstract partial class DynamicType<T>
{
    public class Bytes(byte[] value) : DynamicType<byte[]>(value)
    {
        public override uint PayloadSize => (((uint) Value.Length + 31) / 32 * 32) + 32;

        public override void Encode(Span<byte> metadata, Span<byte> payload, uint payloadOffset)
        {
            if(!BitConverter.TryWriteBytes(metadata, payloadOffset))
            {
                throw new InvalidOperationException("Failed to write bytes");
            }
            if(BitConverter.IsLittleEndian)
            {
                metadata.Reverse();
            }

            if(!BitConverter.TryWriteBytes(payload[..32], Value.Length))
            {
                throw new InvalidOperationException("Failed to write bytes");
            }
            if(BitConverter.IsLittleEndian)
            {
                payload[..32].Reverse();
            }

            Value.CopyTo(payload[32..]);
        }

        public static ReadOnlyMemory<byte> Decode(ReadOnlyMemory<byte> bytes, uint metaDataOffset)
        {
            uint bytesOffset = BitConverter.ToUInt32(bytes[(32 - 4)..].Span);

            if(BitConverter.IsLittleEndian)
            {
                bytesOffset = BinaryPrimitives.ReverseEndianness(bytesOffset);
            }

            long index = bytesOffset - metaDataOffset;

            if(index < 0 || index > int.MaxValue)
            {
                throw new IndexOutOfRangeException("Index out of range");
            }

            uint leng = BitConverter.ToUInt32(bytes[(int) (index + 32 - 4)..(int) (index + 32)].Span);

            if(BitConverter.IsLittleEndian)
            {
                leng = BinaryPrimitives.ReverseEndianness(leng);
            }

            return bytes[((int) index + 32)..(int) ((int) index + 32 + leng)];
        }
    }
}
