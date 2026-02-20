using EtherSharp.ABI.Decode.Interfaces;
using EtherSharp.ABI.Encode.Interfaces;
using EtherSharp.ABI.Types.Base;
using System.Buffers.Binary;

namespace EtherSharp.ABI.Types;

public static partial class AbiTypes
{
    public class Array : DynamicType<IArrayAbiEncoder>
    {
        public override int PayloadSize => Value.MetadataSize + Value.PayloadSize + 32;

        private readonly uint _length;

        internal Array(IArrayAbiEncoder value, uint length) : base(value)
        {
            _length = length;
        }

        public override void Encode(Span<byte> metadata, Span<byte> payload, int payloadOffset)
        {
            BinaryPrimitives.WriteUInt32BigEndian(metadata[28..32], (uint) payloadOffset);
            BinaryPrimitives.WriteUInt32BigEndian(payload[28..32], _length);

            if(!Value.TryWriteTo(payload[32..]))
            {
                throw new InvalidOperationException("Failed to write bytes");
            }
        }

        public static T[] Decode<T>(ReadOnlyMemory<byte> bytes, int metaDataOffset, Func<IArrayAbiDecoder, T> decoder)
        {
            int payloadOffset = (int) BinaryPrimitives.ReadUInt32BigEndian(bytes.Span[(metaDataOffset + 28)..(metaDataOffset + 32)]);

            var payload = bytes[(payloadOffset + 32)..];

            uint arrayLength = BinaryPrimitives.ReadUInt32BigEndian(bytes.Span[(payloadOffset + 28)..(payloadOffset + 32)]);

            var output = new T[arrayLength];

            var innerDecoder = new AbiDecoder(payload);

            for(uint i = 0; i < arrayLength; i++)
            {
                output[i] = decoder.Invoke(innerDecoder);
            }

            return output;
        }
    }
}
