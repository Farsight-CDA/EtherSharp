using EtherSharp.ABI.Types.Base;
using System.Buffers.Binary;

namespace EtherSharp.ABI.Types;

public static partial class AbiTypes
{
    public class UInt256Array : DynamicType<Numerics.UInt256[]>
    {
        public override uint PayloadSize => (32 * (uint) Value.Length) + 32;

        internal UInt256Array(Numerics.UInt256[] value, int bitSize)
            : base(value)
        {
            if(bitSize < 64 || bitSize > 256 || bitSize % 8 != 0)
            {
                throw new ArgumentException("Invalid bit size for fixed type", nameof(bitSize));
            }

            for(int i = 0; i < Value.Length; i++)
            {
                var entry = Value[i];

                if(entry.BitLength > bitSize)
                {
                    throw new ArgumentException($"Value is too large to fit in a {bitSize}-bit unsigned integer", nameof(value));
                }
            }
        }

        public override void Encode(Span<byte> metadata, Span<byte> payload, uint payloadOffset)
        {
            BinaryPrimitives.WriteUInt32BigEndian(metadata[28..32], payloadOffset);
            BinaryPrimitives.WriteUInt32BigEndian(payload[28..32], (uint) Value.Length);

            for(int i = 0; i < Value.Length; i++)
            {
                var slot = payload.Slice(32 + (i * 32), 32);
                AbiTypes.UInt256.EncodeInto(Value[i], slot, false);
            }
        }

        public static Numerics.UInt256[] Decode(ReadOnlyMemory<byte> bytes, uint metaDataOffset, uint bitSize)
        {
            uint arrayOffest = BinaryPrimitives.ReadUInt32BigEndian(bytes.Span[((int) metaDataOffset + 28)..((int) metaDataOffset + 32)]);

            uint arrLength = BinaryPrimitives.ReadUInt32BigEndian(bytes.Span[((int) arrayOffest + 28)..((int) arrayOffest + 32)]);

            var data = bytes[((int) arrayOffest + 32)..];
            var arr = new Numerics.UInt256[arrLength];

            for(int i = 0; i < arrLength; i++)
            {
                var slot = data[(i * 32)..((i * 32) + 32)];
                arr[i] = AbiTypes.UInt256.Decode(slot.Span);
            }

            return arr;
        }
    }
}