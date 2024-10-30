namespace EtherSharp.ABI.Fixed;
internal abstract partial class FixedEncodeType<T>
{
    public class ULong : FixedEncodeType<ulong>
    {
        public ULong(ulong value, int length) : base(value)
        {
            if(length < 32 || length > 64 || length % 8 != 0)
            {
                throw new ArgumentException("Invalid bit size for fixed type", nameof(length));
            }
            if(value >> length != 0 && length != 64)
            {
                throw new ArgumentException($"Value is too large to fit in a {length}-bit unsigned integer", nameof(value));
            }
        }

        public override void Encode(Span<byte> values)
        {
            if(!BitConverter.TryWriteBytes(values[(32 - 8)..], Value))
            {
                throw new InvalidOperationException("Could Not Wryte Bytes");
            }
            if(BitConverter.IsLittleEndian)
            {
                values[(32 - 8)..].Reverse();
            }
        }
    }
}
