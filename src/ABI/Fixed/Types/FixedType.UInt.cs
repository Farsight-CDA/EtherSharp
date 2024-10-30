namespace EtherSharp.ABI.Fixed;
internal abstract partial class FixedType<T>
{
    public class UInt : FixedType<uint>
    {
        public UInt(uint value, int length) : base(value)
        {
            if(length < 24 || length > 32 || length % 8 != 0)
            {
                throw new ArgumentException("Invalid bit size for fixed type", nameof(length));
            }
            if(value >> length != 0 && length != 32)
            {
                throw new ArgumentException($"Value is too large to fit in a {length}-bit unsigned integer", nameof(value));
            }
        }

        public override void Encode(Span<byte> values)
        {
            if(!BitConverter.TryWriteBytes(values[(32 - 4)..], Value))
            {
                throw new InvalidOperationException("Could Not Wryte Bytes");
            }
            if(BitConverter.IsLittleEndian)
            {
                values[..(32 - 4)].Reverse();
            }
        }
    }
}
