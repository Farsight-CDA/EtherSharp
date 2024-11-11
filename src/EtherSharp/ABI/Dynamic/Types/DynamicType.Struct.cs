using EtherSharp.ABI.Decode;
using EtherSharp.ABI.Encode.Interfaces;

namespace EtherSharp.ABI.Dynamic;
internal abstract partial class DynamicType<T>
{
    public class Struct(uint typeId, IStructAbiEncoder value) : DynamicType<IStructAbiEncoder>(value)
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

            if(!BitConverter.TryWriteBytes(payload[..32], typeId))
            {
                throw new InvalidOperationException("Failed to write bytes");
            }
            if(BitConverter.IsLittleEndian)
            {
                payload[..32].Reverse();
            }

            Value.WritoTo(payload[32..]);
        }

        public static T Decode(Memory<byte> bytes, uint metaDataOffset, Func<IStructAbiDecoder, T> decoder)
        {
            uint structOffset = BitConverter.ToUInt32(bytes[(32 - 4)..].Span);

            long index = structOffset - metaDataOffset;
            if(index < 0 || index > int.MaxValue)
            {
                throw new IndexOutOfRangeException("Index out of range");
            }

            _ = BitConverter.ToUInt32(bytes[(int) index..(int) (index + 32)].Span);

            var structAbiDecoder = new AbiDecoder(bytes[(int) index..]);

            var innerValue = decoder.Invoke(structAbiDecoder);

            return innerValue;
        }
    }
}
