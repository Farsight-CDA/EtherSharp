using EtherSharp.ABI.Types.Base;
using System.Buffers.Binary;

namespace EtherSharp.ABI.Types;
public static partial class AbiTypes
{
    public class BigIntegerArray : DynamicType<System.Numerics.BigInteger[]>
    {
        private readonly bool _isUnsigned;
        public override uint PayloadSize => (32 * (uint) Value.Length) + 32;

        internal BigIntegerArray(System.Numerics.BigInteger[] value, bool isUnsigned, int bitSize)
            : base(value)
        {
            if(bitSize < 64 || bitSize > 256 || bitSize % 8 != 0)
            {
                throw new ArgumentException("Invalid bit size for fixed type", nameof(bitSize));
            }

            for(int i = 0; i < Value.Length; i++)
            {
                var entry = Value[i];

                if(isUnsigned && entry.Sign == -1)
                {
                    throw new ArgumentException("Value was negative for unsigned fixed type");
                }
                if(entry.GetByteCount(isUnsigned) > bitSize / 8)
                {
                    throw new ArgumentException($"Value is too large to fit in a {bitSize}-bit {(isUnsigned ? "un" : "")}signed integer", nameof(value));
                }
            }

            _isUnsigned = isUnsigned;
        }

        public override void Encode(Span<byte> metadata, Span<byte> payload, uint payloadOffset)
        {
            BinaryPrimitives.WriteUInt32BigEndian(metadata[28..32], payloadOffset);
            BinaryPrimitives.WriteUInt32BigEndian(payload[28..32], (uint) Value.Length);

            for(int i = 0; i < Value.Length; i++)
            {
                var slot = payload.Slice(32 + (i * 32), 32);
                AbiTypes.BigInteger.EncodeInto(Value[i], _isUnsigned, slot);
            }
        }

        public static System.Numerics.BigInteger[] Decode(ReadOnlyMemory<byte> bytes, uint metaDataOffset, uint bitSize, bool isUnsinght)
        {
            uint arrayOffest = BinaryPrimitives.ReadUInt32BigEndian(bytes[(32 - 4)..].Span);

            long index = arrayOffest - metaDataOffset;

            ArgumentOutOfRangeException.ThrowIfLessThan(index, 0, nameof(metaDataOffset));
            ArgumentOutOfRangeException.ThrowIfGreaterThan(index, int.MaxValue, nameof(metaDataOffset));

            uint arrLength = BinaryPrimitives.ReadUInt32BigEndian(bytes[(int) (index + 32 - 4)..(int) (index + 32)].Span);

            var data = bytes[(int) (index + 32)..];
            var arr = new System.Numerics.BigInteger[arrLength];

            for(int i = 0; i < arrLength; i++)
            {
                var slot = data[(i * 32)..((i * 32) + 32)];
                arr[i] = AbiTypes.BigInteger.Decode(slot.Span, isUnsinght);
            }

            return arr;
        }
    }
}