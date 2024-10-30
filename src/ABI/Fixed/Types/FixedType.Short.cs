namespace EtherSharp.ABI.Fixed;
internal abstract partial class FixedType<T>
{
    internal class Short(short value) : FixedType<short>(value)
    {
        public override void Encode(Span<byte> buffer)
            => EncodeInto(Value, buffer);

        public static void EncodeInto(short value, Span<byte> buffer)
        {
            if(!BitConverter.TryWriteBytes(buffer[(32 - 2)..], value))
            {
                throw new InvalidOperationException("Could Not Wryte Bytes");
            }
            if(BitConverter.IsLittleEndian)
            {
                buffer[(32 - 2)..].Reverse();

            }
            if(value < 0)
            {
                buffer[..(32 - 2)].Fill(byte.MaxValue);
            }
        }

        public static byte Decode(Span<byte> bytes, int _) => bytes[31];
    }
}
