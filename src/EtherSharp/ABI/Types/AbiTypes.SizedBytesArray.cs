using EtherSharp.ABI.Types.Base;
using System.Buffers.Binary;

namespace EtherSharp.ABI.Types;

public static partial class AbiTypes
{
    /// <summary>
    /// Represents a dynamic array of fixed-size byte arrays.
    /// </summary>
    public class SizedBytesArray : DynamicType<ReadOnlyMemory<byte>[]>
    {
        /// <inheritdoc />
        public override int PayloadSize => (32 * Value.Length) + 32;

        internal SizedBytesArray(ReadOnlyMemory<byte>[] values, int length)
            : base(values)
        {
            for(int i = 0; i < Value.Length; i++)
            {
                if(Value[i].Length != length)
                {
                    throw new ArgumentException($"Expected all members of Bytes{length}Array to be of length {length}, but got element of length {Value[i].Length}");
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

                SizedBytes.EncodeInto(Value[i], slot);
            }
        }

        /// <summary>
        /// Decodes an array of fixed-size byte arrays.
        /// </summary>
        public static ReadOnlyMemory<byte>[] Decode(ReadOnlyMemory<byte> bytes, int metaDataOffset, int byteSize)
        {
            int arrayOffest = (int) BinaryPrimitives.ReadUInt32BigEndian(bytes.Span[(metaDataOffset + 28)..(metaDataOffset + 32)]);
            int length = (int) BinaryPrimitives.ReadUInt32BigEndian(bytes.Span[(arrayOffest + 28)..(arrayOffest + 32)]);
            var data = bytes[(arrayOffest + 32)..];

            ReadOnlyMemory<byte>[] results = new ReadOnlyMemory<byte>[length];

            for(int i = 0; i < length; i++)
            {
                results[i] = data.Slice(i * 32, byteSize);
            }

            return results;
        }
    }
}