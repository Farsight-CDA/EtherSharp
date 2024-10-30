namespace EtherSharp.ABI.Fixed;
internal abstract partial class FixedType<T>
{
    public class BigInteger : FixedType<System.Numerics.BigInteger>
    {
        private readonly int _length;
        private readonly bool _isUnsigned;
        private readonly int _byteCount;

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

            _length = length;
            _isUnsigned = isUnsigned;
            _byteCount = value.GetByteCount(isUnsigned);

            if(_byteCount > length / 8)
            {
                throw new ArgumentException($"Value is too large to fit in a {_length}-bit {(isUnsigned ? "un" : "")}signed integer", nameof(value));
            }
        }
        public override void Encode(Span<byte> values)
        {
            if(!Value.TryWriteBytes(values[(32 - _byteCount)..], out _, isBigEndian: true, isUnsigned: _isUnsigned))
            {
                throw new InvalidOperationException("Could Not Wryte Bytes");
            }
            if(Value.Sign < 0)
            {
                values[..(32 - _byteCount)].Fill(byte.MaxValue);
            }
        }
    }
}
