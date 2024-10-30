using System.Numerics;

namespace EtherSharp.ABI.Dynamic;
internal abstract partial class DynamicType<T>
{
    public class BigIntegerArray : DynamicType<BigInteger[]>
    {
        private readonly bool _isUnsigned;

        public override uint PayloadSize => 32 * (uint) Value.Length + 32;

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

                if (!Value[i].TryWriteBytes(slot, out _, _isUnsigned, true))
                {
                    throw new InvalidOperationException("Failed to write bytes");
                }
            }
        }
    }
}