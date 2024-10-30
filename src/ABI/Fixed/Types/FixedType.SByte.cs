namespace EtherSharp.ABI.Fixed;
internal abstract partial class FixedType<T>
{
    public class SByte(sbyte value) : FixedType<sbyte>(value)
    {
        public override void Encode(Span<byte> values)
        {
            values[^1] = (byte) Value;

            if(Value < 0)
            {
                values[..(32 - 1)].Fill(byte.MaxValue);
            }
        }

        public static sbyte Decode(Span<byte> bytes, int _) => (sbyte) bytes[31];
    }
}
