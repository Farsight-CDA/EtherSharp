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
            BinaryPrimitives.WriteUInt32BigEndian(metadata[28..32], payloadOffset);
            BinaryPrimitives.WriteUInt32BigEndian(payload[28..32], (uint) Value.Length);

            if(!Encoding.UTF8.TryGetBytes(Value, payload[32..], out _))
            {
                throw new InvalidOperationException("Failed to write bytes");
            }
        }

        public static string Decode(ReadOnlyMemory<byte> bytes, uint metaDataOffset)
        {
            uint bytesOffset = BinaryPrimitives.ReadUInt32BigEndian(bytes[(32 - 4)..32].Span);

            long index = bytesOffset - metaDataOffset;
            if(index < 0 || index > int.MaxValue)
            {
                throw new IndexOutOfRangeException("Index out of range");
            }
            int validatedIndex = (int) index;

            uint stringLength = BinaryPrimitives.ReadUInt32BigEndian(bytes[(validatedIndex + 32 - 4)..(validatedIndex + 32)].Span);
            var stringBytes = bytes[(validatedIndex + 32)..(validatedIndex + 32 + (int) stringLength)];

            return Encoding.UTF8.GetString(stringBytes.Span);
        }
    }
}
