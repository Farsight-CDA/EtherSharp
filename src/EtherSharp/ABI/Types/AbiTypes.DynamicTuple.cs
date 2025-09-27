using EtherSharp.ABI.Decode.Interfaces;
using EtherSharp.ABI.Encode.Interfaces;
using EtherSharp.ABI.Types.Base;
using System.Buffers.Binary;

namespace EtherSharp.ABI.Types;
public static partial class AbiTypes
{
    public class DynamicTuple : DynamicType<IDynamicTupleEncoder>
    {
        public override uint PayloadSize => Value.MetadataSize + Value.PayloadSize;

        internal DynamicTuple(IDynamicTupleEncoder value) : base(value) { }

        public override void Encode(Span<byte> metadata, Span<byte> payload, uint payloadOffset)
        {
            if(Value.PayloadSize == 0)
            {
                throw new InvalidOperationException("Tried to encode a fixed value as a dynamic tuple");
            }

            BinaryPrimitives.WriteUInt32BigEndian(metadata[28..32], payloadOffset);

            if(!Value.TryWritoTo(payload))
            {
                throw new InvalidOperationException("Failed to write bytes");
            }
        }

        public static T Decode<T>(ReadOnlyMemory<byte> bytes, uint metaDataOffset, Func<IDynamicTupleDecoder, T> decoder)
        {
            uint structOffset = BinaryPrimitives.ReadUInt32BigEndian(bytes[(32 - 4)..].Span);

            long index = structOffset - metaDataOffset;

            ArgumentOutOfRangeException.ThrowIfLessThan(index, 0, nameof(metaDataOffset));
            ArgumentOutOfRangeException.ThrowIfGreaterThan(index, Int32.MaxValue, nameof(metaDataOffset));

            var structAbiDecoder = new AbiDecoder(bytes[(int) index..]);

            var innerValue = decoder.Invoke(structAbiDecoder);

            return innerValue;
        }
    }
}
