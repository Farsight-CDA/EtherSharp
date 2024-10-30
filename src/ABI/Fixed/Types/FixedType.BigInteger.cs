namespace EtherSharp.ABI.Fixed;
internal abstract partial class FixedType<T>
{
    internal class BigInteger : FixedType<System.Numerics.BigInteger>
    {
        private readonly bool _isUnsigned;

        public BigInteger(System.Numerics.BigInteger value, bool isUnsigned, int length) : base(value)
        {
            if(length < 64 || length > 256 || length % 8 != 0)
            {
                throw new ArgumentException("Invalid bit size for fixed type", nameof(length));
            }
            if(isUnsigned && value.Sign == -1)
            {
                throw new ArgumentException("Value was negative for unsigned fixed type");
            }

            _isUnsigned = isUnsigned;

            if(value.GetByteCount(isUnsigned) > length / 8)
            {
                throw new ArgumentException($"Value is too large to fit in a {length}-bit {(isUnsigned ? "un" : "")}signed integer", nameof(value));
            }
        }

        public override void Encode(Span<byte> buffer)
            => EncodeInto(Value, _isUnsigned, buffer);

        public static void EncodeInto(System.Numerics.BigInteger value, bool isUnsigned, Span<byte> buffer)
        {
            if(!value.TryWriteBytes(buffer, out int bytesWritten, isBigEndian: false, isUnsigned: isUnsigned))
            {
                throw new InvalidOperationException("Could Not Wryte Bytes");
            }

            buffer.Reverse();

            if(value.Sign < 0)
            {
                buffer[..(32 - bytesWritten)].Fill(byte.MaxValue);
            }
        }
    }
}
