using EtherSharp.ABI.Fixed;
using System.Buffers.Binary;
using System.Numerics;

namespace EtherSharp.ABI.Dynamic;
internal abstract partial class DynamicType<T>
{
    public class BigIntegerArray : DynamicType<BigInteger[]>
    {
        private readonly bool _isUnsigned;
        public override uint PayloadSize => (32 * (uint) Value.Length) + 32;

        public BigIntegerArray(BigInteger[] value, bool isUnsigned, int length)
            : base(value)
        {
            if(length < 64 || length > 256 || length % 8 != 0)
            {
                throw new ArgumentException("Invalid bit size for fixed type", nameof(length));
            }

            for(int i = 0; i < Value.Length; i++)
            {
                var entry = Value[i];

                if(isUnsigned && entry.Sign == -1)
                {
                    throw new ArgumentException("Value was negative for unsigned fixed type");
                }
                if(entry.GetByteCount(isUnsigned) > length / 8)
                {
                    throw new ArgumentException($"Value is too large to fit in a {length}-bit {(isUnsigned ? "un" : "")}signed integer", nameof(value));
                }
            }

            _isUnsigned = isUnsigned;
        }

        public override void Encode(Span<byte> metadata, Span<byte> payload, uint payloadOffset)
        {
            if(!BitConverter.TryWriteBytes(metadata, payloadOffset))
            {
                throw new InvalidOperationException("Failed to write bytes");
            }
            if(BitConverter.IsLittleEndian)
            {
                metadata.Reverse();
            }

            if(!BitConverter.TryWriteBytes(payload.Slice(28, 4), (uint) Value.Length))
            {
                throw new InvalidOperationException("Failed to write bytes");
            }
            if(BitConverter.IsLittleEndian)
            {
                payload.Slice(28, 4).Reverse();
            }

            for(int i = 0; i < Value.Length; i++)
            {
                var slot = payload.Slice(32 + (i * 32), 32);
                FixedType<object>.BigInteger.EncodeInto(Value[i], _isUnsigned, slot);
            }
        }

        public static BigInteger[] Decode(ReadOnlyMemory<byte> bytes, uint metaDataOffset, uint length, bool isUnsinght)
        {
            uint arrayOffest = BitConverter.ToUInt32(bytes[(32 - 4)..].Span);

            if(BitConverter.IsLittleEndian)
            {
                arrayOffest = BinaryPrimitives.ReverseEndianness(arrayOffest);
            }

            long index = arrayOffest - metaDataOffset;
            if(index < 0 || index > int.MaxValue)
            {
                throw new IndexOutOfRangeException("Index out of range");
            }

            uint leng = BitConverter.ToUInt32(bytes[(int) (index + 32 - 4)..(int) (index + 32)].Span);

            if(BitConverter.IsLittleEndian)
            {
                leng = BinaryPrimitives.ReverseEndianness(leng);
            }

            var data = bytes[(int) (index + 32)..];
            var arr = new BigInteger[leng];
            for(int i = 0; i < leng; i++)
            {
                var slot = data[(i * 32)..((i * 32) + 32)];
                arr[i] = FixedType<object>.BigInteger.Decode(slot.Span, isUnsinght);
            }
            return arr;

        }
    }
}