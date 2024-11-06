using EtherSharp.ABI.Decode;
using EtherSharp.ABI.Encode.Interfaces;

namespace EtherSharp.ABI.Dynamic;
internal abstract partial class DynamicType<T>
{
    public class Array(IArrayAbiEncoder value) : DynamicType<IArrayAbiEncoder>(value)
    {
        public override uint PayloadSize => Value.PayloadSize + Value.MetadataSize + 32;

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

            if(!BitConverter.TryWriteBytes(payload[..32], Value.MetadataSize / 32))
            {
                throw new InvalidOperationException("Failed to write bytes");
            }
            if(BitConverter.IsLittleEndian)
            {
                payload[..32].Reverse();
            }

            Value.WritoTo(payload[32..]);
        }

        public static T[] Decode(Memory<byte> bytes, uint metaDataOffset, Func<ArrayAbiDecoder, T[]> decoder)
        {
            uint structOffset = BitConverter.ToUInt32(bytes[(32 - 4)..].Span);

            long index = structOffset - metaDataOffset;
            if(index < 0 || index > int.MaxValue)
            {
                throw new IndexOutOfRangeException("Index out of range");
            }

            var structAbiDecoder = new ArrayAbiDecoder(bytes[(int) index..]);

            var innerValue = decoder.Invoke(structAbiDecoder);

            return innerValue;
        }
    }
}
