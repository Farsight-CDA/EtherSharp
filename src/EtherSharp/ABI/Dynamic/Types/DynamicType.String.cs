using System.Buffers.Binary;
using System.Text;

namespace EtherSharp.ABI.Dynamic;
internal abstract partial class DynamicType<T>
{
    public class String(string value) : DynamicType<string>(value ?? throw new ArgumentNullException(nameof(value)))
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

            if(!Encoding.UTF8.TryGetBytes(Value, payload[32..], out _))
            {
                throw new InvalidOperationException("Failed to write bytes");
            }
        }

        public static string Decode(ReadOnlyMemory<byte> bytes, uint metaDataOffset)
        {

            uint bytesOffset = BitConverter.ToUInt32(bytes[(32 - 4)..32].Span);
            if(BitConverter.IsLittleEndian)
            {
                bytesOffset = BinaryPrimitives.ReverseEndianness(bytesOffset);
            }

            long index = bytesOffset - metaDataOffset;
            if(index < 0 || index > int.MaxValue)
            {
                throw new IndexOutOfRangeException("Index out of range");
            }
            int validatedIndex = (int) index;

            uint length = BitConverter.ToUInt32(bytes[(validatedIndex + 32 - 4)..(validatedIndex + 32)].Span);
            if(BitConverter.IsLittleEndian)
            {
                length = BinaryPrimitives.ReverseEndianness(length);
            }

            var stringBytes = bytes[(validatedIndex + 32)..(validatedIndex + 32 + (int) length)];
            return Encoding.UTF8.GetString(stringBytes.Span);
        }
    }
}
