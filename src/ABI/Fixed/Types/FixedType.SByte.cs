namespace EtherSharp.ABI.Fixed;
internal abstract partial class FixedType<T>
{
    internal class SByte(sbyte value) : FixedType<sbyte>(value)
    {
        public override void Encode(Span<byte> buffer)
            => EncodeInto(Value, buffer);

        public static void EncodeInto(sbyte value, Span<byte> buffer)
        {
            buffer[^1] = (byte) value;

            if(value < 0)
            {
                buffer[..(32 - 1)].Fill(byte.MaxValue);
            }
        }

        public static sbyte Decode(Span<byte> bytes, int _) => (sbyte) bytes[31];
    }
}
