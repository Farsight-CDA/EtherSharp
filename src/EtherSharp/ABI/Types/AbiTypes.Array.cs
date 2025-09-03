using EtherSharp.ABI.Decode.Interfaces;
using EtherSharp.ABI.Encode.Interfaces;
using EtherSharp.ABI.Types.Base;
using System.Buffers.Binary;

namespace EtherSharp.ABI.Types;
public static partial class AbiTypes
{
    public class Array : DynamicType<IArrayAbiEncoder>
    {
        public override uint PayloadSize => Value.MetadataSize + Value.PayloadSize + 32;

        private readonly uint _length;

        internal Array(IArrayAbiEncoder value, uint length) : base(value)
        {
            _length = length;
        }

        public override void Encode(Span<byte> metadata, Span<byte> payload, uint payloadOffset)
        {
            BinaryPrimitives.WriteUInt32BigEndian(metadata[28..32], payloadOffset);
            BinaryPrimitives.WriteUInt32BigEndian(payload[28..32], _length);

            if(!Value.TryWritoTo(payload[32..]))
            {
                throw new InvalidOperationException("Failed to write bytes");
            }
        }

        public static T[] Decode<T>(ReadOnlyMemory<byte> bytes, uint metaDataOffset, Func<IArrayAbiDecoder, T> decoder)
        {
            uint payloadOffset = BinaryPrimitives.ReadUInt32BigEndian(bytes[28..32].Span);

            if(payloadOffset < metaDataOffset)
            {
                throw new IndexOutOfRangeException("Index out of range");
            }

            long relativePayloadOffset = payloadOffset - metaDataOffset;
            var payload = bytes[(int) relativePayloadOffset..];

            uint arrayLength = BinaryPrimitives.ReadUInt32BigEndian(payload[28..32].Span);

            var output = new T[arrayLength];

            var innerDecoder = new AbiDecoder(payload[32..]);

            for(uint i = 0; i < arrayLength; i++)
            {
                output[i] = decoder.Invoke(innerDecoder);
            }

            return output;
        }
    }
}
