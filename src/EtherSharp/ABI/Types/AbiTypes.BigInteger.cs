using EtherSharp.ABI.Types.Base;

namespace EtherSharp.ABI.Types;
internal static partial class AbiTypes
{
    internal class BigInteger : FixedType<System.Numerics.BigInteger>, IPackedEncodeType
    {
        private readonly bool _isUnsigned;

        public int PackedSize { get; }

        public BigInteger(System.Numerics.BigInteger value, bool isUnsigned, int byteLength) : base(value)
        {
            if(byteLength < 8 || byteLength > 32)
            {
                throw new ArgumentException("Invalid bit size for fixed type", nameof(byteLength));
            }
            if(isUnsigned && value.Sign == -1)
            {
                throw new ArgumentException("Value was negative for unsigned fixed type");
            }
            if(value.GetByteCount(isUnsigned) > byteLength)
            {
                throw new ArgumentException($"Value is too large to fit in a {byteLength * 8}-bit {(isUnsigned ? "un" : "")}signed integer", nameof(value));
            }

            _isUnsigned = isUnsigned;
            PackedSize = byteLength;
        }

        public override void Encode(Span<byte> buffer)
            => EncodeInto(Value, _isUnsigned, buffer);
        public void EncodePacked(Span<byte> buffer)
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
                buffer[..(buffer.Length - bytesWritten)].Fill(byte.MaxValue);
            }
        }

        public static System.Numerics.BigInteger Decode(ReadOnlySpan<byte> bytes, bool isUnsigned)
            => new System.Numerics.BigInteger(bytes, isBigEndian: true, isUnsigned: isUnsigned);
    }
}
