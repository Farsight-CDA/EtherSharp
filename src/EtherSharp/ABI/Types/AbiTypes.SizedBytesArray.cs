using EtherSharp.ABI.Types.Base;
using System.Buffers.Binary;

namespace EtherSharp.ABI.Types;

public static partial class AbiTypes
{
    public class SizedBytesArray : DynamicType<byte[][]>
    {
        public override int PayloadSize => (32 * Value.Length) + 32;

        internal SizedBytesArray(byte[][] values, int length)
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

        public static byte[][] Decode(ReadOnlyMemory<byte> bytes, int metaDataOffset, int byteSize)
        {
            int arrayOffest = (int) BinaryPrimitives.ReadUInt32BigEndian(bytes.Span[(metaDataOffset + 28)..(metaDataOffset + 32)]);
            int length = (int) BinaryPrimitives.ReadUInt32BigEndian(bytes.Span[(arrayOffest + 28)..(arrayOffest + 32)]);
            var data = bytes[(arrayOffest + 32)..];

            byte[][] results = new byte[length][];

            for(int i = 0; i < length; i++)
            {
                results[i] = data.Slice(i * 32, byteSize).ToArray();
            }

            return results;
        }
    }
}