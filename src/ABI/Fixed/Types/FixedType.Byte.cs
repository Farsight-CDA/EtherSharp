namespace EtherSharp.ABI.Fixed;
internal abstract partial class FixedType<T>
{
    internal class Byte(byte value) : FixedType<byte>(value)
    {
        public override void Encode(Span<byte> buffer) 
            => EncodeInto(Value, buffer);

        public static void EncodeInto(byte value, Span<byte> buffer)
            => buffer[^1] = value;
    }
}
