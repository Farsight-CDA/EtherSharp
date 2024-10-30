namespace EtherSharp.ABI.Fixed;
internal abstract partial class FixedEncodeType<T>
{
    public class SByte(sbyte value) : FixedEncodeType<sbyte>(value)
    {
        public override void Encode(Span<byte> values)
        {
            values[^1] = (byte) Value;

            if(Value < 0)
            {
                values[..(32 - 1)].Fill(byte.MaxValue);
            }
        }
    }
}
