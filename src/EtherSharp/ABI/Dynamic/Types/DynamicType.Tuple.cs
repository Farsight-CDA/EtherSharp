using EtherSharp.ABI.Decode.Interfaces;
using EtherSharp.ABI.Encode.Interfaces;

namespace EtherSharp.ABI.Dynamic;
internal abstract partial class DynamicType<T>
{
    public class Tuple(IDynamicTupleEncoder value) : DynamicType<IDynamicTupleEncoder>(value)
    {
        public override uint PayloadSize => Value.MetadataSize + Value.PayloadSize;

        public override void Encode(Span<byte> metadata, Span<byte> payload, uint payloadOffset)
        {
            if (Value.PayloadSize == 0)
            {
                throw new InvalidOperationException("Tried to encode a fixed value as a dynamic tuple");
            }

            if(!BitConverter.TryWriteBytes(metadata, payloadOffset))
            {
                throw new InvalidOperationException("Failed to write bytes");
            }
            if(BitConverter.IsLittleEndian)
            {
                metadata.Reverse();
            }

            if(!Value.TryWritoTo(payload))
            {
                throw new InvalidOperationException("Failed to write bytes");
            }
        }

        public static T Decode(ReadOnlyMemory<byte> bytes, uint metaDataOffset, Func<IStructAbiDecoder, T> decoder)
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
