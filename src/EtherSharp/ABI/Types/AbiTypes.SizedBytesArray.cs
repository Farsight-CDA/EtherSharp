using EtherSharp.ABI.Types.Base;
using EtherSharp.Types;
using System.Buffers.Binary;

namespace EtherSharp.ABI.Types;

public static partial class AbiTypes
{
    /// <summary>
    /// Represents a dynamic array of fixed-size byte arrays.
    /// </summary>
    public class SizedBytesArray<TBytes> : DynamicType<TBytes[]>
        where TBytes : struct, IFixedBytes<TBytes>
    {
        /// <inheritdoc />
        public override int PayloadSize => (32 * Value.Length) + 32;

        internal SizedBytesArray(TBytes[] values)
            : base(values)
        { }

        /// <inheritdoc />
        public override void Encode(Span<byte> metadata, Span<byte> payload, int payloadOffset)
        {
            BinaryPrimitives.WriteUInt32BigEndian(metadata[28..32], (uint) payloadOffset);
            BinaryPrimitives.WriteUInt32BigEndian(payload[28..32], (uint) Value.Length);

            for(int i = 0; i < Value.Length; i++)
            {
                var slot = payload.Slice(32 + (i * 32), 32);

                SizedBytes<TBytes>.EncodeInto(Value[i], slot);
            }
        }

        /// <summary>
        /// Decodes an array of fixed-size byte arrays.
        /// </summary>
        public static TBytes[] Decode(ReadOnlyMemory<byte> bytes, int metaDataOffset)
        {
            int arrayOffest = (int) BinaryPrimitives.ReadUInt32BigEndian(bytes.Span[(metaDataOffset + 28)..(metaDataOffset + 32)]);
            int length = (int) BinaryPrimitives.ReadUInt32BigEndian(bytes.Span[(arrayOffest + 28)..(arrayOffest + 32)]);
            var data = bytes[(arrayOffest + 32)..];

            var results = new TBytes[length];

            for(int i = 0; i < length; i++)
            {
                results[i] = TBytes.FromBytes(data.Slice(i * 32, TBytes.BYTE_LENGTH).Span);
            }

            return results;
        }
    }
}
