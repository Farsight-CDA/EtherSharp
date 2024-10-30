namespace EtherSharp.ABI.Fixed;
internal abstract partial class FixedEncodeType<T>
{
    public class UShort(ushort value) : FixedEncodeType<ushort>(value)
    {
        public override void Encode(Span<byte> buffer)
        {

            if(!BitConverter.TryWriteBytes(buffer[(32 - 2)..], Value))
            {
                throw new InvalidOperationException("Could Not Wryte Bytes");
            }
            if(BitConverter.IsLittleEndian)
            {
                buffer[(32 - 2)..].Reverse();

            }
        }
    }
}
