using EtherSharp.ABI.Decode;
using EtherSharp.ABI.Decode.Interfaces;
using EtherSharp.ABI.Encode.Interfaces;
using System.Buffers.Binary;

namespace EtherSharp.ABI.Dynamic;
internal abstract partial class DynamicType<T>
{
    public class Array(IArrayAbiEncoder value) : DynamicType<IArrayAbiEncoder>(value)
    {
        public override uint PayloadSize => Value.MetadataSize + Value.PayloadSize + 32;

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

            if(!Value.TryWritoTo(payload[32..]))
            {
                throw new InvalidOperationException("Failed to write bytes");
            }
        }

        public static T[] Decode(ReadOnlyMemory<byte> bytes, uint metaDataOffset, Func<IArrayAbiDecoder, T> decoder)
        {
            uint payloadOffset = BitConverter.ToUInt32(bytes[28..32].Span);
            if(BitConverter.IsLittleEndian)
            {
                payloadOffset = BinaryPrimitives.ReverseEndianness(payloadOffset);
            }

            if(payloadOffset < metaDataOffset)
            {
                throw new IndexOutOfRangeException("Index out of range");
            }

            long relativePayloadOffset = payloadOffset - metaDataOffset;
            var payload = bytes[(int) relativePayloadOffset..];

            uint arrayLength = BitConverter.ToUInt32(payload[28..32].Span);
            if(BitConverter.IsLittleEndian)
            {
                arrayLength = BinaryPrimitives.ReverseEndianness(arrayLength);
            }

            var output = new T[arrayLength];

            for(uint i = 0; i < arrayLength; i++)
            {
                output[i] = decoder.Invoke(new AbiDecoder(payload[32..]));
            }

            return output;
        }
    }
}
