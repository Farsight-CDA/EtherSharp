using EtherSharp.ABI.Types.Base;
using System.Buffers.Binary;

namespace EtherSharp.ABI.Types;

public static partial class AbiTypes
{
    /// <summary>
    /// Represents a dynamic array of unsigned 256-bit values.
    /// </summary>
    public class UInt256Array : DynamicType<Numerics.UInt256[]>
    {
        /// <inheritdoc />
        public override int PayloadSize => (32 * Value.Length) + 32;

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

        /// <inheritdoc />
        public override void Encode(Span<byte> metadata, Span<byte> payload, int payloadOffset)
        {
            BinaryPrimitives.WriteUInt32BigEndian(metadata[28..32], (uint) payloadOffset);
            BinaryPrimitives.WriteUInt32BigEndian(payload[28..32], (uint) Value.Length);

            for(int i = 0; i < Value.Length; i++)
            {
                var slot = payload.Slice(32 + (i * 32), 32);
                AbiTypes.UInt256.EncodeInto(Value[i], slot, false);
            }
        }

        /// <summary>
        /// Decodes an array of unsigned 256-bit values.
        /// </summary>
        public static Numerics.UInt256[] Decode(ReadOnlyMemory<byte> bytes, int metaDataOffset, uint bitSize)
        {
            if(bitSize < 64 || bitSize > 256 || bitSize % 8 != 0)
            {
                throw new ArgumentException("Invalid bit size for fixed type", nameof(bitSize));
            }

            int arrayOffest = (int) BinaryPrimitives.ReadUInt32BigEndian(bytes.Span[(metaDataOffset + 28)..(metaDataOffset + 32)]);
            int length = (int) BinaryPrimitives.ReadUInt32BigEndian(bytes.Span[(arrayOffest + 28)..(arrayOffest + 32)]);
            var data = bytes[(arrayOffest + 32)..];

            var arr = new Numerics.UInt256[length];

            for(int i = 0; i < length; i++)
            {
                var slot = data[(i * 32)..((i * 32) + 32)];
                var value = AbiTypes.UInt256.Decode(slot.Span);
                if(value.BitLength > bitSize)
                {
                    throw new ArgumentException($"Value is too large to fit in a {bitSize}-bit unsigned integer", nameof(bytes));
                }

                arr[i] = value;
            }

            return arr;
        }
    }
}