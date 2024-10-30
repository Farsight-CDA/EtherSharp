namespace EtherSharp.ABI.Fixed;
internal abstract partial class FixedType<T>
{
    public class Long : FixedType<long>
    {
        public Long(long value, int length) : base(value)
        {
            if(length < 24 || length > 64 || length % 8 != 0)
            {
                throw new ArgumentException("Invalid bit size for fixed type", nameof(length));
            }
            if (length != 64 && ((value > 0 && value >> (length - 1) != 0) || (value < 0 && value >> (length - 1) != -1)))
            {
                throw new ArgumentException($"Value is too large to fit in a {length}-bit signed integer", nameof(value));
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
            if(Value < 0)
            {
                values[..(32 - 8)].Fill(byte.MaxValue);
            }
        }
    }
}
