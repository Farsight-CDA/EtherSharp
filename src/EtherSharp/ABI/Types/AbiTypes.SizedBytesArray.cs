using EtherSharp.ABI.Types.Base;
using System.Buffers.Binary;

namespace EtherSharp.ABI.Types;
internal static partial class AbiTypes
{
    public class SizedBytesArray : DynamicType<byte[][]>
    {
        public override uint PayloadSize => (32 * (uint) Value.Length) + 32;

        public SizedBytesArray(byte[][] values, int length)
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

        public override void Encode(Span<byte> metadata, Span<byte> payload, uint payloadOffset)
        {
            BinaryPrimitives.WriteUInt32BigEndian(metadata[28..32], payloadOffset);
            BinaryPrimitives.WriteUInt32BigEndian(payload[28..32], (uint) Value.Length);

            for(int i = 0; i < Value.Length; i++)
            {
                var slot = payload.Slice(32 + (i * 32), 32);

                SizedBytes.EncodeInto(Value[i], slot);
            }
        }

        public static byte[][] Decode(ReadOnlyMemory<byte> bytes, uint metaDataOffset, int byteSize)
        {
            uint arrayOffest = BinaryPrimitives.ReadUInt32BigEndian(bytes[(32 - 4)..].Span);

            long index = arrayOffest - metaDataOffset;

            ArgumentOutOfRangeException.ThrowIfLessThan(index, 0, nameof(metaDataOffset));
            ArgumentOutOfRangeException.ThrowIfGreaterThan(index, int.MaxValue, nameof(metaDataOffset));

            uint length = BinaryPrimitives.ReadUInt32BigEndian(bytes[(int) (index + 32 - 4)..(int) (index + 32)].Span);

            var data = bytes[(int) (index + 32)..];

            byte[][] results = new byte[length][];

            for(int i = 0; i < length; i++)
            {
                results[i] = data.Slice((i * 32) + 32 - byteSize, byteSize).ToArray();
            }

            return results;
        }
    }
}